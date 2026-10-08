using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KelimeEzberApp.Models;

namespace KelimeEzberApp.Services;

public interface IGamificationService
{
    event Action? OnStateChanged;
    event Action<int, string>? OnLevelUp;

    UserProfile Profile { get; }
    Task InitializeAsync();
    Task AddXpAsync(int xp, string reason);
    int GetXpRequiredForNextLevel();
    int GetCurrentLevelXp();
    double GetLevelProgressPercentage();
    
    List<DailyMission> GetDailyMissions();
    Task ClaimMissionRewardAsync(string missionId);

    List<AchievementBadge> GetAllBadges();
    Task TrackWordReviewedAsync(WordItem word, WordStatus newStatus);
    Task TrackQuizAnswerAsync(bool isCorrect);
    Task TrackFavoriteToggledAsync(bool isFavorite);
}
