using Microsoft.AspNetCore.Mvc;
using DrivingSchoolApp.Services;

namespace DrivingSchoolApp.Controllers
{
    [Route("api/views")]
    [ApiController]
    public class ViewsController : ControllerBase
    {
        private readonly ViewsService _viewsService;

        public ViewsController(ViewsService viewsService)
        {
            _viewsService = viewsService;
        }

        [HttpGet("students_info")]
        public async Task<IActionResult> GetStudentsInfo()
        {
            var data = await _viewsService.GetStudentsInfoAsync();
            return Ok(data);
        }

        [HttpGet("driving_schedule")]
        public async Task<IActionResult> GetDrivingSchedule()
        {
            var data = await _viewsService.GetDrivingScheduleAsync();
            return Ok(data);
        }

        [HttpGet("groups_summary")]
        public async Task<IActionResult> GetGroupsSummary()
        {
            var data = await _viewsService.GetGroupsSummaryAsync();
            return Ok(data);
        }

        [HttpGet("exam_results")]
        public async Task<IActionResult> GetExamResults()
        {
            var data = await _viewsService.GetExamResultsAsync();
            return Ok(data);
        }

        // Если вы хотите универсальный метод по имени представления (как раньше):
        [HttpGet("{viewName}")]
        public async Task<IActionResult> GetView(string viewName)
        {
            switch (viewName)
            {
                case "students_info":
                    return Ok(await _viewsService.GetStudentsInfoAsync());
                case "driving_schedule":
                    return Ok(await _viewsService.GetDrivingScheduleAsync());
                case "groups_summary":
                    return Ok(await _viewsService.GetGroupsSummaryAsync());
                case "exam_results":
                    return Ok(await _viewsService.GetExamResultsAsync());
                default:
                    return NotFound($"Представление '{viewName}' не найдено");
            }
        }
    }
}
