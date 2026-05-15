namespace DrivingSchoolApp.Models;

public class Ученик
{
    public int id_ученика { get; set; }
    public int id_группы { get; set; }
    public int id_тарифа { get; set; }
    public string ФИО { get; set; } = null!;
    public DateOnly дата_рождения { get; set; }
    public string телефон { get; set; } = null!;
    public string паспорт_серия { get; set; } = null!;
    public string паспорт_номер { get; set; } = null!;

    public virtual Группа? id_группыNavigation { get; set; }
    public virtual Тариф? id_тарифаNavigation { get; set; }
    public virtual ICollection<Практическое_занятие> Практическое_занятиеs { get; set; } = new List<Практическое_занятие>();
    public virtual ICollection<РезультатыЭкзамена> РезультатыЭкзаменаs { get; set; } = new List<РезультатыЭкзамена>();
    public virtual ICollection<ЗакреплениеУченика> ЗакрепленияУченика { get; set; } = new List<ЗакреплениеУченика>();
}