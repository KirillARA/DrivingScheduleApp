using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class AuthService
    {
        private readonly LibraryContext _context;

        public AuthService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateUserAsync(
            string login,
            string password)
        {
            return await _context.Users.AnyAsync(u =>
                u.login == login &&
                u.password == password);
        }
    }
}
