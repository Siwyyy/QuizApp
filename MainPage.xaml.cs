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

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width > 0 && width < 900)
        {
            // Tryb pionowy (wąskie okno lub telefon)
            MainLayoutGrid.ColumnDefinitions.Clear();
            MainLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            MainLayoutGrid.RowDefinitions.Clear();
            MainLayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            MainLayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            Grid.SetColumn(LeftColumnPanel, 0);
            Grid.SetRow(LeftColumnPanel, 0);

            Grid.SetColumn(RightColumnPanel, 0);
            Grid.SetRow(RightColumnPanel, 1);

            Grid.SetColumnSpan(ToastNotification, 1);
        }
        else if (width >= 900)
        {
            // Tryb poziomy (szerokie okno)
            MainLayoutGrid.ColumnDefinitions.Clear();
            MainLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            MainLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            MainLayoutGrid.RowDefinitions.Clear();
            MainLayoutGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            Grid.SetColumn(LeftColumnPanel, 0);
            Grid.SetRow(LeftColumnPanel, 0);

            Grid.SetColumn(RightColumnPanel, 1);
            Grid.SetRow(RightColumnPanel, 0);

            Grid.SetColumnSpan(ToastNotification, 2);
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshQuizList();
    }

    private async Task RefreshQuizList()
    {
        var quizzes = await QuizManager.LoadQuizzesAsync();
        
        var activeQuizzes = quizzes.Where(q => !q.IsArchived).OrderBy(q => q.Title).ToList();
        var archivedQuizzes = quizzes.Where(q => q.IsArchived).OrderBy(q => q.Title).ToList();
        
        QuizzesCollectionView.ItemsSource = activeQuizzes;
        ArchivedQuizzesCollectionView.ItemsSource = archivedQuizzes;
        
        if (archivedQuizzes.Any())
        {
            ToggleArchivedBtn.IsVisible = true;
            ToggleArchivedBtn.Text = ArchivedQuizzesCollectionView.IsVisible 
                ? $"Ukryj zarchiwizowane quizy ({archivedQuizzes.Count})" 
                : $"Pokaż zarchiwizowane quizy ({archivedQuizzes.Count})";
        }
        else
        {
            ToggleArchivedBtn.IsVisible = false;
            ArchivedQuizzesCollectionView.IsVisible = false;
        }
    }

    private void OnToggleArchivedClicked(object sender, EventArgs e)
    {
        ArchivedQuizzesCollectionView.IsVisible = !ArchivedQuizzesCollectionView.IsVisible;
        
        var archivedQuizzes = ArchivedQuizzesCollectionView.ItemsSource as List<Quiz>;
        int count = archivedQuizzes?.Count ?? 0;
        
        ToggleArchivedBtn.Text = ArchivedQuizzesCollectionView.IsVisible 
            ? $"Ukryj zarchiwizowane quizy ({count})" 
            : $"Pokaż zarchiwizowane quizy ({count})";
    }

    private async void OnCopyPromptClicked(object sender, EventArgs e)
    {
        string prompt = @"Pamiętaj, aby Twoja odpowiedź składała się WYŁĄCZNIE z kodu w pliku w formacie JSON zgodnego z poniższym formatem (bez bloków markdown np. ```json).
Każde pytanie ma dowolną ilość odpowiedzi, dowolną ilość poprawnych odpowiedzi, oraz może, ale nie musi zawierać wyjaśnienia (explanation).
Jeśli quiz generowany jest z pytań podanych przez użytkownika z pliku, masz NIE modyfikować pytań ani odpowiedzi, a pytania, które nie są a,b,c,d po prostu pominąć.
Jeśli wśród pytań są takie, które nie mają zaznaczonej poprawnej odpowiedzi, pomiń je.
Na koniec wypisz użytkownikowi pytania, które pominąłeś.

WZÓR:
{
  ""title"": ""Nazwa Quizu"",
  ""questions"": [
    {
      ""text"": ""Treść pytania"",
      ""options"": [
        { ""text"": ""Opcja 1"", ""isCorrect"": false },
        { ""text"": ""Opcja 2"", ""isCorrect"": true }
      ],
      ""explanation"": ""Krótkie wyjaśnienie poprawnej odpowiedzi.""
    }
  ]
}";
        await Clipboard.Default.SetTextAsync(prompt);
        ShowToast("Prompt ze wzorem formatowania został skopiowany do schowka.");
    }

    private async void ShowToast(string message)
    {
        ToastLabel.Text = message;
        ToastNotification.Opacity = 1;
        await Task.Delay(2500);
        await ToastNotification.FadeTo(0, 500);
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
            
            ShowToast("Quiz został pomyślnie dodany do Twojej listy!");
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
