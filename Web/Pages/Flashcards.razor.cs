namespace Web.Pages;

public partial class Flashcards : ComponentBase
{
	[Parameter] public int SubjectId { get; set; }
	[Inject] public SubjectService SubjectService { get; set; }
	[Inject] public FlashcardService FlashcardService { get; set; }
	[Inject] public NavigationManager NavigationManager { get; set; }
	[Inject] public IJSRuntime JSRuntime { get; set; }

	private Subject subject;
	private List<Flashcard> flashcards = new();
	private Dictionary<int, FlashcardChart> parsedCharts = new();
	private HashSet<int> flippedCards = new();
	private HashSet<int> renderedCharts = new();
	private bool isLoading = true;
	private bool showEditor = false;
	private bool showChartBuilder = false;
	private bool isSaving = false;
	private Flashcard editingFlashcard = new();
	private FlashcardChart editingChart = new();
	private string categoriesRaw = string.Empty;
	private string valuesRaw = string.Empty;
	private string editorError = string.Empty;
	private string chartPreviewError = string.Empty;
	private List<string> chartTypes = new() { "Bar", "Line", "Area", "Pie", "Donut" };

	protected override async Task OnInitializedAsync()
	{
		subject = await SubjectService.SelectSubject(SubjectId);
		flashcards = await FlashcardService.SelectFlashcards_Subject(SubjectId);
		ParseAllCharts();
		isLoading = false;
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		await JSRuntime.InvokeVoidAsync("flashcardHelpers.renderKatex");
		foreach (int cardId in flippedCards)
		{
			if (parsedCharts.ContainsKey(cardId) && !renderedCharts.Contains(cardId))
			{
				FlashcardChart chart = parsedCharts[cardId];
				await JSRuntime.InvokeVoidAsync("flashcardHelpers.renderChart",
					$"chart-{cardId}",
					chart.ChartType,
					chart.Categories.ToArray(),
					chart.Values.ToArray(),
					chart.Title,
					chart.SeriesName);
				renderedCharts.Add(cardId);
			}
		}
	}

	private void ParseAllCharts()
	{
		parsedCharts.Clear();
		foreach (Flashcard card in flashcards)
		{
			if (!string.IsNullOrEmpty(card.ChartJson))
			{
				FlashcardChart chart = JsonSerializer.Deserialize<FlashcardChart>(card.ChartJson, Helper.JsonSerializerOptions);
				if (chart is not null)
					parsedCharts[card.Id] = chart;
			}
		}
	}

	private bool IsCardFlipped(int cardId)
	{
		return flippedCards.Contains(cardId);
	}

	private void FlipCard(int cardId)
	{
		if (flippedCards.Contains(cardId))
		{
			flippedCards.Remove(cardId);
			renderedCharts.Remove(cardId);
		}
		else
			flippedCards.Add(cardId);
		StateHasChanged();
	}

	private void GoBack()
	{
		NavigationManager.NavigateTo($"/materias/{SubjectId}");
	}

	private void OpenNewCard()
	{
		editingFlashcard = new Flashcard { SubjectId = SubjectId };
		editingChart = new FlashcardChart();
		categoriesRaw = string.Empty;
		valuesRaw = string.Empty;
		showChartBuilder = false;
		showEditor = true;
		editorError = string.Empty;
		chartPreviewError = string.Empty;
	}

	private void EditCard(Flashcard card)
	{
		editingFlashcard = new Flashcard
		{
			Id = card.Id,
			SubjectId = card.SubjectId,
			Front = card.Front,
			Back = card.Back,
			Tags = card.Tags,
			ChartJson = card.ChartJson,
			SortOrder = card.SortOrder
		};
		if (!string.IsNullOrEmpty(card.ChartJson))
		{
			editingChart = JsonSerializer.Deserialize<FlashcardChart>(card.ChartJson, Helper.JsonSerializerOptions) ?? new FlashcardChart();
			categoriesRaw = string.Join(", ", editingChart.Categories);
			valuesRaw = string.Join(", ", editingChart.Values);
			showChartBuilder = true;
		}
		else
		{
			editingChart = new FlashcardChart();
			categoriesRaw = string.Empty;
			valuesRaw = string.Empty;
			showChartBuilder = false;
		}
		showEditor = true;
		editorError = string.Empty;
		chartPreviewError = string.Empty;
	}

	private void CancelEdit()
	{
		showEditor = false;
		editingFlashcard = new();
	}

	private void ToggleChartBuilder()
	{
		showChartBuilder = !showChartBuilder;
		if (!showChartBuilder)
		{
			editingChart = new FlashcardChart();
			categoriesRaw = string.Empty;
			valuesRaw = string.Empty;
			chartPreviewError = string.Empty;
		}
	}

	private bool BuildChartJson()
	{
		if (!showChartBuilder)
		{
			editingFlashcard.ChartJson = string.Empty;
			return true;
		}
		List<string> categories = categoriesRaw
			.Split(',', StringSplitOptions.RemoveEmptyEntries)
			.Select(x => x.Trim())
			.ToList();
		List<double> values = new();
		foreach (string valuePart in valuesRaw.Split(',', StringSplitOptions.RemoveEmptyEntries))
		{
			if (double.TryParse(valuePart.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double parsed))
				values.Add(parsed);
			else
			{
				chartPreviewError = $"Valor inválido: '{valuePart.Trim()}'. Ingresa solo números separados por coma.";
				return false;
			}
		}
		if (values.Count == 0)
		{
			chartPreviewError = "Ingresa al menos un valor numérico para la gráfica.";
			return false;
		}
		editingChart.Categories = categories;
		editingChart.Values = values;
		editingFlashcard.ChartJson = JsonSerializer.Serialize(editingChart, Helper.JsonSerializerOptions);
		chartPreviewError = string.Empty;
		return true;
	}

	private async Task SaveFlashcard()
	{
		if (string.IsNullOrWhiteSpace(editingFlashcard.Front))
		{
			editorError = "El frente de la tarjeta es requerido.";
			return;
		}
		if (string.IsNullOrWhiteSpace(editingFlashcard.Back))
		{
			editorError = "El reverso de la tarjeta es requerido.";
			return;
		}
		if (!BuildChartJson())
			return;
		isSaving = true;
		editorError = string.Empty;
		try
		{
			if (editingFlashcard.Id == 0)
			{
				Flashcard created = await FlashcardService.InsertFlashcard(editingFlashcard);
				flashcards.Add(created);
			}
			else
			{
				Flashcard updated = await FlashcardService.UpdateFlashcard(editingFlashcard);
				int index = flashcards.FindIndex(x => x.Id == updated.Id);
				if (index >= 0)
					flashcards[index] = updated;
				renderedCharts.Remove(updated.Id);
			}
			ParseAllCharts();
			showEditor = false;
		}
		catch (Exception exception)
		{
			editorError = $"Error al guardar: {exception.Message}";
		}
		isSaving = false;
	}

	private async Task DeleteCard(Flashcard card)
	{
		await FlashcardService.DeleteFlashcard(card.Id);
		flashcards.Remove(card);
		flippedCards.Remove(card.Id);
		renderedCharts.Remove(card.Id);
		parsedCharts.Remove(card.Id);
	}
}
