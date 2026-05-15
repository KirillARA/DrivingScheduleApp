namespace DrivingSchoolApp.Models;

public class Теоретическое_занятие
{
    public int id_теорзан { get; set; }
    public int id_группы { get; set; }
    public int id_преподавателя { get; set; }
    public string тема { get; set; } = null!;
    public DateOnly дата { get; set; }
    public TimeOnly время_начала { get; set; }
    public TimeOnly время_окончания { get; set; }
    public string? аудитория { get; set; }
    public int? номер_занятия { get; set; }

    public virtual Группа? id_группыNavigation { get; set; }
    public virtual Сотрудник? id_преподавателяNavigation { get; set; }
}