namespace Web.Pages;

public partial class QuizSimulator
{
	[Inject] QuestionService QuestionService { get; set; }

	private bool quizStarted = false;
	private bool quizFinished = false;
	private int questionCount = 10;
	private List<Question> questions = new();
	private int currentIndex = 0;
	private Question currentQuestion;
	private string selectedAnswer;
	private int correctAnswers = 0;

	private async Task StartQuiz()
	{
		questions = await QuestionService.SelectQuestions_Random(questionCount);
		if (questions.Count == 0)
			return;
		currentIndex = 0;
		correctAnswers = 0;
		quizStarted = true;
		quizFinished = false;
		currentQuestion = questions[currentIndex];
		selectedAnswer = null;
	}

	private void SelectAnswer(string answer)
	{
		if (selectedAnswer is not null)
			return;
		selectedAnswer = answer;
		if (answer == currentQuestion.CorrectOption)
			correctAnswers++;
	}

	private void NextQuestion()
	{
		currentIndex++;
		if (currentIndex >= questions.Count)
		{
			quizFinished = true;
			return;
		}
		currentQuestion = questions[currentIndex];
		selectedAnswer = null;
	}

	private void RestartQuiz()
	{
		quizStarted = false;
		quizFinished = false;
		questions = new();
		selectedAnswer = null;
	}

	private string GetOptionClass(string option)
	{
		if (selectedAnswer is null)
			return string.Empty;
		if (option == currentQuestion.CorrectOption)
			return "correct";
		if (option == selectedAnswer)
			return "incorrect";
		return string.Empty;
	}
}
