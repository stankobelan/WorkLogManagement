using CSOB_Interview_WorkLogger.Data;
using CSOB_Interview_WorkLogger.Data.Classes;
using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSOB_Interview_WorkLogger.Services
{
    /// <summary>
    /// Interface defining methods for managing employees.
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// Retrieves a list of employees along with their total hours worked.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A list of EmployeeViewModel objects.</returns>
        Task<List<EmployeeViewModel>> GetEmployeesWithHoursWorkedAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a select list of employees.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <param name="employeeId">Optional selected employee ID.</param>
        /// <returns>A SelectList of employees.</returns>
        Task<SelectList> GetEmployeeSelectListAsync(CancellationToken cancellationToken, int employeeId = 0);

        /// <summary>
        /// Retrieves a list of all employees.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A list of Employee entities.</returns>
        Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a specific employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee to retrieve.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>The Employee entity if found; otherwise, null.</returns>
        Task<Employee> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new employee to the database.
        /// </summary>
        /// <param name="employee">The Employee entity to add.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task AddAsync(Employee employee, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing employee in the database.
        /// </summary>
        /// <param name="model">The view model containing updated employee data.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task UpdateAsync(EditEmployeeViewModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a specific employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task DeleteAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if an employee with a specific ID exists in the database.
        /// </summary>
        /// <param name="id">The ID of the employee to check.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>True if the employee exists; otherwise, false.</returns>
        Task<bool> EmployeeExistsAsync(int id, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Service implementation for managing employees.
    /// </summary>
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITimeTrackingService _timeTrackingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmployeeService"/> class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        /// <param name="timeTrackingService">The service used to calculate total hours worked.</param>
        public EmployeeService(ApplicationDbContext context, ITimeTrackingService timeTrackingService)
        {
            _context = context;
            _timeTrackingService = timeTrackingService;
        }

        /// <summary>
        /// Retrieves a list of employees along with their total hours worked.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A list of EmployeeViewModel objects.</returns>
        public async Task<List<EmployeeViewModel>> GetEmployeesWithHoursWorkedAsync(CancellationToken cancellationToken)
        {
            // Retrieve employees and their work logs from the database.
            var employeesWithLogs = await _context.Employees
                .Include(e => e.WorkLogs)
                .Select(e => new
                {
                    e.ID,
                    e.FirstName,
                    e.LastName,
                    e.JobPosition,
                    e.OnboardingDay,
                    WorkLogs = e.WorkLogs.Select(wl => new WorkLogEntry
                    {
                        Created = wl.Created,
                        Status = wl.Status
                    }).ToList()
                })
                .ToListAsync(cancellationToken);

            // Map to EmployeeViewModel and calculate total hours worked.
            return employeesWithLogs.Select(e => new EmployeeViewModel
            {
                Id = e.ID,
                FirstName = e.FirstName,
                LastName = e.LastName,
                JobPosition = e.JobPosition,
                OnboardingDay = e.OnboardingDay,
                TotalHoursWorked = _timeTrackingService.CalculateTotalHoursWorked(e.WorkLogs)
            }).ToList();
        }

        /// <summary>
        /// Retrieves a select list of employees.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <param name="employeeId">Optional selected employee ID.</param>
        /// <returns>A SelectList of employees.</returns>
        public async Task<SelectList> GetEmployeeSelectListAsync(CancellationToken cancellationToken, int employeeId = 0)
        {
            // Retrieve employees from the database, ordered by first name.
            var employees = await _context.Employees
                .OrderBy(e => e.FirstName)
                .ToListAsync(cancellationToken);

            // Return the select list with the specified employee ID selected by default.
            return new SelectList(employees, "ID", "FirstName", employeeId);
        }

        /// <summary>
        /// Retrieves a list of all employees.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A list of Employee entities.</returns>
        public async Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken)
        {
            // Retrieve all employees from the database.
            return await _context.Employees.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves a specific employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee to retrieve.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>The Employee entity if found; otherwise, null.</returns>
        public async Task<Employee> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            // Find the employee by ID in the database.
            return await _context.Employees.FindAsync(new object[] { id }, cancellationToken);
        }

        /// <summary>
        /// Adds a new employee to the database.
        /// </summary>
        /// <param name="employee">The Employee entity to add.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
        {
            // Add the new employee to the database context.
            _context.Employees.Add(employee);

            // Save changes to the database.
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Updates an existing employee in the database.
        /// </summary>
        /// <param name="model">The view model containing updated employee data.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        public async Task UpdateAsync(EditEmployeeViewModel model, CancellationToken cancellationToken)
        {
            // Find the existing employee by ID.
            var employee = await _context.Employees.SingleAsync(x => x.ID == model.Id, cancellationToken);
            if (employee != null)
            {
                // Update the employee fields with the data from the view model.
                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                employee.JobPosition = model.JobPosition;
                employee.Email = model.Email;

                // Mark the employee entity as updated in the database context.
                _context.Employees.Update(employee);

                // Save the changes to the database.
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Deletes a specific employee by ID.
        /// </summary>
        /// <param name="id">The ID of the employee to delete.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            // Find the employee by ID.
            var employee = await _context.Employees.FindAsync(new object[] { id }, cancellationToken);
            if (employee != null)
            {
                // Remove the employee from the database context.
                _context.Employees.Remove(employee);

                // Save the changes to the database.
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Checks if an employee with a specific ID exists in the database.
        /// </summary>
        /// <param name="id">The ID of the employee to check.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>True if the employee exists; otherwise, false.</returns>
        public async Task<bool> EmployeeExistsAsync(int id, CancellationToken cancellationToken)
        {
            // Check if any employee with the specified ID exists in the database.
            return await _context.Employees.AnyAsync(e => e.ID == id, cancellationToken);
        }
    }
}
