namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class FlashcardController(ILogger<FlashcardController> logger, FlashcardDL flashcardDL) : ControllerBase
{
	[HttpGet("SelectFlashcards_Subject")]
	public async Task<ActionResult<List<Flashcard>>> SelectFlashcards_Subject(int subjectId)
	{
		logger.LogInformation("SelectFlashcards_Subject: {SubjectId}", subjectId);
		List<Flashcard> items = await flashcardDL.SelectFlashcards_Subject(subjectId);
		return Ok(items);
	}

	[HttpPost("InsertFlashcard")]
	public async Task<ActionResult<Flashcard>> InsertFlashcard([FromBody] Flashcard flashcard)
	{
		logger.LogInformation("InsertFlashcard for Subject: {SubjectId}", flashcard.SubjectId);
		Flashcard item = await flashcardDL.InsertFlashcard(flashcard);
		return Ok(item);
	}

	[HttpPut("UpdateFlashcard")]
	public async Task<ActionResult<Flashcard>> UpdateFlashcard([FromBody] Flashcard flashcard)
	{
		logger.LogInformation("UpdateFlashcard: {Id}", flashcard.Id);
		Flashcard item = await flashcardDL.UpdateFlashcard(flashcard);
		return Ok(item);
	}

	[HttpDelete("DeleteFlashcard")]
	public async Task<ActionResult<int>> DeleteFlashcard(int id)
	{
		logger.LogInformation("DeleteFlashcard: {Id}", id);
		int result = await flashcardDL.DeleteFlashcard(id);
		return Ok(result);
	}
}
