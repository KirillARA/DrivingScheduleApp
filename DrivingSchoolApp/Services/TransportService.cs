using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class TransportService
    {
        private readonly LibraryContext _context;
        public TransportService(LibraryContext context) => _context = context;

        public async Task<IEnumerable<TransportDto>> GetAllAsync()
        {
            return await _context.Транспортs
                .Include(t => t.id_категорииNavigation)
                .Include(t => t.id_инструктораNavigation)
                    .ThenInclude(i => i.id_сотрудникаNavigation)
                .Select(t => new TransportDto
                {
                    Id = t.id_транспорта,
                    Марка = t.марка,
                    Модель = t.модель,
                    Госномер = t.госномер,
                    Категория = t.id_категорииNavigation.название,
                    Инструктор = t.id_инструктораNavigation.id_сотрудникаNavigation.фио,
                    Пробег = t.пробег
                })
                .ToListAsync();
        }

        public async Task<TransportDto?> GetByIdAsync(int id)
        {
            var t = await _context.Транспортs
                .Include(t => t.id_категорииNavigation)
                .Include(t => t.id_инструктораNavigation)
                    .ThenInclude(i => i.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(t => t.id_транспорта == id);
            if (t == null) return null;
            return new TransportDto
            {
                Id = t.id_транспорта,
                Марка = t.марка,
                Модель = t.модель,
                Госномер = t.госномер,
                Категория = t.id_категорииNavigation.название,
                Инструктор = t.id_инструктораNavigation.id_сотрудникаNavigation.фио,
                Пробег = t.пробег
            };
        }

        public async Task<TransportDto> CreateAsync(TransportDto dto)
        {
            var category = await _context.Категория_правs.FirstOrDefaultAsync(c => c.название == dto.Категория);
            if (category == null) throw new Exception("Категория не найдена");
            var instructor = await _context.Инструкторs
                .Include(i => i.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(i => i.id_сотрудникаNavigation.фио == dto.Инструктор);
            if (instructor == null) throw new Exception("Инструктор не найден");
            var transport = new Транспорт
            {
                марка = dto.Марка,
                модель = dto.Модель,
                госномер = dto.Госномер,
                id_категории = category.id_категории,
                id_инструктора = instructor.id_инструктора,
                пробег = dto.Пробег
            };
            _context.Транспортs.Add(transport);
            await _context.SaveChangesAsync();
            dto.Id = transport.id_транспорта;
            return dto;
        }

        public async Task<TransportDto?> UpdateAsync(int id, TransportDto dto)
        {
            var transport = await _context.Транспортs.FindAsync(id);
            if (transport == null) return null;
            var category = await _context.Категория_правs.FirstOrDefaultAsync(c => c.название == dto.Категория);
            if (category == null) throw new Exception("Категория не найдена");
            var instructor = await _context.Инструкторs
                .Include(i => i.id_сотрудникаNavigation)
                .FirstOrDefaultAsync(i => i.id_сотрудникаNavigation.фио == dto.Инструктор);
            if (instructor == null) throw new Exception("Инструктор не найден");
            transport.марка = dto.Марка;
            transport.модель = dto.Модель;
            transport.госномер = dto.Госномер;
            transport.id_категории = category.id_категории;
            transport.id_инструктора = instructor.id_инструктора;
            transport.пробег = dto.Пробег;
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var t = await _context.Транспортs.FindAsync(id);
            if (t == null) return false;
            _context.Транспортs.Remove(t);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}