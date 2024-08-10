using CSOB_Interview_WorkLogger.Data;
using CSOB_Interview_WorkLogger.Data.Classes;
using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace CSOB_Interview_WorkLogger.Services
{
    public interface IEmployeeService
    {
        Task<List<EmployeeViewModel>> GetEmployeesWithHoursWorkedAsync(CancellationToken cancellationToken);
        Task<SelectList> GetEmployeeSelectListAsync(CancellationToken cancellationToken, int employeeId = 0);
        Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken);
        Task<Employee> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task AddAsync(Employee employee, CancellationToken cancellationToken);
        Task UpdateAsync(EditEmployeeViewModel employee, CancellationToken cancellationToken);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task<bool> EmployeeExistsAsync(int id, CancellationToken cancellationToken);
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITimeTrackingService _timeTrackingService;

        public EmployeeService(ApplicationDbContext context, ITimeTrackingService timeTrackingService)
        {
            _context = context;
            _timeTrackingService = timeTrackingService;
        }

        public async Task<List<EmployeeViewModel>> GetEmployeesWithHoursWorkedAsync(CancellationToken cancellationToken)
        {
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

        public async Task<SelectList> GetEmployeeSelectListAsync(CancellationToken cancellationToken, int employeeId = 0)
        {
            var employees = await _context.Employees
                .OrderBy(e => e.FirstName)
                .ToListAsync(cancellationToken);

            return new SelectList(employees, "ID", "FirstName", employeeId);
        }

        public async Task<List<Employee>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Employees.ToListAsync(cancellationToken);
        }

        public async Task<Employee> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Employees.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task AddAsync(Employee employee, CancellationToken cancellationToken)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(EditEmployeeViewModel model, CancellationToken cancellationToken)
        {
            var employee = await _context.Employees.SingleAsync(x => x.ID == model.Id, cancellationToken);
            if (employee != null)
            {
                employee.FirstName = model.FirstName;
                employee.LastName = model.LastName;
                employee.JobPosition = model.JobPosition;
                employee.Email = model.Email;
                
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync(cancellationToken);
            }
            
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var employee = await _context.Employees.FindAsync(new object[] { id }, cancellationToken);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<bool> EmployeeExistsAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Employees.AnyAsync(e => e.ID == id, cancellationToken);
        }
    }
}