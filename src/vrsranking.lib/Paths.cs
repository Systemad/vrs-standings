namespace vrsranking.lib;

public static class Paths
{
    public static string WorkRoot = "C:\\Users\\yeahg\\Documents\\webdev\\wheteverstest\\workdir";
    public static string ExportedRoot = $"{WorkRoot}/exported";
    public static string ReposRoot = $"{WorkRoot}/repos";

    public static string ExportedRules = $"{ExportedRoot}/counter-strike_rules_and_regs";
    //public static string ExportedRankings = $"{ExportedRoot}/counter-strike_regional_standings";

    public static string RulesRepo = $"{ReposRoot}/counter-strike_rules_and_regs";
    public static string RankingsRepo = $"{ReposRoot}/counter-strike_regional_standings";

    public static class Rankings
    {
        public static string Root = $"{WorkRoot}/counter-strike_regional_standings";
        public static string Y2025 = $"{WorkRoot}/counter-strike_regional_standings/live/2025";
        public static string Y2025Details = $"{WorkRoot}/counter-strike_regional_standings/live/2025/details";
    }
}