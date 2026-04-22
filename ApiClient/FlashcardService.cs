namespace ApiClient;

public class FlashcardService(IHttpClientFactory httpClientFactory)
{
	private readonly string apiName = "Flashcard";

	public async Task<List<Flashcard>> SelectFlashcards_Subject(int subjectId)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.GetAsync($"{apiName}/SelectFlashcards_Subject?subjectId={subjectId}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<List<Flashcard>>(Helper.JsonSerializerOptions);
	}

	public async Task<Flashcard> InsertFlashcard(Flashcard flashcard)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{apiName}/InsertFlashcard", flashcard);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<Flashcard>(Helper.JsonSerializerOptions);
	}

	public async Task<Flashcard> UpdateFlashcard(Flashcard flashcard)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.PutAsJsonAsync($"{apiName}/UpdateFlashcard", flashcard);
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<Flashcard>(Helper.JsonSerializerOptions);
	}

	public async Task<int> DeleteFlashcard(int id)
	{
		HttpClient httpClient = httpClientFactory.CreateClient("AcademicPlannerApi");
		HttpResponseMessage response = await httpClient.DeleteAsync($"{apiName}/DeleteFlashcard?id={id}");
		if (!response.IsSuccessStatusCode)
			throw new ApplicationException(await response.Content.ReadAsStringAsync());
		return await response.Content.ReadFromJsonAsync<int>(Helper.JsonSerializerOptions);
	}
}
