using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using KelimeEzberApp.Models;

namespace KelimeEzberApp.Services;

public class WordService : IWordService
{
    private readonly HttpClient _http;
    private readonly IGamificationService _gamification;
    private readonly IJSRuntime _js;

    private List<WordItem> _allWords = new();
    private static readonly Random _random = new();

    public event Action? OnWordsUpdated;

    public bool IsLoaded { get; private set; }
    public IReadOnlyList<WordItem> AllWords => _allWords;

    public WordService(HttpClient http, IGamificationService gamification, IJSRuntime js)
    {
        _http = http;
        _gamification = gamification;
        _js = js;
    }

    public async Task InitializeAsync()
    {
        if (IsLoaded) return;

        try
        {
            var data = await _http.GetFromJsonAsync<List<WordItem>>("data/words.json");
            if (data != null)
            {
                _allWords = data;
                SyncProgressWithUserProfile();
                IsLoaded = true;
                OnWordsUpdated?.Invoke();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WordService] Load error: {ex.Message}");
        }
    }

    private void SyncProgressWithUserProfile()
    {
        var progressMap = _gamification.Profile.WordProgress;
        foreach (var word in _allWords)
        {
            if (progressMap.TryGetValue(word.Id, out var userProg))
            {
                word.Status = userProg.Status;
                word.IsFavorite = userProg.IsFavorite;
                word.ReviewCount = userProg.ReviewCount;
                word.CorrectCount = userProg.CorrectCount;
                word.WrongCount = userProg.WrongCount;
                word.LastReviewedAt = userProg.LastReviewedAt;
            }
            else
            {
                word.Status = WordStatus.New;
                word.IsFavorite = false;
                word.ReviewCount = 0;
                word.CorrectCount = 0;
                word.WrongCount = 0;
                word.LastReviewedAt = null;
            }
        }
    }

    public List<WordItem> FilterWords(string? listId = null, int? unit = null, WordStatus? status = null, bool? onlyFavorites = null, string? search = null)
    {
        var query = _allWords.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(listId) && listId != "all")
        {
            query = query.Where(w => w.ListId.Equals(listId, StringComparison.OrdinalIgnoreCase));
        }

        if (unit.HasValue && unit.Value > 0)
        {
            query = query.Where(w => w.Unit == unit.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(w => w.Status == status.Value);
        }

        if (onlyFavorites == true)
        {
            query = query.Where(w => w.IsFavorite);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(w =>
                w.Word.ToLowerInvariant().Contains(term) ||
                w.Meaning.ToLowerInvariant().Contains(term) ||
                w.Pronunciation.ToLowerInvariant().Contains(term) ||
                w.Details.ToLowerInvariant().Contains(term));
        }

        return query.ToList();
    }

    public List<int> GetAvailableUnits(string listId)
    {
        var query = _allWords.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(listId) && listId != "all")
        {
            query = query.Where(w => w.ListId.Equals(listId, StringComparison.OrdinalIgnoreCase));
        }
        return query.Select(w => w.Unit).Distinct().OrderBy(u => u).ToList();
    }

    public async Task UpdateWordStatusAsync(WordItem word, WordStatus newStatus)
    {
        word.Status = newStatus;
        await _gamification.TrackWordReviewedAsync(word, newStatus);
        OnWordsUpdated?.Invoke();
    }

    public async Task ToggleFavoriteAsync(WordItem word)
    {
        word.IsFavorite = !word.IsFavorite;

        if (!_gamification.Profile.WordProgress.TryGetValue(word.Id, out var prog))
        {
            prog = new UserWordProgress();
            _gamification.Profile.WordProgress[word.Id] = prog;
        }
        prog.IsFavorite = word.IsFavorite;

        await _gamification.TrackFavoriteToggledAsync(word.IsFavorite);
        OnWordsUpdated?.Invoke();
    }

    public List<QuizQuestion> GenerateQuiz(string listId, int count = 10, int? unit = null, bool englishToTurkish = true)
    {
        var pool = FilterWords(listId: listId, unit: unit);
        if (pool.Count < 4)
        {
            pool = _allWords;
        }

        var selectedTargets = pool.OrderBy(_ => _random.Next()).Take(count).ToList();
        var questions = new List<QuizQuestion>();

        foreach (var target in selectedTargets)
        {
            string correctAnswer = englishToTurkish ? target.Meaning : target.Word;
            string questionText = englishToTurkish ? target.Word : target.Meaning;
            string prompt = englishToTurkish ? "kelimesinin Türkçe karşılığı nedir?" : "ifadesinin İngilizce karşılığı nedir?";

            // Select 3 distractors
            var distractors = pool
                .Where(w => w.Id != target.Id)
                .OrderBy(_ => _random.Next())
                .Select(w => englishToTurkish ? w.Meaning : w.Word)
                .Where(ans => !string.IsNullOrWhiteSpace(ans) && !ans.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .Take(3)
                .ToList();

            // If not enough distractors from pool, fill from all words
            if (distractors.Count < 3)
            {
                var extra = _allWords
                    .Where(w => w.Id != target.Id)
                    .OrderBy(_ => _random.Next())
                    .Select(w => englishToTurkish ? w.Meaning : w.Word)
                    .Where(ans => !distractors.Contains(ans) && !ans.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                    .Take(3 - distractors.Count);
                distractors.AddRange(extra);
            }

            var options = new List<string>(distractors) { correctAnswer };
            options = options.OrderBy(_ => _random.Next()).ToList();

            questions.Add(new QuizQuestion
            {
                TargetWord = target,
                QuestionText = questionText,
                QuestionPrompt = prompt,
                IsEnglishToTurkish = englishToTurkish,
                Options = options,
                CorrectAnswer = correctAnswer
            });
        }

        return questions;
    }

    public (List<WordItem> EnglishSide, List<WordItem> TurkishSide) GenerateMatchGame(string listId, int count = 5, int? unit = null)
    {
        var pool = FilterWords(listId: listId, unit: unit);
        if (pool.Count < count)
        {
            pool = _allWords;
        }

        var selected = pool.OrderBy(_ => _random.Next()).Take(count).ToList();
        var enList = selected.OrderBy(_ => _random.Next()).ToList();
        var trList = selected.OrderBy(_ => _random.Next()).ToList();

        return (enList, trList);
    }

    public async Task SpeakAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        try
        {
            await _js.InvokeVoidAsync("appJs.speak", text);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[WordService] Speech error: {ex.Message}");
        }
    }
}
