using BMTest.Services.DbContexts;
using BMTest.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BMTest.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TaskController : ControllerBase
	{
		private ITaskService _taskService;

		public TaskController(ITaskService taskService)
		{
			_taskService = taskService;
		}

		[HttpPost]
		public async Task<IActionResult> PostTaskAsync()
		{
			try
			{
				var taskId = await _taskService.CreateTaskAsync();
				_taskService.RunAndFinishTaskAsync(taskId);
			    return Accepted(taskId);
			}
			catch (Exception ex)
			{
				Log.Error(ex, ex.Message);
				return StatusCode(500, "Во время обработки запроса произошла ошибка");
			}
		}

		[HttpGet("{id}")]
		public IActionResult GetTask(string id)
		{
			try
			{
				if (!Guid.TryParse(id, out var guidTaskId))
					return BadRequest("В параметр запроса был отправлен не Guid");
				if (!_taskService.GetTask(guidTaskId, out var task))
					return NotFound($"Не найдена задача с id {guidTaskId}");
				return Ok(task);
			}
			catch (Exception ex)
			{
				Log.Error(ex, ex.Message);
				return StatusCode(500, "Во время обработки запроса произошла ошибка");
			}
		}
	}
}
