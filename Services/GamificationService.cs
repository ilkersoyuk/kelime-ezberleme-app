using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using KelimeEzberApp.Models;

namespace KelimeEzberApp.Services;

public class GamificationService : IGamificationService
{
    private readonly IStorageService _storage;
    private readonly IJSRuntime _js;

    public event Action? OnStateChanged;
    public event Action<int, string>? OnLevelUp;

    public UserProfile Profile { get; private set; } = new();

    public GamificationService(IStorageService storage, IJSRuntime js)
    {
        _storage = storage;
        _js = js;
    }

    public async Task InitializeAsync()
    {
        Profile = await _storage.LoadProfileAsync();
        CheckBadges();
        OnStateChanged?.Invoke();
    }

    // Level formula: XP required to reach Level L+1 from Level L is (L * 150)
    // Total XP threshold for Level L is Sum(i * 150 for i=1..(L-1)) = 75 * (L-1) * L
    private static int TotalXpForLevel(int lvl)
    {
        if (lvl <= 1) return 0;
        return 75 * (lvl - 1) * lvl;
    }

    public int GetXpRequiredForNextLevel()
    {
        return Profile.Level * 150;
    }

    public int GetCurrentLevelXp()
    {
        var startOfLevel = TotalXpForLevel(Profile.Level);
        return Math.Max(0, Profile.Xp - startOfLevel);
    }

    public double GetLevelProgressPercentage()
    {
        var needed = GetXpRequiredForNextLevel();
        var current = GetCurrentLevelXp();
        if (needed <= 0) return 100;
        return Math.Min(100.0, Math.Max(0.0, (double)current / needed * 100.0));
    }

    public async Task AddXpAsync(int xp, string reason)
    {
        if (xp <= 0) return;

        Profile.Xp += xp;
        var oldLevel = Profile.Level;

        // Calculate new level
        while (Profile.Xp >= TotalXpForLevel(Profile.Level + 1))
        {
            Profile.Level++;
        }

        var leveledUp = Profile.Level > oldLevel;
        CheckBadges();

        await _storage.SaveProfileAsync(Profile);
        OnStateChanged?.Invoke();

        if (leveledUp)
        {
            var rank = UserProfile.GetRankTitle(Profile.Level);
            try
            {
                await _js.InvokeVoidAsync("appJs.triggerLevelUpVibration");
            }
            catch { }
            OnLevelUp?.Invoke(Profile.Level, rank);
        }
    }

    public List<DailyMission> GetDailyMissions()
    {
        var list = new List<DailyMission>
        {
            new DailyMission
            {
                Id = "mission_review_15",
                Title = "Hafıza Tazeleme",
                Description = "Bugün en az 15 kelime kartı incele",
                Icon = "🗂️",
                Target = 15,
                Current = Math.Min(15, Profile.WordsReviewedToday),
                XpReward = 35,
                IsClaimed = Profile.ClaimedDailyMissions.Contains("mission_review_15")
            },
            new DailyMission
            {
                Id = "mission_quiz_10",
                Title = "Quiz Şampiyonu",
                Description = "Quiz testinde 10 doğru cevap ver",
                Icon = "🎯",
                Target = 10,
                Current = Math.Min(10, Profile.CorrectAnswersToday),
                XpReward = 50,
                IsClaimed = Profile.ClaimedDailyMissions.Contains("mission_quiz_10")
            },
            new DailyMission
            {
                Id = "mission_master_5",
                Title = "Kelime Fatihi",
                Description = "Bugün 5 kelimeyi 'Ezberledim' durumuna getir",
                Icon = "✅",
                Target = 5,
                Current = Math.Min(5, Profile.WordProgress.Values.Count(v => v.Status == WordStatus.Mastered && v.LastReviewedAt?.Date == DateTime.Today)),
                XpReward = 60,
                IsClaimed = Profile.ClaimedDailyMissions.Contains("mission_master_5")
            },
            new DailyMission
            {
                Id = "mission_fav_3",
                Title = "Yıldızlı Kelimeler",
                Description = "3 zorlandığın kelimeyi yıldızla (favoriye al)",
                Icon = "⭐",
                Target = 3,
                Current = Math.Min(3, Profile.WordProgress.Values.Count(v => v.IsFavorite)),
                XpReward = 25,
                IsClaimed = Profile.ClaimedDailyMissions.Contains("mission_fav_3")
            }
        };

        return list;
    }

    public async Task ClaimMissionRewardAsync(string missionId)
    {
        if (Profile.ClaimedDailyMissions.Contains(missionId)) return;

        var missions = GetDailyMissions();
        var m = missions.FirstOrDefault(x => x.Id == missionId);
        if (m != null && m.IsCompleted)
        {
            Profile.ClaimedDailyMissions.Add(missionId);
            await AddXpAsync(m.XpReward, $"Görev Tamamlandı: {m.Title}");
        }
    }

    public List<AchievementBadge> GetAllBadges()
    {
        var badges = new List<AchievementBadge>
        {
            new() { Id = "badge_first_step", Title = "İlk Adım 👣", Description = "İlk kelimeni çalıştın", RequiredValue = 1, Icon = "🌱" },
            new() { Id = "badge_words_25", Title = "Hafıza Çırağı 🧠", Description = "25 kelimeyi gözden geçirdin", RequiredValue = 25, Icon = "💡" },
            new() { Id = "badge_master_50", Title = "Kelime Avcısı 🏹", Description = "50 kelimeyi başarıyla ezberledin", RequiredValue = 50, Icon = "🎯" },
            new() { Id = "badge_master_100", Title = "YDS Ustası 📜", Description = "100 kelimeyi hafızana kazıdın", RequiredValue = 100, Icon = "🎖️" },
            new() { Id = "badge_master_300", Title = "Sözlük Bilgini 📚", Description = "300 kelimeyi tamamladın", RequiredValue = 300, Icon = "🏛️" },
            new() { Id = "badge_streak_3", Title = "İstikrarlı Zihin 🔥", Description = "3 günlük çalışma serisine ulaştın", RequiredValue = 3, Icon = "🔥" },
            new() { Id = "badge_streak_7", Title = "Haftalık Maraton ⚡", Description = "7 günlük aralıksız seri yakaladın", RequiredValue = 7, Icon = "⚡" },
            new() { Id = "badge_quiz_50", Title = "Test Kurdu 📝", Description = "50 quiz sorusu çözdün", RequiredValue = 50, Icon = "📋" },
            new() { Id = "badge_level_5", Title = "YDS Savaşçısı ⚔️", Description = "Seviye 5'e ulaştın", RequiredValue = 5, Icon = "⚔️" },
            new() { Id = "badge_level_10", Title = "Kelime Üstadı 👑", Description = "Seviye 10'a ulaştın", RequiredValue = 10, Icon = "👑" },
        };

        foreach (var b in badges)
        {
            b.IsUnlocked = Profile.UnlockedBadges.Contains(b.Id);
        }

        return badges;
    }

    public async Task TrackWordReviewedAsync(WordItem word, WordStatus newStatus)
    {
        Profile.WordsReviewedToday++;
        Profile.TotalReviews++;

        if (!Profile.WordProgress.TryGetValue(word.Id, out var prog))
        {
            prog = new UserWordProgress();
            Profile.WordProgress[word.Id] = prog;
        }

        prog.Status = newStatus;
        prog.ReviewCount++;
        prog.LastReviewedAt = DateTime.Now;

        int xpReward = newStatus switch
        {
            WordStatus.Mastered => 15,
            WordStatus.Learning => 8,
            WordStatus.Review => 4,
            _ => 2
        };

        await AddXpAsync(xpReward, "Kelime Kartı Çalışması");
    }

    public async Task TrackQuizAnswerAsync(bool isCorrect)
    {
        Profile.QuizzesSolvedToday++;
        Profile.TotalQuizzesSolved++;
        if (isCorrect)
        {
            Profile.CorrectAnswersToday++;
            Profile.TotalCorrectAnswers++;
            await AddXpAsync(12, "Doğru Quiz Cevabı");
        }
        else
        {
            await AddXpAsync(2, "Quiz Pratiği");
        }
    }

    public async Task TrackFavoriteToggledAsync(bool isFavorite)
    {
        if (isFavorite)
        {
            await AddXpAsync(5, "Kelimeyi Favorilere Ekleme");
        }
        else
        {
            await _storage.SaveProfileAsync(Profile);
            OnStateChanged?.Invoke();
        }
    }

    private void CheckBadges()
    {
        var masteredCount = Profile.WordProgress.Values.Count(p => p.Status == WordStatus.Mastered);
        var totalReviewed = Profile.TotalReviews;

        void Unlock(string id, bool cond)
        {
            if (cond && !Profile.UnlockedBadges.Contains(id))
            {
                Profile.UnlockedBadges.Add(id);
            }
        }

        Unlock("badge_first_step", totalReviewed >= 1);
        Unlock("badge_words_25", totalReviewed >= 25);
        Unlock("badge_master_50", masteredCount >= 50);
        Unlock("badge_master_100", masteredCount >= 100);
        Unlock("badge_master_300", masteredCount >= 300);
        Unlock("badge_streak_3", Profile.StreakDays >= 3);
        Unlock("badge_streak_7", Profile.StreakDays >= 7);
        Unlock("badge_quiz_50", Profile.TotalQuizzesSolved >= 50);
        Unlock("badge_level_5", Profile.Level >= 5);
        Unlock("badge_level_10", Profile.Level >= 10);
    }
}
