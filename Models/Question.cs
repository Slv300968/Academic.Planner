namespace Models;

public class Question
{
	public int Id { get; set; }
	public int TopicId { get; set; }
	[Required(ErrorMessage = "- El campo Pregunta es requerido"), MaxLength(1000)]
	public string QuestionText { get; set; }
	[Required(ErrorMessage = "- La opción A es requerida"), MaxLength(500)]
	public string OptionA { get; set; }
	[Required(ErrorMessage = "- La opción B es requerida"), MaxLength(500)]
	public string OptionB { get; set; }
	[Required(ErrorMessage = "- La opción C es requerida"), MaxLength(500)]
	public string OptionC { get; set; }
	[Required(ErrorMessage = "- La opción D es requerida"), MaxLength(500)]
	public string OptionD { get; set; }
	[MaxLength(500)]
	public string OptionE { get; set; }
	[Required(ErrorMessage = "- La respuesta correcta es requerida"), MaxLength(1)]
	public string CorrectOption { get; set; }
	[MaxLength(1000)]
	public string Explanation { get; set; }
	public Topic Topic { get; set; }
}
