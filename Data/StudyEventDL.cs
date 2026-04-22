namespace Data;

public class StudyEventDL(AcademicPlannerDBContext context)
{
	public async Task<StudyEvent> SelectStudyEvent(int id)
	{
		StudyEvent item = await context.StudyEvents
			.Include(x => x.Topic)
			.ThenInclude(x => x.Subject)
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == id);
		return item;
	}

	public async Task<List<StudyEventGrid>> SelectStudyEventsGrid()
	{
		List<StudyEventGrid> items = await context.StudyEvents
			.OrderBy(x => x.EventDate)
			.Select(e => new StudyEventGrid
			{
				Id = e.Id,
				Title = e.Title,
				EventDate = e.EventDate,
				DurationMinutes = e.DurationMinutes,
				IsCompleted = e.IsCompleted,
				TopicName = e.Topic.Name,
				SubjectName = e.Topic.Subject.Name,
				SubjectColor = e.Topic.Subject.Color
			})
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<List<StudyEventGrid>> SelectStudyEventsGrid_DateRange(DateOnly startDate, DateOnly endDate)
	{
		List<StudyEventGrid> items = await context.StudyEvents
			.Where(x => x.EventDate >= startDate && x.EventDate <= endDate)
			.OrderBy(x => x.EventDate)
			.Select(e => new StudyEventGrid
			{
				Id = e.Id,
				Title = e.Title,
				EventDate = e.EventDate,
				DurationMinutes = e.DurationMinutes,
				IsCompleted = e.IsCompleted,
				TopicName = e.Topic.Name,
				SubjectName = e.Topic.Subject.Name,
				SubjectColor = e.Topic.Subject.Color
			})
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<StudyEvent> InsertStudyEvent(StudyEvent studyEvent)
	{
		await context.StudyEvents.AddAsync(studyEvent);
		await context.SaveChangesAsync();
		return studyEvent;
	}

	public async Task<StudyEvent> UpdateStudyEvent(StudyEvent studyEvent)
	{
		context.StudyEvents.Update(studyEvent);
		await context.SaveChangesAsync();
		return studyEvent;
	}

	public async Task<int> DeleteStudyEvent(int id)
	{
		int result = await context.StudyEvents.Where(x => x.Id == id).ExecuteDeleteAsync();
		return result;
	}
}
