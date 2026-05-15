namespace DrivingSchoolApp.Models;

public class ПринадлежностьСотрудника
{
    public int id_сотрудника { get; set; }
    public int id_категории { get; set; }
    public DateOnly? дата_получения { get; set; }

    public virtual Сотрудник? id_сотрудникаNavigation { get; set; }
    public virtual Категория_прав? id_категорииNavigation { get; set; }
}