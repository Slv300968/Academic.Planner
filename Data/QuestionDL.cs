namespace Data;

public class QuestionDL(AcademicPlannerDBContext context)
{
	public async Task<Question> SelectQuestion(int id)
	{
		Question item = await context.Questions
			.Include(x => x.Topic)
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id);
		return item;
	}

	public async Task<List<Question>> SelectQuestions_Topic(int topicId)
	{
		List<Question> items = await context.Questions
			.Where(x => x.TopicId == topicId)
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<List<Question>> SelectQuestions_Subject(int subjectId)
	{
		List<Question> items = await context.Questions
			.Where(x => x.Topic.SubjectId == subjectId)
			.Include(x => x.Topic)
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<List<Question>> SelectQuestions_Random(int count)
	{
		List<Question> items = await context.Questions
			.Include(x => x.Topic)
			.OrderBy(x => Guid.NewGuid())
			.Take(count)
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<Question> InsertQuestion(Question question)
	{
		await context.Questions.AddAsync(question);
		await context.SaveChangesAsync();
		return question;
	}

	public async Task<Question> UpdateQuestion(Question question)
	{
		context.Questions.Update(question);
		await context.SaveChangesAsync();
		return question;
	}

	public async Task<int> DeleteQuestion(int id)
	{
		int result = await context.Questions.Where(x => x.Id == id).ExecuteDeleteAsync();
		return result;
	}
}
