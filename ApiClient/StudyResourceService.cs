namespace ApiClient;

public class StudyResourceService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "StudyResource";

	public async Task<List<StudyResource>> SelectStudyResources_Topic(int topicId)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectStudyResources_Topic?topicId={topicId}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<StudyResource>>(Helper.JsonSerializerOptions);
	}

	public async Task<StudyResource> InsertStudyResource(StudyResource studyResource)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertStudyResource", studyResource);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<StudyResource>(Helper.JsonSerializerOptions);
	}

	public async Task<StudyResource> UpdateStudyResource(StudyResource studyResource)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateStudyResource", studyResource);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<StudyResource>(Helper.JsonSerializerOptions);
	}

	public async Task<int> DeleteStudyResource(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteStudyResource?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<int>(Helper.JsonSerializerOptions);
	}
}
