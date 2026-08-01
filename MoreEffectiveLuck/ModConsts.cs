namespace MoreEffectiveLuck;

public static class ModConsts
{
    public static class SourceId
    {
        private const string PREFIX = "mc_mel_";
        private static string Prefixed(string id) => $"{PREFIX}{id}";

        public static readonly string DaBane = Prefixed("daBane");
     }
}