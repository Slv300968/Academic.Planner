namespace Data;

public class TopicDL(AcademicPlannerDBContext context)
{
	public async Task<Topic> SelectTopic(int id)
	{
		Topic item = await context.Topics
			.Include(x => x.Subject)
			.Include(x => x.Questions)
			.Include(x => x.StudyEvents)
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id);
		return item;
	}

	public async Task<List<Topic>> SelectTopics_Subject(int subjectId)
	{
		List<Topic> items = await context.Topics
			.Where(x => x.SubjectId == subjectId)
			.OrderBy(x => x.SortOrder)
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<List<TopicGrid>> SelectTopicsGrid()
	{
		List<TopicGrid> items = await context.Topics
			.OrderBy(x => x.Subject.SortOrder)
			.ThenBy(x => x.SortOrder)
			.Select(t => new TopicGrid
			{
				Id = t.Id,
				Name = t.Name,
				Status = t.Status,
				SubjectName = t.Subject.Name,
				SubjectColor = t.Subject.Color,
				QuestionCount = t.Questions.Count
			})
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<Topic> InsertTopic(Topic topic)
	{
		await context.Topics.AddAsync(topic);
		await context.SaveChangesAsync();
		return topic;
	}

	public async Task<Topic> UpdateTopic(Topic topic)
	{
		context.Topics.Update(topic);
		await context.SaveChangesAsync();
		return topic;
	}

	public async Task<int> UpdateTopicStatus(int id, string status)
	{
		int result = await context.Topics
			.Where(x => x.Id == id)
			.ExecuteUpdateAsync(x => x.SetProperty(t => t.Status, status));
		return result;
	}

	public async Task<int> UpdateTopicNotes(int id, string notes)
	{
		int result = await context.Topics
			.Where(x => x.Id == id)
			.ExecuteUpdateAsync(x => x.SetProperty(t => t.Notes, notes));
		return result;
	}
	public async Task<int> DeleteTopic(int id)
	{
		int result = await context.Topics.Where(x => x.Id == id).ExecuteDeleteAsync();
		return result;
	}
}
