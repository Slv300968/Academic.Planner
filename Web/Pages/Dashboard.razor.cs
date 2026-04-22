namespace Web.Pages;

public partial class Dashboard : ComponentBase
{
	[Inject] public DashboardService DashboardService { get; set; }

	private DashboardStats stats;
	private bool isLoading = true;

	protected override async Task OnInitializedAsync()
	{
		stats = await DashboardService.SelectDashboardStats();
		isLoading = false;
	}
}
