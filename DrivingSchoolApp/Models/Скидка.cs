namespace DrivingSchoolApp.Models;

public class Скидка
{
    public int id_скидки { get; set; }
    public string название { get; set; } = null!;
    public int размер { get; set; }
    public string? описание { get; set; }

    public virtual ICollection<СкидкаТариф> СкидкаТарифы { get; set; } = new List<СкидкаТариф>();
}