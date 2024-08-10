using CSOB_Interview_WorkLogger.Data;
using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Models;
using Microsoft.EntityFrameworkCore;

namespace CSOB_Interview_WorkLogger.Services;

public interface IWorkLogService
{
    Task<List<WorkLog>> GetAllAsync(CancellationToken cancellationToken);
    Task<WorkLog> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task AddAsync(WorkLog workLog, CancellationToken cancellationToken);
    Task UpdateAsync(WorkLogViewModel workLog, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<bool> WorkLogExistsAsync(int id, CancellationToken cancellationToken);
}

public class WorkLogService : IWorkLogService
{
    private readonly ApplicationDbContext _context;

    public WorkLogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WorkLog>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.WorkLogs.Include(p => p.Employee).ToListAsync(cancellationToken);
    }

    public async Task<WorkLog> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.WorkLogs.Include(p => p.Employee).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(WorkLog workLog, CancellationToken cancellationToken)
    {
        _context.WorkLogs.Add(workLog);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WorkLogViewModel model, CancellationToken cancellationToken)
    {
        var workLog = await _context.WorkLogs.FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);
        if (workLog != null)
        {
            workLog.Description = model.Description;
            workLog.Created = model.Created;
            workLog.EmployeeId = model.EmployeeId;

            _context.WorkLogs.Update(workLog);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var workLog = await _context.WorkLogs.FindAsync(new object[] { id }, cancellationToken);
        if (workLog != null)
        {
            _context.WorkLogs.Remove(workLog);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> WorkLogExistsAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.WorkLogs.AnyAsync(e => e.Id == id, cancellationToken);
    }
}