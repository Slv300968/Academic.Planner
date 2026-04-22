namespace Models;

public class StudyEvent
{
	public int Id { get; set; }
	public int TopicId { get; set; }
	[Required(ErrorMessage = "- El campo Título es requerido"), MaxLength(200)]
	public string Title { get; set; }
	public DateOnly EventDate { get; set; }
	public int DurationMinutes { get; set; }
	[MaxLength(500)]
	public string Notes { get; set; }
	public bool IsCompleted { get; set; }
	public Topic Topic { get; set; }
}

public class StudyEventGrid
{
	public int Id { get; set; }
	public string Title { get; set; }
	public DateOnly EventDate { get; set; }
	public int DurationMinutes { get; set; }
	public bool IsCompleted { get; set; }
	public string TopicName { get; set; }
	public string SubjectName { get; set; }
	public string SubjectColor { get; set; }
}
