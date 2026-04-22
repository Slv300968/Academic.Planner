namespace Models;

public class Subject
{
	public int Id { get; set; }
	[Required(ErrorMessage = "- El campo Nombre es requerido"), MaxLength(100)]
	public string Name { get; set; } = string.Empty;
	[MaxLength(500)]
	public string Description { get; set; } = string.Empty;
	[MaxLength(20)]
	public string Color { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public List<Topic> Topics { get; set; } = new();
	public List<Flashcard> Flashcards { get; set; } = new();
}

public class SubjectProgress
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Color { get; set; }
	public int TotalTopics { get; set; }
	public int PendingTopics { get; set; }
	public int InProgressTopics { get; set; }
	public int MasteredTopics { get; set; }
	public double ProgressPercentage { get; set; }
}
