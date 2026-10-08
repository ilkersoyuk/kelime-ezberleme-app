using System.Collections.Generic;

namespace KelimeEzberApp.Models;

public class QuizQuestion
{
    public WordItem TargetWord { get; set; } = new();
    public string QuestionText { get; set; } = "";
    public string QuestionPrompt { get; set; } = "";
    public bool IsEnglishToTurkish { get; set; } = true;
    public List<string> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = "";
    public string? SelectedAnswer { get; set; }
    public bool? IsAnswered => SelectedAnswer != null;
    public bool IsCorrect => SelectedAnswer == CorrectAnswer;
}
