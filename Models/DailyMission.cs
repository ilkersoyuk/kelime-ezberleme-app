namespace KelimeEzberApp.Models;

public class DailyMission
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "🎯";
    public int Target { get; set; }
    public int Current { get; set; }
    public int XpReward { get; set; }
    public bool IsCompleted => Current >= Target;
    public bool IsClaimed { get; set; }
}
