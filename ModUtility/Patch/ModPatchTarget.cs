using System.Reflection;

namespace ModUtility.Patch;

/// <summary>Harmonyパッチの対象設定</summary>
/// <param name="MinVersion">サポートするゲーム本体の最小バージョン</param>
/// <param name="MaxVersion">サポートするゲーム本体の最大バージョン</param>
public record class ModPatchTarget(Version? MinVersion = null, Version? MaxVersion = null)
{
    /// <summary>パッチ可能であるかをチェックする</summary>
    /// <param name="targetMethod">パッチ対象のメソッド</param>
    /// <returns>パッチ可能であればtrue</returns>
    public bool IsPatchable(MethodBase? targetMethod)
    {
        if (targetMethod is not null)
        {
            return true;
        }

        var isPatchable = true;
        if (isPatchable && MinVersion is Version minVersion)
        {
            isPatchable = !EClass.core.version.IsBelow(minVersion);
        }
        if (isPatchable && MaxVersion is Version maxVersion)
        {
            isPatchable = EClass.core.version.IsSameOrBelow(maxVersion);
        }

        return isPatchable;
    }
}
