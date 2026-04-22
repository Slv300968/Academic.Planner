namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class StudyResourceController(ILogger<StudyResourceController> logger, StudyResourceDL studyResourceDL) : ControllerBase
{
	[HttpGet("SelectStudyResources_Topic")]
	public async Task<ActionResult<List<StudyResource>>> SelectStudyResources_Topic(int topicId)
	{
		logger.LogInformation("SelectStudyResources_Topic: {TopicId}", topicId);
		List<StudyResource> items = await studyResourceDL.SelectStudyResources_Topic(topicId);
		return Ok(items);
	}

	[HttpPost("InsertStudyResource")]
	public async Task<ActionResult<StudyResource>> InsertStudyResource([FromBody] StudyResource studyResource)
	{
		logger.LogInformation("InsertStudyResource for Topic: {TopicId}", studyResource.TopicId);
		StudyResource item = await studyResourceDL.InsertStudyResource(studyResource);
		return Ok(item);
	}

	[HttpPut("UpdateStudyResource")]
	public async Task<ActionResult<StudyResource>> UpdateStudyResource([FromBody] StudyResource studyResource)
	{
		logger.LogInformation("UpdateStudyResource: {Id}", studyResource.Id);
		StudyResource item = await studyResourceDL.UpdateStudyResource(studyResource);
		return Ok(item);
	}

	[HttpDelete("DeleteStudyResource")]
	public async Task<ActionResult<int>> DeleteStudyResource(int id)
	{
		logger.LogInformation("DeleteStudyResource: {Id}", id);
		int result = await studyResourceDL.DeleteStudyResource(id);
		return Ok(result);
	}
}
