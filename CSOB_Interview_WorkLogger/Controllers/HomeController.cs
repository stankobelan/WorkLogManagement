using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CSOB_Interview_WorkLogger.Models;
using CSOB_Interview_WorkLogger.Services;

public class HomeController : Controller
{
    private readonly IEmployeeService _employeeService;

    public HomeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var employeeViewModels = await _employeeService.GetEmployeesWithHoursWorkedAsync(cancellationToken);

        return View(employeeViewModels);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult About()
    {
        return View();
    }
}