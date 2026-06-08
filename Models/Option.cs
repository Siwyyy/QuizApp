using System.Text.Json.Serialization;

namespace QuizApp.Models;

public class Option
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("isCorrect")]
    public bool IsCorrect { get; set; }

    // Pole używane tylko w interfejsie użytkownika do śledzenia wyboru
    [JsonIgnore]
    public bool IsSelected { get; set; }
}
