namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class SubjectController(ILogger<SubjectController> logger, SubjectDL subjectDL) : ControllerBase
{
	[HttpGet("SelectSubject")]
	public async Task<ActionResult<Subject>> SelectSubject(int id)
	{
		logger.LogInformation("SelectSubject: {Id}", id);
		Subject item = await subjectDL.SelectSubject(id);
		return Ok(item);
	}

	[HttpGet("SelectSubjects")]
	public async Task<ActionResult<List<Subject>>> SelectSubjects()
	{
		logger.LogInformation("SelectSubjects");
		List<Subject> items = await subjectDL.SelectSubjects();
		return Ok(items);
	}

	[HttpGet("SelectSubjectProgressList")]
	public async Task<ActionResult<List<SubjectProgress>>> SelectSubjectProgressList()
	{
		logger.LogInformation("SelectSubjectProgressList");
		List<SubjectProgress> items = await subjectDL.SelectSubjectProgressList();
		return Ok(items);
	}

	[HttpPost("InsertSubject")]
	public async Task<ActionResult<Subject>> InsertSubject([FromBody] Subject subject)
	{
		logger.LogInformation("InsertSubject: {Name}", subject.Name);
		Subject item = await subjectDL.InsertSubject(subject);
		return Ok(item);
	}

	[HttpPut("UpdateSubject")]
	public async Task<ActionResult<Subject>> UpdateSubject([FromBody] Subject subject)
	{
		logger.LogInformation("UpdateSubject: {Id}", subject.Id);
		Subject item = await subjectDL.UpdateSubject(subject);
		return Ok(item);
	}

	[HttpDelete("DeleteSubject")]
	public async Task<ActionResult<int>> DeleteSubject(int id)
	{
		logger.LogInformation("DeleteSubject: {Id}", id);
		int result = await subjectDL.DeleteSubject(id);
		return Ok(result);
	}
}
