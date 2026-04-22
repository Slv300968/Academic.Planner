namespace Web.Pages;

public partial class SubjectsGrid : ComponentBase
{
	[Inject] SubjectService SubjectService { get; set; }

	private List<SubjectProgress> subjectProgressList = new();
	private bool isLoading = true;

	protected override async Task OnInitializedAsync()
	{
		subjectProgressList = await SubjectService.SelectSubjectProgressList();
		isLoading = false;
	}
}
