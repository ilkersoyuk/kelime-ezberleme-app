using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using KelimeEzberApp.Models;

namespace KelimeEzberApp.Services;

public class StorageService : IStorageService
{
    private readonly IJSRuntime _js;
    private const string StorageKey = "kelime_ezber_user_profile_v1";

    public StorageService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task<UserProfile> LoadProfileAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var profile = JsonSerializer.Deserialize<UserProfile>(json);
                if (profile != null)
                {
                    CheckDailyReset(profile);
                    return profile;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StorageService] Load error: {ex.Message}");
        }

        var newProfile = new UserProfile();
        CheckDailyReset(newProfile);
        return newProfile;
    }

    public async Task SaveProfileAsync(UserProfile profile)
    {
        try
        {
            var json = JsonSerializer.Serialize(profile);
            await _js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StorageService] Save error: {ex.Message}");
        }
    }

    public async Task ExportBackupAsync(UserProfile profile)
    {
        var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
        var filename = $"kelime-ezber-yedek-{DateTime.Now:yyyyMMdd-HHmm}.json";
        await _js.InvokeVoidAsync("appJs.downloadFile", filename, json);
    }

    public async Task<UserProfile?> ImportBackupAsync(string jsonContent)
    {
        try
        {
            var profile = JsonSerializer.Deserialize<UserProfile>(jsonContent);
            if (profile != null)
            {
                CheckDailyReset(profile);
                await SaveProfileAsync(profile);
                return profile;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StorageService] Import error: {ex.Message}");
        }
        return null;
    }

    public async Task ResetDataAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StorageService] Reset error: {ex.Message}");
        }
    }

    private static void CheckDailyReset(UserProfile profile)
    {
        var todayStr = DateTime.Today.ToString("yyyy-MM-dd");
        if (profile.DailyTrackerDate != todayStr)
        {
            // Calculate streak
            if (profile.LastActiveDate.HasValue)
            {
                var daysDiff = (DateTime.Today - profile.LastActiveDate.Value.Date).TotalDays;
                if (daysDiff == 1)
                {
                    profile.StreakDays++;
                }
                else if (daysDiff > 1)
                {
                    profile.StreakDays = 1;
                }
            }
            else
            {
                profile.StreakDays = 1;
            }

            profile.DailyTrackerDate = todayStr;
            profile.LastActiveDate = DateTime.Today;
            profile.WordsReviewedToday = 0;
            profile.QuizzesSolvedToday = 0;
            profile.CorrectAnswersToday = 0;
            profile.ClaimedDailyMissions.Clear();
        }
    }
}
