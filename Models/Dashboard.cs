namespace Models;

public class DashboardStats
{
	public int TotalSubjects { get; set; }
	public int TotalTopics { get; set; }
	public int PendingTopics { get; set; }
	public int InProgressTopics { get; set; }
	public int MasteredTopics { get; set; }
	public double GlobalProgressPercentage { get; set; }
	public int TotalQuestions { get; set; }
	public int TotalStudyMinutes { get; set; }
	public List<SubjectProgress> SubjectProgressList { get; set; } = new();
}
