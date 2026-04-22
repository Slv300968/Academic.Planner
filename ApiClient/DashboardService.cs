namespace ApiClient;

public class DashboardService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Dashboard";

	public async Task<DashboardStats> SelectDashboardStats()
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectDashboardStats");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<DashboardStats>(Helper.JsonSerializerOptions);
	}
}
