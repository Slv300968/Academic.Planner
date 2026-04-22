namespace Models;

public class StudyResource
{
	public int Id { get; set; }
	public int TopicId { get; set; }
	[Required(ErrorMessage = "- El campo Título es requerido"), MaxLength(300)]
	public string Title { get; set; } = string.Empty;
	[MaxLength(1000)]
	public string Url { get; set; } = string.Empty;
	[MaxLength(50)]
	public string ResourceType { get; set; } = string.Empty;
	[MaxLength(1000)]
	public string Notes { get; set; } = string.Empty;
	public Topic Topic { get; set; }
}
