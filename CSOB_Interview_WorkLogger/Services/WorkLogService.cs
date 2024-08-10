using CSOB_Interview_WorkLogger.Data;
using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSOB_Interview_WorkLogger.Services
{
    /// <summary>
    /// Interface defining methods for managing work logs.
    /// </summary>
    public interface IWorkLogService
    {
        /// <summary>
        /// Retrieves all work logs from the database.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A list of work logs.</returns>
        Task<List<WorkLog>> GetAllAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a specific work log by its ID.
        /// </summary>
        /// <param name="id">The ID of the work log to retrieve.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>The work log if found; otherwise, null.</returns>
        Task<WorkLog> GetByIdAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new work log to the database.
        /// </summary>
        /// <param name="workLog">The work log entity to add.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task AddAsync(WorkLog workLog, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing work log in the database.
        /// </summary>
        /// <param name="model">The view model containing updated work log data.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task UpdateAsync(WorkLogViewModel model, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a specific work log by its ID.
        /// </summary>
        /// <param name="id">The ID of the work log to delete.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task DeleteAsync(int id, CancellationToken cancellationToken);

        /// <summary>
        /// Checks if a work log with a specific ID exists in the database.
        /// </summary>
        /// <param name="id">The ID of the work log to check.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>True if the work log exists; otherwise, false.</returns>
        Task<bool> WorkLogExistsAsync(int id, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Service implementation for managing work logs.
    /// </summary>
    public class WorkLogService : IWorkLogService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkLogService"/> class.
        /// </summary>
        /// <param name="context">The database context to use for data access.</param>
        public WorkLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously retrieves all work logs from the database.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>A list of all work logs, including associated employees.</returns>
        public async Task<List<WorkLog>> GetAllAsync(CancellationToken cancellationToken)
        {
            // Retrieve all work logs from the database, including associated employee data.
            return await _context.WorkLogs.Include(p => p.Employee).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously retrieves a specific work log by its ID.
        /// </summary>
        /// <param name="id">The ID of the work log to retrieve.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>The work log with the specified ID, or null if not found.</returns>
        public async Task<WorkLog> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            // Find the work log by ID and include the associated employee data.
            return await _context.WorkLogs.Include(p => p.Employee).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        /// <summary>
        /// Asynchronously adds a new work log to the database.
        /// </summary>
        /// <param name="workLog">The work log entity to add.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        public async Task AddAsync(WorkLog workLog, CancellationToken cancellationToken)
        {
            // Add the new work log to the database context.
            _context.WorkLogs.Add(workLog);

            // Save changes to the database.
            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously updates an existing work log in the database.
        /// </summary>
        /// <param name="model">The view model containing updated work log data.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        public async Task UpdateAsync(WorkLogViewModel model, CancellationToken cancellationToken)
        {
            // Find the existing work log by ID.
            var workLog = await _context.WorkLogs.FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);
            if (workLog != null)
            {
                // Update the work log fields with the data from the view model.
                workLog.Description = model.Description;
                workLog.Created = model.Created;
                workLog.EmployeeId = model.EmployeeId;

                // Mark the work log entity as updated in the database context.
                _context.WorkLogs.Update(workLog);

                // Save the changes to the database.
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
        

        /// <summary>
        /// Asynchronously deletes a specific work log by its ID.
        /// </summary>
        /// <param name="id">The ID of the work log to delete.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            // Find the work log by ID.
            var workLog = await _context.WorkLogs.FindAsync(new object[] { id }, cancellationToken);
            if (workLog != null)
            {
                // Remove the work log from the database context.
                _context.WorkLogs.Remove(workLog);

                // Save the changes to the database.
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously checks if a work log with a specific ID exists in the database.
        /// </summary>
        /// <param name="id">The ID of the work log to check.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>True if the work log exists; otherwise, false.</returns>
        public async Task<bool> WorkLogExistsAsync(int id, CancellationToken cancellationToken)
        {
            // Check if any work log with the specified ID exists in the database.
            return await _context.WorkLogs.AnyAsync(e => e.Id == id, cancellationToken);
        }
    }
}
