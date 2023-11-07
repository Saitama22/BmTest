using BMTest;
using BMTest.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		Log.Logger = new LoggerConfiguration()
			.WriteTo.File($"log-{DateTime.Now.ToString("yyyy-MM-dd")}.txt")
			.CreateLogger();

		// Add services to the container.
		builder.InitDi();

		var app = builder.Build(); 
		
		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}