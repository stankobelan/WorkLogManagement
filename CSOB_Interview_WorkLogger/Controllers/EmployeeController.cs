using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Factories;
using CSOB_Interview_WorkLogger.Models;
using CSOB_Interview_WorkLogger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSOB_Interview_WorkLogger.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;

        public EmployeeController(IEmployeeService employeeService, IEmployeeFactory employeeFactory)
        {
            _employeeService = employeeService;
            _employeeFactory = employeeFactory;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var employees = await _employeeService.GetAllAsync(cancellationToken);
            return View(employees);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var employee = _employeeFactory.CreateEmployee(model);
                await _employeeService.AddAsync(employee, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetByIdAsync(id.Value, cancellationToken);
            if (employee == null)
            {
                return NotFound();
            }
            return View( new EditEmployeeViewModel( employee));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  EditEmployeeViewModel employee, CancellationToken cancellationToken)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.UpdateAsync(employee, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _employeeService.EmployeeExistsAsync(id, cancellationToken))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetByIdAsync(id.Value, cancellationToken);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            await _employeeService.DeleteAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetByIdAsync(id.Value, cancellationToken);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
    }
}
