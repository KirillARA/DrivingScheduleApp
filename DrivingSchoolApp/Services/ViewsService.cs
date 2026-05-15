using DrivingSchoolApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Services
{
    public class ViewsService
    {
        private readonly LibraryContext _context;

        public ViewsService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<view_students_info>> GetStudentsInfoAsync()
        {
            return await _context.view_students_infos.ToListAsync();
        }

        public async Task<IEnumerable<view_driving_schedule>> GetDrivingScheduleAsync()
        {
            return await _context.view_driving_schedules.ToListAsync();
        }

        public async Task<IEnumerable<view_groups_summary>> GetGroupsSummaryAsync()
        {
            return await _context.view_groups_summaries.ToListAsync();
        }

        public async Task<IEnumerable<view_exam_result>> GetExamResultsAsync()
        {
            return await _context.view_exam_results.ToListAsync();
        }
    }
}