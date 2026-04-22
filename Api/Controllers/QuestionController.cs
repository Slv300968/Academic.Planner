namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class QuestionController(ILogger<QuestionController> logger, QuestionDL questionDL) : ControllerBase
{
	[HttpGet("SelectQuestion")]
	public async Task<ActionResult<Question>> SelectQuestion(int id)
	{
		logger.LogInformation("SelectQuestion: {Id}", id);
		Question item = await questionDL.SelectQuestion(id);
		return Ok(item);
	}

	[HttpGet("SelectQuestions_Topic")]
	public async Task<ActionResult<List<Question>>> SelectQuestions_Topic(int topicId)
	{
		logger.LogInformation("SelectQuestions_Topic: {TopicId}", topicId);
		List<Question> items = await questionDL.SelectQuestions_Topic(topicId);
		return Ok(items);
	}

	[HttpGet("SelectQuestions_Subject")]
	public async Task<ActionResult<List<Question>>> SelectQuestions_Subject(int subjectId)
	{
		logger.LogInformation("SelectQuestions_Subject: {SubjectId}", subjectId);
		List<Question> items = await questionDL.SelectQuestions_Subject(subjectId);
		return Ok(items);
	}

	[HttpGet("SelectQuestions_Random")]
	public async Task<ActionResult<List<Question>>> SelectQuestions_Random(int count)
	{
		logger.LogInformation("SelectQuestions_Random: {Count}", count);
		List<Question> items = await questionDL.SelectQuestions_Random(count);
		return Ok(items);
	}

	[HttpPost("InsertQuestion")]
	public async Task<ActionResult<Question>> InsertQuestion([FromBody] Question question)
	{
		logger.LogInformation("InsertQuestion for Topic: {TopicId}", question.TopicId);
		Question item = await questionDL.InsertQuestion(question);
		return Ok(item);
	}

	[HttpPut("UpdateQuestion")]
	public async Task<ActionResult<Question>> UpdateQuestion([FromBody] Question question)
	{
		logger.LogInformation("UpdateQuestion: {Id}", question.Id);
		Question item = await questionDL.UpdateQuestion(question);
		return Ok(item);
	}

	[HttpDelete("DeleteQuestion")]
	public async Task<ActionResult<int>> DeleteQuestion(int id)
	{
		logger.LogInformation("DeleteQuestion: {Id}", id);
		int result = await questionDL.DeleteQuestion(id);
		return Ok(result);
	}
}
