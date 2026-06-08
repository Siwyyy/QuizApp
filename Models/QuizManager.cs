using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuizApp.Models;

public static class QuizManager
{
    private static readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "saved_quizzes.json");
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = true };

    public static async Task<List<Quiz>> LoadQuizzesAsync()
    {
        if (!File.Exists(FilePath))
            return new List<Quiz>();

        try
        {
            var json = await File.ReadAllTextAsync(FilePath);
            return JsonSerializer.Deserialize<List<Quiz>>(json, Options) ?? new List<Quiz>();
        }
        catch
        {
            return new List<Quiz>();
        }
    }

    public static async Task SaveQuizzesAsync(List<Quiz> quizzes)
    {
        try
        {
            var json = JsonSerializer.Serialize(quizzes, Options);
            await File.WriteAllTextAsync(FilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving quizzes: {ex.Message}");
        }
    }

    public static async Task AddQuizAsync(Quiz quiz)
    {
        var quizzes = await LoadQuizzesAsync();
        quizzes.Add(quiz);
        await SaveQuizzesAsync(quizzes);
    }
    
    public static async Task UpdateQuizAsync(Quiz updatedQuiz)
    {
        var quizzes = await LoadQuizzesAsync();
        var index = quizzes.FindIndex(q => q.Id == updatedQuiz.Id);
        if (index != -1)
        {
            quizzes[index] = updatedQuiz;
            await SaveQuizzesAsync(quizzes);
        }
    }

    public static async Task DeleteQuizAsync(string quizId)
    {
        var quizzes = await LoadQuizzesAsync();
        quizzes.RemoveAll(q => q.Id == quizId);
        await SaveQuizzesAsync(quizzes);
    }
}
