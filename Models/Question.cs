using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace QuizApp.Models;

public class Question
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("options")]
    public List<Option> Options { get; set; } = new();

    [JsonPropertyName("explanation")]
    public string Explanation { get; set; }

    [JsonIgnore]
    public bool IsMultipleChoice => Options.Count(o => o.IsCorrect) > 1;
}
