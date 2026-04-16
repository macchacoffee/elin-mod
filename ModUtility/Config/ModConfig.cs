using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ModUtility.Config;

public abstract class ModConfigBase<T> where T : ModConfigBase<T>
{
    public string Serialize()
    {
        return JsonConvert.SerializeObject(this, GameIO.formatting, GameIO.jsWriteGame);
    }

    public static T Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, GameIO.jsReadGame);
    }

    public T DeepCopy()
    {
        return Deserialize(Serialize());
    }
} 

public class ModColorConverter : JsonConverter<Color?>
{
    public override bool CanWrite => true;

    public override Color? ReadJson(JsonReader reader, Type objectType, Color? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.Value is null && Nullable.GetUnderlyingType(objectType) is not null)
        {
            return null;
        }
        if (reader.Value is not string colorString)
        {
            throw new JsonSerializationException($"Unexpected JSON format in ColorConverter: {reader.Value}");
        }
        if (!ColorUtility.TryParseHtmlString(colorString, out var color))
        {
            throw new JsonSerializationException($"Unexpected color format in ColorConverter: {colorString}");
        }
        return color;
    }

    public override void WriteJson(JsonWriter writer, Color? value, JsonSerializer serializer)
    {
        writer.WriteValue(value is Color color ? $"#{ColorUtility.ToHtmlStringRGBA(color)}" : null);
    }
}
