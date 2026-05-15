namespace DrivingSchoolApp.DTO
{
    public class TransportDto
    {
        public int Id { get; set; }
        public string Марка { get; set; }
        public string Модель { get; set; }
        public string Госномер { get; set; }
        public string Категория { get; set; }          // название категории
        public int Пробег { get; set; }
    }
}