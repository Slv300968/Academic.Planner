namespace Models;

public class Helper
{
	public static JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

	public const string STATUS_PENDING = "Pendiente";
	public const string STATUS_IN_PROGRESS = "En Progreso";
	public const string STATUS_MASTERED = "Dominado";
}
