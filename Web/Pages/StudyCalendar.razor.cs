namespace Web.Pages;

public partial class StudyCalendar : ComponentBase
{
[Inject] SubjectService SubjectService { get; set; }

private List<SubjectProgress> subjects = new();
private bool isLoading = true;
private DateTime startDate = DateTime.Today;
private DateTime endDate = DateTime.Today.AddMonths(3);
private int hoursPerDay = 3;
private bool weekdaysOnly = true;
private string errorMessage = string.Empty;
private bool planGenerated = false;
private int totalStudyDays = 0;
private string activeView = "gantt";
private List<PlanColumn> planColumns = new();
private List<GanttRow> ganttRows = new();
private List<GanttMonth> ganttMonths = new();
private List<GanttWeek> ganttWeeks = new();
private double todayPercent = 0;
private int reviewPeriodDays = 7;
private List<StudyDay> calendarDays = new();

protected override async Task OnInitializedAsync()
{
subjects = await SubjectService.SelectSubjectProgressList();
isLoading = false;
}

private void GeneratePlan()
{
errorMessage = string.Empty;
planGenerated = false;

if (endDate <= startDate)
{
errorMessage = "La fecha de termino debe ser posterior a la fecha de inicio.";
return;
}

List<SubjectProgress> activeSubjects = subjects
.Where(s => s.PendingTopics + s.InProgressTopics > 0)
.OrderByDescending(s => s.PendingTopics + s.InProgressTopics)
.ToList();

if (!activeSubjects.Any())
{
errorMessage = "No hay temas pendientes en ninguna materia.";
return;
}

List<DateOnly> studyDays = BuildStudyDays();
totalStudyDays = studyDays.Count;

if (totalStudyDays == 0)
{
errorMessage = "No hay dias de estudio en el rango seleccionado.";
return;
}

int totalPending = activeSubjects.Sum(s => s.PendingTopics + s.InProgressTopics);
List<SubjectPlanItem> items = new();
int assignedSoFar = 0;

for (int i = 0; i < activeSubjects.Count; i++)
{
SubjectProgress subject = activeSubjects[i];
int pending = subject.PendingTopics + subject.InProgressTopics;
int daysForSubject;
if (i == activeSubjects.Count - 1)
daysForSubject = Math.Max(1, totalStudyDays - assignedSoFar);
else
daysForSubject = Math.Max(1, (int)Math.Round((double)pending / totalPending * totalStudyDays));

assignedSoFar += daysForSubject;
string color = string.IsNullOrEmpty(subject.Color) ? "#3b7dd8" : subject.Color;
items.Add(new SubjectPlanItem
{
SubjectName = subject.Name,
Color = color,
AssignedDays = daysForSubject,
PendingTopics = pending,
TopicsPerDay = daysForSubject > 0 ? (double)pending / daysForSubject : 0
});
}

planColumns = BuildPlanColumns(items, studyDays);
ganttRows = BuildGanttRows(planColumns);
ganttMonths = BuildGanttMonths();
ganttWeeks = BuildGanttWeeks();
todayPercent = BuildTodayPercent();
calendarDays = BuildInterleavedSchedule(items, studyDays);
planGenerated = true;
}

private List<DateOnly> BuildStudyDays()
{
List<DateOnly> days = new();
DateOnly current = DateOnly.FromDateTime(startDate);
DateOnly endOnly = DateOnly.FromDateTime(endDate);
while (current <= endOnly)
{
bool isWeekend = current.DayOfWeek == DayOfWeek.Saturday || current.DayOfWeek == DayOfWeek.Sunday;
if (!weekdaysOnly || !isWeekend)
days.Add(current);
current = current.AddDays(1);
}
return days;
}

private List<PlanColumn> BuildPlanColumns(List<SubjectPlanItem> items, List<DateOnly> studyDays)
{
List<PlanColumn> columns = new();
int dayIndex = 0;

foreach (SubjectPlanItem item in items)
{
List<DateOnly> subjectDays = studyDays.Skip(dayIndex).Take(item.AssignedDays).ToList();
dayIndex += item.AssignedDays;

if (!subjectDays.Any())
continue;

IEnumerable<IGrouping<int, DateOnly>> weekGroups = subjectDays
.GroupBy(d => System.Globalization.ISOWeek.GetWeekOfYear(d.ToDateTime(TimeOnly.MinValue)))
.OrderBy(g => g.Min());

List<PlanCard> cards = new();
int cardNum = 1;
foreach (IGrouping<int, DateOnly> group in weekGroups)
{
DateOnly weekStart = group.OrderBy(d => d).First();
DateOnly weekEnd = group.OrderBy(d => d).Last();
int daysCount = group.Count();
cards.Add(new PlanCard
{
WeekNumber = cardNum++,
WeekStart = weekStart,
WeekEnd = weekEnd,
DaysCount = daysCount,
Hours = daysCount * hoursPerDay,
TopicsEstimated = daysCount * item.TopicsPerDay
});
}

columns.Add(new PlanColumn
{
SubjectName = item.SubjectName,
SubjectShort = GetSubjectShort(item.SubjectName),
Color = item.Color,
BackgroundColor = HexToRgba(item.Color, 0.10),
TotalDays = item.AssignedDays,
PendingTopics = item.PendingTopics,
StartDate = subjectDays.First(),
EndDate = subjectDays.Last(),
WidthPercent = (double)item.AssignedDays / totalStudyDays * 100,
Cards = cards
});
}

return columns;
}

private List<GanttRow> BuildGanttRows(List<PlanColumn> columns)
{
List<GanttRow> rows = new();
double totalCalendarDays = (endDate - startDate).TotalDays;
if (totalCalendarDays <= 0)
return rows;

foreach (PlanColumn col in columns)
{
double offsetDays = (col.StartDate.ToDateTime(TimeOnly.MinValue) - startDate).TotalDays;
double durationDays = (col.EndDate.ToDateTime(TimeOnly.MinValue) - col.StartDate.ToDateTime(TimeOnly.MinValue)).TotalDays + 1;
rows.Add(new GanttRow
{
SubjectName = col.SubjectName,
SubjectShort = col.SubjectShort,
Color = col.Color,
PendingTopics = col.PendingTopics,
TotalDays = col.TotalDays,
StartDate = col.StartDate,
EndDate = col.EndDate,
OffsetPercent = offsetDays / totalCalendarDays * 100,
WidthPercent = Math.Max(0.5, durationDays / totalCalendarDays * 100)
});
}

return rows;
}

private List<GanttMonth> BuildGanttMonths()
{
List<GanttMonth> months = new();
double totalDays = (endDate - startDate).TotalDays;
if (totalDays <= 0)
return months;

DateTime cursor = new DateTime(startDate.Year, startDate.Month, 1);
while (cursor <= endDate)
{
DateTime monthEnd = cursor.AddMonths(1).AddDays(-1);
DateTime segStart = cursor < startDate ? startDate : cursor;
DateTime segEnd = monthEnd > endDate ? endDate : monthEnd;
double width = (segEnd - segStart).TotalDays / totalDays * 100;
months.Add(new GanttMonth
{
Label = cursor.ToString("MMM yyyy"),
WidthPercent = width
});
cursor = cursor.AddMonths(1);
}

return months;
}

private List<GanttWeek> BuildGanttWeeks()
{
List<GanttWeek> weeks = new();
double totalDays = (endDate - startDate).TotalDays;
if (totalDays <= 0)
return weeks;

int maxWeeks = 52;
DateTime cursor = startDate.AddDays(-(((int)startDate.DayOfWeek - 1 + 7) % 7));
int count = 0;
while (cursor <= endDate && count++ < maxWeeks)
{
DateTime weekEnd = cursor.AddDays(6);
DateTime segStart = cursor < startDate ? startDate : cursor;
DateTime segEnd = weekEnd > endDate ? endDate : weekEnd;
double width = (segEnd - segStart).TotalDays / totalDays * 100;
weeks.Add(new GanttWeek
{
Label = segStart.Day.ToString(),
WidthPercent = width
});
cursor = cursor.AddDays(7);
}

return weeks;
}

private double BuildTodayPercent()
{
if (DateTime.Today < startDate || DateTime.Today > endDate)
return -1;
return (DateTime.Today - startDate).TotalDays / (endDate - startDate).TotalDays * 100;
}

private static string GetSubjectShort(string name)
{
if (name.Length <= 10)
return name;
string[] words = name.Split(' ');
return words.Length > 1 ? string.Concat(words[0][0], ". ", words[1]) : name[..9];
}

private static string HexToRgba(string hex, double alpha)
{
if (hex.StartsWith('#'))
hex = hex[1..];
if (hex.Length < 6)
return $"rgba(59,125,216,{alpha})";
int r = Convert.ToInt32(hex[..2], 16);
int g = Convert.ToInt32(hex[2..4], 16);
int b = Convert.ToInt32(hex[4..6], 16);
return $"rgba({r},{g},{b},{alpha})";
}

private class SubjectPlanItem
{
public string SubjectName { get; set; } = string.Empty;
public string Color { get; set; } = string.Empty;
public int AssignedDays { get; set; }
public int PendingTopics { get; set; }
public double TopicsPerDay { get; set; }
}

private class PlanColumn
{
public string SubjectName { get; set; } = string.Empty;
public string SubjectShort { get; set; } = string.Empty;
public string Color { get; set; } = string.Empty;
public string BackgroundColor { get; set; } = string.Empty;
public int TotalDays { get; set; }
public int PendingTopics { get; set; }
public DateOnly StartDate { get; set; }
public DateOnly EndDate { get; set; }
public double WidthPercent { get; set; }
public List<PlanCard> Cards { get; set; } = new();
}

private class PlanCard
{
public int WeekNumber { get; set; }
public DateOnly WeekStart { get; set; }
public DateOnly WeekEnd { get; set; }
public int DaysCount { get; set; }
public int Hours { get; set; }
public double TopicsEstimated { get; set; }
}

private class GanttRow
{
public string SubjectName { get; set; } = string.Empty;
public string SubjectShort { get; set; } = string.Empty;
public string Color { get; set; } = string.Empty;
public int PendingTopics { get; set; }
public int TotalDays { get; set; }
public DateOnly StartDate { get; set; }
public DateOnly EndDate { get; set; }
public double OffsetPercent { get; set; }
public double WidthPercent { get; set; }
}

private class GanttMonth
{
public string Label { get; set; } = string.Empty;
public double WidthPercent { get; set; }
}

private class GanttWeek
{
public string Label { get; set; } = string.Empty;
public double WidthPercent { get; set; }
}

private List<StudyDay> BuildInterleavedSchedule(List<SubjectPlanItem> items, List<DateOnly> allStudyDays)
{
int totalCount = allStudyDays.Count;
int reviewCount = Math.Min(reviewPeriodDays, Math.Max(1, totalCount / 4));
int learningCount = totalCount - reviewCount;

List<DateOnly> learningDaysList = allStudyDays.Take(learningCount).ToList();
List<DateOnly> reviewDaysList = allStudyDays.Skip(learningCount).ToList();

int slotsPerDay = hoursPerDay <= 2 ? 1 : hoursPerDay <= 4 ? 2 : hoursPerDay <= 6 ? 3 : 4;
double hoursPerSlot = Math.Round((double)hoursPerDay / slotsPerDay, 1);

Dictionary<string, int> remaining = items.ToDictionary(x => x.SubjectName, x => x.AssignedDays);
Dictionary<string, string> colors = items.ToDictionary(x => x.SubjectName, x => x.Color);
Dictionary<string, string> types = items.ToDictionary(x => x.SubjectName, x => ClassifySubject(x.SubjectName));

List<StudyDay> result = new();
HashSet<string> yesterdaySubjects = new();

foreach (DateOnly day in learningDaysList)
{
List<string> available = remaining.Where(kv => kv.Value > 0).Select(kv => kv.Key).ToList();
StudyDay studyDay = new() { Date = day, Phase = "Aprendizaje" };
if (!available.Any())
{
result.Add(studyDay);
continue;
}

List<string> analytical = available
.OrderBy(s => yesterdaySubjects.Contains(s) ? 1 : 0)
.ThenByDescending(s => remaining[s])
.Where(s => types[s] == "Analitica")
.ToList();

List<string> humanistic = available
.OrderBy(s => yesterdaySubjects.Contains(s) ? 1 : 0)
.ThenByDescending(s => remaining[s])
.Where(s => types[s] != "Analitica")
.ToList();

HashSet<string> pickedToday = new();
int ai = 0, hi = 0;
for (int slot = 0; slot < slotsPerDay; slot++)
{
string picked = string.Empty;
if (slot % 2 == 0)
{
while (ai < analytical.Count && pickedToday.Contains(analytical[ai])) ai++;
if (ai < analytical.Count) { picked = analytical[ai++]; }
else { while (hi < humanistic.Count && pickedToday.Contains(humanistic[hi])) hi++; if (hi < humanistic.Count) picked = humanistic[hi++]; }
}
else
{
while (hi < humanistic.Count && pickedToday.Contains(humanistic[hi])) hi++;
if (hi < humanistic.Count) { picked = humanistic[hi++]; }
else { while (ai < analytical.Count && pickedToday.Contains(analytical[ai])) ai++; if (ai < analytical.Count) picked = analytical[ai++]; }
}

if (string.IsNullOrEmpty(picked)) break;
pickedToday.Add(picked);
remaining[picked]--;
studyDay.Slots.Add(new StudySlot { SubjectName = picked, Color = colors[picked], Hours = hoursPerSlot, SlotType = "Nuevo" });
}

yesterdaySubjects = pickedToday;
result.Add(studyDay);
}

List<string> allSubjectNames = items.OrderByDescending(x => x.PendingTopics).Select(x => x.SubjectName).ToList();
int cycleIdx = 0;
foreach (DateOnly day in reviewDaysList)
{
StudyDay studyDay = new() { Date = day, Phase = "Repaso" };
HashSet<string> pickedToday = new();
for (int slot = 0; slot < slotsPerDay; slot++)
{
int attempts = 0;
while (attempts < allSubjectNames.Count)
{
string candidate = allSubjectNames[cycleIdx % allSubjectNames.Count];
cycleIdx++;
if (!pickedToday.Contains(candidate))
{
pickedToday.Add(candidate);
studyDay.Slots.Add(new StudySlot { SubjectName = candidate, Color = colors[candidate], Hours = hoursPerSlot, SlotType = "Repaso" });
break;
}
attempts++;
}
}
result.Add(studyDay);
}

return result;
}

private static string ClassifySubject(string name)
{
string lower = name.ToLowerInvariant();
if (lower.Contains("matem") || lower.Contains("fisica") || lower.Contains("quim") ||
lower.Contains("biolog") || lower.Contains("calculo") || lower.Contains("estadist") ||
lower.Contains("logica") || lower.Contains("contab"))
return "Analitica";
return "Humanistica";
}

private class StudyDay
{
public DateOnly Date { get; set; }
public string Phase { get; set; } = string.Empty;
public List<StudySlot> Slots { get; set; } = new();
}

private class StudySlot
{
public string SubjectName { get; set; } = string.Empty;
public string Color { get; set; } = string.Empty;
public double Hours { get; set; }
public string SlotType { get; set; } = string.Empty;
}
}