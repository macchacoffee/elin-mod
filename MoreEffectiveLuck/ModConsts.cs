namespace MoreEffectiveLuck;

internal static class ModConsts
{
    internal static class SourceId
    {
        private const string Prefix = "mc_mel_";
        private static string Prefixed(string id) => $"{Prefix}{id}";

        public static readonly string DaBane = Prefixed("daBane");
        public static readonly string IsLuckyFood = Prefixed("isLuckyFood");
     }
}
