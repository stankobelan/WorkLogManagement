using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CSOB_Interview_WorkLogger.Models;
using CSOB_Interview_WorkLogger.Services;
using System.Threading;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly IEmployeeService _employeeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="employeeService">Service for managing employee data.</param>
    public HomeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    /// <summary>
    /// Displays the home page with a list of employees and their total hours worked.
    /// </summary>
    /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
    /// <returns>A view displaying the list of employees with their total hours worked.</returns>
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        // Retrieve the list of employees with their total hours worked using the employee service.
        var employeeViewModels = await _employeeService.GetEmployeesWithHoursWorkedAsync(cancellationToken);

        // Return the view with the list of employees.
        return View(employeeViewModels);
    }

    /// <summary>
    /// Displays the error page.
    /// </summary>
    /// <returns>A view displaying error information.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Return the error view with the current request ID or trace identifier.
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// Displays the about page.
    /// </summary>
    /// <returns>A view displaying information about the application or organization.</returns>
    public IActionResult About()
    {
        // Return the about view.
        return View();
    }
}
