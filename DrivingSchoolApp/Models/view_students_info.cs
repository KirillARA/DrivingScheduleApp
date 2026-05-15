using System;
using System.Collections.Generic;

namespace DrivingSchoolApp.Models;

public partial class view_students_info
{
    public int? id_ученика { get; set; }

    public string? ФИО_ученика { get; set; }

    public DateOnly? дата_рождения { get; set; }

    public string? телефон { get; set; }

    public string? Группа { get; set; }

    public string? Категория_прав { get; set; }

    public string? Инструктор { get; set; }
}
