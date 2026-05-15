using System;
using System.Collections.Generic;

namespace DrivingSchoolApp.Models;

public partial class view_driving_schedule
{
    public int? id_практзан { get; set; }

    public string? Ученик { get; set; }

    public string? Инструктор { get; set; }

    public string? Автомобиль { get; set; }

    public DateOnly? дата { get; set; }

    public TimeOnly? время_начала { get; set; }

    public TimeOnly? время_окончания { get; set; }
}
