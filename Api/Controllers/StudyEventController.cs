namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class StudyEventController(ILogger<StudyEventController> logger, StudyEventDL studyEventDL) : ControllerBase
{
	[HttpGet("SelectStudyEvent")]
	public async Task<ActionResult<StudyEvent>> SelectStudyEvent(int id)
	{
		logger.LogInformation("SelectStudyEvent: {Id}", id);
		StudyEvent item = await studyEventDL.SelectStudyEvent(id);
		return Ok(item);
	}

	[HttpGet("SelectStudyEventsGrid")]
	public async Task<ActionResult<List<StudyEventGrid>>> SelectStudyEventsGrid()
	{
		logger.LogInformation("SelectStudyEventsGrid");
		List<StudyEventGrid> items = await studyEventDL.SelectStudyEventsGrid();
		return Ok(items);
	}

	[HttpGet("SelectStudyEventsGrid_DateRange")]
	public async Task<ActionResult<List<StudyEventGrid>>> SelectStudyEventsGrid_DateRange(DateOnly startDate, DateOnly endDate)
	{
		logger.LogInformation("SelectStudyEventsGrid_DateRange: {StartDate} - {EndDate}", startDate, endDate);
		List<StudyEventGrid> items = await studyEventDL.SelectStudyEventsGrid_DateRange(startDate, endDate);
		return Ok(items);
	}

	[HttpPost("InsertStudyEvent")]
	public async Task<ActionResult<StudyEvent>> InsertStudyEvent([FromBody] StudyEvent studyEvent)
	{
		logger.LogInformation("InsertStudyEvent: {Title}", studyEvent.Title);
		StudyEvent item = await studyEventDL.InsertStudyEvent(studyEvent);
		return Ok(item);
	}

	[HttpPut("UpdateStudyEvent")]
	public async Task<ActionResult<StudyEvent>> UpdateStudyEvent([FromBody] StudyEvent studyEvent)
	{
		logger.LogInformation("UpdateStudyEvent: {Id}", studyEvent.Id);
		StudyEvent item = await studyEventDL.UpdateStudyEvent(studyEvent);
		return Ok(item);
	}

	[HttpDelete("DeleteStudyEvent")]
	public async Task<ActionResult<int>> DeleteStudyEvent(int id)
	{
		logger.LogInformation("DeleteStudyEvent: {Id}", id);
		int result = await studyEventDL.DeleteStudyEvent(id);
		return Ok(result);
	}
}
