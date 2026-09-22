using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

using Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog.Config;

namespace Macchacoffee.ElinMods.AddPalmiaTimesNewsToLog.News;

internal class NewsFeeder
{
    private enum RunningState
    {
        Stopped = 0,
        WaitingInterval,
        WaitingFetchRequest,
        FetchRequestAccepted,
    }

    private List<NewsList.Item> NewsItems { get; set; } = [];
    private List<ModChatLog> ChatItems { get; set; } = [];
    private int? Seed { get; set; }

    private CancellationTokenSource Cancellation { get; } = new();

    private RunningState State { get; set; } = RunningState.Stopped;
    private bool IsNewsReady { get; set; } = false;
    public bool IsRunning => State != RunningState.Stopped;

    private static ModWorldConfig Config => ModContext.WorldConfig;

    public List<string> GetRandomNews()
    {
        if (!IsNewsReady)
        {
            return [];
        }

        return [
             .. PopRandomNewItems().Select(i => i.content),
            .. ChatItems.Copy().Shuffle().Where(IsValidChat).Take(Config.Chat.MaxCount).Select(i => i.Msg),
        ];
    }

    private List<NewsList.Item> PopRandomNewItems()
    {
        var items = NewsItems.Copy().Shuffle();
        var newsItems = items.Take(Config.News.MaxCount).ToList();
        NewsItems = [.. items.Skip(newsItems.Count)];
        return newsItems;
    }

    private static bool IsValidChat(ModChatLog chat)
    {
        return chat.Cat switch
        {
            ChatCategory.Dead => ModContext.WorldConfig.Chat.FetchDead,
            ChatCategory.Wish => ModContext.WorldConfig.Chat.FetchWish,
            ChatCategory.Marriage => ModContext.WorldConfig.Chat.FetchMarriage,
            _ => false,
        };
    }

    public void InvalidateNews()
    {
        IsNewsReady = false;
    }

    public async void StartFetching()
    {
        var token = Cancellation.Token;
        State = RunningState.WaitingInterval;
        while (IsRunning)
        {
            // 特定の時間待機し、取得リクエストがあるまでさらに待機を続ける
            // (無操作時などにおいて、不必要にデータを取得しないようにするため)
            State = RunningState.WaitingInterval;
            await UniTask.Delay(Config.FrequencyMinute * 60 * 1000, cancellationToken: token);
            State = RunningState.WaitingFetchRequest;
            await UniTask.WaitUntil(() => State == RunningState.FetchRequestAccepted, cancellationToken: token);
            await FetchAsync(token);
        }
    }

    public async void StopFetching()
    {
        Cancellation.Cancel();
        State = RunningState.Stopped;
    }

    public void RequestFetch()
    {
        if (State != RunningState.WaitingFetchRequest)
        {
            return;
        }
        State = RunningState.FetchRequestAccepted;
    }

    private async UniTask FetchAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        IsNewsReady = false;
        token.ThrowIfCancellationRequested();
        NewsItems = FetchNews();
        token.ThrowIfCancellationRequested();
        ChatItems = await FetchChatAsync(token, Lang.langCode);
        IsNewsReady = true;
    }

    private List<NewsList.Item> FetchNews()
    {
        if (!Config.News.Enable)
        {
            return [];
        }

        var daySeed = ELayer.world.dayData.seed;
        if (Seed is not null && Seed == daySeed)
        {
            return NewsItems;
        }

        Seed = daySeed;
        return NewsList.GetNews(daySeed);
    }

    private async UniTask<List<ModChatLog>> FetchChatAsync(CancellationToken token, string idLang)
    {
        if (!Config.Chat.Enable)
        {
            return [];
        }

        string uri = $"{Net.urlChat}logs/all_{idLang}.json";
        using var request = UnityWebRequest.Get(uri);
        await request.SendWebRequest().ToUniTask(cancellationToken: token);
        if (request.result == UnityWebRequest.Result.ConnectionError
            || request.result == UnityWebRequest.Result.ProtocolError)
        {
            return [];
        }

        var chatList = JsonConvert.DeserializeObject<List<ModChatLog>>(request.downloadHandler.text);
        chatList.Reverse();
        foreach (var chat in chatList)
        {
            chat.Msg = chat.Msg.Replace("\n", "").Replace("\r", "").Replace("&quot;", "\"").ToTitleCase();
        }

        return chatList;
    }

    private record ModChatLog
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Name { get; set; } = "";
        [JsonProperty("msg", DefaultValueHandling = DefaultValueHandling.Include)]
        public string Msg { get; set; } = "";
        [JsonProperty("cat", DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(ModChatCategoryConverter))]
        public ChatCategory Cat { get; set; } = ChatCategory.Test;
    }

    private class ModChatCategoryConverter : JsonConverter<ChatCategory>
    {
        public override bool CanWrite => false;

        public override ChatCategory ReadJson(
            JsonReader reader,
            Type objectType,
            ChatCategory existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.Value is not string catString)
            {
                throw new JsonSerializationException($"Unexpected JSON format in ModChatCategoryConverter: {reader.Value}");
            }

            if (!Enum.TryParse<ChatCategory>(catString, out var cat))
            {
                throw new JsonSerializationException($"Unexpected ChatCategory format in ModChatCategoryConverter: {catString}");
            }
            return cat;
        }

        public override void WriteJson(JsonWriter writer, ChatCategory value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
