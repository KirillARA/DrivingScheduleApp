namespace DrivingSchoolApp.Models;

public class Группа
{
    public int id_группы { get; set; }
    public string название { get; set; } = null!;
    public int id_категории { get; set; }
    public DateOnly дата_начала { get; set; }
    public DateOnly? дата_окончания { get; set; }
    public GroupStatus статус { get; set; } 
    public int макс_учеников { get; set; }
    public int текущ_учеников { get; set; }

    public virtual Категория_прав? id_категорииNavigation { get; set; }
    public virtual ICollection<Ученик> Ученикs { get; set; } = new List<Ученик>();
    public virtual ICollection<Теоретическое_занятие> Теоретическое_занятиеs { get; set; } = new List<Теоретическое_занятие>();
}