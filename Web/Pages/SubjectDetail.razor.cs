namespace Web.Pages;

public partial class SubjectDetail : ComponentBase
{
	[Parameter] public int SubjectId { get; set; }
	[Inject] SubjectService SubjectService { get; set; }
	[Inject] TopicService TopicService { get; set; }
	[Inject] StudyResourceService StudyResourceService { get; set; }
	[Inject] NavigationManager NavigationManager { get; set; }

	private Subject subject;
	private List<Topic> topics = new();
	private bool isLoading = true;
	private Topic selectedTopic;
	private List<StudyResource> selectedTopicResources = new();
	private bool showTopicDetail = false;
	private bool savingNotes = false;
	private bool showAddResource = false;
	private StudyResource newResource = new();
	private List<string> resourceTypes = new() { "Video", "PDF", "Artículo", "Apunte", "Otro" };
	private string addResourceError = string.Empty;
	private int expandedResourceId = 0;
	private bool savingResourceNotes = false;

	protected override async Task OnInitializedAsync()
	{
		subject = await SubjectService.SelectSubject(SubjectId);
		topics = await TopicService.SelectTopics_Subject(SubjectId);
		isLoading = false;
	}

	private string GetStatusColor(string status)
	{
		return status switch
		{
			Helper.STATUS_PENDING => "base",
			Helper.STATUS_IN_PROGRESS => "warning",
			Helper.STATUS_MASTERED => "success",
			_ => "base"
		};
	}

	private async Task ChangeStatus(Topic topic, string status)
	{
		await TopicService.UpdateTopicStatus(topic.Id, status);
		topic.Status = status;
		StateHasChanged();
	}

	private async Task OpenTopicDetail(Topic topic)
	{
		selectedTopic = topic;
		showTopicDetail = true;
		showAddResource = false;
		addResourceError = string.Empty;
		selectedTopicResources = new();
		try
		{
			selectedTopicResources = await StudyResourceService.SelectStudyResources_Topic(topic.Id);
		}
		catch (Exception exception)
		{
			addResourceError = $"Error al cargar recursos: {exception.Message}";
		}
		newResource = new StudyResource { TopicId = topic.Id, ResourceType = "Otro" };
	}

	private void ToggleAddResource()
	{
		showAddResource = !showAddResource;
		addResourceError = string.Empty;
		StateHasChanged();
	}

	private void CloseTopicDetail()
	{
		showTopicDetail = false;
		selectedTopic = null;
	}

	private async Task SaveNotes()
	{
		savingNotes = true;
		await TopicService.UpdateTopicNotes(selectedTopic.Id, selectedTopic.Notes);
		savingNotes = false;
	}

	private async Task AddResource()
	{
		if (string.IsNullOrWhiteSpace(newResource.Title))
		{
			addResourceError = "El título es requerido.";
			return;
		}
		try
		{
			newResource.TopicId = selectedTopic.Id;
			StudyResource created = await StudyResourceService.InsertStudyResource(newResource);
			selectedTopicResources.Add(created);
			newResource = new StudyResource { TopicId = selectedTopic.Id, ResourceType = "Otro" };
			showAddResource = false;
			addResourceError = string.Empty;
		}
		catch (Exception ex)
		{
			addResourceError = $"Error al guardar: {ex.Message}";
		}
	}

	private async Task DeleteResource(StudyResource resource)
	{
		await StudyResourceService.DeleteStudyResource(resource.Id);
		selectedTopicResources.Remove(resource);
		if (expandedResourceId == resource.Id)
			expandedResourceId = 0;
	}

	private void ToggleResourceNotes(int resourceId)
	{
		expandedResourceId = expandedResourceId == resourceId ? 0 : resourceId;
		savingResourceNotes = false;
	}

	private async Task SaveResourceNotes(StudyResource resource)
	{
		savingResourceNotes = true;
		await StudyResourceService.UpdateStudyResource(resource);
		savingResourceNotes = false;
	}

	private string GetResourceIcon(string resourceType)
	{
		return resourceType switch
		{
			"Video" => "▶",
			"PDF" => "📄",
			"Artículo" => "📰",
			"Apunte" => "📝",
			_ => "🔗"
		};
	}

	private void GoToFlashcards()
	{
		NavigationManager.NavigateTo($"/materias/{SubjectId}/flashcards");
	}
}
