public class DiscountTariffDto
{
    public int СкидкаId { get; set; }
    public int ТарифId { get; set; }
    public string СкидкаНазвание { get; set; }
    public string ТарифНазвание { get; set; }
    public DateOnly ДатаНазначения { get; set; }
}