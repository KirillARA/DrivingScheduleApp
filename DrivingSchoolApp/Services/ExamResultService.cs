using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class ExamResultService
    {
        private readonly LibraryContext _context;
        public ExamResultService(LibraryContext context) => _context = context;

        public async Task<IEnumerable<ExamResultDto>> GetAllAsync()
        {
            return await _context.РезультатыЭкзаменаs
                .Include(r => r.id_ученикаNavigation)
                .Include(r => r.id_экзаменаNavigation)
                    .ThenInclude(e => e.id_категорииNavigation)
                .Select(r => new ExamResultDto
                {
                    Ученик = r.id_ученикаNavigation.фио,
                    Экзамен = $"{r.id_экзаменаNavigation.id_категорииNavigation.название} ({r.id_экзаменаNavigation.тип})",
                    ДатаПопытки = r.дата_попытки,
                    Результат = r.результат
                })
                .ToListAsync();
        }

        public async Task<ExamResultDto?> GetByIdAsync(int studentId, int examId, DateOnly date)
        {
            var r = await _context.РезультатыЭкзаменаs
                .Include(r => r.id_ученикаNavigation)
                .Include(r => r.id_экзаменаNavigation)
                    .ThenInclude(e => e.id_категорииNavigation)
                .FirstOrDefaultAsync(r => r.id_ученика == studentId && r.id_экзамена == examId && r.дата_попытки == date);
            if (r == null) return null;
            return new ExamResultDto
            {
                Ученик = r.id_ученикаNavigation.фио,
                Экзамен = $"{r.id_экзаменаNavigation.id_категорииNavigation.название} ({r.id_экзаменаNavigation.тип})",
                ДатаПопытки = r.дата_попытки,
                Результат = r.результат
            };
        }

        public async Task<ExamResultDto> CreateAsync(ExamResultDto dto)
        {
            var student = await _context.Ученикs.FirstOrDefaultAsync(s => s.фио == dto.Ученик);
            if (student == null) throw new Exception("Ученик не найден");
            // Находим экзамен по строке "Категория (тип)"
            var parts = dto.Экзамен.Split(' ');
            var categoryName = parts[0];
            var examType = parts[1].Trim('(', ')');
            var exam = await _context.Экзаменs
                .Include(e => e.id_категорииNavigation)
                .FirstOrDefaultAsync(e => e.id_категорииNavigation.название == categoryName && e.тип == examType);
            if (exam == null) throw new Exception("Экзамен не найден");
            var result = new РезультатыЭкзамена
            {
                id_ученика = student.id_ученика,
                id_экзамена = exam.id_экзамена,
                дата_попытки = dto.ДатаПопытки,
                результат = dto.Результат
            };
            _context.РезультатыЭкзаменаs.Add(result);
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<ExamResultDto?> UpdateAsync(int studentId, int examId, DateOnly oldDate, ExamResultDto dto)
        {
            var result = await _context.РезультатыЭкзаменаs.FindAsync(studentId, examId, oldDate);
            if (result == null) return null;
            var student = await _context.Ученикs.FirstOrDefaultAsync(s => s.фио == dto.Ученик);
            if (student == null) throw new Exception("Ученик не найден");
            var parts = dto.Экзамен.Split(' ');
            var categoryName = parts[0];
            var examType = parts[1].Trim('(', ')');
            var exam = await _context.Экзаменs
                .Include(e => e.id_категорииNavigation)
                .FirstOrDefaultAsync(e => e.id_категорииNavigation.название == categoryName && e.тип == examType);
            if (exam == null) throw new Exception("Экзамен не найден");
            result.id_ученика = student.id_ученика;
            result.id_экзамена = exam.id_экзамена;
            result.дата_попытки = dto.ДатаПопытки;
            result.результат = dto.Результат;
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int studentId, int examId, DateOnly date)
        {
            var r = await _context.РезультатыЭкзаменаs.FindAsync(studentId, examId, date);
            if (r == null) return false;
            _context.РезультатыЭкзаменаs.Remove(r);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}