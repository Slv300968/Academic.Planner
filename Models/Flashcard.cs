namespace Models;

public class Flashcard
{
	public int Id { get; set; }
	public int SubjectId { get; set; }
	[Required(ErrorMessage = "- El frente de la tarjeta es requerido")]
	public string Front { get; set; } = string.Empty;
	[Required(ErrorMessage = "- El reverso de la tarjeta es requerido")]
	public string Back { get; set; } = string.Empty;
	[MaxLength(500)]
	public string Tags { get; set; } = string.Empty;
	public string ChartJson { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public Subject Subject { get; set; }
}

public class FlashcardChart
{
	public string ChartType { get; set; } = "Bar";
	public string Title { get; set; } = string.Empty;
	public List<string> Categories { get; set; } = new();
	public List<double> Values { get; set; } = new();
	public string SeriesName { get; set; } = string.Empty;
}
