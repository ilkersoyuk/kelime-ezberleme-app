using System;

namespace KelimeEzberApp.Models;

public class AchievementBadge
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "🏅";
    public int RequiredValue { get; set; }
    public string Category { get; set; } = "general";
    public bool IsUnlocked { get; set; }
}
