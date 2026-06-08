using QuizApp.Models;

namespace QuizApp;

public partial class ResultPage : ContentPage
{
    public ResultPage(Quiz quiz)
    {
        InitializeComponent();
        CalculateScore(quiz);
    }

    private void CalculateScore(Quiz quiz)
    {
        int totalQuestions = quiz.Questions.Count;
        int correctAnswers = 0;

        foreach (var q in quiz.Questions)
        {
            bool isCompletelyCorrect = true;
            foreach (var opt in q.Options)
            {
                if (opt.IsCorrect != opt.IsSelected)
                {
                    isCompletelyCorrect = false;
                    break;
                }
            }

            if (isCompletelyCorrect)
            {
                correctAnswers++;
            }
        }

        ScoreLabel.Text = $"Twój wynik: {correctAnswers} / {totalQuestions} ({(double)correctAnswers/totalQuestions:P0})";
    }

    private async void OnBackToMenuClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}
