using DrivingSchoolApp.DTO;
using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class EmployeeService
    {
        private readonly LibraryContext _context;
        public EmployeeService(LibraryContext context) => _context = context;

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            return await _context.Сотрудникs
                .Select(e => new EmployeeDto
                {
                    Id = e.id_сотрудника,
                    ФИО = e.фио,
                    Телефон = e.телефон,
                    Email = e.email ?? "",
                    ДатаРождения = e.дата_рождения,
                    Паспорт = $"{e.паспорт_серия_сотр} {e.паспорт_номер_сотр}",
                    ДатаПриема = e.дата_приема,
                    Адрес = e.адрес ?? ""
                })
                .ToListAsync();
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var e = await _context.Сотрудникs.FindAsync(id);
            if (e == null) return null;
            return new EmployeeDto
            {
                Id = e.id_сотрудника,
                ФИО = e.фио,
                Телефон = e.телефон,
                Email = e.email ?? "",
                ДатаРождения = e.дата_рождения,
                Паспорт = $"{e.паспорт_серия_сотр} {e.паспорт_номер_сотр}",
                ДатаПриема = e.дата_приема,
                Адрес = e.адрес ?? ""
            };
        }

        public async Task<EmployeeDto> CreateAsync(EmployeeDto dto)
        {
            var employee = new Сотрудник
            {
                фио = dto.ФИО,
                телефон = dto.Телефон,
                email = dto.Email,
                дата_рождения = dto.ДатаРождения,
                паспорт_серия_сотр = dto.Паспорт.Split(' ')[0],
                паспорт_номер_сотр = dto.Паспорт.Split(' ')[1],
                дата_приема = dto.ДатаПриема,
                адрес = dto.Адрес
            };
            _context.Сотрудникs.Add(employee);
            await _context.SaveChangesAsync();
            dto.Id = employee.id_сотрудника;
            return dto;
        }

        public async Task<EmployeeDto?> UpdateAsync(int id, EmployeeDto dto)
        {
            var employee = await _context.Сотрудникs.FindAsync(id);
            if (employee == null) return null;
            employee.фио = dto.ФИО;
            employee.телефон = dto.Телефон;
            employee.email = dto.Email;
            employee.дата_рождения = dto.ДатаРождения;
            var parts = dto.Паспорт.Split(' ');
            employee.паспорт_серия_сотр = parts[0];
            employee.паспорт_номер_сотр = parts[1];
            employee.дата_приема = dto.ДатаПриема;
            employee.адрес = dto.Адрес;
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var e = await _context.Сотрудникs.FindAsync(id);
            if (e == null) return false;
            _context.Сотрудникs.Remove(e);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}