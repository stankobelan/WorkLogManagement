using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Models;
using CSOB_Interview_WorkLogger.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSOB_Interview_WorkLogger.Controllers
{
    public class WorkLogController : Controller
    {
        private readonly IWorkLogService _workLogService;
        private readonly IEmployeeService _employeeService;

        public WorkLogController(IWorkLogService workLogService, IEmployeeService employeeService)
        {
            _workLogService = workLogService;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index(int? employeeId, CancellationToken cancellationToken)
        {
            var workLogs = await _workLogService.GetAllAsync(cancellationToken);
            return View(workLogs.Select(x=> new WorkLogViewModel(x)));
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var viewModel = new WorkLogViewModel
            {
                EmployeeSelectList = await _employeeService.GetEmployeeSelectListAsync(cancellationToken)
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkLogViewModel viewModel, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var workLog = new WorkLog
                {
                    EmployeeId = viewModel.EmployeeId,
                    Created = viewModel.Created,
                    Description = viewModel.Description,
                    Status = (int)viewModel.Status
                };
                await _workLogService.AddAsync(workLog, cancellationToken);
                return RedirectToAction(nameof(Index));
            }

            viewModel.EmployeeSelectList = await _employeeService.GetEmployeeSelectListAsync(cancellationToken);
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workLog = await _workLogService.GetByIdAsync(id.Value, cancellationToken);
            if (workLog == null)
            {
                return NotFound();
            }

            var viewModel = new WorkLogViewModel(workLog);
            viewModel.EmployeeSelectList = await _employeeService.GetEmployeeSelectListAsync(cancellationToken, workLog.EmployeeId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkLogViewModel workLog,
            CancellationToken cancellationToken)
        {
            if (id != workLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _workLogService.UpdateAsync(workLog, cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _workLogService.WorkLogExistsAsync(workLog.Id, cancellationToken))
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

            workLog.EmployeeSelectList = await _employeeService.GetEmployeeSelectListAsync(cancellationToken, workLog.EmployeeId);
            return View(workLog);
        }

        public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workLog = await _workLogService.GetByIdAsync(id.Value, cancellationToken);
            if (workLog == null)
            {
                return NotFound();
            }

            return View(workLog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
        {
            await _workLogService.DeleteAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workLog = await _workLogService.GetByIdAsync(id.Value, cancellationToken);
            if (workLog == null)
            {
                return NotFound();
            }

            return View(workLog);
        }


    }
}