using System.Linq;
using System.Collections.Generic;
using System.Threading;
using AddPalmiaTimesNewsToLog.Config;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace AddPalmiaTimesNewsToLog.News;

public class ModNewsFeeder
{
    private List<NewsList.Item> NewsItems { get; set; } = [];
    private List<ModChatLog> ChatItems { get; set; } = [];

    private CancellationTokenSource Cancellation { get; }= new();

    public bool IsRunning { get; private set; } = false;
    public bool IsNewsReady { get; set; } = false;

    private static ModConfig Config => Mod.Config;

    public List<string> GetRandomNews()
    {
        if (!IsNewsReady)
        {
            return [];
        }
       
        return [
            .. NewsItems.Copy().Shuffle().Take(Config.News.MaxCount).Select(i => i.content),
            .. ChatItems.Copy().Shuffle().Where(IsValidChat).Take(Config.Chat.MaxCount).Select(i => i.Msg),
        ];
    }

    private static bool IsValidChat(ModChatLog chat)
    {
        return chat.Cat switch
        {
            ChatCategory.Test => false,
            ChatCategory.Dead => Mod.Config.Chat.FetchDead,
            ChatCategory.Wish => Mod.Config.Chat.FetchWish,
            ChatCategory.Marriage => Mod.Config.Chat.FetchMarriage,
            _ => false,
        };
    }

    public async void StartFetching()
    {
        IsRunning = true;
        var token = Cancellation.Token;
        while (IsRunning)
        {
            await UniTask.Delay(Config.FrequencyMinute * 60 * 1000, cancellationToken: token);
            await FetchAsync(token);
        }
    }

    public async void StopFetching()
    {
        Cancellation.Cancel();
        IsRunning = false;
    }

    private async UniTask FetchAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        IsNewsReady = false;
        token.ThrowIfCancellationRequested();
        NewsItems = NewsList.GetNews(ELayer.world.dayData.seed);
        token.ThrowIfCancellationRequested();
        ChatItems = await FetchChatAsync(Lang.langCode);
        IsNewsReady = true;
    }

    private async UniTask<List<ModChatLog>> FetchChatAsync(string idLang)
    {
        string uri = $"{Net.urlChat}logs/all_{idLang}.json";
        using var request = UnityWebRequest.Get(uri);
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
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

    public class ModChatCategoryConverter : JsonConverter<ChatCategory>
    {
        public override bool CanWrite => false;

        public override ChatCategory ReadJson(JsonReader reader, Type objectType, ChatCategory existingValue, bool hasExistingValue, JsonSerializer serializer)
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
