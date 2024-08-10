using CSOB_Interview_WorkLogger.Data.Classes;
using CSOB_Interview_WorkLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSOB_Interview_WorkLogger.Services
{
    /// <summary>
    /// Interface defining methods for time tracking operations.
    /// </summary>
    public interface ITimeTrackingService
    {
        /// <summary>
        /// Calculates the total hours worked based on the provided work logs.
        /// </summary>
        /// <param name="workLogs">List of work log entries.</param>
        /// <returns>The total hours worked.</returns>
        double CalculateTotalHoursWorked(List<WorkLogEntry> workLogs);

        /// <summary>
        /// Pairs start and finish logs into intervals.
        /// </summary>
        /// <param name="sortedLogs">List of sorted work log entries.</param>
        /// <returns>A list of intervals defined by start and end times.</returns>
        List<(DateTime Start, DateTime End)> PairIntervals(List<WorkLogEntry> sortedLogs);

        /// <summary>
        /// Merges overlapping intervals into a single continuous interval.
        /// </summary>
        /// <param name="intervals">List of intervals to merge.</param>
        /// <returns>A list of merged intervals.</returns>
        List<(DateTime Start, DateTime End)> MergeIntervals(List<(DateTime Start, DateTime End)> intervals);
    }

    /// <summary>
    /// Service implementation for managing time tracking operations.
    /// </summary>
    public class TimeTrackingService : ITimeTrackingService
    {
        /// <summary>
        /// Calculates the total hours worked based on the provided work logs.
        /// </summary>
        /// <param name="workLogs">List of work log entries.</param>
        /// <returns>The total hours worked.</returns>
        public double CalculateTotalHoursWorked(List<WorkLogEntry> workLogs)
        {
            // Sort the logs by the created date to ensure chronological order.
            var sortedLogs = workLogs.OrderBy(wl => wl.Created).ToList();

            // Pair the sorted logs into intervals.
            var workIntervals = PairIntervals(sortedLogs);

            // Merge any overlapping intervals.
            var mergedIntervals = MergeIntervals(workIntervals);

            // Calculate the total hours worked by summing the duration of each interval.
            return mergedIntervals.Sum(interval => (interval.End - interval.Start).TotalHours);
        }

        /// <summary>
        /// Pairs start and finish logs into intervals.
        /// </summary>
        /// <param name="sortedLogs">List of sorted work log entries.</param>
        /// <returns>A list of intervals defined by start and end times.</returns>
        public List<(DateTime Start, DateTime End)> PairIntervals(List<WorkLogEntry> sortedLogs)
        {
            var workIntervals = new List<(DateTime Start, DateTime End)>();
            bool canContinue = true;

            // Continue pairing intervals as long as possible.
            while (canContinue)
            {
                // Find the earliest "start" log.
                var startLog = sortedLogs
                    .Where(log => log.Status == (int)WorkLogStatus.Started)
                    .OrderBy(log => log.Created)
                    .FirstOrDefault();

                // Find the earliest "finish" log.
                var finishLog = sortedLogs
                    .Where(log => log.Status == (int)WorkLogStatus.Finished)
                    .OrderBy(log => log.Created)
                    .FirstOrDefault();

                // Check if both logs exist and if the start is before or equal to the finish.
                if (startLog != null && finishLog != null && startLog.Created <= finishLog.Created)
                {
                    // Create the interval by pairing the start and finish logs.
                    workIntervals.Add((startLog.Created, finishLog.Created));

                    // Remove the paired logs from the list.
                    sortedLogs.Remove(startLog);
                    sortedLogs.Remove(finishLog);
                }
                else
                {
                    // Stop the loop if no valid interval can be formed.
                    canContinue = false;
                }
            }

            return workIntervals;
        }

        /// <summary>
        /// Merges overlapping intervals into a single continuous interval.
        /// </summary>
        /// <param name="intervals">List of intervals to merge.</param>
        /// <returns>A list of merged intervals.</returns>
        public List<(DateTime Start, DateTime End)> MergeIntervals(List<(DateTime Start, DateTime End)> intervals)
        {
            if (intervals.Count <= 1)
                return intervals;

            // Sort the intervals by their start times.
            var sorted = intervals.OrderBy(i => i.Start).ToList();
            var merged = new List<(DateTime Start, DateTime End)>();
            var current = sorted[0];

            // Iterate through the sorted intervals.
            for (int i = 1; i < sorted.Count; i++)
            {
                // If the current interval overlaps with the next, merge them.
                if (sorted[i].Start <= current.End)
                {
                    current.End = Max(current.End, sorted[i].End);
                }
                else
                {
                    // If no overlap, add the current interval to the merged list and move to the next.
                    merged.Add(current);
                    current = sorted[i];
                }
            }

            // Add the last interval.
            merged.Add(current);

            return merged;
        }

        /// <summary>
        /// Returns the maximum of two DateTime values.
        /// </summary>
        /// <param name="a">First DateTime value.</param>
        /// <param name="b">Second DateTime value.</param>
        /// <returns>The later of the two DateTime values.</returns>
        private DateTime Max(DateTime a, DateTime b)
        {
            return a > b ? a : b;
        }
    }
}
