namespace ApiClient;

public class TopicService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Topic";

	public async Task<Topic> SelectTopic(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectTopic?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		if (response.StatusCode == HttpStatusCode.NoContent)
			return null;
		return await response.Content.ReadFromJsonAsync<Topic>(Helper.JsonSerializerOptions);
	}

	public async Task<List<Topic>> SelectTopics_Subject(int subjectId)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectTopics_Subject?subjectId={subjectId}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<Topic>>(Helper.JsonSerializerOptions);
	}

	public async Task<List<TopicGrid>> SelectTopicsGrid()
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectTopicsGrid");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<TopicGrid>>(Helper.JsonSerializerOptions);
	}

	public async Task<Topic> InsertTopic(Topic topic)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertTopic", topic);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<Topic>(Helper.JsonSerializerOptions);
	}

	public async Task<Topic> UpdateTopic(Topic topic)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateTopic", topic);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<Topic>(Helper.JsonSerializerOptions);
	}

	public async Task<int> UpdateTopicStatus(int id, string status)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PutAsync($"{apiName}/UpdateTopicStatus?id={id}&status={Uri.EscapeDataString(status)}", null);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<int>(Helper.JsonSerializerOptions);
	}

	public async Task<int> UpdateTopicNotes(int id, string notes)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateTopicNotes?id={id}", notes);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<int>(Helper.JsonSerializerOptions);
	}
	public async Task<int> DeleteTopic(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteTopic?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<int>(Helper.JsonSerializerOptions);
	}
}
