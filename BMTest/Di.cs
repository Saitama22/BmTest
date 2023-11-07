using BMTest.Models;
using BMTest.Services;
using BMTest.Services.DbContexts;
using BMTest.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BMTest
{
	/// <summary>
	/// Класс для внедрения зависимостей
	/// </summary>
	public static class Di
	{
		public static void InitDi(this WebApplicationBuilder builder)
		{
			builder.ConfigureServices();
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddBmTestServices();
			builder.Services.AddContexts(builder.Configuration);
		}

		private static void AddBmTestServices(this IServiceCollection services)
		{
			services.AddScoped<ITaskService, TaskService>();
		}

		public static void ConfigureServices(this WebApplicationBuilder builder)
		{
			builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("ConnectionStrings"));
		}
		private static void AddContexts(this IServiceCollection services, ConfigurationManager configuration)
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
			services.AddDbContext<BmDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("DbConnectionString")));
		}
	}
}
 