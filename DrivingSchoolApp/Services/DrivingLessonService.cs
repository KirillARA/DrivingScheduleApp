using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class DrivingLessonService
    {
        private readonly LibraryContext _context;
        public DrivingLessonService(LibraryContext context) => _context = context;

        public async Task<IEnumerable<DrivingLessonDto>> GetAllAsync()
        {
            return await _context.Практическое_занятиеs
                .Include(l => l.id_ученикаNavigation)
                .Include(l => l.id_инструктораNavigation)
                    .ThenInclude(i => i.id_сотрудникаNavigation)
                .Include(l => l.id_транспортаNavigation)
                .Select(l => new DrivingLessonDto
                {
                    Id = l.id_практзан,
                    Ученик = l.id_ученикаNavigation.фио,
                    Инструктор = l.id_инструктораNavigation.id_сотрудникаNavigation.фио,
                    Автомобиль = $"{l.id_транспортаNavigation.марка} {l.id_транспортаNavigation.модель} ({l.id_транспортаNavigation.госномер})",
                    Дата = l.дата,
                    ВремяНачала = l.время_начала,
                    ВремяОкончания = l.время_окончания
                })
                .ToListAsync();
        }

        public async Task<DrivingLessonDto?> GetByIdAsync(int id)
        {
            var l = await _context.Практическое_занятиеs
                .Include(l => l.id_ученикаNavigation)
                .Include(l => l.id_инструктораNavigation)
                    .ThenInclude(i => i.id_сотрудникаNavigation)
                .Include(l => l.id_транспортаNavigation)
                .FirstOrDefaultAsync(l => l.id_практзан == id);
            if (l == null) return null;
            return new DrivingLessonDto
            {
                Id = l.id_практзан,
                Ученик = l.id_ученикаNavigation.фио,
                Инструктор = l.id_инструктораNavigation.id_сотрудникаNavigation.фио,
                Автомобиль = $"{l.id_транспортаNavigation.марка} {l.id_транспортаNavigation.модель} ({l.id_транспортаNavigation.госномер})",
                Дата = l.дата,
                ВремяНачала = l.время_начала,
                ВремяОкончания = l.время_окончания
            };
        }

        public async Task<DrivingLessonDto> CreateAsync(DrivingLessonDto dto)
        {
            var student = await _context.Ученикs.FirstOrDefaultAsync(s => s.фио == dto.Ученик);
            if (student == null) throw new Exception("Ученик не найден");
            var instructor = await _context.Инструкторs
                .Include(i => i.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(i => i.id_сотрудникаNavigation.фио == dto.Инструктор);
            if (instructor == null) throw new Exception("Инструктор не найден");
            var transport = await _context.Транспортs
                .FirstOrDefaultAsync(t => $"{t.марка} {t.модель} ({t.госномер})" == dto.Автомобиль);
            if (transport == null) throw new Exception("Автомобиль не найден");
            var lesson = new Практическое_занятие
            {
                id_ученика = student.id_ученика,
                id_инструктора = instructor.id_инструктора,
                id_транспорта = transport.id_транспорта,
                дата = dto.Дата,
                время_начала = dto.ВремяНачала,
                время_окончания = dto.ВремяОкончания
            };
            _context.Практическое_занятиеs.Add(lesson);
            await _context.SaveChangesAsync();
            dto.Id = lesson.id_практзан;
            return dto;
        }

        public async Task<DrivingLessonDto?> UpdateAsync(int id, DrivingLessonDto dto)
        {
            var lesson = await _context.Практическое_занятиеs.FindAsync(id);
            if (lesson == null) return null;
            var student = await _context.Ученикs.FirstOrDefaultAsync(s => s.фио == dto.Ученик);
            if (student == null) throw new Exception("Ученик не найден");
            var instructor = await _context.Инструкторs
                .Include(i => i.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(i => i.id_сотрудникаNavigation.фио == dto.Инструктор);
            if (instructor == null) throw new Exception("Инструктор не найден");
            var transport = await _context.Транспортs
                .FirstOrDefaultAsync(t => $"{t.марка} {t.модель} ({t.госномер})" == dto.Автомобиль);
            if (transport == null) throw new Exception("Автомобиль не найден");
            lesson.id_ученика = student.id_ученика;
            lesson.id_инструктора = instructor.id_инструктора;
            lesson.id_транспорта = transport.id_транспорта;
            lesson.дата = dto.Дата;
            lesson.время_начала = dto.ВремяНачала;
            lesson.время_окончания = dto.ВремяОкончания;
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var l = await _context.Практическое_занятиеs.FindAsync(id);
            if (l == null) return false;
            _context.Практическое_занятиеs.Remove(l);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}