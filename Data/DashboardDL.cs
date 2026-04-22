namespace Data;

public class DashboardDL(AcademicPlannerDBContext context)
{
	public async Task<DashboardStats> SelectDashboardStats()
	{
		int totalTopics = await context.Topics.CountAsync();
		int pendingTopics = await context.Topics.CountAsync(t => t.Status == Helper.STATUS_PENDING);
		int inProgressTopics = await context.Topics.CountAsync(t => t.Status == Helper.STATUS_IN_PROGRESS);
		int masteredTopics = await context.Topics.CountAsync(t => t.Status == Helper.STATUS_MASTERED);
		int totalStudyMinutes = await context.StudyEvents.Where(e => e.IsCompleted).SumAsync(e => e.DurationMinutes);

		List<SubjectProgress> subjectProgressList = await context.Subjects
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

		DashboardStats stats = new()
		{
			TotalSubjects = await context.Subjects.CountAsync(),
			TotalTopics = totalTopics,
			PendingTopics = pendingTopics,
			InProgressTopics = inProgressTopics,
			MasteredTopics = masteredTopics,
			GlobalProgressPercentage = totalTopics == 0 ? 0 : Math.Round((double)masteredTopics / totalTopics * 100, 1),
			TotalQuestions = await context.Questions.CountAsync(),
			TotalStudyMinutes = totalStudyMinutes,
			SubjectProgressList = subjectProgressList
		};
		return stats;
	}
}
