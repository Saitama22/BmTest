using BMTest.Models;
using BMTest.Services.DbContexts;

namespace BMTest.Services.Interfaces
{
	public interface ITaskService
	{
		Task<Guid> CreateTaskAsync();
		bool GetTask(Guid id, out TaskModel task);
		Task RunAndFinishTaskAsync(Guid id);
	}
}
