namespace DrivingSchoolApp.Models;

public class РезультатыЭкзамена
{
    public int id_ученика { get; set; }
    public int id_экзамена { get; set; }
    public DateOnly дата_попытки { get; set; }
    public string результат { get; set; } = null!; 

    public virtual Ученик? id_ученикаNavigation { get; set; }
    public virtual Экзамен? id_экзаменаNavigation { get; set; }
}