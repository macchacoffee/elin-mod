using BepInEx.Configuration;
using ModUtility.Extensions;

namespace ModUtility.Config;

internal abstract class BepInExModConfigBase<T> where T : BepInExModConfigBase<T>
{
    public void Bind(ConfigFile configFile)
    {
        var type = GetType();
        foreach (var propInfo in type.GetProperties())
        {
            if (!typeof(IBepInExModConfigEntryBase).IsAssignableFrom(propInfo.PropertyType))
            {
                continue;
            }
            propInfo.GetGetter<T, IBepInExModConfigEntryBase>()((T)this).Bind(configFile);
        }
    }
}

internal interface IBepInExModConfigEntryBase
{
    public void Bind(ConfigFile configFile);
}

internal class BepInExModConfigEntry<T>(string section, string key, T defaultValue, string? description = null, AcceptableValueBase? acceptableValue = null, params object[] tags) : IBepInExModConfigEntryBase
{
    private string Section { get; } = section;
    private string Key { get; } = key;
    private T DefaultValue { get; } = defaultValue;
    private string? Description { get; } = description;
    private AcceptableValueBase? AcceptableValue { get; } = acceptableValue;
    private object[] Tags { get; } = tags;

    private ConfigEntry<T>? Entry { get; set; }
    public T Value
    {
        get => Entry!.Value;
        set => Entry!.Value = value;
    }

    public void Bind(ConfigFile configFile)
    {
        var configDescription = Description is not null ? new ConfigDescription(Description, AcceptableValue, Tags) : null;
        Entry = configFile.Bind(Section, Key, DefaultValue, configDescription);
    }
}
