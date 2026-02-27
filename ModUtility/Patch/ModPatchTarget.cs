using System.Reflection;

namespace ModUtility.Patch;

public record class ModPatchTarget(Version? MinVersion = null, Version? MaxVersion = null)
{
    public bool IsPatchable(MethodBase? targetMethod)
    {
        if (targetMethod != null)
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
