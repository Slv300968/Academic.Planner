namespace Models;

public class Topic
{
	public int Id { get; set; }
	public int SubjectId { get; set; }
	[Required(ErrorMessage = "- El campo Nombre es requerido"), MaxLength(200)]
	public string Name { get; set; } = string.Empty;
	[MaxLength(1000)]
	public string Description { get; set; } = string.Empty;
	[MaxLength(20)]
	public string Status { get; set; } = Helper.STATUS_PENDING;
	[MaxLength(4000)]
	public string Notes { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public Subject Subject { get; set; }
	public List<Question> Questions { get; set; } = new();
	public List<StudyEvent> StudyEvents { get; set; } = new();
	public List<StudyResource> StudyResources { get; set; } = new();
}

public class TopicGrid
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Status { get; set; }
	public string SubjectName { get; set; }
	public string SubjectColor { get; set; }
	public int QuestionCount { get; set; }
}
