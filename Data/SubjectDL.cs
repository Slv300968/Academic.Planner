namespace Data;

public class SubjectDL(AcademicPlannerDBContext context)
{
	public async Task<Subject> SelectSubject(int id)
	{
		Subject item = await context.Subjects
			.Include(x => x.Topics)
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id);
		return item;
	}

	public async Task<List<Subject>> SelectSubjects()
	{
		List<Subject> items = await context.Subjects
			.OrderBy(x => x.SortOrder)
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<List<SubjectProgress>> SelectSubjectProgressList()
	{
		List<SubjectProgress> items = await context.Subjects
			.OrderBy(x => x.SortOrder)
			.Select(s => new SubjectProgress
			{
				Id = s.Id,
				Name = s.Name,
				Color = s.Color,
				TotalTopics = s.Topics.Count,
				PendingTopics = s.Topics.Count(t => t.Status == Helper.STATUS_PENDING),
				InProgressTopics = s.Topics.Count(t => t.Status == Helper.STATUS_IN_PROGRESS),
				MasteredTopics = s.Topics.Count(t => t.Status == Helper.STATUS_MASTERED),
				ProgressPercentage = s.Topics.Count == 0 ? 0 : Math.Round((double)s.Topics.Count(t => t.Status == Helper.STATUS_MASTERED) / s.Topics.Count * 100, 1)
			})
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<Subject> InsertSubject(Subject subject)
	{
		await context.Subjects.AddAsync(subject);
		await context.SaveChangesAsync();
		return subject;
	}

	public async Task<Subject> UpdateSubject(Subject subject)
	{
		context.Subjects.Update(subject);
		await context.SaveChangesAsync();
		return subject;
	}

	public async Task<int> DeleteSubject(int id)
	{
		int result = await context.Subjects.Where(x => x.Id == id).ExecuteDeleteAsync();
		return result;
	}
}
