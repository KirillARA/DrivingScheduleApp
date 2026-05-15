using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class StudentService
    {
        private readonly LibraryContext _context;
        public StudentService(LibraryContext context) => _context = context;

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            return await _context.Ученикs
                .Include(s => s.id_группыNavigation)
                .Include(s => s.id_инструктораNavigation)
                    .ThenInclude(i => i.id_сотрудникаNavigation)
                .Select(s => new StudentDto
                {
                    Id = s.id_ученика,
                    ФИО = s.фио,
                    Паспорт = $"{s.паспорт_серия_уч} {s.паспорт_номер_уч}",
                    ДатаРождения = s.дата_рождения,
                    Телефон = s.телефон,
                    Группа = s.id_группыNavigation.название,
                    Инструктор = s.id_инструктораNavigation.id_сотрудникаNavigation.фио
                })
                .ToListAsync();
        }

        public async Task<StudentDto?> GetByIdAsync(int id)
        {
            var s = await _context.Ученикs
                .Include(s => s.id_группыNavigation)
                .Include(s => s.id_инструктораNavigation)
                    .ThenInclude(i => i.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(s => s.id_ученика == id);
            if (s == null) return null;
            return new StudentDto
            {
                Id = s.id_ученика,
                ФИО = s.фио,
                Паспорт = $"{s.паспорт_серия_уч} {s.паспорт_номер_уч}",
                ДатаРождения = s.дата_рождения,
                Телефон = s.телефон,
                Группа = s.id_группыNavigation.название,
                Инструктор = s.id_инструктораNavigation.id_сотрудникаNavigation.фио
            };
        }

        public async Task<StudentDto> CreateAsync(StudentDto dto)
        {
            var group = await _context.Группаs.FirstOrDefaultAsync(g => g.название == dto.Группа);
            if (group == null) throw new Exception("Группа не найдена");
            var instructor = await _context.Инструкторs
                .Include(i => i.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(i => i.id_сотрудникаNavigation.фио == dto.Инструктор);
            if (instructor == null) throw new Exception("Инструктор не найден");
            var parts = dto.Паспорт.Split(' ');
            var student = new Ученик
            {
                фио = dto.ФИО,
                паспорт_серия_уч = parts[0],
                паспорт_номер_уч = parts[1],
                дата_рождения = dto.ДатаРождения,
                телефон = dto.Телефон,
                id_группы = group.id_группы,
                id_инструктора = instructor.id_инструктора
            };
            _context.Ученикs.Add(student);
            await _context.SaveChangesAsync();
            dto.Id = student.id_ученика;
            return dto;
        }

        public async Task<StudentDto?> UpdateAsync(int id, StudentDto dto)
        {
            var student = await _context.Ученикs.FindAsync(id);
            if (student == null) return null;
            var group = await _context.Группаs.FirstOrDefaultAsync(g => g.название == dto.Группа);
            if (group == null) throw new Exception("Группа не найдена");
            var instructor = await _context.Инструкторs
                .Include(i => i.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(i => i.id_сотрудникаNavigation.фио == dto.Инструктор);
            if (instructor == null) throw new Exception("Инструктор не найден");
            var parts = dto.Паспорт.Split(' ');
            student.фио = dto.ФИО;
            student.паспорт_серия_уч = parts[0];
            student.паспорт_номер_уч = parts[1];
            student.дата_рождения = dto.ДатаРождения;
            student.телефон = dto.Телефон;
            student.id_группы = group.id_группы;
            student.id_инструктора = instructor.id_инструктора;
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var s = await _context.Ученикs.FindAsync(id);
            if (s == null) return false;
            _context.Ученикs.Remove(s);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}