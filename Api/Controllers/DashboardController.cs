namespace Api.Controllers;

[Route("[controller]")]
[ApiController]
public class DashboardController(ILogger<DashboardController> logger, DashboardDL dashboardDL) : ControllerBase
{
	[HttpGet("SelectDashboardStats")]
	public async Task<ActionResult<DashboardStats>> SelectDashboardStats()
	{
		logger.LogInformation("SelectDashboardStats");
		DashboardStats stats = await dashboardDL.SelectDashboardStats();
		return Ok(stats);
	}
}
