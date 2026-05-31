using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
namespace DrivingSchoolApp.Services;

public class DataService
{
    private readonly LibraryContext _context;

    public DataService(LibraryContext context)
    {
        _context = context;
    }

    

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(string? lastName, string? phone, string? role)
    {
        var query = _context.Сотрудники.AsQueryable();
        if (!string.IsNullOrEmpty(lastName))
            query = query.Where(e => e.ФИО.Contains(lastName));
        if (!string.IsNullOrEmpty(phone))
            query = query.Where(e => e.телефон.Contains(phone));
        if (!string.IsNullOrEmpty(role))
        {
            if (Enum.TryParse<EmployeeRole>(role, true, out var roleEnum))
                query = query.Where(e => e.роль == roleEnum);
            else
                return Enumerable.Empty<EmployeeDto>();
        }

        var employees = await query.ToListAsync();
        return employees.Select(e => new EmployeeDto
        {
            Id = e.id_сотрудника,
            ФИО = e.ФИО,
            Роль = e.роль.ToString(),
            Телефон = e.телефон,
            Email = e.email ?? "",
            ДатаРождения = e.дата_рождения,
            Паспорт = $"{e.паспорт_серия} {e.паспорт_номер}",
            ДатаПриема = e.дата_приема
        });
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
            Роль = e.роль.ToString()
        };
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ФИО) || !ValidationHelper.IsValidRussianText(dto.ФИО))
            throw new ArgumentException("ФИО должно содержать только русские буквы, пробелы, дефисы и точки.");
        if (dto.ФИО.Length > 100)
            throw new ArgumentException("ФИО не может быть длиннее 100 символов.");
        if (!ValidationHelper.IsValidPhone(dto.Телефон))
            throw new ArgumentException("Некорректный формат телефона. Пример: +7(123)456-78-90 или 81234567890.");
        if (!string.IsNullOrWhiteSpace(dto.Email) && !ValidationHelper.IsValidEmail(dto.Email))
            throw new ArgumentException("Некорректный формат email.");
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dto.ДатаРождения.Year;
        if (dto.ДатаРождения > today || age < 16 || age > 70)
            throw new ArgumentException("Дата рождения не может быть в будущем. Возраст сотрудника должен быть от 16 до 70 лет.");
        if (!ValidationHelper.IsValidPassport(dto.Паспорт))
            throw new ArgumentException("Некорректный формат паспорта. Ожидается: XXXX XXXXXX (4 цифры, пробел, 6 цифр).");
        if (dto.ДатаПриема > today)
            throw new ArgumentException("Дата приёма не может быть в будущем.");
        if (!ValidationHelper.ValidEmployeeRoles.Contains(dto.Роль))
            throw new ArgumentException($"Недопустимая роль. Разрешённые значения: {string.Join(", ", ValidationHelper.ValidEmployeeRoles)}");

        var exists = await _context.Сотрудники.AnyAsync(e => e.ФИО == dto.ФИО);
        if (exists) throw new ArgumentException("Сотрудник с таким ФИО уже существует.");

        var parts = dto.Паспорт.Split(' ');
        if (!Enum.TryParse<EmployeeRole>(dto.Роль, true, out var roleEnum))
            throw new ArgumentException("Недопустимая роль");

        var employee = new Сотрудник
        {
            ФИО = dto.ФИО,
            телефон = dto.Телефон,
            email = dto.Email,
            дата_рождения = dto.ДатаРождения,
            паспорт_серия = parts[0],
            паспорт_номер = parts[1],
            дата_приема = dto.ДатаПриема,
            роль = roleEnum
        };
        _context.Сотрудники.Add(employee);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = employee.id_сотрудника;
        return dto;
    }

    public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, EmployeeDto dto)
    {
        var employee = await _context.Сотрудники.FindAsync(id);
        if (employee == null) return null;

        if (string.IsNullOrWhiteSpace(dto.ФИО) || !ValidationHelper.IsValidRussianText(dto.ФИО))
            throw new ArgumentException("ФИО должно содержать только русские буквы, пробелы, дефисы и точки.");
        if (dto.ФИО.Length > 100) throw new ArgumentException("ФИО слишком длинное.");
        if (!ValidationHelper.IsValidPhone(dto.Телефон))
            throw new ArgumentException("Некорректный формат телефона.");
        if (!string.IsNullOrWhiteSpace(dto.Email) && !ValidationHelper.IsValidEmail(dto.Email))
            throw new ArgumentException("Некорректный формат email.");
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dto.ДатаРождения.Year;
        if (dto.ДатаРождения > today || age < 16 || age > 70)
            throw new ArgumentException("Некорректная дата рождения.");
        if (!ValidationHelper.IsValidPassport(dto.Паспорт))
            throw new ArgumentException("Некорректный формат паспорта.");
        if (dto.ДатаПриема > today)
            throw new ArgumentException("Дата приёма не может быть в будущем.");
        if (!ValidationHelper.ValidEmployeeRoles.Contains(dto.Роль))
            throw new ArgumentException($"Недопустимая роль.");

        var parts = dto.Паспорт.Split(' ');
        if (!Enum.TryParse<EmployeeRole>(dto.Роль, true, out var roleEnum))
            throw new ArgumentException("Недопустимая роль");

        employee.ФИО = dto.ФИО;
        employee.телефон = dto.Телефон;
        employee.email = dto.Email;
        employee.дата_рождения = dto.ДатаРождения;
        employee.паспорт_серия = parts[0];
        employee.паспорт_номер = parts[1];
        employee.дата_приема = dto.ДатаПриема;
        employee.роль = roleEnum;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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

    

    public async Task<IEnumerable<LicenseCategoryDto>> GetAllLicenseCategoriesAsync(string? name)
    {
        var query = _context.Категории_прав.AsQueryable();
        if (!string.IsNullOrEmpty(name))
            query = query.Where(c => c.название.Contains(name));
        var categories = await query.ToListAsync();
        return categories.Select(c => new LicenseCategoryDto
        {
            Id = c.id_категории,
            Название = c.название,
            Описание = c.описание ?? ""
        });
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
        if (dto.Название.Length > 10)
            throw new ArgumentException("Название категории не может быть длиннее 10 символов.");
        if (!string.IsNullOrWhiteSpace(dto.Описание) && !ValidationHelper.IsValidRussianText(dto.Описание))
            throw new ArgumentException("Описание должно содержать только русские буквы, пробелы и дефисы.");
        var exists = await _context.Категории_прав.AnyAsync(c => c.название == dto.Название);
        if (exists) throw new ArgumentException("Категория с таким названием уже существует.");
        var category = new Категория_прав { название = dto.Название, описание = dto.Описание };
        _context.Категории_прав.Add(category);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = category.id_категории;
        return dto;
    }

    public async Task<LicenseCategoryDto?> UpdateLicenseCategoryAsync(int id, LicenseCategoryDto dto)
    {
        var cat = await _context.Категории_прав.FindAsync(id);
        if (cat == null) return null;
        if (string.IsNullOrWhiteSpace(dto.Название) || !ValidationHelper.IsValidRussianText(dto.Название))
            throw new ArgumentException("Название категории должно содержать только русские буквы, пробелы и дефисы.");
        if (dto.Название.Length > 10) throw new ArgumentException("Название слишком длинное.");
        if (!string.IsNullOrWhiteSpace(dto.Описание) && !ValidationHelper.IsValidRussianText(dto.Описание))
            throw new ArgumentException("Описание должно содержать только русские буквы, пробелы и дефисы.");
        cat.название = dto.Название;
        cat.описание = dto.Описание;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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



    public async Task<IEnumerable<GroupDto>> GetAllGroupsAsync(string? name, string? status, string? category)
    {
        var sql = @"
        SELECT 
            g.id_группы AS Id,
            g.название AS Название,
            c.название AS Категория,
            g.дата_начала AS ДатаНачала,
            g.дата_окончания AS ДатаОкончания,
            g.макс_учеников AS МаксУчеников,
            g.текущ_учеников AS ТекущУчеников,
            g.статус::text AS Статус
        FROM ""Группа"" g
        LEFT JOIN ""Категория прав"" c ON g.id_категории = c.id_категории
        WHERE (@name IS NULL OR g.название ILIKE '%' || @name || '%')
          AND (@status IS NULL OR g.статус::text ILIKE @status)
          AND (@category IS NULL OR c.название ILIKE '%' || @category || '%')
    ";
        var parameters = new[]
        {
        new NpgsqlParameter("@name", NpgsqlDbType.Text) { Value = name ?? (object)DBNull.Value },
        new NpgsqlParameter("@status", NpgsqlDbType.Text) { Value = status ?? (object)DBNull.Value },
        new NpgsqlParameter("@category", NpgsqlDbType.Text) { Value = category ?? (object)DBNull.Value }
    };
        return await _context.Database.SqlQueryRaw<GroupDto>(sql, parameters).ToListAsync();
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
            Статус = g.статус.ToString()
        };
    }

    public async Task<GroupDto> CreateGroupAsync(GroupDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Название) || !ValidationHelper.IsValidNameWithDigits(dto.Название))
            throw new ArgumentException("Название группы должно содержать только русские буквы, цифры, пробелы и дефисы.");
        if (dto.Название.Length > 50) throw new ArgumentException("Название группы слишком длинное.");
        if (dto.МаксУчеников <= 0 || dto.МаксУчеников > 50)
            throw new ArgumentException("Максимальное количество учеников должно быть от 1 до 50.");
        if (dto.ТекущУчеников < 0 || dto.ТекущУчеников > dto.МаксУчеников)
            throw new ArgumentException("Текущее количество учеников не может превышать максимальное.");
        if (!ValidationHelper.ValidGroupStatuses.Contains(dto.Статус))
            throw new ArgumentException($"Недопустимый статус группы. Разрешены: {string.Join(", ", ValidationHelper.ValidGroupStatuses)}");
        if (dto.ДатаНачала > dto.ДатаОкончания)
            throw new ArgumentException("Дата начала не может быть позже даты окончания.");
        

        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new ArgumentException("Категория не найдена");

        if (!Enum.TryParse<GroupStatus>(dto.Статус, true, out var statusEnum))
            throw new ArgumentException("Недопустимый статус группы");

        var group = new Группа
        {
            название = dto.Название,
            id_категории = category.id_категории,
            дата_начала = dto.ДатаНачала,
            дата_окончания = dto.ДатаОкончания,
            макс_учеников = dto.МаксУчеников,
            текущ_учеников = dto.ТекущУчеников,
            статус = statusEnum
        };
        _context.Группы.Add(group);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = group.id_группы;
        return dto;
    }

    public async Task<GroupDto?> UpdateGroupAsync(int id, GroupDto dto)
    {
        var group = await _context.Группы.FindAsync(id);
        if (group == null) return null;

        if (string.IsNullOrWhiteSpace(dto.Название) || !ValidationHelper.IsValidNameWithDigits(dto.Название))
            throw new ArgumentException("Название группы должно содержать только русские буквы, пробелы и дефисы.");
        if (dto.Название.Length > 50) throw new ArgumentException("Название группы слишком длинное.");
        if (dto.МаксУчеников <= 0 || dto.МаксУчеников > 50)
            throw new ArgumentException("Максимальное количество учеников должно быть от 1 до 50.");
        if (dto.ТекущУчеников < 0 || dto.ТекущУчеников > dto.МаксУчеников)
            throw new ArgumentException("Текущее количество учеников не может превышать максимальное.");
        if (!ValidationHelper.ValidGroupStatuses.Contains(dto.Статус))
            throw new ArgumentException($"Недопустимый статус группы.");
        if (dto.ДатаНачала > dto.ДатаОкончания)
            throw new ArgumentException("Дата начала не может быть позже даты окончания.");
        

        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new ArgumentException("Категория не найдена");

        if (!Enum.TryParse<GroupStatus>(dto.Статус, true, out var statusEnum))
            throw new ArgumentException("Недопустимый статус группы");

        group.название = dto.Название;
        group.id_категории = category.id_категории;
        group.дата_начала = dto.ДатаНачала;
        group.дата_окончания = dto.ДатаОкончания;
        group.макс_учеников = dto.МаксУчеников;
        group.текущ_учеников = dto.ТекущУчеников;
        group.статус = statusEnum;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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



    public async Task<IEnumerable<TariffDto>> GetAllTariffsAsync(string? name, decimal? minPrice, decimal? maxPrice,
    int? minHours, int? maxHours, string? category, string? transmission)
    {
        var sql = @"
        SELECT 
            t.id_тарифа AS Id,
            c.название AS Категория,
            t.название AS Название,
            t.стоимость AS Стоимость,
            t.количество_часов AS КоличествоЧасов,
            t.описание AS Описание,
            t.коробка_передач::text AS КоробкаПередач
        FROM ""Тариф"" t
        LEFT JOIN ""Категория прав"" c ON t.id_категории = c.id_категории
        WHERE (@name IS NULL OR t.название ILIKE '%' || @name || '%')
          AND (@minPrice IS NULL OR t.стоимость >= @minPrice)
          AND (@maxPrice IS NULL OR t.стоимость <= @maxPrice)
          AND (@minHours IS NULL OR t.количество_часов >= @minHours)
          AND (@maxHours IS NULL OR t.количество_часов <= @maxHours)
          AND (@category IS NULL OR c.название ILIKE '%' || @category || '%')
          AND (@transmission IS NULL OR t.коробка_передач::text ILIKE @transmission)
    ";
        var parameters = new[]
        {
        new NpgsqlParameter("@name", NpgsqlDbType.Text) { Value = name ?? (object)DBNull.Value },
        new NpgsqlParameter("@minPrice", NpgsqlDbType.Numeric) { Value = minPrice ?? (object)DBNull.Value },
        new NpgsqlParameter("@maxPrice", NpgsqlDbType.Numeric) { Value = maxPrice ?? (object)DBNull.Value },
        new NpgsqlParameter("@minHours", NpgsqlDbType.Integer) { Value = minHours ?? (object)DBNull.Value },
        new NpgsqlParameter("@maxHours", NpgsqlDbType.Integer) { Value = maxHours ?? (object)DBNull.Value },
        new NpgsqlParameter("@category", NpgsqlDbType.Text) { Value = category ?? (object)DBNull.Value },
        new NpgsqlParameter("@transmission", NpgsqlDbType.Text) { Value = transmission ?? (object)DBNull.Value }
    };
        return await _context.Database.SqlQueryRaw<TariffDto>(sql, parameters).ToListAsync();
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
            Описание = t.описание ?? "",
            КоробкаПередач = t.коробка_передач.ToString()
        };
    }

    public async Task<TariffDto> CreateTariffAsync(TariffDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Название) || !ValidationHelper.IsValidNameWithDigits(dto.Название))
            throw new ArgumentException("Название тарифа должно содержать только русские буквы, пробелы и дефисы.");
        if (dto.Название.Length > 100) throw new ArgumentException("Название слишком длинное.");
        if (dto.Стоимость <= 0 || dto.Стоимость > 500000)
            throw new ArgumentException("Стоимость должна быть положительной и не более 500 000 руб.");
        if (dto.КоличествоЧасов <= 0 || dto.КоличествоЧасов > 200)
            throw new ArgumentException("Количество часов должно быть от 1 до 200.");
        if (!string.IsNullOrWhiteSpace(dto.Описание) && !ValidationHelper.IsValidRussianText(dto.Описание))
            throw new ArgumentException("Описание должно содержать только русские буквы, пробелы и дефисы.");

        
        if (!Enum.TryParse<TransmissionType>(dto.КоробкаПередач, true, out var transmission))
            throw new ArgumentException("Недопустимое значение коробки передач. Разрешены: механика, автомат");

        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new ArgumentException("Категория не найдена");

        var tariff = new Тариф
        {
            id_категории = category.id_категории,
            название = dto.Название,
            стоимость = dto.Стоимость,
            количество_часов = dto.КоличествоЧасов,
            описание = dto.Описание,
            коробка_передач = transmission
        };
        _context.Тарифы.Add(tariff);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = tariff.id_тарифа;
        return dto;
    }

    public async Task<TariffDto?> UpdateTariffAsync(int id, TariffDto dto)
    {
        var tariff = await _context.Тарифы.FindAsync(id);
        if (tariff == null) return null;

        if (string.IsNullOrWhiteSpace(dto.Название) || !ValidationHelper.IsValidNameWithDigits(dto.Название))
            throw new ArgumentException("Название тарифа должно содержать только русские буквы, пробелы и дефисы.");
        if (dto.Название.Length > 100) throw new ArgumentException("Название слишком длинное.");
        if (dto.Стоимость <= 0 || dto.Стоимость > 500000)
            throw new ArgumentException("Стоимость должна быть положительной и не более 500 000 руб.");
        if (dto.КоличествоЧасов <= 0 || dto.КоличествоЧасов > 200)
            throw new ArgumentException("Количество часов должно быть от 1 до 200.");
        if (!string.IsNullOrWhiteSpace(dto.Описание) && !ValidationHelper.IsValidRussianText(dto.Описание))
            throw new ArgumentException("Описание должно содержать только русские буквы, пробелы и дефисы.");

        if (!Enum.TryParse<TransmissionType>(dto.КоробкаПередач, true, out var transmission))
            throw new ArgumentException("Недопустимое значение коробки передач.");

        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new ArgumentException("Категория не найдена");

        tariff.id_категории = category.id_категории;
        tariff.название = dto.Название;
        tariff.стоимость = dto.Стоимость;
        tariff.количество_часов = dto.КоличествоЧасов;
        tariff.описание = dto.Описание;
        tariff.коробка_передач = transmission;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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

    

    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync(string? fullName, string? group, string? tariff, string? instructor)
    {
        var query = _context.Ученики
            .Include(s => s.id_группыNavigation)
            .Include(s => s.id_тарифаNavigation)
            .AsQueryable();
        if (!string.IsNullOrEmpty(fullName))
            query = query.Where(s => s.ФИО.Contains(fullName));
        if (!string.IsNullOrEmpty(group))
            query = query.Where(s => s.id_группыNavigation != null && s.id_группыNavigation.название.Contains(group));
        if (!string.IsNullOrEmpty(tariff))
            query = query.Where(s => s.id_тарифаNavigation != null && s.id_тарифаNavigation.название.Contains(tariff));
        if (!string.IsNullOrEmpty(instructor))
        {
            var instructorIds = await _context.Сотрудники
                .Where(e => e.ФИО.Contains(instructor) && e.роль == EmployeeRole.инструктор)
                .Select(e => e.id_сотрудника)
                .ToListAsync();
            var studentIds = await _context.Закрепления_учеников
                .Where(a => instructorIds.Contains(a.id_сотрудника) && a.дата_окончания == null)
                .Select(a => a.id_ученика)
                .ToListAsync();
            query = query.Where(s => studentIds.Contains(s.id_ученика));
        }

        var students = await query.ToListAsync();
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
        if (string.IsNullOrWhiteSpace(dto.ФИО) || !ValidationHelper.IsValidRussianText(dto.ФИО))
            throw new ArgumentException("ФИО должно содержать только русские буквы, пробелы, дефисы и точки.");
        if (!ValidationHelper.IsValidPassport(dto.Паспорт))
            throw new ArgumentException("Некорректный формат паспорта.");
        if (!ValidationHelper.IsValidPhone(dto.Телефон))
            throw new ArgumentException("Некорректный формат телефона.");

        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dto.ДатаРождения.Year;
        if (dto.ДатаРождения > today || age < 16 || age > 70)
            throw new ArgumentException("Некорректная дата рождения (возраст должен быть 16–70 лет).");

        var group = await _context.Группы.FirstOrDefaultAsync(g => g.название == dto.Группа);
        if (group == null) throw new ArgumentException("Группа не найдена");
        if (group.текущ_учеников >= group.макс_учеников)
            throw new ArgumentException("Группа переполнена.");

        var tariff = await _context.Тарифы.FirstOrDefaultAsync(t => t.название == dto.Тариф);
        if (tariff == null) throw new ArgumentException("Тариф не найден");

        var passportDigits = dto.Паспорт.Replace(" ", "");
        var exists = await _context.Ученики.AnyAsync(s => s.паспорт_серия + s.паспорт_номер == passportDigits);
        if (exists) throw new ArgumentException("Ученик с таким паспортом уже зарегистрирован.");

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
        group.текущ_учеников++;
        _context.Ученики.Add(student);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = student.id_ученика;
        return dto;
    }

    public async Task<StudentDto?> UpdateStudentAsync(int id, StudentDto dto)
    {
        var student = await _context.Ученики.FindAsync(id);
        if (student == null) return null;

        if (string.IsNullOrWhiteSpace(dto.ФИО) || !ValidationHelper.IsValidRussianText(dto.ФИО))
            throw new ArgumentException("ФИО должно содержать только русские буквы, пробелы, дефисы и точки.");
        if (!ValidationHelper.IsValidPassport(dto.Паспорт))
            throw new ArgumentException("Некорректный формат паспорта.");
        if (!ValidationHelper.IsValidPhone(dto.Телефон))
            throw new ArgumentException("Некорректный формат телефона.");

        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dto.ДатаРождения.Year;
        if (dto.ДатаРождения > today || age < 16 || age > 70)
            throw new ArgumentException("Некорректная дата рождения.");

        var group = await _context.Группы.FirstOrDefaultAsync(g => g.название == dto.Группа);
        if (group == null) throw new ArgumentException("Группа не найдена");
        var tariff = await _context.Тарифы.FirstOrDefaultAsync(t => t.название == dto.Тариф);
        if (tariff == null) throw new ArgumentException("Тариф не найден");

        var parts = dto.Паспорт.Split(' ');
        student.ФИО = dto.ФИО;
        student.паспорт_серия = parts[0];
        student.паспорт_номер = parts[1];
        student.дата_рождения = dto.ДатаРождения;
        student.телефон = dto.Телефон;
        student.id_группы = group.id_группы;
        student.id_тарифа = tariff.id_тарифа;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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



    public async Task<IEnumerable<TransportDto>> GetAllVehiclesAsync(string? mark, string? model, string? category, string? transmission)
    {
        var sql = @"
        SELECT 
            v.id_транспорта AS Id,
            v.марка AS Марка,
            v.модель AS Модель,
            v.госномер AS Госномер,
            c.название AS Категория,
            v.пробег AS Пробег,
            v.коробка_передач::text AS КоробкаПередач
        FROM ""Транспорт"" v
        LEFT JOIN ""Категория прав"" c ON v.id_категории = c.id_категории
        WHERE (@mark IS NULL OR v.марка ILIKE '%' || @mark || '%')
          AND (@model IS NULL OR v.модель ILIKE '%' || @model || '%')
          AND (@category IS NULL OR c.название ILIKE '%' || @category || '%')
          AND (@transmission IS NULL OR v.коробка_передач::text ILIKE @transmission)
    ";
        var parameters = new[]
        {
        new NpgsqlParameter("@mark", NpgsqlDbType.Text) { Value = mark ?? (object)DBNull.Value },
        new NpgsqlParameter("@model", NpgsqlDbType.Text) { Value = model ?? (object)DBNull.Value },
        new NpgsqlParameter("@category", NpgsqlDbType.Text) { Value = category ?? (object)DBNull.Value },
        new NpgsqlParameter("@transmission", NpgsqlDbType.Text) { Value = transmission ?? (object)DBNull.Value }
    };
        return await _context.Database.SqlQueryRaw<TransportDto>(sql, parameters).ToListAsync();
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
            Пробег = t.пробег,
            КоробкаПередач = t.коробка_передач.ToString()
        };
    }

    public async Task<TransportDto> CreateVehicleAsync(TransportDto dto)
    {
      
        if (string.IsNullOrWhiteSpace(dto.Госномер) || dto.Госномер.Length > 15)
            throw new ArgumentException("Некорректный государственный номер.");
        if (dto.Пробег < 0)
            throw new ArgumentException("Пробег не может быть отрицательным.");

        if (!Enum.TryParse<TransmissionType>(dto.КоробкаПередач, true, out var transmission))
            throw new ArgumentException("Недопустимое значение коробки передач. Разрешены: механика, автомат");

        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new ArgumentException("Категория не найдена");

        var exists = await _context.Транспорт.AnyAsync(v => v.госномер == dto.Госномер);
        if (exists) throw new ArgumentException("Автомобиль с таким госномером уже существует.");

        var vehicle = new Транспорт
        {
            марка = dto.Марка,
            модель = dto.Модель,
            госномер = dto.Госномер,
            id_категории = category.id_категории,
            пробег = dto.Пробег,
            коробка_передач = transmission
        };
        _context.Транспорт.Add(vehicle);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = vehicle.id_транспорта;
        return dto;
    }

    public async Task<TransportDto?> UpdateVehicleAsync(int id, TransportDto dto)
    {
        var vehicle = await _context.Транспорт.FindAsync(id);
        if (vehicle == null) return null;

        if (string.IsNullOrWhiteSpace(dto.Госномер) || dto.Госномер.Length > 15)
            throw new ArgumentException("Некорректный государственный номер.");
        if (dto.Пробег < 0)
            throw new ArgumentException("Пробег не может быть отрицательным.");

        if (!Enum.TryParse<TransmissionType>(dto.КоробкаПередач, true, out var transmission))
            throw new ArgumentException("Недопустимое значение коробки передач.");

        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new ArgumentException("Категория не найдена");

        vehicle.марка = dto.Марка;
        vehicle.модель = dto.Модель;
        vehicle.госномер = dto.Госномер;
        vehicle.id_категории = category.id_категории;
        vehicle.пробег = dto.Пробег;
        vehicle.коробка_передач = transmission;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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

    

    public async Task<IEnumerable<TheoryLessonDto>> GetAllTheoryLessonsAsync(string? group, string? teacher, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var query = _context.Теоретические_занятия
            .Include(l => l.id_группыNavigation)
            .Include(l => l.id_преподавателяNavigation)
            .AsQueryable();
        if (!string.IsNullOrEmpty(group))
            query = query.Where(l => l.id_группыNavigation != null && l.id_группыNavigation.название.Contains(group));
        if (!string.IsNullOrEmpty(teacher))
            query = query.Where(l => l.id_преподавателяNavigation != null && l.id_преподавателяNavigation.ФИО.Contains(teacher));
        if (dateFrom.HasValue)
            query = query.Where(l => l.дата >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(l => l.дата <= dateTo.Value);

        var lessons = await query.ToListAsync();
        return lessons.Select(l => new TheoryLessonDto
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
        });
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
        if (string.IsNullOrWhiteSpace(dto.Тема) || !ValidationHelper.IsValidRussianText(dto.Тема))
            throw new ArgumentException("Тема должна содержать только русские буквы, пробелы и дефисы.");
        if (dto.НомерЗанятия <= 0)
            throw new ArgumentException("Номер занятия должен быть положительным.");
        if (dto.ВремяНачала >= dto.ВремяОкончания)
            throw new ArgumentException("Время начала должно быть раньше времени окончания.");
        if (dto.Дата < DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата занятия не может быть в прошлом.");

        var group = await _context.Группы.FirstOrDefaultAsync(g => g.название == dto.Группа);
        if (group == null) throw new ArgumentException("Группа не найдена");
        var teacher = await _context.Сотрудники.FirstOrDefaultAsync(s => s.ФИО == dto.Преподаватель && s.роль == EmployeeRole.преподаватель);
        if (teacher == null) throw new ArgumentException("Преподаватель не найден");

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
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = lesson.id_теорзан;
        return dto;
    }

    public async Task<TheoryLessonDto?> UpdateTheoryLessonAsync(int id, TheoryLessonDto dto)
    {
        var lesson = await _context.Теоретические_занятия.FindAsync(id);
        if (lesson == null) return null;

        if (string.IsNullOrWhiteSpace(dto.Тема) || !ValidationHelper.IsValidRussianText(dto.Тема))
            throw new ArgumentException("Тема должна содержать только русские буквы, пробелы и дефисы.");
        if (dto.НомерЗанятия <= 0)
            throw new ArgumentException("Номер занятия должен быть положительным.");
        if (dto.ВремяНачала >= dto.ВремяОкончания)
            throw new ArgumentException("Время начала должно быть раньше времени окончания.");
        if (dto.Дата < DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата занятия не может быть в прошлом.");

        var group = await _context.Группы.FirstOrDefaultAsync(g => g.название == dto.Группа);
        if (group == null) throw new ArgumentException("Группа не найдена");
        var teacher = await _context.Сотрудники.FirstOrDefaultAsync(s => s.ФИО == dto.Преподаватель && s.роль == EmployeeRole.преподаватель);
        if (teacher == null) throw new ArgumentException("Преподаватель не найден");

        lesson.id_группы = group.id_группы;
        lesson.id_преподавателя = teacher.id_сотрудника;
        lesson.тема = dto.Тема;
        lesson.дата = dto.Дата;
        lesson.время_начала = dto.ВремяНачала;
        lesson.время_окончания = dto.ВремяОкончания;
        lesson.аудитория = dto.Аудитория;
        lesson.номер_занятия = dto.НомерЗанятия;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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

    

    public async Task<IEnumerable<DrivingLessonDto>> GetAllDrivingLessonsAsync(string? student, string? instructor, string? vehicle, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var query = _context.Практические_занятия
            .Include(l => l.id_ученикаNavigation)
            .Include(l => l.id_инструктораNavigation)
            .Include(l => l.id_транспортаNavigation)
            .AsQueryable();
        if (!string.IsNullOrEmpty(student))
            query = query.Where(l => l.id_ученикаNavigation != null && l.id_ученикаNavigation.ФИО.Contains(student));
        if (!string.IsNullOrEmpty(instructor))
            query = query.Where(l => l.id_инструктораNavigation != null && l.id_инструктораNavigation.ФИО.Contains(instructor));
        if (!string.IsNullOrEmpty(vehicle))
            query = query.Where(l => l.id_транспортаNavigation != null &&
                (l.id_транспортаNavigation.марка.Contains(vehicle) ||
                 l.id_транспортаNavigation.модель.Contains(vehicle) ||
                 l.id_транспортаNavigation.госномер.Contains(vehicle)));
        if (dateFrom.HasValue)
            query = query.Where(l => l.дата >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(l => l.дата <= dateTo.Value);

        var lessons = await query.ToListAsync();
        return lessons.Select(l => new DrivingLessonDto
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
        });
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
        if (student == null) throw new ArgumentException("Ученик не найден");
        var instructor = await _context.Сотрудники.FirstOrDefaultAsync(s => s.ФИО == dto.Инструктор && s.роль == EmployeeRole.инструктор);
        if (instructor == null) throw new ArgumentException("Инструктор не найден");
        var vehicleParts = dto.Автомобиль.Split(' ');
        if (vehicleParts.Length < 3) throw new ArgumentException("Неверный формат автомобиля");
        var mark = vehicleParts[0];
        var model = vehicleParts[1];
        var plate = vehicleParts[2].Trim('(', ')');
        var vehicle = await _context.Транспорт.FirstOrDefaultAsync(v => v.марка == mark && v.модель == model && v.госномер == plate);
        if (vehicle == null) throw new ArgumentException("Автомобиль не найден");
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
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = lesson.id_практзан;
        return dto;
    }

    public async Task<DrivingLessonDto?> UpdateDrivingLessonAsync(int id, DrivingLessonDto dto)
    {
        var lesson = await _context.Практические_занятия.FindAsync(id);
        if (lesson == null) return null;
        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.Ученик);
        if (student == null) throw new ArgumentException("Ученик не найден");
        var instructor = await _context.Сотрудники.FirstOrDefaultAsync(s => s.ФИО == dto.Инструктор && s.роль == EmployeeRole.инструктор);
        if (instructor == null) throw new ArgumentException("Инструктор не найден");
        var vehicleParts = dto.Автомобиль.Split(' ');
        if (vehicleParts.Length < 3) throw new ArgumentException("Неверный формат автомобиля");
        var mark = vehicleParts[0];
        var model = vehicleParts[1];
        var plate = vehicleParts[2].Trim('(', ')');
        var vehicle = await _context.Транспорт.FirstOrDefaultAsync(v => v.марка == mark && v.модель == model && v.госномер == plate);
        if (vehicle == null) throw new ArgumentException("Автомобиль не найден");
        lesson.id_ученика = student.id_ученика;
        lesson.id_инструктора = instructor.id_сотрудника;
        lesson.id_транспорта = vehicle.id_транспорта;
        lesson.дата = dto.Дата;
        lesson.время_начала = dto.ВремяНачала;
        lesson.время_окончания = dto.ВремяОкончания;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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



    public async Task<IEnumerable<ExamDto>> GetAllExamsAsync(string? category, string? type, string? transmission)
    {
        var sql = @"
        SELECT 
            e.id_экзамена AS Id,
            c.название AS Категория,
            e.тип::text AS Тип,
            e.коробка_передач::text AS КоробкаПередач
        FROM ""Экзамен"" e
        LEFT JOIN ""Категория прав"" c ON e.id_категории = c.id_категории
        WHERE (@category IS NULL OR c.название ILIKE '%' || @category || '%')
          AND (@type IS NULL OR e.тип::text ILIKE @type)
          AND (@transmission IS NULL OR e.коробка_передач::text ILIKE @transmission)
    ";
        var parameters = new[]
        {
        new NpgsqlParameter("@category", NpgsqlDbType.Text) { Value = category ?? (object)DBNull.Value },
        new NpgsqlParameter("@type", NpgsqlDbType.Text) { Value = type ?? (object)DBNull.Value },
        new NpgsqlParameter("@transmission", NpgsqlDbType.Text) { Value = transmission ?? (object)DBNull.Value }
    };
        return await _context.Database.SqlQueryRaw<ExamDto>(sql, parameters).ToListAsync();
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
            Тип = e.тип.ToString(),
            КоробкаПередач = e.коробка_передач?.ToString() ?? ""
        };
    }

    public async Task<ExamDto> CreateExamAsync(ExamDto dto)
    {
        if (!ValidationHelper.ValidExamTypes.Contains(dto.Тип))
            throw new ArgumentException($"Недопустимый тип экзамена. Разрешены: {string.Join(", ", ValidationHelper.ValidExamTypes)}");

        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new ArgumentException("Категория не найдена");

        if (!Enum.TryParse<ExamType>(dto.Тип, true, out var typeEnum))
            throw new ArgumentException("Недопустимый тип экзамена");

        // Преобразование коробки передач (только для вождения)
        TransmissionType? transmission = null;
        if (typeEnum == ExamType.вождение)
        {
            if (string.IsNullOrEmpty(dto.КоробкаПередач))
                throw new ArgumentException("Для экзамена по вождению необходимо указать коробку передач");
            if (!Enum.TryParse<TransmissionType>(dto.КоробкаПередач, true, out var t))
                throw new ArgumentException("Недопустимое значение коробки передач. Разрешены: механика, автомат");
            transmission = t;
        }

        var exists = await _context.Экзамены.AnyAsync(e => e.id_категории == category.id_категории && e.тип == typeEnum && e.коробка_передач == transmission);
        if (exists) throw new ArgumentException("Экзамен для этой категории и типа уже существует.");

        var exam = new Экзамен
        {
            id_категории = category.id_категории,
            тип = typeEnum,
            коробка_передач = transmission   
        };
        _context.Экзамены.Add(exam);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = exam.id_экзамена;
        return dto;
    }

    public async Task<ExamDto?> UpdateExamAsync(int id, ExamDto dto)
    {
        var exam = await _context.Экзамены.FindAsync(id);
        if (exam == null) return null;

        if (!ValidationHelper.ValidExamTypes.Contains(dto.Тип))
            throw new ArgumentException($"Недопустимый тип экзамена.");

        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.Категория);
        if (category == null) throw new ArgumentException("Категория не найдена");

        if (!Enum.TryParse<ExamType>(dto.Тип, true, out var typeEnum))
            throw new ArgumentException("Недопустимый тип экзамена");

        TransmissionType? transmission = null;
        if (typeEnum == ExamType.вождение)
        {
            if (string.IsNullOrEmpty(dto.КоробкаПередач))
                throw new ArgumentException("Для экзамена по вождению необходимо указать коробку передач");
            if (!Enum.TryParse<TransmissionType>(dto.КоробкаПередач, true, out var t))
                throw new ArgumentException("Недопустимое значение коробки передач");
            transmission = t;
        }

        exam.id_категории = category.id_категории;
        exam.тип = typeEnum;
        exam.коробка_передач = transmission;  
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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



    public async Task<IEnumerable<ExamResultDto>> GetAllExamResultsAsync(string? student, string? exam, string? result, DateOnly? dateFrom, DateOnly? dateTo)
    {
        string? dbResult = string.IsNullOrEmpty(result) ? null : result.Replace("_", " ");

        string? category = null;
        string? examType = null;
        if (!string.IsNullOrEmpty(exam))
        {
            var match = System.Text.RegularExpressions.Regex.Match(exam, @"^(.*)\s*\((.*)\)$");
            if (match.Success)
            {
                category = match.Groups[1].Value.Trim();
                examType = match.Groups[2].Value.Trim();
            }
            else
            {
                category = exam;
                examType = exam;
            }
        }

        var sql = @"
        SELECT 
            р.id_ученика AS УченикId,
            р.id_экзамена AS ЭкзаменId,
            р.дата_попытки AS ДатаПопытки,
            у.""ФИО"" AS Ученик,
            к.название || ' (' || э.тип::text || ')' AS Экзамен,
            р.результат::text AS Результат
        FROM ""РезультатыЭкзамена"" р
        JOIN ""Ученик"" у ON р.id_ученика = у.id_ученика
        JOIN ""Экзамен"" э ON р.id_экзамена = э.id_экзамена
        JOIN ""Категория прав"" к ON э.id_категории = к.id_категории
        WHERE (@student IS NULL OR у.""ФИО"" ILIKE '%' || @student || '%')
          AND (@category IS NULL OR к.название ILIKE '%' || @category || '%')
          AND (@examType IS NULL OR э.тип::text ILIKE '%' || @examType || '%')
          AND (@result IS NULL OR р.результат::text = @result)
          AND (@dateFrom IS NULL OR р.дата_попытки >= @dateFrom)
          AND (@dateTo IS NULL OR р.дата_попытки <= @dateTo)
    ";

        var parameters = new[]
        {
        new NpgsqlParameter("@student", NpgsqlDbType.Text) { Value = student ?? (object)DBNull.Value },
        new NpgsqlParameter("@category", NpgsqlDbType.Text) { Value = category ?? (object)DBNull.Value },
        new NpgsqlParameter("@examType", NpgsqlDbType.Text) { Value = examType ?? (object)DBNull.Value },
        new NpgsqlParameter("@result", NpgsqlDbType.Text) { Value = dbResult ?? (object)DBNull.Value },
        new NpgsqlParameter("@dateFrom", NpgsqlDbType.Date) { Value = dateFrom ?? (object)DBNull.Value },
        new NpgsqlParameter("@dateTo", NpgsqlDbType.Date) { Value = dateTo ?? (object)DBNull.Value }
    };

        return await _context.Database.SqlQueryRaw<ExamResultDto>(sql, parameters).ToListAsync();
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
                ? $"{r.id_экзаменаNavigation.id_категорииNavigation.название} ({r.id_экзаменаNavigation.тип.ToString()})"
                : "",
            ДатаПопытки = r.дата_попытки,
            Результат = r.результат.ToString()
        };
    }

    public async Task<ExamResultDto> CreateExamResultAsync(ExamResultDto dto)
    {
        var validResults = new[] { "сдал", "не сдал", "зачет", "незачет" };
        if (string.IsNullOrWhiteSpace(dto.Результат) || !validResults.Contains(dto.Результат.ToLower()))
            throw new ArgumentException("Результат должен быть: 'сдал' или 'не сдал'.");

        if (dto.ДатаПопытки > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата попытки не может быть в будущем.");

        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.Ученик);
        if (student == null) throw new ArgumentException("Ученик не найден");

        var parts = dto.Экзамен.Split(' ');
        if (parts.Length < 2) throw new ArgumentException("Неверный формат экзамена");
        var categoryName = parts[0];
        var examType = parts[1].Trim('(', ')');
        var exam = await _context.Экзамены
            .Include(e => e.id_категорииNavigation)
            .FirstOrDefaultAsync(e => e.id_категорииNavigation != null && e.id_категорииNavigation.название == categoryName && e.тип.ToString() == examType);
        if (exam == null) throw new ArgumentException("Экзамен не найден");

        if (!Enum.TryParse<ExamResult>(dto.Результат, true, out var resultEnum))
            throw new ArgumentException("Недопустимый результат экзамена");

        var result = new РезультатыЭкзамена
        {
            id_ученика = student.id_ученика,
            id_экзамена = exam.id_экзамена,
            дата_попытки = dto.ДатаПопытки,
            результат = resultEnum
        };
        _context.Результаты_экзаменов.Add(result);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        return dto;
    }

    public async Task<ExamResultDto?> UpdateExamResultAsync(int studentId, int examId, DateOnly oldDate, ExamResultDto dto)
    {
        var result = await _context.Результаты_экзаменов.FindAsync(studentId, examId, oldDate);
        if (result == null) return null;

        var validResults = new[] { "сдал", "не сдал", "зачет", "незачет" };
        if (string.IsNullOrWhiteSpace(dto.Результат) || !validResults.Contains(dto.Результат.ToLower()))
            throw new ArgumentException("Результат должен быть: 'сдал' или 'не сдал'.");

        if (dto.ДатаПопытки > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата попытки не может быть в будущем.");

        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.Ученик);
        if (student == null) throw new ArgumentException("Ученик не найден");

        var parts = dto.Экзамен.Split(' ');
        if (parts.Length < 2) throw new ArgumentException("Неверный формат экзамена");
        var categoryName = parts[0];
        var examType = parts[1].Trim('(', ')');
        var exam = await _context.Экзамены
            .Include(e => e.id_категорииNavigation)
            .FirstOrDefaultAsync(e => e.id_категорииNavigation != null && e.id_категорииNavigation.название == categoryName && e.тип.ToString() == examType);
        if (exam == null) throw new ArgumentException("Экзамен не найден");

        if (!Enum.TryParse<ExamResult>(dto.Результат, true, out var resultEnum))
            throw new ArgumentException("Недопустимый результат экзамена");

        result.id_ученика = student.id_ученика;
        result.id_экзамена = exam.id_экзамена;
        result.дата_попытки = dto.ДатаПопытки;
        result.результат = resultEnum;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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

    

    public async Task<IEnumerable<DiscountDto>> GetAllDiscountsAsync(string? name, int? minPercent, int? maxPercent)
    {
        var query = _context.Скидки.AsQueryable();
        if (!string.IsNullOrEmpty(name))
            query = query.Where(d => d.название.Contains(name));
        if (minPercent.HasValue)
            query = query.Where(d => d.размер >= minPercent.Value);
        if (maxPercent.HasValue)
            query = query.Where(d => d.размер <= maxPercent.Value);

        var discounts = await query.ToListAsync();
        return discounts.Select(d => new DiscountDto
        {
            Id = d.id_скидки,
            Название = d.название,
            Размер = d.размер,
            Описание = d.описание ?? ""
        });
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
        if (string.IsNullOrWhiteSpace(dto.Название) || !ValidationHelper.IsValidRussianText(dto.Название))
            throw new ArgumentException("Название скидки должно содержать только русские буквы, пробелы и дефисы.");
        if (dto.Размер <= 0 || dto.Размер > 100)
            throw new ArgumentException("Размер скидки должен быть от 1 до 100%.");
        if (!string.IsNullOrWhiteSpace(dto.Описание) && !ValidationHelper.IsValidRussianText(dto.Описание))
            throw new ArgumentException("Описание должно содержать только русские буквы, пробелы и дефисы.");

        var discount = new Скидка
        {
            название = dto.Название,
            размер = dto.Размер,
            описание = dto.Описание
        };
        _context.Скидки.Add(discount);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        dto.Id = discount.id_скидки;
        return dto;
    }

    public async Task<DiscountDto?> UpdateDiscountAsync(int id, DiscountDto dto)
    {
        var discount = await _context.Скидки.FindAsync(id);
        if (discount == null) return null;

        if (string.IsNullOrWhiteSpace(dto.Название) || !ValidationHelper.IsValidRussianText(dto.Название))
            throw new ArgumentException("Название скидки должно содержать только русские буквы, пробелы и дефисы.");
        if (dto.Размер <= 0 || dto.Размер > 100)
            throw new ArgumentException("Размер скидки должен быть от 1 до 100%.");
        if (!string.IsNullOrWhiteSpace(dto.Описание) && !ValidationHelper.IsValidRussianText(dto.Описание))
            throw new ArgumentException("Описание должно содержать только русские буквы, пробелы и дефисы.");

        discount.название = dto.Название;
        discount.размер = dto.Размер;
        discount.описание = dto.Описание;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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

     

    public async Task<IEnumerable<DiscountTariffDto>> GetAllDiscountTariffsAsync(string? discount, string? tariff, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var query = _context.Скидки_тарифы
            .Include(st => st.id_скидкиNavigation)
            .Include(st => st.id_тарифаNavigation)
            .AsQueryable();
        if (!string.IsNullOrEmpty(discount))
            query = query.Where(st => st.id_скидкиNavigation != null && st.id_скидкиNavigation.название.Contains(discount));
        if (!string.IsNullOrEmpty(tariff))
            query = query.Where(st => st.id_тарифаNavigation != null && st.id_тарифаNavigation.название.Contains(tariff));
        if (dateFrom.HasValue)
            query = query.Where(st => st.дата_назначения >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(st => st.дата_назначения <= dateTo.Value);

        var dt = await query.ToListAsync();
        return dt.Select(st => new DiscountTariffDto
        {
            СкидкаId = st.id_скидки,
            ТарифId = st.id_тарифа,
            СкидкаНазвание = st.id_скидкиNavigation?.название ?? "",
            ТарифНазвание = st.id_тарифаNavigation?.название ?? "",
            ДатаНазначения = st.дата_назначения
        });
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
        if (dto.ДатаНазначения > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата назначения не может быть в будущем.");

        var discount = await _context.Скидки.FirstOrDefaultAsync(d => d.название == dto.СкидкаНазвание);
        if (discount == null) throw new ArgumentException("Скидка не найдена");
        var tariff = await _context.Тарифы.FirstOrDefaultAsync(t => t.название == dto.ТарифНазвание);
        if (tariff == null) throw new ArgumentException("Тариф не найден");

        var exists = await _context.Скидки_тарифы.AnyAsync(st => st.id_скидки == discount.id_скидки && st.id_тарифа == tariff.id_тарифа);
        if (exists) throw new ArgumentException("Связь скидки и тарифа уже существует.");

        var st = new СкидкаТариф
        {
            id_скидки = discount.id_скидки,
            id_тарифа = tariff.id_тарифа,
            дата_назначения = dto.ДатаНазначения
        };
        _context.Скидки_тарифы.Add(st);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        return dto;
    }

    public async Task<DiscountTariffDto?> UpdateDiscountTariffAsync(int discountId, int tariffId, DiscountTariffDto dto)
    {
        var dt = await _context.Скидки_тарифы.FindAsync(discountId, tariffId);
        if (dt == null) return null;

        if (dto.ДатаНазначения > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата назначения не может быть в будущем.");

        var discount = await _context.Скидки.FirstOrDefaultAsync(d => d.название == dto.СкидкаНазвание);
        if (discount == null) throw new ArgumentException("Скидка не найдена");
        var tariff = await _context.Тарифы.FirstOrDefaultAsync(t => t.название == dto.ТарифНазвание);
        if (tariff == null) throw new ArgumentException("Тариф не найден");

        dt.id_скидки = discount.id_скидки;
        dt.id_тарифа = tariff.id_тарифа;
        dt.дата_назначения = dto.ДатаНазначения;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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



    public async Task<IEnumerable<EmployeeCategoryDto>> GetAllEmployeeCategoriesAsync(string? employee, string? category)
    {
        var query = _context.Принадлежности_сотрудников
            .Include(pc => pc.id_сотрудникаNavigation)
            .Include(pc => pc.id_категорииNavigation)
            .AsQueryable();
        if (!string.IsNullOrEmpty(employee))
            query = query.Where(pc => pc.id_сотрудникаNavigation != null && pc.id_сотрудникаNavigation.ФИО.Contains(employee));
        if (!string.IsNullOrEmpty(category))
            query = query.Where(pc => pc.id_категорииNavigation != null && pc.id_категорииNavigation.название.Contains(category));

        var ec = await query.ToListAsync();
        return ec.Select(pc => new EmployeeCategoryDto
        {
            СотрудникId = pc.id_сотрудника,
            КатегорияId = pc.id_категории,
            СотрудникФИО = pc.id_сотрудникаNavigation?.ФИО ?? "",
            КатегорияНазвание = pc.id_категорииNavigation?.название ?? "",
            ДатаПолучения = pc.дата_получения
        });
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
        if (dto.ДатаПолучения > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата получения не может быть в будущем.");

        var employee = await _context.Сотрудники.FirstOrDefaultAsync(e => e.ФИО == dto.СотрудникФИО);
        if (employee == null) throw new ArgumentException("Сотрудник не найден");
        var category = await _context.Категории_прав.FirstOrDefaultAsync(c => c.название == dto.КатегорияНазвание);
        if (category == null) throw new ArgumentException("Категория не найдена");

        var exists = await _context.Принадлежности_сотрудников.AnyAsync(pc => pc.id_сотрудника == employee.id_сотрудника && pc.id_категории == category.id_категории);
        if (exists) throw new ArgumentException("Сотрудник уже привязан к этой категории.");

        var pc = new ПринадлежностьСотрудника
        {
            id_сотрудника = employee.id_сотрудника,
            id_категории = category.id_категории,
            дата_получения = dto.ДатаПолучения
        };
        _context.Принадлежности_сотрудников.Add(pc);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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



    public async Task<IEnumerable<StudentAssignmentDto>> GetAllStudentAssignmentsAsync(string? student, string? instructor, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var query = _context.Закрепления_учеников
            .Include(sa => sa.id_ученикаNavigation)
            .Include(sa => sa.id_сотрудникаNavigation)
            .AsQueryable();
        if (!string.IsNullOrEmpty(student))
            query = query.Where(sa => sa.id_ученикаNavigation != null && sa.id_ученикаNavigation.ФИО.Contains(student));
        if (!string.IsNullOrEmpty(instructor))
            query = query.Where(sa => sa.id_сотрудникаNavigation != null && sa.id_сотрудникаNavigation.ФИО.Contains(instructor));
        if (dateFrom.HasValue)
            query = query.Where(sa => sa.дата_закрепления >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(sa => sa.дата_закрепления <= dateTo.Value);

        var assignments = await query.ToListAsync();
        return assignments.Select(sa => new StudentAssignmentDto
        {
            УченикId = sa.id_ученика,
            СотрудникId = sa.id_сотрудника,
            УченикФИО = sa.id_ученикаNavigation?.ФИО ?? "",
            СотрудникФИО = sa.id_сотрудникаNavigation?.ФИО ?? "",
            ДатаЗакрепления = sa.дата_закрепления,
            ДатаОкончания = sa.дата_окончания
        });
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
        if (dto.ДатаЗакрепления > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата закрепления не может быть в будущем.");
        if (dto.ДатаОкончания.HasValue && dto.ДатаОкончания < dto.ДатаЗакрепления)
            throw new ArgumentException("Дата окончания не может быть раньше даты закрепления.");

        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.УченикФИО);
        if (student == null) throw new ArgumentException("Ученик не найден");
        var employee = await _context.Сотрудники.FirstOrDefaultAsync(e => e.ФИО == dto.СотрудникФИО);
        if (employee == null) throw new ArgumentException("Сотрудник не найден");

        var sa = new ЗакреплениеУченика
        {
            id_ученика = student.id_ученика,
            id_сотрудника = employee.id_сотрудника,
            дата_закрепления = dto.ДатаЗакрепления,
            дата_окончания = dto.ДатаОкончания
        };
        _context.Закрепления_учеников.Add(sa);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
        return dto;
    }

    public async Task<StudentAssignmentDto?> UpdateStudentAssignmentAsync(int studentId, int employeeId, DateOnly assignmentDate, StudentAssignmentDto dto)
    {
        var sa = await _context.Закрепления_учеников.FindAsync(studentId, employeeId, assignmentDate);
        if (sa == null) return null;

        if (dto.ДатаЗакрепления > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException("Дата закрепления не может быть в будущем.");
        if (dto.ДатаОкончания.HasValue && dto.ДатаОкончания < dto.ДатаЗакрепления)
            throw new ArgumentException("Дата окончания не может быть раньше даты закрепления.");

        var student = await _context.Ученики.FirstOrDefaultAsync(s => s.ФИО == dto.УченикФИО);
        if (student == null) throw new ArgumentException("Ученик не найден");
        var employee = await _context.Сотрудники.FirstOrDefaultAsync(e => e.ФИО == dto.СотрудникФИО);
        if (employee == null) throw new ArgumentException("Сотрудник не найден");

        sa.id_ученика = student.id_ученика;
        sa.id_сотрудника = employee.id_сотрудника;
        sa.дата_закрепления = dto.ДатаЗакрепления;
        sa.дата_окончания = dto.ДатаОкончания;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            throw new ArgumentException(pgEx.MessageText);
        }
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