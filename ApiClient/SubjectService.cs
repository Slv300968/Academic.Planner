namespace ApiClient;

public class SubjectService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Subject";

	public async Task<Subject> SelectSubject(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSubject?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		if (response.StatusCode == HttpStatusCode.NoContent)
			return null;
		return await response.Content.ReadFromJsonAsync<Subject>(Helper.JsonSerializerOptions);
	}

	public async Task<List<Subject>> SelectSubjects()
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSubjects");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<Subject>>(Helper.JsonSerializerOptions);
	}

	public async Task<List<SubjectProgress>> SelectSubjectProgressList()
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectSubjectProgressList");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<SubjectProgress>>(Helper.JsonSerializerOptions);
	}

	public async Task<Subject> InsertSubject(Subject subject)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertSubject", subject);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<Subject>(Helper.JsonSerializerOptions);
	}

	public async Task<Subject> UpdateSubject(Subject subject)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateSubject", subject);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<Subject>(Helper.JsonSerializerOptions);
	}

	public async Task<int> DeleteSubject(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteSubject?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<int>(Helper.JsonSerializerOptions);
	}
}
