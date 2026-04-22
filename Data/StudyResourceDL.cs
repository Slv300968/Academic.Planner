namespace Data;

public class StudyResourceDL(AcademicPlannerDBContext context)
{
	public async Task<List<StudyResource>> SelectStudyResources_Topic(int topicId)
	{
		List<StudyResource> items = await context.StudyResources
			.Where(x => x.TopicId == topicId)
			.OrderBy(x => x.ResourceType)
			.ThenBy(x => x.Title)
			.AsNoTracking()
			.ToListAsync();
		return items;
	}

	public async Task<StudyResource> InsertStudyResource(StudyResource studyResource)
	{
		await context.StudyResources.AddAsync(studyResource);
		await context.SaveChangesAsync();
		return studyResource;
	}

	public async Task<StudyResource> UpdateStudyResource(StudyResource studyResource)
	{
		context.StudyResources.Update(studyResource);
		await context.SaveChangesAsync();
		return studyResource;
	}

	public async Task<int> DeleteStudyResource(int id)
	{
		int result = await context.StudyResources.Where(x => x.Id == id).ExecuteDeleteAsync();
		return result;
	}
}
