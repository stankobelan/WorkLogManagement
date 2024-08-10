using CSOB_Interview_WorkLogger.Controllers;
using CSOB_Interview_WorkLogger.Data;
using CSOB_Interview_WorkLogger.Factories;
using CSOB_Interview_WorkLogger.Services;
using Microsoft.EntityFrameworkCore;

namespace CSOB_Interview_WorkLogger;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
            optionsBuilder.UseSqlServer(connectionString));

        builder.Services.AddScoped<IEmployeeService, EmployeeService>();
        builder.Services.AddScoped<IWorkLogService, WorkLogService>();
        builder.Services.AddScoped<ITimeTrackingService, TimeTrackingService>();
        builder.Services.AddScoped<IEmployeeFactory, EmployeeFactory>();
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}