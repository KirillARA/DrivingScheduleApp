namespace DrivingSchoolApp.Models;

public class Транспорт
{
    public int id_транспорта { get; set; }
    public int id_категории { get; set; }
    public string марка { get; set; } = null!;
    public string модель { get; set; } = null!;
    public string госномер { get; set; } = null!;
    public int пробег { get; set; }
    public TransmissionType коробка_передач { get; set; }
    public virtual Категория_прав? id_категорииNavigation { get; set; }
    public virtual ICollection<Практическое_занятие> Практическое_занятиеs { get; set; } = new List<Практическое_занятие>();
    
}