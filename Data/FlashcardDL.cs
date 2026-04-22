namespace Data;

public class FlashcardDL(AcademicPlannerDBContext context)
{
	public async Task<List<Flashcard>> SelectFlashcards_Subject(int subjectId)
	{
		List<Flashcard> items = await context.Flashcards
			.Where(x => x.SubjectId == subjectId)
			.OrderBy(x => x.SortOrder)
			.ThenBy(x => x.Id)
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<Flashcard> InsertFlashcard(Flashcard flashcard)
	{
		await context.Flashcards.AddAsync(flashcard);
		await context.SaveChangesAsync();
		return flashcard;
	}

	public async Task<Flashcard> UpdateFlashcard(Flashcard flashcard)
	{
		context.Flashcards.Update(flashcard);
		await context.SaveChangesAsync();
		return flashcard;
	}

	public async Task<int> DeleteFlashcard(int id)
	{
		int result = await context.Flashcards.Where(x => x.Id == id).ExecuteDeleteAsync();
		return result;
	}
}
