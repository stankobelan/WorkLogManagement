using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Models;
using CSOB_Interview_WorkLogger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSOB_Interview_WorkLogger.Controllers
{
    /// <summary>
    /// Controller for managing work log-related actions.
    /// </summary>
    public class WorkLogController : Controller
    {
        private readonly IWorkLogService _workLogService;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkLogController"/> class.
        /// </summary>
        /// <param name="workLogService">Service for managing work log data.</param>
        /// <param name="employeeService">Service for managing employee data.</param>
        public WorkLogController(IWorkLogService workLogService, IEmployeeService employeeService)
        {
            _workLogService = workLogService;
            _employeeService = employeeService;
        }

        /// <summary>
        /// Displays a list of all work logs.
        /// </summary>
        /// <param name="employeeId">Optional ID of the employee to filter work logs by.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A view displaying the list of work logs.</returns>
        public async Task<IActionResult> Index(int? employeeId, CancellationToken cancellationToken)
        {
            // Retrieve all work logs from the service.
            var workLogs = await _workLogService.GetAllAsync(cancellationToken);

            // Convert the work logs to view models and return the view.
            return View(workLogs.Select(x => new WorkLogViewModel(x)));
        }

        /// <summary>
        /// Displays the create work log form.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A view for creating a new work log.</returns>
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            // Prepare the view model with a select list of employees.
            var viewModel = new WorkLogViewModel
            {
                EmployeeSelectList = await _employeeService.GetEmployeeSelectListAsync(cancellationToken)
            };

            // Return the view for creating a new work log.
            return View(viewModel);
        }

        /// <summary>
        /// Handles the form submission for creating a new work log.
        /// </summary>
        /// <param name="viewModel">The view model containing the data for the new work log.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A redirect to the work log list view if successful; otherwise, the create view with validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkLogViewModel viewModel, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                // Create a new work log entity from the view model.
                var workLog = new WorkLog
                {
                    EmployeeId = viewModel.EmployeeId,
                    Created = viewModel.Created,
                    Description = viewModel.Description,
                    Status = (int)viewModel.Status
                };

                // Add the new work log using the service.
                await _workLogService.AddAsync(workLog, cancellationToken);

                // Redirect to the work log list view after successful creation.
                return RedirectToAction(nameof(Index));
            }

            // If the model state is invalid, repopulate the employee select list and return the view with the current data.
            viewModel.EmployeeSelectList = await _employeeService.GetEmployeeSelectListAsync(cancellationToken);
            return View(viewModel);
        }

        /// <summary>
        /// Displays the edit work log form.
        /// </summary>
        /// <param name="id">The ID of the work log to edit.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A view for editing the work log, or a NotFound result if the work log is not found.</returns>
        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the work log by ID.
            var workLog = await _workLogService.GetByIdAsync(id.Value, cancellationToken);
            if (workLog == null)
            {
                return NotFound();
            }

            // Prepare the view model for editing and populate the employee select list.
            var viewModel = new WorkLogViewModel(workLog)
            {
                EmployeeSelectList = await _employeeService.GetEmployeeSelectListAsync(cancellationToken, workLog.EmployeeId)
            };

            // Return the view for editing the work log.
            return View(viewModel);
        }

        /// <summary>
        /// Handles the form submission for editing an existing work log.
        /// </summary>
        /// <param name="id">The ID of the work log being edited.</param>
        /// <param name="workLog">The view model containing the updated work log data.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A redirect to the work log list view if successful; otherwise, the edit view with validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkLogViewModel workLog, CancellationToken cancellationToken)
        {
            if (id != workLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the work log using the service.
                    await _workLogService.UpdateAsync(workLog, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Check if the work log exists before handling concurrency exception.
                    if (!await _workLogService.WorkLogExistsAsync(workLog.Id, cancellationToken))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the work log list view after successful update.
                return RedirectToAction(nameof(Index));
            }

            // If the model state is invalid, repopulate the employee select list and return the view with the current data.
            workLog.EmployeeSelectList = await _employeeService.GetEmployeeSelectListAsync(cancellationToken, workLog.EmployeeId);
            return View(workLog);
        }

        /// <summary>
        /// Displays the delete work log confirmation page.
        /// </summary>
        /// <param name="id">The ID of the work log to delete.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A view for confirming the deletion of the work log, or a NotFound result if the work log is not found.</returns>
        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the work log by ID.
            var workLog = await _workLogService.GetByIdAsync(id.Value, cancellationToken);
            if (workLog == null)
            {
                return NotFound();
            }

            // Return the view for confirming the deletion of the work log.
            return View(workLog);
        }

        /// <summary>
        /// Handles the form submission for deleting a work log.
        /// </summary>
        /// <param name="id">The ID of the work log to delete.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A redirect to the work log list view after successful deletion.</returns>
        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            // Delete the work log using the service.
            await _workLogService.DeleteAsync(id, cancellationToken);

            // Redirect to the work log list view after successful deletion.
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the details of a specific work log.
        /// </summary>
        /// <param name="id">The ID of the work log to view details for.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A view displaying the work log details, or a NotFound result if the work log is not found.</returns>
        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Retrieve the work log by ID.
            var workLog = await _workLogService.GetByIdAsync(id.Value, cancellationToken);
            if (workLog == null)
            {
                return NotFound();
            }

            // Return the view displaying the work log details.
            return View(workLog);
        }
    }
}
