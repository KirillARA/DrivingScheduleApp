using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class TheoryLessonService
    {
        private readonly LibraryContext _context;
        public TheoryLessonService(LibraryContext context) => _context = context;

        public async Task<IEnumerable<TheoryLessonDto>> GetAllAsync()
        {
            return await _context.Теоретическое_занятиеs
                .Include(l => l.id_группыNavigation)
                .Include(l => l.id_преподавателяNavigation)
                    .ThenInclude(t => t.id_сотрудникаNavigation)
                .Select(l => new TheoryLessonDto
                {
                    Id = l.id_теорзан,
                    Группа = l.id_группыNavigation.название,
                    Преподаватель = l.id_преподавателяNavigation.id_сотрудникаNavigation.фио,
                    Тема = l.тема,
                    Дата = l.дата,
                    ВремяНачала = l.время_начала,
                    ВремяОкончания = l.время_окончания,
                    Аудитория = l.аудитория ?? "",
                    НомерЗанятия = l.номер_занятия
                })
                .ToListAsync();
        }

        public async Task<TheoryLessonDto?> GetByIdAsync(int id)
        {
            var l = await _context.Теоретическое_занятиеs
                .Include(l => l.id_группыNavigation)
                .Include(l => l.id_преподавателяNavigation)
                    .ThenInclude(t => t.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(l => l.id_теорзан == id);
            if (l == null) return null;
            return new TheoryLessonDto
            {
                Id = l.id_теорзан,
                Группа = l.id_группыNavigation.название,
                Преподаватель = l.id_преподавателяNavigation.id_сотрудникаNavigation.фио,
                Тема = l.тема,
                Дата = l.дата,
                ВремяНачала = l.время_начала,
                ВремяОкончания = l.время_окончания,
                Аудитория = l.аудитория ?? "",
                НомерЗанятия = l.номер_занятия
            };
        }

        public async Task<TheoryLessonDto> CreateAsync(TheoryLessonDto dto)
        {
            var group = await _context.Группаs.FirstOrDefaultAsync(g => g.название == dto.Группа);
            if (group == null) throw new Exception("Группа не найдена");
            var teacher = await _context.Преподавательs
                .Include(t => t.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(t => t.id_сотрудникаNavigation.фио == dto.Преподаватель);
            if (teacher == null) throw new Exception("Преподаватель не найден");
            var lesson = new Теоретическое_занятие
            {
                id_группы = group.id_группы,
                id_преподавателя = teacher.id_преподавателя,
                тема = dto.Тема,
                дата = dto.Дата,
                время_начала = dto.ВремяНачала,
                время_окончания = dto.ВремяОкончания,
                аудитория = dto.Аудитория,
                номер_занятия = dto.НомерЗанятия
            };
            _context.Теоретическое_занятиеs.Add(lesson);
            await _context.SaveChangesAsync();
            dto.Id = lesson.id_теорзан;
            return dto;
        }

        public async Task<TheoryLessonDto?> UpdateAsync(int id, TheoryLessonDto dto)
        {
            var lesson = await _context.Теоретическое_занятиеs.FindAsync(id);
            if (lesson == null) return null;
            var group = await _context.Группаs.FirstOrDefaultAsync(g => g.название == dto.Группа);
            if (group == null) throw new Exception("Группа не найдена");
            var teacher = await _context.Преподавательs
                .Include(t => t.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(t => t.id_сотрудникаNavigation.фио == dto.Преподаватель);
            if (teacher == null) throw new Exception("Преподаватель не найден");
            lesson.id_группы = group.id_группы;
            lesson.id_преподавателя = teacher.id_преподавателя;
            lesson.тема = dto.Тема;
            lesson.дата = dto.Дата;
            lesson.время_начала = dto.ВремяНачала;
            lesson.время_окончания = dto.ВремяОкончания;
            lesson.аудитория = dto.Аудитория;
            lesson.номер_занятия = dto.НомерЗанятия;
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var l = await _context.Теоретическое_занятиеs.FindAsync(id);
            if (l == null) return false;
            _context.Теоретическое_занятиеs.Remove(l);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}