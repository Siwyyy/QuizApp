using QuizApp.Models;

namespace QuizApp;

public partial class QuizPage : ContentPage
{
    private Quiz _quiz;
    private int _currentQuestionIndex = 0;
    private bool _isAnswerChecked = false;
    private List<View> _optionViews = new();

    public QuizPage(Quiz quiz)
    {
        InitializeComponent();
        _quiz = quiz;
        Title = quiz.Title ?? "Quiz";
        
        LoadQuestion();
    }

    private void LoadQuestion()
    {
        if (_currentQuestionIndex >= _quiz.Questions.Count)
        {
            Navigation.PushAsync(new ResultPage(_quiz));
            return;
        }

        _isAnswerChecked = false;
        var currentQuestion = _quiz.Questions[_currentQuestionIndex];
        
        ProgressLabel.Text = $"Pytanie {_currentQuestionIndex + 1} / {_quiz.Questions.Count}";
        QuestionTextLabel.Text = currentQuestion.Text;
        ExplanationLabel.IsVisible = false;
        
        ActionBtn.Text = "Sprawdź";
        OptionsContainer.Children.Clear();
        _optionViews.Clear();

        foreach (var option in currentQuestion.Options)
        {
            option.IsSelected = false; // Reset przed wyświetleniem
            
            View optionView;
            if (currentQuestion.IsMultipleChoice)
            {
                var cb = new CheckBox { IsChecked = option.IsSelected, VerticalOptions = LayoutOptions.Center };
                cb.CheckedChanged += (s, e) => option.IsSelected = e.Value;
                
                var label = new Label { Text = option.Text, VerticalOptions = LayoutOptions.Center };
                
                var hsl = new HorizontalStackLayout { Spacing = 10, Children = { cb, label }, Padding = new Thickness(10) };
                
                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += (s, e) => cb.IsChecked = !cb.IsChecked;
                hsl.GestureRecognizers.Add(tapGesture);
                
                optionView = new Border 
                { 
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(10) },
                    Stroke = Colors.LightGray,
                    StrokeThickness = 1,
                    BackgroundColor = Colors.Transparent,
                    Content = hsl,
                    Margin = new Thickness(0, 5)
                };
            }
            else
            {
                var rb = new RadioButton { Content = option.Text, GroupName = "OptionsGroup", IsChecked = option.IsSelected, Padding = new Thickness(10) };
                rb.CheckedChanged += (s, e) => option.IsSelected = e.Value;
                
                optionView = new Border 
                { 
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(10) },
                    Stroke = Colors.LightGray,
                    StrokeThickness = 1,
                    BackgroundColor = Colors.Transparent,
                    Content = rb,
                    Margin = new Thickness(0, 5)
                };
            }

            OptionsContainer.Children.Add(optionView);
            _optionViews.Add(optionView);
        }
    }

    private async void OnActionClicked(object sender, EventArgs e)
    {
        if (!_isAnswerChecked)
        {
            CheckAnswer();
        }
        else
        {
            _currentQuestionIndex++;
            if (_currentQuestionIndex >= _quiz.Questions.Count)
            {
                await Navigation.PushAsync(new ResultPage(_quiz));
            }
            else
            {
                LoadQuestion();
            }
        }
    }

    private void CheckAnswer()
    {
        var currentQuestion = _quiz.Questions[_currentQuestionIndex];
        
        // Zablokuj zmiany i pokoloruj odpowiedzi
        for (int i = 0; i < currentQuestion.Options.Count; i++)
        {
            var option = currentQuestion.Options[i];
            var view = _optionViews[i] as Border;
            if (view == null) continue;
            
            Color bgColor = Colors.Transparent;
            Color textColor = Colors.Black; // Wystarczająco kontrastowe na transparent
            
            if (option.IsCorrect)
            {
                bgColor = Color.FromArgb("#1b5e20"); // Ciemna zieleń
                textColor = Colors.White;
            }
            else if (option.IsSelected && !option.IsCorrect)
            {
                bgColor = Color.FromArgb("#b71c1c"); // Ciemna czerwień
                textColor = Colors.White;
            }

            view.BackgroundColor = bgColor;
            if (view.Content is HorizontalStackLayout hsl)
            {
                if (hsl.Children[0] is CheckBox cb) cb.IsEnabled = false;
                if (hsl.Children[1] is Label lbl) 
                {
                    if (bgColor != Colors.Transparent) lbl.TextColor = textColor;
                }
            }
            else if (view.Content is RadioButton rb)
            {
                rb.IsEnabled = false;
                if (bgColor != Colors.Transparent) rb.TextColor = textColor;
            }
        }

        if (!string.IsNullOrWhiteSpace(currentQuestion.Explanation))
        {
            ExplanationLabel.Text = $"Wyjaśnienie: {currentQuestion.Explanation}";
            ExplanationLabel.IsVisible = true;
        }

        _isAnswerChecked = true;
        
        if (_currentQuestionIndex == _quiz.Questions.Count - 1)
        {
            ActionBtn.Text = "Zakończ i pokaż wynik";
        }
        else
        {
            ActionBtn.Text = "Następne pytanie";
        }
    }
}
