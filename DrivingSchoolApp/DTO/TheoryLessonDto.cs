public class TheoryLessonDto
{
    public int Id { get; set; }
    public string Группа { get; set; }
    public string Преподаватель { get; set; }
    public string Тема { get; set; }
    public DateOnly Дата { get; set; }
    public TimeOnly ВремяНачала { get; set; }
    public TimeOnly ВремяОкончания { get; set; }
    public string Аудитория { get; set; }
    public int? НомерЗанятия { get; set; }
}