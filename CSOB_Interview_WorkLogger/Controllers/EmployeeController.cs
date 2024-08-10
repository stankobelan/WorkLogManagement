using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Factories;
using CSOB_Interview_WorkLogger.Models;
using CSOB_Interview_WorkLogger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace CSOB_Interview_WorkLogger.Controllers
{
    /// <summary>
    /// Controller for managing employee-related actions.
    /// </summary>
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeController"/> class.
        /// </summary>
        /// <param name="employeeService">Service for managing employee data.</param>
        /// <param name="employeeFactory">Factory for creating employee entities.</param>
        public EmployeeController(IEmployeeService employeeService, IEmployeeFactory employeeFactory)
        {
            _employeeService = employeeService;
            _employeeFactory = employeeFactory;
        }

        /// <summary>
        /// Displays a list of all employees.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A view displaying the list of employees.</returns>
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            // Retrieve all employees from the service.
            var employees = await _employeeService.GetAllAsync(cancellationToken);

            // Return the view with the list of employees.
            return View(employees);
        }

        /// <summary>
        /// Displays the create employee form.
        /// </summary>
        /// <returns>A view for creating a new employee.</returns>
        public IActionResult Create()
        {
            // Return the view for creating a new employee.
            return View();
        }

        /// <summary>
        /// Handles the form submission for creating a new employee.
        /// </summary>
        /// <param name="model">The view model containing the data for the new employee.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A redirect to the employee list view if successful; otherwise, the create view with validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                // Create a new employee entity using the factory.
                var employee = _employeeFactory.CreateEmployee(model);

                // Add the new employee using the service.
                await _employeeService.AddAsync(employee, cancellationToken);

                // Redirect to the employee list view after successful creation.
                return RedirectToAction(nameof(Index));
            }

            // If the model state is invalid, return the view with the current model data.
            return View(model);
        }

        /// <summary>
        /// Displays the edit employee form.
        /// </summary>
        /// <param name="id">The ID of the employee to edit.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A view for editing the employee, or a NotFound result if the employee is not found.</returns>
        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the employee by ID.
            var employee = await _employeeService.GetByIdAsync(id.Value, cancellationToken);
            if (employee == null)
            {
                return NotFound();
            }

            // Return the view for editing the employee.
            return View(new EditEmployeeViewModel(employee));
        }

        /// <summary>
        /// Handles the form submission for editing an existing employee.
        /// </summary>
        /// <param name="id">The ID of the employee being edited.</param>
        /// <param name="employee">The view model containing the updated employee data.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A redirect to the employee list view if successful; otherwise, the edit view with validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditEmployeeViewModel employee, CancellationToken cancellationToken)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the employee using the service.
                    await _employeeService.UpdateAsync(employee, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Check if the employee exists before handling concurrency exception.
                    if (!await _employeeService.EmployeeExistsAsync(id, cancellationToken))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the employee list view after successful update.
                return RedirectToAction(nameof(Index));
            }

            // If the model state is invalid, return the view with the current model data.
            return View(employee);
        }

        /// <summary>
        /// Displays the delete employee confirmation page.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A view for confirming the deletion of the employee, or a NotFound result if the employee is not found.</returns>
        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the employee by ID.
            var employee = await _employeeService.GetByIdAsync(id.Value, cancellationToken);
            if (employee == null)
            {
                return NotFound();
            }

            // Return the view for confirming the deletion of the employee.
            return View(employee);
        }

        /// <summary>
        /// Handles the form submission for deleting an employee.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A redirect to the employee list view after successful deletion.</returns>
        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            // Delete the employee using the service.
            await _employeeService.DeleteAsync(id, cancellationToken);

            // Redirect to the employee list view after successful deletion.
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the details of a specific employee.
        /// </summary>
        /// <param name="id">The ID of the employee to view details for.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A view displaying the employee details, or a NotFound result if the employee is not found.</returns>
        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the employee by ID.
            var employee = await _employeeService.GetByIdAsync(id.Value, cancellationToken);
            if (employee == null)
            {
                return NotFound();
            }

            // Return the view displaying the employee details.
            return View(employee);
        }


    }
}
