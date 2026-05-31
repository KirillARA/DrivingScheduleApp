namespace DrivingSchoolApp.Models;

public class Сотрудник
{
    public int id_сотрудника { get; set; }
    public string ФИО { get; set; } = null!;
    public string телефон { get; set; } = null!;
    public string? email { get; set; }
    public DateOnly дата_рождения { get; set; }
    public string паспорт_серия { get; set; } = null!;
    public string паспорт_номер { get; set; } = null!;
    public DateOnly дата_приема { get; set; }
    public EmployeeRole роль { get; set; }

    public virtual ICollection<ПринадлежностьСотрудника> ПринадлежностиСотрудника { get; set; } = new List<ПринадлежностьСотрудника>();
    public virtual ICollection<ЗакреплениеУченика> ЗакрепленияУченика { get; set; } = new List<ЗакреплениеУченика>();
    public virtual ICollection<Теоретическое_занятие> Теоретические_занятия { get; set; } = new List<Теоретическое_занятие>();
    public virtual ICollection<Практическое_занятие> Практические_занятия { get; set; } = new List<Практическое_занятие>();
}