namespace DrivingSchoolApp.Models;

public class Экзамен
{
    public int id_экзамена { get; set; }
    public int id_категории { get; set; }
    public string тип { get; set; } = null!; 

    public virtual Категория_прав? id_категорииNavigation { get; set; }
    public virtual ICollection<РезультатыЭкзамена> РезультатыЭкзаменаs { get; set; } = new List<РезультатыЭкзамена>();
}