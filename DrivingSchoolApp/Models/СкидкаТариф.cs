namespace DrivingSchoolApp.Models;

public class СкидкаТариф
{
    public int id_скидки { get; set; }
    public int id_тарифа { get; set; }
    public DateOnly дата_назначения { get; set; }

    public virtual Скидка? id_скидкиNavigation { get; set; }
    public virtual Тариф? id_тарифаNavigation { get; set; }
}