namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class TopicController(ILogger<TopicController> logger, TopicDL topicDL) : ControllerBase
{
	[HttpGet("SelectTopic")]
	public async Task<ActionResult<Topic>> SelectTopic(int id)
	{
		logger.LogInformation("SelectTopic: {Id}", id);
		Topic item = await topicDL.SelectTopic(id);
		return Ok(item);
	}

	[HttpGet("SelectTopics_Subject")]
	public async Task<ActionResult<List<Topic>>> SelectTopics_Subject(int subjectId)
	{
		logger.LogInformation("SelectTopics_Subject: {SubjectId}", subjectId);
		List<Topic> items = await topicDL.SelectTopics_Subject(subjectId);
		return Ok(items);
	}

	[HttpGet("SelectTopicsGrid")]
	public async Task<ActionResult<List<TopicGrid>>> SelectTopicsGrid()
	{
		logger.LogInformation("SelectTopicsGrid");
		List<TopicGrid> items = await topicDL.SelectTopicsGrid();
		return Ok(items);
	}

	[HttpPost("InsertTopic")]
	public async Task<ActionResult<Topic>> InsertTopic([FromBody] Topic topic)
	{
		logger.LogInformation("InsertTopic: {Name}", topic.Name);
		Topic item = await topicDL.InsertTopic(topic);
		return Ok(item);
	}

	[HttpPut("UpdateTopic")]
	public async Task<ActionResult<Topic>> UpdateTopic([FromBody] Topic topic)
	{
		logger.LogInformation("UpdateTopic: {Id}", topic.Id);
		Topic item = await topicDL.UpdateTopic(topic);
		return Ok(item);
	}

	[HttpPut("UpdateTopicStatus")]
	public async Task<ActionResult<int>> UpdateTopicStatus(int id, string status)
	{
		logger.LogInformation("UpdateTopicStatus: {Id} -> {Status}", id, status);
		int result = await topicDL.UpdateTopicStatus(id, status);
		return Ok(result);
	}

	[HttpPut("UpdateTopicNotes")]
	public async Task<ActionResult<int>> UpdateTopicNotes(int id, [FromBody] string notes)
	{
		logger.LogInformation("UpdateTopicNotes: {Id}", id);
		int result = await topicDL.UpdateTopicNotes(id, notes);
		return Ok(result);
	}
	[HttpDelete("DeleteTopic")]
	public async Task<ActionResult<int>> DeleteTopic(int id)
	{
		logger.LogInformation("DeleteTopic: {Id}", id);
		int result = await topicDL.DeleteTopic(id);
		return Ok(result);
	}
}
