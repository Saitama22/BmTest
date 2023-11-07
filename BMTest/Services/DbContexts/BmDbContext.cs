using BMTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BMTest.Services.DbContexts
{
	public class BmDbContext : DbContext
	{
		public BmDbContext(DbContextOptions<BmDbContext> options) : base(options)
		{
		}

		public DbSet<TaskModel> Tasks { get; set; }
	}
}
