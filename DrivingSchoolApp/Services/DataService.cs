using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services;

/// <summary>
/// Единый сервис для работы со всеми сущностями базы данных.
/// </summary>
public class DataService
{
    private readonly LibraryContext _context;

    public DataService(LibraryContext context)
    {
        _context = context;
    }

    // ===================================================================
    // 1. Сотрудник (Employee)
    // ===================================================================
    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        return await _context.Сотрудники
            .Select(e => new EmployeeDto
            {
                Id = e.id_сотрудника,
                ФИО = e.ФИО,
                Телефон = e.телефон,
                Email = e.email ?? "",
                ДатаРождения = e.дата_рождения,
                Паспорт = $"{e.паспорт_серия} {e.паспорт_номер}",
                ДатаПриема = e.дата_приема,
                Роль = e.роль
            })
            .ToListAsync();
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
    {
        var e = await _context.Сотрудники.FindAsync(id);
        if (e == null) return null;
        return new EmployeeDto
        {
            Id = e.id_сотрудника,
            ФИО = e.ФИО,
            Телефон = e.телефон,
            Email = e.email ?? "",
            ДатаРождения = e.дата_рождения,
            Паспорт = $"{e.паспорт_серия} {e.паспорт_номер}",
            ДатаПриема = e.дата_приема,
            Роль = e.роль
        };
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto dto)
    {
        var parts = dto.Паспорт.Split(' ');
        var employee = new Сотрудник
        {
            ФИО = dto.ФИО,
            телефон = dto.Телефон,
            email = dto.Email,
            дата_рождения = dto.ДатаРождения,
            паспорт_серия = parts[0],
            паспорт_номер = parts[1],
            дата_приема = dto.ДатаПриема,
            роль = dto.Роль
        };
        _context.Сотрудники.Add(employee);
        await _context.SaveChangesAsync();
        dto.Id = employee.id_сотрудника;
        return dto;
    }

    public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, EmployeeDto dto)
    {
        var employee = await _context.Сотрудники.FindAsync(id);
        if (employee == null) return null;
        var parts = dto.Паспорт.Split(' ');
        employee.ФИО = dto.ФИО;
        employee.телефон = dto.Телефон;
        employee.email = dto.Email;
        employee.дата_рождения = dto.ДатаРождения;
        employee.паспорт_серия = parts[0];
        employee.паспорт_номер = parts[1];
        employee.дата_приема = dto.ДатаПриема;
        employee.роль = dto.Роль;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var e = await _context.Сотрудники.FindAsync(id);
        if (e == null) return false;
        _context.Сотрудники.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 2. Категория прав (LicenseCategory)
    // ===================================================================
    public async Task<IEnumerable<LicenseCategoryDto>> GetAllLicenseCategoriesAsync()
    {
        return await _context.Категории_прав
            .Select(c => new LicenseCategoryDto
            {
                Id = c.id_категории,
                Название = c.название,
                Описание = c.описание ?? ""
            })
            .ToListAsync();
    }

    public async Task<LicenseCategoryDto?> GetLicenseCategoryByIdAsync(int id)
    {
        var c = await _context.Категории_прав.FindAsync(id);
        if (c == null) return null;
        return new LicenseCategoryDto
        {
            Id = c.id_категории,
            Название = c.название,
            Описание = c.описание ?? ""
        };
    }

    public async Task<LicenseCategoryDto> CreateLicenseCategoryAsync(LicenseCategoryDto dto)
    {
        var category = new Категория_прав
        {
            название = dto.Название,
            описание = dto.Описание
        };
        _context.Категории_прав.Add(category);
        await _context.SaveChangesAsync();
        dto.Id = category.id_категории;
        return dto;
    }

    public async Task<LicenseCategoryDto?> UpdateLicenseCategoryAsync(int id, LicenseCategoryDto dto)
    {
        var cat = await _context.Категории_прав.FindAsync(id);
        if (cat == null) return null;
        cat.название = dto.Название;
        cat.описание = dto.Описание;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteLicenseCategoryAsync(int id)
    {
        var cat = await _context.Категории_прав.FindAsync(id);
        if (cat == null) return false;
        _context.Категории_прав.Remove(cat);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 3. Группа (Group)
    // ===================================================================
    public async Task<IEnumerable<GroupDto>> GetAllGroupsAsync()
    {
        return await _context.Группы
            .Include(g => g.id_категорииNavigation)
            .Select(g => new GroupDto
            {
                Id = g.id_группы,
                Название = g.название,
                Категория = g.id_категорииNavigation != null ? g.id_категорииNavigation.название : "",
                ДатаНачала = g.дата_начала,
                ДатаОкончания = g.дата_окончания,
                МаксУчеников = g.макс_учеников,
                ТекущУчеников = g.текущ_учеников,
                Статус = g.статус
            })
            .ToListAsync();
    }

    public async Task<GroupDto?> GetGroupByIdAsync(int id)
    {
        var g = await _context.Группы
            .Include(g => g.id_категорииNavigation)
            .FirstOrDefaultAsync(g => g.id_группы == id);
        if (g == null) return null;
        return new GroupDto
        {
            Id = g.id_группы,
            Название = g.название,
            Категория = g.id_категорииNavigation?.название ?? "",
            ДатаНачала = g.дата_начала,
            ДатаОкончания = g.дата_окончания,
            МаксУчеников = g.макс_учеников,
            ТекущУчеников = g.текущ_учеников,
            Статус = g.статус
        };
    }

    public async Task<GroupDto> CreateGroupAsync(GroupDto dto)
    {
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new Exception("Категория не найдена");
        var group = new Группа
        {
            название = dto.Название,
            id_категории = category.id_категории,
            дата_начала = dto.ДатаНачала,
            дата_окончания = dto.ДатаОкончания,
            макс_учеников = dto.МаксУчеников,
            текущ_учеников = 0,
            статус = dto.Статус
        };
        _context.Группы.Add(group);
        await _context.SaveChangesAsync();
        dto.Id = group.id_группы;
        return dto;
    }

    public async Task<GroupDto?> UpdateGroupAsync(int id, GroupDto dto)
    {
        var group = await _context.Группы.FindAsync(id);
        if (group == null) return null;
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new Exception("Категория не найдена");
        group.название = dto.Название;
        group.id_категории = category.id_категории;
        group.дата_начала = dto.ДатаНачала;
        group.дата_окончания = dto.ДатаОкончания;
        group.макс_учеников = dto.МаксУчеников;
        group.статус = dto.Статус;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteGroupAsync(int id)
    {
        var g = await _context.Группы.FindAsync(id);
        if (g == null) return false;
        _context.Группы.Remove(g);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 4. Тариф (Tariff)
    // ===================================================================
    public async Task<IEnumerable<TariffDto>> GetAllTariffsAsync()
    {
        return await _context.Тарифы
            .Include(t => t.id_категорииNavigation)
            .Select(t => new TariffDto
            {
                Id = t.id_тарифа,
                Категория = t.id_категорииNavigation != null ? t.id_категорииNavigation.название : "",
                Название = t.название,
                Стоимость = t.стоимость,
                КоличествоЧасов = t.количество_часов,
                Описание = t.описание ?? ""
            })
            .ToListAsync();
    }

    public async Task<TariffDto?> GetTariffByIdAsync(int id)
    {
        var t = await _context.Тарифы
            .Include(t => t.id_категорииNavigation)
            .FirstOrDefaultAsync(t => t.id_тарифа == id);
        if (t == null) return null;
        return new TariffDto
        {
            Id = t.id_тарифа,
            Категория = t.id_категорииNavigation?.название ?? "",
            Название = t.название,
            Стоимость = t.стоимость,
            КоличествоЧасов = t.количество_часов,
            Описание = t.описание ?? ""
        };
    }

    public async Task<TariffDto> CreateTariffAsync(TariffDto dto)
    {
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new Exception("Категория не найдена");
        var tariff = new Тариф
        {
            id_категории = category.id_категории,
            название = dto.Название,
            стоимость = dto.Стоимость,
            количество_часов = dto.КоличествоЧасов,
            описание = dto.Описание
        };
        _context.Тарифы.Add(tariff);
        await _context.SaveChangesAsync();
        dto.Id = tariff.id_тарифа;
        return dto;
    }

    public async Task<TariffDto?> UpdateTariffAsync(int id, TariffDto dto)
    {
        var tariff = await _context.Тарифы.FindAsync(id);
        if (tariff == null) return null;
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new Exception("Категория не найдена");
        tariff.id_категории = category.id_категории;
        tariff.название = dto.Название;
        tariff.стоимость = dto.Стоимость;
        tariff.количество_часов = dto.КоличествоЧасов;
        tariff.описание = dto.Описание;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteTariffAsync(int id)
    {
        var t = await _context.Тарифы.FindAsync(id);
        if (t == null) return false;
        _context.Тарифы.Remove(t);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 5. Ученик (Student)
    // ===================================================================
    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
    {
        var students = await _context.Ученики
            .Include(s => s.id_группыNavigation)
            .Include(s => s.id_тарифаNavigation)
            .ToListAsync();

        var result = new List<StudentDto>();
        foreach (var s in students)
        {
            var assignment = await _context.Закрепления_учеников
                .Include(a => a.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(a => a.id_ученика == s.id_ученика && a.дата_окончания == null);
            result.Add(new StudentDto
            {
                Id = s.id_ученика,
                ФИО = s.ФИО,
                Паспорт = $"{s.паспорт_серия} {s.паспорт_номер}",
                ДатаРождения = s.дата_рождения,
                Телефон = s.телефон,
                Группа = s.id_группыNavigation?.название ?? "",
                Тариф = s.id_тарифаNavigation?.название ?? "",
                Инструктор = assignment?.id_сотрудникаNavigation?.ФИО ?? ""
            });
        }
        return result;
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        var s = await _context.Ученики
            .Include(s => s.id_группыNavigation)
            .Include(s => s.id_тарифаNavigation)
            .FirstOrDefaultAsync(s => s.id_ученика == id);
        if (s == null) return null;
        var assignment = await _context.Закрепления_учеников
            .Include(a => a.id_сотрудникаNavigation)
            .FirstOrDefaultAsync(a => a.id_ученика == id && a.дата_окончания == null);
        return new StudentDto
        {
            Id = s.id_ученика,
            ФИО = s.ФИО,
            Паспорт = $"{s.паспорт_серия} {s.паспорт_номер}",
            ДатаРождения = s.дата_рождения,
            Телефон = s.телефон,
            Группа = s.id_группыNavigation?.название ?? "",
            Тариф = s.id_тарифаNavigation?.название ?? "",
            Инструктор = assignment?.id_сотрудникаNavigation?.ФИО ?? ""
        };
    }

    public async Task<StudentDto> CreateStudentAsync(StudentDto dto)
    {
        var group = await _context.Группы.FirstOrDefaultAsync(g => g.название == dto.Группа);
        if (group == null) throw new Exception("Группа не найдена");
        var tariff = await _context.Тарифы.FirstOrDefaultAsync(t => t.название == dto.Тариф);
        if (tariff == null) throw new Exception("Тариф не найден");
        var parts = dto.Паспорт.Split(' ');
        var student = new Ученик
        {
            ФИО = dto.ФИО,
            паспорт_серия = parts[0],
            паспорт_номер = parts[1],
            дата_рождения = dto.ДатаРождения,
            телефон = dto.Телефон,
            id_группы = group.id_группы,
            id_тарифа = tariff.id_тарифа
        };
        _context.Ученики.Add(student);
        await _context.SaveChangesAsync();
        dto.Id = student.id_ученика;
        return dto;
    }

    public async Task<StudentDto?> UpdateStudentAsync(int id, StudentDto dto)
    {
        var student = await _context.Ученики.FindAsync(id);
        if (student == null) return null;
        var group = await _context.Группы.FirstOrDefaultAsync(g => g.название == dto.Группа);
        if (group == null) throw new Exception("Группа не найдена");
        var tariff = await _context.Тарифы.FirstOrDefaultAsync(t => t.название == dto.Тариф);
        if (tariff == null) throw new Exception("Тариф не найден");
        var parts = dto.Паспорт.Split(' ');
        student.ФИО = dto.ФИО;
        student.паспорт_серия = parts[0];
        student.паспорт_номер = parts[1];
        student.дата_рождения = dto.ДатаРождения;
        student.телефон = dto.Телефон;
        student.id_группы = group.id_группы;
        student.id_тарифа = tariff.id_тарифа;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        var s = await _context.Ученики.FindAsync(id);
        if (s == null) return false;
        _context.Ученики.Remove(s);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 6. Транспорт (Vehicle)
    // ===================================================================
    public async Task<IEnumerable<TransportDto>> GetAllVehiclesAsync()
    {
        return await _context.Транспорт
            .Include(t => t.id_категорииNavigation)
            .Select(t => new TransportDto
            {
                Id = t.id_транспорта,
                Марка = t.марка,
                Модель = t.модель,
                Госномер = t.госномер,
                Категория = t.id_категорииNavigation != null ? t.id_категорииNavigation.название : "",
                Пробег = t.пробег
            })
            .ToListAsync();
    }

    public async Task<TransportDto?> GetVehicleByIdAsync(int id)
    {
        var t = await _context.Транспорт
            .Include(t => t.id_категорииNavigation)
            .FirstOrDefaultAsync(t => t.id_транспорта == id);
        if (t == null) return null;
        return new TransportDto
        {
            Id = t.id_транспорта,
            Марка = t.марка,
            Модель = t.модель,
            Госномер = t.госномер,
            Категория = t.id_категорииNavigation?.название ?? "",
            Пробег = t.пробег
        };
    }

    public async Task<TransportDto> CreateVehicleAsync(TransportDto dto)
    {
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new Exception("Категория не найдена");
        var vehicle = new Транспорт
        {
            марка = dto.Марка,
            модель = dto.Модель,
            госномер = dto.Госномер,
            id_категории = category.id_категории,
            пробег = dto.Пробег
        };
        _context.Транспорт.Add(vehicle);
        await _context.SaveChangesAsync();
        dto.Id = vehicle.id_транспорта;
        return dto;
    }

    public async Task<TransportDto?> UpdateVehicleAsync(int id, TransportDto dto)
    {
        var vehicle = await _context.Транспорт.FindAsync(id);
        if (vehicle == null) return null;
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new Exception("Категория не найдена");
        vehicle.марка = dto.Марка;
        vehicle.модель = dto.Модель;
        vehicle.госномер = dto.Госномер;
        vehicle.id_категории = category.id_категории;
        vehicle.пробег = dto.Пробег;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteVehicleAsync(int id)
    {
        var v = await _context.Транспорт.FindAsync(id);
        if (v == null) return false;
        _context.Транспорт.Remove(v);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 7. Теоретическое занятие (TheoryLesson)
    // ===================================================================
    public async Task<IEnumerable<TheoryLessonDto>> GetAllTheoryLessonsAsync()
    {
        return await _context.Теоретические_занятия
            .Include(l => l.id_группыNavigation)
            .Include(l => l.id_преподавателяNavigation)
            .Select(l => new TheoryLessonDto
            {
                Id = l.id_теорзан,
                Группа = l.id_группыNavigation != null ? l.id_группыNavigation.название : "",
                Преподаватель = l.id_преподавателяNavigation != null ? l.id_преподавателяNavigation.ФИО : "",
                Тема = l.тема,
                Дата = l.дата,
                ВремяНачала = l.время_начала,
                ВремяОкончания = l.время_окончания,
                Аудитория = l.аудитория ?? "",
                НомерЗанятия = l.номер_занятия
            })
            .ToListAsync();
    }

    public async Task<TheoryLessonDto?> GetTheoryLessonByIdAsync(int id)
    {
        var l = await _context.Теоретические_занятия
            .Include(l => l.id_группыNavigation)
            .Include(l => l.id_преподавателяNavigation)
            .FirstOrDefaultAsync(l => l.id_теорзан == id);
        if (l == null) return null;
        return new TheoryLessonDto
        {
            Id = l.id_теорзан,
            Группа = l.id_группыNavigation?.название ?? "",
            Преподаватель = l.id_преподавателяNavigation?.ФИО ?? "",
            Тема = l.тема,
            Дата = l.дата,
            ВремяНачала = l.время_начала,
            ВремяОкончания = l.время_окончания,
            Аудитория = l.аудитория ?? "",
            НомерЗанятия = l.номер_занятия
        };
    }

    public async Task<TheoryLessonDto> CreateTheoryLessonAsync(TheoryLessonDto dto)
    {
        var group = await _context.Группы.FirstOrDefaultAsync(g => g.название == dto.Группа);
        if (group == null) throw new Exception("Группа не найдена");
        var teacher = await _context.Сотрудники.FirstOrDefaultAsync(s => s.ФИО == dto.Преподаватель && s.роль == "преподаватель");
        if (teacher == null) throw new Exception("Преподаватель не найден");
        var lesson = new Теоретическое_занятие
        {
            id_группы = group.id_группы,
            id_преподавателя = teacher.id_сотрудника,
            тема = dto.Тема,
            дата = dto.Дата,
            время_начала = dto.ВремяНачала,
            время_окончания = dto.ВремяОкончания,
            аудитория = dto.Аудитория,
            номер_занятия = dto.НомерЗанятия
        };
        _context.Теоретические_занятия.Add(lesson);
        await _context.SaveChangesAsync();
        dto.Id = lesson.id_теорзан;
        return dto;
    }

    public async Task<TheoryLessonDto?> UpdateTheoryLessonAsync(int id, TheoryLessonDto dto)
    {
        var lesson = await _context.Теоретические_занятия.FindAsync(id);
        if (lesson == null) return null;
        var group = await _context.Группы.FirstOrDefaultAsync(g => g.название == dto.Группа);
        if (group == null) throw new Exception("Группа не найдена");
        var teacher = await _context.Сотрудники.FirstOrDefaultAsync(s => s.ФИО == dto.Преподаватель && s.роль == "преподаватель");
        if (teacher == null) throw new Exception("Преподаватель не найден");
        lesson.id_группы = group.id_группы;
        lesson.id_преподавателя = teacher.id_сотрудника;
        lesson.тема = dto.Тема;
        lesson.дата = dto.Дата;
        lesson.время_начала = dto.ВремяНачала;
        lesson.время_окончания = dto.ВремяОкончания;
        lesson.аудитория = dto.Аудитория;
        lesson.номер_занятия = dto.НомерЗанятия;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteTheoryLessonAsync(int id)
    {
        var l = await _context.Теоретические_занятия.FindAsync(id);
        if (l == null) return false;
        _context.Теоретические_занятия.Remove(l);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 8. Практическое занятие (DrivingLesson)
    // ===================================================================
    public async Task<IEnumerable<DrivingLessonDto>> GetAllDrivingLessonsAsync()
    {
        return await _context.Практические_занятия
            .Include(l => l.id_ученикаNavigation)
            .Include(l => l.id_инструктораNavigation)
            .Include(l => l.id_транспортаNavigation)
            .Select(l => new DrivingLessonDto
            {
                Id = l.id_практзан,
                Ученик = l.id_ученикаNavigation != null ? l.id_ученикаNavigation.ФИО : "",
                Инструктор = l.id_инструктораNavigation != null ? l.id_инструктораNavigation.ФИО : "",
                Автомобиль = l.id_транспортаNavigation != null
                    ? $"{l.id_транспортаNavigation.марка} {l.id_транспортаNavigation.модель} ({l.id_транспортаNavigation.госномер})"
                    : "",
                Дата = l.дата,
                ВремяНачала = l.время_начала,
                ВремяОкончания = l.время_окончания
            })
            .ToListAsync();
    }

    public async Task<DrivingLessonDto?> GetDrivingLessonByIdAsync(int id)
    {
        var l = await _context.Практические_занятия
            .Include(l => l.id_ученикаNavigation)
            .Include(l => l.id_инструктораNavigation)
            .Include(l => l.id_транспортаNavigation)
            .FirstOrDefaultAsync(l => l.id_практзан == id);
        if (l == null) return null;
        return new DrivingLessonDto
        {
            Id = l.id_практзан,
            Ученик = l.id_ученикаNavigation?.ФИО ?? "",
            Инструктор = l.id_инструктораNavigation?.ФИО ?? "",
            Автомобиль = l.id_транспортаNavigation != null
                ? $"{l.id_транспортаNavigation.марка} {l.id_транспортаNavigation.модель} ({l.id_транспортаNavigation.госномер})"
                : "",
            Дата = l.дата,
            ВремяНачала = l.время_начала,
            ВремяОкончания = l.время_окончания
        };
    }

    public async Task<DrivingLessonDto> CreateDrivingLessonAsync(DrivingLessonDto dto)
    {
        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.Ученик);
        if (student == null) throw new Exception("Ученик не найден");
        var instructor = await _context.Сотрудники.FirstOrDefaultAsync(s => s.ФИО == dto.Инструктор && s.роль == "инструктор");
        if (instructor == null) throw new Exception("Инструктор не найден");
        var vehicleParts = dto.Автомобиль.Split(' ');
        if (vehicleParts.Length < 3) throw new Exception("Неверный формат автомобиля");
        var mark = vehicleParts[0];
        var model = vehicleParts[1];
        var plate = vehicleParts[2].Trim('(', ')');
        var vehicle = await _context.Транспорт.FirstOrDefaultAsync(v => v.марка == mark && v.модель == model && v.госномер == plate);
        if (vehicle == null) throw new Exception("Автомобиль не найден");
        var lesson = new Практическое_занятие
        {
            id_ученика = student.id_ученика,
            id_инструктора = instructor.id_сотрудника,
            id_транспорта = vehicle.id_транспорта,
            дата = dto.Дата,
            время_начала = dto.ВремяНачала,
            время_окончания = dto.ВремяОкончания
        };
        _context.Практические_занятия.Add(lesson);
        await _context.SaveChangesAsync();
        dto.Id = lesson.id_практзан;
        return dto;
    }

    public async Task<DrivingLessonDto?> UpdateDrivingLessonAsync(int id, DrivingLessonDto dto)
    {
        var lesson = await _context.Практические_занятия.FindAsync(id);
        if (lesson == null) return null;
        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.Ученик);
        if (student == null) throw new Exception("Ученик не найден");
        var instructor = await _context.Сотрудники.FirstOrDefaultAsync(s => s.ФИО == dto.Инструктор && s.роль == "инструктор");
        if (instructor == null) throw new Exception("Инструктор не найден");
        var vehicleParts = dto.Автомобиль.Split(' ');
        if (vehicleParts.Length < 3) throw new Exception("Неверный формат автомобиля");
        var mark = vehicleParts[0];
        var model = vehicleParts[1];
        var plate = vehicleParts[2].Trim('(', ')');
        var vehicle = await _context.Транспорт.FirstOrDefaultAsync(v => v.марка == mark && v.модель == model && v.госномер == plate);
        if (vehicle == null) throw new Exception("Автомобиль не найден");
        lesson.id_ученика = student.id_ученика;
        lesson.id_инструктора = instructor.id_сотрудника;
        lesson.id_транспорта = vehicle.id_транспорта;
        lesson.дата = dto.Дата;
        lesson.время_начала = dto.ВремяНачала;
        lesson.время_окончания = dto.ВремяОкончания;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteDrivingLessonAsync(int id)
    {
        var l = await _context.Практические_занятия.FindAsync(id);
        if (l == null) return false;
        _context.Практические_занятия.Remove(l);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 9. Экзамен (Exam)
    // ===================================================================
    public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
    {
        return await _context.Экзамены
            .Include(e => e.id_категорииNavigation)
            .Select(e => new ExamDto
            {
                Id = e.id_экзамена,
                Категория = e.id_категорииNavigation != null ? e.id_категорииNavigation.название : "",
                Тип = e.тип
            })
            .ToListAsync();
    }

    public async Task<ExamDto?> GetExamByIdAsync(int id)
    {
        var e = await _context.Экзамены
            .Include(e => e.id_категорииNavigation)
            .FirstOrDefaultAsync(e => e.id_экзамена == id);
        if (e == null) return null;
        return new ExamDto
        {
            Id = e.id_экзамена,
            Категория = e.id_категорииNavigation?.название ?? "",
            Тип = e.тип
        };
    }

    public async Task<ExamDto> CreateExamAsync(ExamDto dto)
    {
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new Exception("Категория не найдена");
        var exam = new Экзамен
        {
            id_категории = category.id_категории,
            тип = dto.Тип
        };
        _context.Экзамены.Add(exam);
        await _context.SaveChangesAsync();
        dto.Id = exam.id_экзамена;
        return dto;
    }

    public async Task<ExamDto?> UpdateExamAsync(int id, ExamDto dto)
    {
        var exam = await _context.Экзамены.FindAsync(id);
        if (exam == null) return null;
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new Exception("Категория не найдена");
        exam.id_категории = category.id_категории;
        exam.тип = dto.Тип;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteExamAsync(int id)
    {
        var e = await _context.Экзамены.FindAsync(id);
        if (e == null) return false;
        _context.Экзамены.Remove(e);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 10. Результаты экзамена (ExamResult)
    // ===================================================================
    public async Task<IEnumerable<ExamResultDto>> GetAllExamResultsAsync()
    {
        return await _context.Результаты_экзаменов
            .Include(r => r.id_ученикаNavigation)
            .Include(r => r.id_экзаменаNavigation)
                .ThenInclude(e => e!.id_категорииNavigation)
            .Select(r => new ExamResultDto
            {
                Ученик = r.id_ученикаNavigation != null ? r.id_ученикаNavigation.ФИО : "",
                Экзамен = r.id_экзаменаNavigation != null && r.id_экзаменаNavigation.id_категорииNavigation != null
                    ? $"{r.id_экзаменаNavigation.id_категорииNavigation.название} ({r.id_экзаменаNavigation.тип})"
                    : "",
                ДатаПопытки = r.дата_попытки,
                Результат = r.результат
            })
            .ToListAsync();
    }

    public async Task<ExamResultDto?> GetExamResultByIdAsync(int studentId, int examId, DateOnly date)
    {
        var r = await _context.Результаты_экзаменов
            .Include(r => r.id_ученикаNavigation)
            .Include(r => r.id_экзаменаNavigation)
                .ThenInclude(e => e!.id_категорииNavigation)
            .FirstOrDefaultAsync(r => r.id_ученика == studentId && r.id_экзамена == examId && r.дата_попытки == date);
        if (r == null) return null;
        return new ExamResultDto
        {
            Ученик = r.id_ученикаNavigation?.ФИО ?? "",
            Экзамен = r.id_экзаменаNavigation != null && r.id_экзаменаNavigation.id_категорииNavigation != null
                ? $"{r.id_экзаменаNavigation.id_категорииNavigation.название} ({r.id_экзаменаNavigation.тип})"
                : "",
            ДатаПопытки = r.дата_попытки,
            Результат = r.результат
        };
    }

    public async Task<ExamResultDto> CreateExamResultAsync(ExamResultDto dto)
    {
        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.Ученик);
        if (student == null) throw new Exception("Ученик не найден");
        var parts = dto.Экзамен.Split(' ');
        if (parts.Length < 2) throw new Exception("Неверный формат экзамена");
        var categoryName = parts[0];
        var examType = parts[1].Trim('(', ')');
        var exam = await _context.Экзамены
            .Include(e => e.id_категорииNavigation)
            .FirstOrDefaultAsync(e => e.id_категорииNavigation != null && e.id_категорииNavigation.название == categoryName && e.тип == examType);
        if (exam == null) throw new Exception("Экзамен не найден");
        var result = new РезультатыЭкзамена
        {
            id_ученика = student.id_ученика,
            id_экзамена = exam.id_экзамена,
            дата_попытки = dto.ДатаПопытки,
            результат = dto.Результат
        };
        _context.Результаты_экзаменов.Add(result);
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<ExamResultDto?> UpdateExamResultAsync(int studentId, int examId, DateOnly oldDate, ExamResultDto dto)
    {
        var result = await _context.Результаты_экзаменов.FindAsync(studentId, examId, oldDate);
        if (result == null) return null;
        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.Ученик);
        if (student == null) throw new Exception("Ученик не найден");
        var parts = dto.Экзамен.Split(' ');
        if (parts.Length < 2) throw new Exception("Неверный формат экзамена");
        var categoryName = parts[0];
        var examType = parts[1].Trim('(', ')');
        var exam = await _context.Экзамены
            .Include(e => e.id_категорииNavigation)
            .FirstOrDefaultAsync(e => e.id_категорииNavigation != null && e.id_категорииNavigation.название == categoryName && e.тип == examType);
        if (exam == null) throw new Exception("Экзамен не найден");
        result.id_ученика = student.id_ученика;
        result.id_экзамена = exam.id_экзамена;
        result.дата_попытки = dto.ДатаПопытки;
        result.результат = dto.Результат;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteExamResultAsync(int studentId, int examId, DateOnly date)
    {
        var r = await _context.Результаты_экзаменов.FindAsync(studentId, examId, date);
        if (r == null) return false;
        _context.Результаты_экзаменов.Remove(r);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 11. Скидка (Discount)
    // ===================================================================
    public async Task<IEnumerable<DiscountDto>> GetAllDiscountsAsync()
    {
        return await _context.Скидки
            .Select(d => new DiscountDto
            {
                Id = d.id_скидки,
                Название = d.название,
                Размер = d.размер,
                Описание = d.описание ?? ""
            })
            .ToListAsync();
    }

    public async Task<DiscountDto?> GetDiscountByIdAsync(int id)
    {
        var d = await _context.Скидки.FindAsync(id);
        if (d == null) return null;
        return new DiscountDto
        {
            Id = d.id_скидки,
            Название = d.название,
            Размер = d.размер,
            Описание = d.описание ?? ""
        };
    }

    public async Task<DiscountDto> CreateDiscountAsync(DiscountDto dto)
    {
        var discount = new Скидка
        {
            название = dto.Название,
            размер = dto.Размер,
            описание = dto.Описание
        };
        _context.Скидки.Add(discount);
        await _context.SaveChangesAsync();
        dto.Id = discount.id_скидки;
        return dto;
    }

    public async Task<DiscountDto?> UpdateDiscountAsync(int id, DiscountDto dto)
    {
        var discount = await _context.Скидки.FindAsync(id);
        if (discount == null) return null;
        discount.название = dto.Название;
        discount.размер = dto.Размер;
        discount.описание = dto.Описание;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        var d = await _context.Скидки.FindAsync(id);
        if (d == null) return false;
        _context.Скидки.Remove(d);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 12. СкидкаТариф (DiscountTariff)
    // ===================================================================
    public async Task<IEnumerable<DiscountTariffDto>> GetAllDiscountTariffsAsync()
    {
        return await _context.Скидки_тарифы
            .Include(st => st.id_скидкиNavigation)
            .Include(st => st.id_тарифаNavigation)
            .Select(st => new DiscountTariffDto
            {
                СкидкаНазвание = st.id_скидкиNavigation != null ? st.id_скидкиNavigation.название : "",
                ТарифНазвание = st.id_тарифаNavigation != null ? st.id_тарифаNavigation.название : "",
                ДатаНазначения = st.дата_назначения
            })
            .ToListAsync();
    }

    public async Task<DiscountTariffDto?> GetDiscountTariffByIdAsync(int discountId, int tariffId)
    {
        var st = await _context.Скидки_тарифы
            .Include(st => st.id_скидкиNavigation)
            .Include(st => st.id_тарифаNavigation)
            .FirstOrDefaultAsync(st => st.id_скидки == discountId && st.id_тарифа == tariffId);
        if (st == null) return null;
        return new DiscountTariffDto
        {
            СкидкаНазвание = st.id_скидкиNavigation?.название ?? "",
            ТарифНазвание = st.id_тарифаNavigation?.название ?? "",
            ДатаНазначения = st.дата_назначения
        };
    }

    public async Task<DiscountTariffDto> CreateDiscountTariffAsync(DiscountTariffDto dto)
    {
        var discount = await _context.Скидки.FirstOrDefaultAsync(d => d.название == dto.СкидкаНазвание);
        if (discount == null) throw new Exception("Скидка не найдена");
        var tariff = await _context.Тарифы.FirstOrDefaultAsync(t => t.название == dto.ТарифНазвание);
        if (tariff == null) throw new Exception("Тариф не найден");
        var st = new СкидкаТариф
        {
            id_скидки = discount.id_скидки,
            id_тарифа = tariff.id_тарифа,
            дата_назначения = dto.ДатаНазначения
        };
        _context.Скидки_тарифы.Add(st);
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteDiscountTariffAsync(int discountId, int tariffId)
    {
        var st = await _context.Скидки_тарифы.FindAsync(discountId, tariffId);
        if (st == null) return false;
        _context.Скидки_тарифы.Remove(st);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 13. ПринадлежностьСотрудника (EmployeeCategory)
    // ===================================================================
    public async Task<IEnumerable<EmployeeCategoryDto>> GetAllEmployeeCategoriesAsync()
    {
        return await _context.Принадлежности_сотрудников
            .Include(pc => pc.id_сотрудникаNavigation)
            .Include(pc => pc.id_категорииNavigation)
            .Select(pc => new EmployeeCategoryDto
            {
                СотрудникФИО = pc.id_сотрудникаNavigation != null ? pc.id_сотрудникаNavigation.ФИО : "",
                КатегорияНазвание = pc.id_категорииNavigation != null ? pc.id_категорииNavigation.название : "",
                ДатаПолучения = pc.дата_получения
            })
            .ToListAsync();
    }

    public async Task<EmployeeCategoryDto?> GetEmployeeCategoryByIdAsync(int employeeId, int categoryId)
    {
        var pc = await _context.Принадлежности_сотрудников
            .Include(pc => pc.id_сотрудникаNavigation)
            .Include(pc => pc.id_категорииNavigation)
            .FirstOrDefaultAsync(pc => pc.id_сотрудника == employeeId && pc.id_категории == categoryId);
        if (pc == null) return null;
        return new EmployeeCategoryDto
        {
            СотрудникФИО = pc.id_сотрудникаNavigation?.ФИО ?? "",
            КатегорияНазвание = pc.id_категорииNavigation?.название ?? "",
            ДатаПолучения = pc.дата_получения
        };
    }

    public async Task<EmployeeCategoryDto> CreateEmployeeCategoryAsync(EmployeeCategoryDto dto)
    {
        var employee = await _context.Сотрудники.FirstOrDefaultAsync(e => e.ФИО == dto.СотрудникФИО);
        if (employee == null) throw new Exception("Сотрудник не найден");
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.КатегорияНазвание);
        if (category == null) throw new Exception("Категория не найдена");
        var pc = new ПринадлежностьСотрудника
        {
            id_сотрудника = employee.id_сотрудника,
            id_категории = category.id_категории,
            дата_получения = dto.ДатаПолучения
        };
        _context.Принадлежности_сотрудников.Add(pc);
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteEmployeeCategoryAsync(int employeeId, int categoryId)
    {
        var pc = await _context.Принадлежности_сотрудников.FindAsync(employeeId, categoryId);
        if (pc == null) return false;
        _context.Принадлежности_сотрудников.Remove(pc);
        await _context.SaveChangesAsync();
        return true;
    }

    // ===================================================================
    // 14. Закрепление ученика (StudentAssignment)
    // ===================================================================
    public async Task<IEnumerable<StudentAssignmentDto>> GetAllStudentAssignmentsAsync()
    {
        return await _context.Закрепления_учеников
            .Include(sa => sa.id_ученикаNavigation)
            .Include(sa => sa.id_сотрудникаNavigation)
            .Select(sa => new StudentAssignmentDto
            {
                УченикФИО = sa.id_ученикаNavigation != null ? sa.id_ученикаNavigation.ФИО : "",
                СотрудникФИО = sa.id_сотрудникаNavigation != null ? sa.id_сотрудникаNavigation.ФИО : "",
                ДатаЗакрепления = sa.дата_закрепления,
                ДатаОкончания = sa.дата_окончания
            })
            .ToListAsync();
    }

    public async Task<StudentAssignmentDto?> GetStudentAssignmentByIdAsync(int studentId, int employeeId, DateOnly assignmentDate)
    {
        var sa = await _context.Закрепления_учеников
            .Include(sa => sa.id_ученикаNavigation)
            .Include(sa => sa.id_сотрудникаNavigation)
            .FirstOrDefaultAsync(sa => sa.id_ученика == studentId && sa.id_сотрудника == employeeId && sa.дата_закрепления == assignmentDate);
        if (sa == null) return null;
        return new StudentAssignmentDto
        {
            УченикФИО = sa.id_ученикаNavigation?.ФИО ?? "",
            СотрудникФИО = sa.id_сотрудникаNavigation?.ФИО ?? "",
            ДатаЗакрепления = sa.дата_закрепления,
            ДатаОкончания = sa.дата_окончания
        };
    }

    public async Task<StudentAssignmentDto> CreateStudentAssignmentAsync(StudentAssignmentDto dto)
    {
        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.УченикФИО);
        if (student == null) throw new Exception("Ученик не найден");
        var employee = await _context.Сотрудники.FirstOrDefaultAsync(e => e.ФИО == dto.СотрудникФИО);
        if (employee == null) throw new Exception("Сотрудник не найден");
        var sa = new ЗакреплениеУченика
        {
            id_ученика = student.id_ученика,
            id_сотрудника = employee.id_сотрудника,
            дата_закрепления = dto.ДатаЗакрепления,
            дата_окончания = dto.ДатаОкончания
        };
        _context.Закрепления_учеников.Add(sa);
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<StudentAssignmentDto?> UpdateStudentAssignmentAsync(int studentId, int employeeId, DateOnly assignmentDate, StudentAssignmentDto dto)
    {
        var sa = await _context.Закрепления_учеников.FindAsync(studentId, employeeId, assignmentDate);
        if (sa == null) return null;
        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.УченикФИО);
        if (student == null) throw new Exception("Ученик не найден");
        var employee = await _context.Сотрудники.FirstOrDefaultAsync(e => e.ФИО == dto.СотрудникФИО);
        if (employee == null) throw new Exception("Сотрудник не найден");
        sa.id_ученика = student.id_ученика;
        sa.id_сотрудника = employee.id_сотрудника;
        sa.дата_закрепления = dto.ДатаЗакрепления;
        sa.дата_окончания = dto.ДатаОкончания;
        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteStudentAssignmentAsync(int studentId, int employeeId, DateOnly assignmentDate)
    {
        var sa = await _context.Закрепления_учеников.FindAsync(studentId, employeeId, assignmentDate);
        if (sa == null) return false;
        _context.Закрепления_учеников.Remove(sa);
        await _context.SaveChangesAsync();
        return true;
    }
}