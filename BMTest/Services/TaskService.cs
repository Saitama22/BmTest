using System.Threading;
using BMTest.Models;
using BMTest.Services.DbContexts;
using BMTest.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;

namespace BMTest.Services
{
	/// <summary>
	/// Вся логика для работы с задачами (вообще лучше вывести операции с бд в отдельный класс репозиторий, 
	/// но я решил этого не делать, потому что он бы методы, которые есть тут)
	/// </summary>
	public class TaskService : ITaskService
	{
		private BmDbContext _bmDbContext;
		private AppConfiguration _bmTestSettings;

		public TaskService(BmDbContext bmDbContext, IOptions<AppConfiguration> myConfiguration)
		{
			_bmDbContext = bmDbContext;
			_bmTestSettings = myConfiguration.Value;
		}

		public async Task<Guid> CreateTaskAsync()
		{
			Guid taskId = Guid.NewGuid();
			await _bmDbContext.Tasks.AddAsync(new TaskModel
			{
				Date = DateTime.Now,
				Id = taskId,
				Status = TaskStates.created.ToString(),
			});
			await _bmDbContext.SaveChangesAsync();
			return taskId;
		}

		public async Task RunAndFinishTaskAsync(Guid id)
		{
			try
			{
				//В этом методе необходимо пересоздать Dbcontext, потому что иначе Dbcontext будет диспознутым
				ReCreateDbContext();

				await RunTaskAsync(id);
				await Task.Delay(2000);
				await FinishTaskAsync(id);
			}
			catch (Exception ex)
			{
				Log.Error(ex, ex.Message);
			}
		}

		/// <summary>
		/// Функция пересоздания DbContext. Используется в методах, в которых возможен вызов DbContext, в котором уже вызвали dispose
		/// </summary>
		private void ReCreateDbContext()
		{
			var dbContextOptionsBuilder = new DbContextOptionsBuilder<BmDbContext>();
			dbContextOptionsBuilder.UseNpgsql(_bmTestSettings.DbConnectionString);
			_bmDbContext = new(dbContextOptionsBuilder.Options);
		}

		private async Task RunTaskAsync(Guid id)
		{
			UpdateTask(id, TaskStates.running);
			await _bmDbContext.SaveChangesAsync();
		}

		private async Task FinishTaskAsync(Guid id)
		{
			UpdateTask(id, TaskStates.finished);
		
			await _bmDbContext.SaveChangesAsync();
		}

		public bool GetTask(Guid id, out TaskModel task)
		{
			task = _bmDbContext.Tasks.FirstOrDefault(r => r.Id == id);
			if (task is null)
				return false;
			return true;
		}

		private void UpdateTask(Guid taskId, TaskStates taskStatus)
		{
			var isTaskExist = GetTask(taskId, out var task);
			if (!isTaskExist)
				throw new InvalidOperationException($"Нет задачи с id {taskId}");
			task.Status = taskStatus.ToString();
			task.Date = DateTime.Now;
		}
	}
}
