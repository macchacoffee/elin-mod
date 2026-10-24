namespace Macchacoffee.ElinMods.AdditionalRacesAndJobs;

internal static class ModConsts
{
    public static class SourceNumericId
    {
        private const int _base = 395830000;
        private static int Id(int id) => _base + id;

        public static readonly int FeatMonk = Id(1);
    }

    public static class Value
    {
        public static readonly int MonkMartialPotential = 50;
        public static readonly int MonkNonMartialPotential = -20;
        public static readonly float MonkMartialAttackMultiple = 0.1f;
        public static readonly float MonkNonMartialAttackMultiple = -0.15f;
    }
}
