using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class LicenseCategoryService
    {
        private readonly LibraryContext _context;
        public LicenseCategoryService(LibraryContext context) => _context = context;

        public async Task<IEnumerable<LicenseCategoryDto>> GetAllAsync()
        {
            return await _context.Категория_правs
                .Select(c => new LicenseCategoryDto
                {
                    Id = c.id_категории,
                    Название = c.название,
                    Описание = c.описание ?? ""
                })
                .ToListAsync();
        }

        public async Task<LicenseCategoryDto?> GetByIdAsync(int id)
        {
            var c = await _context.Категория_правs.FindAsync(id);
            if (c == null) return null;
            return new LicenseCategoryDto
            {
                Id = c.id_категории,
                Название = c.название,
                Описание = c.описание ?? ""
            };
        }

        public async Task<LicenseCategoryDto> CreateAsync(LicenseCategoryDto dto)
        {
            var category = new Категория_прав
            {
                название = dto.Название,
                описание = dto.Описание
            };
            _context.Категория_правs.Add(category);
            await _context.SaveChangesAsync();
            dto.Id = category.id_категории;
            return dto;
        }

        public async Task<LicenseCategoryDto?> UpdateAsync(int id, LicenseCategoryDto dto)
        {
            var cat = await _context.Категория_правs.FindAsync(id);
            if (cat == null) return null;
            cat.название = dto.Название;
            cat.описание = dto.Описание;
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cat = await _context.Категория_правs.FindAsync(id);
            if (cat == null) return false;
            _context.Категория_правs.Remove(cat);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}