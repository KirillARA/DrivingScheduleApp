namespace DrivingSchoolApp.DTO
{
    public class TariffDto
    {
        public int Id { get; set; }
        public string Категория { get; set; }       
        public string Название { get; set; }
        public decimal Стоимость { get; set; }
        public int КоличествоЧасов { get; set; }
        public string Описание { get; set; }
        public string КоробкаПередач {  get; set; }

    }
}