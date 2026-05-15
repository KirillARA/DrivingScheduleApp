using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DrivingSchoolApp.Models;

public partial class view_exam_result
{
    public string? Ученик { get; set; }
    public string? Категория { get; set; }
    public DateOnly? дата_попытки { get; set; }
    [Column("Тип экзамена")]   
    public string? Тип_экзамена { get; set; }

    [Column("результат")]       
    public string? результат { get; set; }
}