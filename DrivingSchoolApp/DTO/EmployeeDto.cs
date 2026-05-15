namespace DrivingSchoolApp.DTO
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string ФИО { get; set; }
        public string Телефон { get; set; }
        public string Email { get; set; }
        public DateOnly ДатаРождения { get; set; }
        public string Паспорт { get; set; }          // серия + номер
        public DateOnly ДатаПриема { get; set; }
        public string Адрес { get; set; }
        public string Роль { get; set; }
    }
}