namespace DrivingSchoolApp.Models;

public class Категория_прав
{
    public int id_категории { get; set; }
    public string название { get; set; } = null!;
    public string? описание { get; set; }

    public virtual ICollection<Группа> Группаs { get; set; } = new List<Группа>();
    public virtual ICollection<Тариф> Тарифы { get; set; } = new List<Тариф>();
    public virtual ICollection<Транспорт> Транспортs { get; set; } = new List<Транспорт>();
    public virtual ICollection<Экзамен> Экзаменs { get; set; } = new List<Экзамен>();
    public virtual ICollection<ПринадлежностьСотрудника> ПринадлежностиСотрудника { get; set; } = new List<ПринадлежностьСотрудника>();
}