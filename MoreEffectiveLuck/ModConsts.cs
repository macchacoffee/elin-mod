namespace MoreEffectiveLuck;

internal static class ModConsts
{
    internal static class SourceId
    {
        private const string _prefix = "mc_mel_";
        private static string Prefixed(string id) => $"{_prefix}{id}";

        public static readonly string DaBane = Prefixed("daBane");
        public static readonly string IsLuckyFood = Prefixed("isLuckyFood");
     }
}
