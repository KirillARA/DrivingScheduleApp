namespace DrivingSchoolApp.DTO
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string ФИО { get; set; }
        public DateOnly ДатаРождения { get; set; }
        public string Телефон { get; set; }
        public string Паспорт { get; set; }
        public string Группа { get; set; }             // название группы
        public string Тариф { get; set; }              // название тарифа
        public string Инструктор { get; set; }         // ФИО активного инструктора
    }
}