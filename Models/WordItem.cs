using System;
using System.Text.Json.Serialization;

namespace KelimeEzberApp.Models;

public enum WordStatus
{
    New = 0,
    Learning = 1,
    Mastered = 2,
    Review = 3
}

public class WordItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("listId")]
    public string ListId { get; set; } = "zafer1124";

    [JsonPropertyName("listName")]
    public string ListName { get; set; } = "";

    [JsonPropertyName("originalNumber")]
    public int OriginalNumber { get; set; }

    [JsonPropertyName("word")]
    public string Word { get; set; } = "";

    [JsonPropertyName("pronunciation")]
    public string Pronunciation { get; set; } = "";

    [JsonPropertyName("meaning")]
    public string Meaning { get; set; } = "";

    [JsonPropertyName("details")]
    public string Details { get; set; } = "";

    [JsonPropertyName("unit")]
    public int Unit { get; set; } = 1;

    // User Progress Fields (merged from local storage)
    public WordStatus Status { get; set; } = WordStatus.New;
    public bool IsFavorite { get; set; }
    public int ReviewCount { get; set; }
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public DateTime? LastReviewedAt { get; set; }
}

public class UserWordProgress
{
    public WordStatus Status { get; set; } = WordStatus.New;
    public bool IsFavorite { get; set; }
    public int ReviewCount { get; set; }
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public DateTime? LastReviewedAt { get; set; }
}
