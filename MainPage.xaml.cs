using System.Text.Json;
using QuizApp.Models;
using System.Globalization;

namespace QuizApp;

public class ArchiveTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isArchived)
            return isArchived ? "Przywróć" : "Archiwizuj";
        return "Archiwizuj";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshQuizList();
    }

    private async Task RefreshQuizList()
    {
        var quizzes = await QuizManager.LoadQuizzesAsync();
        var sorted = quizzes.OrderBy(q => q.IsArchived).ThenBy(q => q.Title).ToList();
        QuizzesCollectionView.ItemsSource = sorted;
    }

    private async void OnStartPastedClicked(object? sender, EventArgs e)
    {
        ErrorLabel.IsVisible = false;
        string jsonText = JsonEditor.Text;

        if (string.IsNullOrWhiteSpace(jsonText))
        {
            ShowError("Proszę wkleić kod JSON przed dodaniem.");
            return;
        }

        await LoadAndSaveQuizFromJsonAsync(jsonText);
        JsonEditor.Text = string.Empty;
    }

    private async void OnLoadFileClicked(object? sender, EventArgs e)
    {
        try
        {
            ErrorLabel.IsVisible = false;
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Wybierz plik z quizem (.json)"
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);
                string jsonText = await reader.ReadToEndAsync();
                await LoadAndSaveQuizFromJsonAsync(jsonText);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Błąd podczas wczytywania pliku: {ex.Message}");
        }
    }

    private async Task LoadAndSaveQuizFromJsonAsync(string jsonText)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var quiz = JsonSerializer.Deserialize<Quiz>(jsonText, options);

            if (quiz == null || quiz.Questions == null || quiz.Questions.Count == 0)
            {
                ShowError("Wczytany plik JSON nie zawiera poprawnych pytań.");
                return;
            }

            if (string.IsNullOrEmpty(quiz.Id)) quiz.Id = Guid.NewGuid().ToString();

            await QuizManager.AddQuizAsync(quiz);
            await RefreshQuizList();
            
            await DisplayAlert("Sukces", "Quiz został pomyślnie dodany do Twojej listy!", "OK");
        }
        catch (JsonException ex)
        {
            ShowError($"Nieprawidłowy format JSON. Upewnij się, że użyłeś podanego wzoru.\nSzczegóły: {ex.Message}");
        }
        catch (Exception ex)
        {
            ShowError($"Wystąpił nieoczekiwany błąd: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.IsVisible = true;
    }

    private async void OnRunQuizClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Quiz quiz)
        {
            await Navigation.PushAsync(new QuizPage(quiz));
        }
    }

    private async void OnArchiveToggleClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Quiz quiz)
        {
            quiz.IsArchived = !quiz.IsArchived;
            await QuizManager.UpdateQuizAsync(quiz);
            await RefreshQuizList();
        }
    }

    private async void OnDeleteQuizClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Quiz quiz)
        {
            bool answer = await DisplayAlert("Potwierdzenie", $"Czy na pewno chcesz usunąć quiz '{quiz.Title}'?", "Tak", "Nie");
            if (answer)
            {
                await QuizManager.DeleteQuizAsync(quiz.Id);
                await RefreshQuizList();
            }
        }
    }
}
