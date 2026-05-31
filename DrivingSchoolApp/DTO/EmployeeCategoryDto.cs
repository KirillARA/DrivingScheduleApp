public class EmployeeCategoryDto
{
    public int СотрудникId { get; set; }   
    public int КатегорияId { get; set; }
    public string СотрудникФИО { get; set; }
    public string КатегорияНазвание { get; set; }
    public DateOnly? ДатаПолучения { get; set; }
}