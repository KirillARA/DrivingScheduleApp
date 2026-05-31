namespace DrivingSchoolApp.Models;

public class Экзамен
{
    public int id_экзамена { get; set; }
    public int id_категории { get; set; }
    public ExamType тип { get; set; }
    public TransmissionType? коробка_передач { get; set; }

    public virtual Категория_прав? id_категорииNavigation { get; set; }
    public virtual ICollection<РезультатыЭкзамена> РезультатыЭкзаменаs { get; set; } = new List<РезультатыЭкзамена>();
}