namespace DrivingSchoolApp.Models;

public class ЗакреплениеУченика
{
    public int id_ученика { get; set; }
    public int id_сотрудника { get; set; }
    public DateOnly дата_закрепления { get; set; }
    public DateOnly? дата_окончания { get; set; }

    public virtual Ученик? id_ученикаNavigation { get; set; }
    public virtual Сотрудник? id_сотрудникаNavigation { get; set; }
}