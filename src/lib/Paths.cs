namespace lib;

public static class Paths
{
    
    public static string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public static string WorkRoot = Path.Combine(documentsPath, "webdev", "workdir");

    // FOR deployment
    //public static string WorkRoot = "/workdir";
    
    public static string ExportedRoot = Path.Combine(WorkRoot, "exported");
    public static string ReposRoot = Path.Combine(WorkRoot, "repos");

    public static string RankingsRepo = Path.Combine(ReposRoot, "counter-strike_regional_standings");

    public static class Rankings
    {
        public static string Root = Path.Combine(WorkRoot, "counter-strike_regional_standings");
        public static string Y2025 = Path.Combine(WorkRoot, "counter-strike_regional_standings", "live", "2025");
        public static string Y2025Details = Path.Combine(RankingsRepo, "live", "2025", "details");
    }
}