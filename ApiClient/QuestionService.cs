namespace ApiClient;

public class QuestionService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Question";

	public async Task<Question> SelectQuestion(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectQuestion?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		if (response.StatusCode == HttpStatusCode.NoContent)
			return null;
		return await response.Content.ReadFromJsonAsync<Question>(Helper.JsonSerializerOptions);
	}

	public async Task<List<Question>> SelectQuestions_Topic(int topicId)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectQuestions_Topic?topicId={topicId}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<Question>>(Helper.JsonSerializerOptions);
	}

	public async Task<List<Question>> SelectQuestions_Subject(int subjectId)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectQuestions_Subject?subjectId={subjectId}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<Question>>(Helper.JsonSerializerOptions);
	}

	public async Task<List<Question>> SelectQuestions_Random(int count)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectQuestions_Random?count={count}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<Question>>(Helper.JsonSerializerOptions);
	}

	public async Task<Question> InsertQuestion(Question question)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertQuestion", question);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<Question>(Helper.JsonSerializerOptions);
	}

	public async Task<Question> UpdateQuestion(Question question)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateQuestion", question);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<Question>(Helper.JsonSerializerOptions);
	}

	public async Task<int> DeleteQuestion(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteQuestion?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<int>(Helper.JsonSerializerOptions);
	}
}
