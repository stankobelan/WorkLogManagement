using CSOB_Interview_WorkLogger.Data.Classes;
using CSOB_Interview_WorkLogger.Models;

namespace CSOB_Interview_WorkLogger.Services;
public interface ITimeTrackingService
{
    double CalculateTotalHoursWorked(List<WorkLogEntry> workLogs);
    List<(DateTime Start, DateTime End)> PairIntervals(List<WorkLogEntry> sortedLogs);
    List<(DateTime Start, DateTime End)> MergeIntervals(List<(DateTime Start, DateTime End)> intervals);
}
public class TimeTrackingService : ITimeTrackingService
    {
        public double CalculateTotalHoursWorked(List<WorkLogEntry> workLogs)
        {
            var sortedLogs = workLogs.OrderBy(wl => wl.Created).ToList();
            var workIntervals = PairIntervals(sortedLogs);

            // Merge overlapping intervals
            var mergedIntervals = MergeIntervals(workIntervals);

            // Calculate total hours
            return mergedIntervals.Sum(interval => (interval.End - interval.Start).TotalHours);
        }

        public List<(DateTime Start, DateTime End)> PairIntervals(List<WorkLogEntry> sortedLogs)
        {
            var workIntervals = new List<(DateTime Start, DateTime End)>();
            bool canContinue = true;

            while (canContinue)
            {
                // Find the earliest "start" log
                var startLog = sortedLogs
                    .Where(log => log.Status == (int)WorkLogStatus.Started)
                    .OrderBy(log => log.Created)
                    .FirstOrDefault();

                // Find the earliest "finish" log
                var finishLog = sortedLogs
                    .Where(log => log.Status == (int)WorkLogStatus.Finished)
                    .OrderBy(log => log.Created)
                    .FirstOrDefault();

                // Check if both logs exist and if the start is before or equal to the finish
                if (startLog != null && finishLog != null && startLog.Created <= finishLog.Created)
                {
                    // Create the interval
                    workIntervals.Add((startLog.Created, finishLog.Created));

                    // Remove the logs from the list
                    sortedLogs.Remove(startLog);
                    sortedLogs.Remove(finishLog);
                }
                else
                {
                    // Stop the loop if no valid interval can be formed
                    canContinue = false;
                }
            }

            return workIntervals;
        }

        public List<(DateTime Start, DateTime End)> MergeIntervals(List<(DateTime Start, DateTime End)> intervals)
        {
            if (intervals.Count <= 1)
                return intervals;

            var sorted = intervals.OrderBy(i => i.Start).ToList();
            var merged = new List<(DateTime Start, DateTime End)>();
            var current = sorted[0];

            for (int i = 1; i < sorted.Count; i++)
            {
                if (sorted[i].Start <= current.End)
                {
                    // Overlap found, merge
                    current.End = Max(current.End, sorted[i].End);
                }
                else
                {
                    // No overlap, add current to result and move to next
                    merged.Add(current);
                    current = sorted[i];
                }
            }

            // Add the last interval
            merged.Add(current);

            return merged;
        }

        private DateTime Max(DateTime a, DateTime b)
        {
            return a > b ? a : b;
        }
    }