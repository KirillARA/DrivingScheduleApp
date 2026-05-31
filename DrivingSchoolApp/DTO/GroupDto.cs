using DrivingSchoolApp.Models;

namespace DrivingSchoolApp.DTO
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string Название { get; set; }
        public string Категория { get; set; }         
        public DateOnly ДатаНачала { get; set; }
        public DateOnly? ДатаОкончания { get; set; }
        public string Статус { get; set; }
        public int МаксУчеников { get; set; }
        public int ТекущУчеников { get; set; }
    }
}