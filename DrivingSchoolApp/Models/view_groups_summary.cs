using System;
using System.Collections.Generic;

namespace DrivingSchoolApp.Models;

public partial class view_groups_summary
{
    public int? id_группы { get; set; }
    public string? Группа { get; set; }
    public string? Категория { get; set; }
    public DateOnly? дата_начала { get; set; }
    public DateOnly? дата_окончания { get; set; }
    public int? Текущее_кол_во_учеников { get; set; }
    public int? Максимум { get; set; }

    public string? Статус { get; set; }
}