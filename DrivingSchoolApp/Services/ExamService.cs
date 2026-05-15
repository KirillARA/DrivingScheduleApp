using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class ExamService
    {
        private readonly LibraryContext _context;
        public ExamService(LibraryContext context) => _context = context;

        public async Task<IEnumerable<ExamDto>> GetAllAsync()
        {
            return await _context.Экзаменs
                .Include(e => e.id_категорииNavigation)
                .Select(e => new ExamDto
                {
                    Id = e.id_экзамена,
                    Категория = e.id_категорииNavigation.название,
                    Тип = e.тип
                })
                .ToListAsync();
        }

        public async Task<ExamDto?> GetByIdAsync(int id)
        {
            var e = await _context.Экзаменs
                .Include(e => e.id_категорииNavigation)
                .FirstOrDefaultAsync(e => e.id_экзамена == id);
            if (e == null) return null;
            return new ExamDto
            {
                Id = e.id_экзамена,
                Категория = e.id_категорииNavigation.название,
                Тип = e.тип
            };
        }

        public async Task<ExamDto> CreateAsync(ExamDto dto)
        {
            var category = await _context.Категория_правs.FirstOrDefaultAsync(c => c.название == dto.Категория);
            if (category == null) throw new Exception("Категория не найдена");
            var exam = new Экзамен
            {
                id_категории = category.id_категории,
                тип = dto.Тип
            };
            _context.Экзаменs.Add(exam);
            await _context.SaveChangesAsync();
            dto.Id = exam.id_экзамена;
            return dto;
        }

        public async Task<ExamDto?> UpdateAsync(int id, ExamDto dto)
        {
            var exam = await _context.Экзаменs.FindAsync(id);
            if (exam == null) return null;
            var category = await _context.Категория_правs.FirstOrDefaultAsync(c => c.название == dto.Категория);
            if (category == null) throw new Exception("Категория не найдена");
            exam.id_категории = category.id_категории;
            exam.тип = dto.Тип;
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exam = await _context.Экзаменs.FindAsync(id);
            if (exam == null) return false;
            _context.Экзаменs.Remove(exam);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}