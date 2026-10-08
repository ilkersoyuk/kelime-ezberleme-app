using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KelimeEzberApp.Models;

namespace KelimeEzberApp.Services;

public interface IWordService
{
    event Action? OnWordsUpdated;

    bool IsLoaded { get; }
    IReadOnlyList<WordItem> AllWords { get; }

    Task InitializeAsync();
    List<WordItem> FilterWords(string? listId = null, int? unit = null, WordStatus? status = null, bool? onlyFavorites = null, string? search = null);
    List<int> GetAvailableUnits(string listId);
    
    Task UpdateWordStatusAsync(WordItem word, WordStatus newStatus);
    Task ToggleFavoriteAsync(WordItem word);
    
    List<QuizQuestion> GenerateQuiz(string listId, int count = 10, int? unit = null, bool englishToTurkish = true);
    (List<WordItem> EnglishSide, List<WordItem> TurkishSide) GenerateMatchGame(string listId, int count = 5, int? unit = null);
    
    Task SpeakAsync(string text);
}
