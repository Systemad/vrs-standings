namespace Features.Rankings.Models;

public class Details
{
    public string Key { get; set; }
    public string Filename { get; set; }
}

    
public class TeamStanding
{
    public string Standings { get; set; }
    public string Points { get; set; }
    public string TeamName { get; set; }
    public string[] Roster { get; set; }
    public Details Details { get; set; }
}