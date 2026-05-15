namespace DrivingSchoolApp.Models;

public class Практическое_занятие
{
    public int id_практзан { get; set; }
    public int id_ученика { get; set; }
    public int id_инструктора { get; set; }
    public int id_транспорта { get; set; }
    public DateOnly дата { get; set; }
    public TimeOnly время_начала { get; set; }
    public TimeOnly время_окончания { get; set; }

    public virtual Ученик? id_ученикаNavigation { get; set; }
    public virtual Сотрудник? id_инструктораNavigation { get; set; }
    public virtual Транспорт? id_транспортаNavigation { get; set; }
}