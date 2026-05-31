public class StudentAssignmentDto
{
    public int УченикId { get; set; }
    public int СотрудникId { get; set; }
    public string УченикФИО { get; set; }
    public string СотрудникФИО { get; set; }
    public DateOnly ДатаЗакрепления { get; set; }
    public DateOnly? ДатаОкончания { get; set; }
}
