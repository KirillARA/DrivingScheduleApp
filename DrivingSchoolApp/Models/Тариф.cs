namespace DrivingSchoolApp.Models;

public class Тариф
{
    public int id_тарифа { get; set; }
    public int id_категории { get; set; }
    public string название { get; set; } = null!;
    public decimal стоимость { get; set; }
    public int количество_часов { get; set; }
    public string? описание { get; set; }
    public TransmissionType коробка_передач { get; set; }

    public virtual Категория_прав? id_категорииNavigation { get; set; }
    public virtual ICollection<СкидкаТариф> СкидкаТарифы { get; set; } = new List<СкидкаТариф>();
    public virtual ICollection<Ученик> Ученики { get; set; } = new List<Ученик>();
}