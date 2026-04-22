namespace ApiClient;

public class StudyEventService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "StudyEvent";

	public async Task<StudyEvent> SelectStudyEvent(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectStudyEvent?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		if (response.StatusCode == HttpStatusCode.NoContent)
			return null;
		return await response.Content.ReadFromJsonAsync<StudyEvent>(Helper.JsonSerializerOptions);
	}

	public async Task<List<StudyEventGrid>> SelectStudyEventsGrid()
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectStudyEventsGrid");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<StudyEventGrid>>(Helper.JsonSerializerOptions);
	}

	public async Task<List<StudyEventGrid>> SelectStudyEventsGrid_DateRange(DateOnly startDate, DateOnly endDate)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectStudyEventsGrid_DateRange?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<StudyEventGrid>>(Helper.JsonSerializerOptions);
	}

	public async Task<StudyEvent> InsertStudyEvent(StudyEvent studyEvent)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertStudyEvent", studyEvent);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<StudyEvent>(Helper.JsonSerializerOptions);
	}

	public async Task<StudyEvent> UpdateStudyEvent(StudyEvent studyEvent)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateStudyEvent", studyEvent);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<StudyEvent>(Helper.JsonSerializerOptions);
	}

	public async Task<int> DeleteStudyEvent(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteStudyEvent?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<int>(Helper.JsonSerializerOptions);
	}
}
