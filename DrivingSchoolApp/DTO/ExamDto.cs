using DrivingSchoolApp.Models;

public class ExamDto
{
    public int Id { get; set; }
    public string Категория { get; set; } = null!;
    public string Тип { get; set; }
    public string КоробкаПередач {  get; set; }
}