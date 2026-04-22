namespace Data;

public class AcademicPlannerDBContext(DbContextOptions<AcademicPlannerDBContext> options) : DbContext(options)
{
	public DbSet<Subject> Subjects { get; set; }
	public DbSet<Topic> Topics { get; set; }
	public DbSet<Question> Questions { get; set; }
	public DbSet<StudyEvent> StudyEvents { get; set; }
	public DbSet<StudyResource> StudyResources { get; set; }
	public DbSet<Flashcard> Flashcards { get; set; }
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);
	}
}
