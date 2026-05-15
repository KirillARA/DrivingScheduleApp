using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class GroupService
    {
        private readonly LibraryContext _context;
        public GroupService(LibraryContext context) => _context = context;

        public async Task<IEnumerable<GroupDto>> GetAllAsync()
        {
            return await _context.Группаs
                .Include(g => g.id_категорииNavigation)
                .Select(g => new GroupDto
                {
                    Id = g.id_группы,
                    Название = g.название,
                    Категория = g.id_категорииNavigation.название,
                    ДатаНачала = g.дата_начала,
                    ДатаОкончания = g.дата_окончания,
                    МаксУчеников = g.макс_учеников,
                    ТекущУчеников = g.текущ_учеников,
                    Статус = g.статус
                })
                .ToListAsync();
        }

        public async Task<GroupDto?> GetByIdAsync(int id)
        {
            var g = await _context.Группаs
                .Include(g => g.id_категорииNavigation)
                .FirstOrDefaultAsync(g => g.id_группы == id);
            if (g == null) return null;
            return new GroupDto
            {
                Id = g.id_группы,
                Название = g.название,
                Категория = g.id_категорииNavigation.название,
                ДатаНачала = g.дата_начала,
                ДатаОкончания = g.дата_окончания,
                МаксУчеников = g.макс_учеников,
                ТекущУчеников = g.текущ_учеников,
                Статус = g.статус
            };
        }

        public async Task<GroupDto> CreateAsync(GroupDto dto)
        {
            var category = await _context.Категория_правs.FirstOrDefaultAsync(c => c.название == dto.Категория);
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
            _context.Группаs.Add(group);
            await _context.SaveChangesAsync();
            dto.Id = group.id_группы;
            return dto;
        }

        public async Task<GroupDto?> UpdateAsync(int id, GroupDto dto)
        {
            var group = await _context.Группаs.FindAsync(id);
            if (group == null) return null;
            var category = await _context.Категория_правs.FirstOrDefaultAsync(c => c.название == dto.Категория);
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

        public async Task<bool> DeleteAsync(int id)
        {
            var g = await _context.Группаs.FindAsync(id);
            if (g == null) return false;
            _context.Группаs.Remove(g);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}