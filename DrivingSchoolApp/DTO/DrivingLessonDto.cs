public class DrivingLessonDto
{
    public int Id { get; set; }
    public string Ученик { get; set; }
    public string Инструктор { get; set; }
    public string Автомобиль { get; set; }
    public DateOnly Дата { get; set; }
    public TimeOnly ВремяНачала { get; set; }
    public TimeOnly ВремяОкончания { get; set; }
}