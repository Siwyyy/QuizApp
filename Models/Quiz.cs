using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizApp.Models;

public class Quiz
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("questions")]
    public List<Question> Questions { get; set; } = new();

    [JsonPropertyName("isArchived")]
    public bool IsArchived { get; set; }
}
