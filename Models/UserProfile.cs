using System;
using System.Collections.Generic;

namespace KelimeEzberApp.Models;

public class UserProfile
{
    public int Xp { get; set; } = 0;
    public int Level { get; set; } = 1;
    public int StreakDays { get; set; } = 1;
    public DateTime? LastActiveDate { get; set; } = DateTime.Today;

    // Daily Counters
    public string DailyTrackerDate { get; set; } = DateTime.Today.ToString("yyyy-MM-dd");
    public int WordsReviewedToday { get; set; } = 0;
    public int QuizzesSolvedToday { get; set; } = 0;
    public int CorrectAnswersToday { get; set; } = 0;

    // Lifetime Stats
    public int TotalReviews { get; set; } = 0;
    public int TotalQuizzesSolved { get; set; } = 0;
    public int TotalCorrectAnswers { get; set; } = 0;

    // Unlocked Badges (Badge IDs)
    public List<string> UnlockedBadges { get; set; } = new();

    // Word Progress Map (Word Id -> Progress)
    public Dictionary<int, UserWordProgress> WordProgress { get; set; } = new();

    // Claimed daily missions for today
    public List<string> ClaimedDailyMissions { get; set; } = new();

    public static string GetRankTitle(int level)
    {
        return level switch
        {
            1 => "Kelime Çırağı 🐣",
            2 => "Kelime Kaşifi 🧭",
            3 => "Hafıza Yolcusu 🎒",
            4 => "Kelime Avcısı 🏹",
            5 => "YDS Savaşçısı ⚔️",
            6 => "Dil Meraklısı 🔍",
            7 => "Kavram Ustası 🧠",
            8 => "Kelime Koleksiyoncusu 💎",
            9 => "Hafıza Şampiyonu 🏆",
            10 => "Kelime Üstadı 🌟",
            11 => "Sözlük Fatihi 🏰",
            12 => "YDS Uzmanı 📜",
            13 => "Kelime Sihirbazı 🧙",
            14 => "Akıcı Zihin ⚡",
            15 => "Kelime Virtüözü 🎻",
            16 => "Dil Bilgini 📚",
            17 => "Sözcük Mimarı 🏛️",
            18 => "YDS Hakimi ⚖️",
            19 => "Kelimelerin Efendisi 👑",
            >= 20 => "YDS Efsanesi 🔥",
            _ => "Öğrenci 📖"
        };
    }
}
