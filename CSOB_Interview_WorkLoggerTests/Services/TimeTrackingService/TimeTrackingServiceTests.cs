using CSOB_Interview_WorkLogger.Models;
using CSOB_Interview_WorkLogger.Data.Classes;
using CSOB_Interview_WorkLogger.Services;


namespace CSOB_Interview_WorkLoggerTests;

public class TimeTrackingServiceTests
{
    private readonly ITimeTrackingService _service;

    public TimeTrackingServiceTests()
    {
        // Inicializácia služby - môžete použiť mock pre ApplicationDbContext, ak je to potrebné
        _service = new TimeTrackingService();
    }

    [Fact]
    public void CalculateTotalHoursWorked_SingleInterval_ReturnsCorrectHours()
    {
        var workLogs = new List<WorkLogEntry>
        {
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 9, 0, 0), Status = (int)WorkLogStatus.Started },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 17, 0, 0), Status = (int)WorkLogStatus.Finished }
        };

        var result = _service.CalculateTotalHoursWorked(workLogs);

        Assert.Equal(8, result);
    }

    [Fact]
    public void CalculateTotalHoursWorked_MultipleNonOverlappingIntervals_ReturnsCorrectHours()
    {
        var workLogs = new List<WorkLogEntry>
        {
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 9, 0, 0), Status = (int)WorkLogStatus.Started },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 12, 0, 0), Status = (int)WorkLogStatus.Finished },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 13, 0, 0), Status = (int)WorkLogStatus.Started },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 17, 0, 0), Status = (int)WorkLogStatus.Finished }
        };

        var result = _service.CalculateTotalHoursWorked(workLogs);

        Assert.Equal(7, result);
    }

    [Fact]
    public void CalculateTotalHoursWorked_OverlappingIntervals_ReturnsCorrectHours()
    {
        var workLogs = new List<WorkLogEntry>
        {
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 9, 0, 0), Status = (int)WorkLogStatus.Started },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 10, 0, 0), Status = (int)WorkLogStatus.Started },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 13, 0, 0), Status = (int)WorkLogStatus.Finished },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 18, 0, 0), Status = (int)WorkLogStatus.Finished }
        };

        var result = _service.CalculateTotalHoursWorked(workLogs);

        Assert.Equal(9, result);
    }

    [Fact]
    public void CalculateTotalHoursWorked_MultipleStartsBeforeFinish_ReturnsCorrectHours()
    {
        var workLogs = new List<WorkLogEntry>
        {
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 9, 0, 0), Status = (int)WorkLogStatus.Started },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 10, 0, 0), Status = (int)WorkLogStatus.Started },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 11, 0, 0), Status = (int)WorkLogStatus.Started },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 17, 0, 0), Status = (int)WorkLogStatus.Finished }
        };

        var result = _service.CalculateTotalHoursWorked(workLogs);

        Assert.Equal(8, result);
    }

    [Fact]
    public void CalculateTotalHoursWorked_EmptyList_ReturnsZero()
    {
        var workLogs = new List<WorkLogEntry>();

        var result = _service.CalculateTotalHoursWorked(workLogs);

        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateTotalHoursWorked_OnlyStartedLogs_ReturnsZero()
    {
        var workLogs = new List<WorkLogEntry>
        {
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 9, 0, 0), Status = (int)WorkLogStatus.Started },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 10, 0, 0), Status = (int)WorkLogStatus.Started }
        };

        var result = _service.CalculateTotalHoursWorked(workLogs);

        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateTotalHoursWorked_OnlyFinishedLogs_ReturnsZero()
    {
        var workLogs = new List<WorkLogEntry>
        {
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 17, 0, 0), Status = (int)WorkLogStatus.Finished },
            new WorkLogEntry { Created = new DateTime(2024, 1, 1, 18, 0, 0), Status = (int)WorkLogStatus.Finished }
        };

        var result = _service.CalculateTotalHoursWorked(workLogs);

        Assert.Equal(0, result);
    }
    
    
    [Fact]
        public void MergeIntervals_NoOverlap_ReturnsSameIntervals()
        {
            var intervals = new List<(DateTime Start, DateTime End)>
            {
                (new DateTime(2024, 1, 1, 9, 0, 0), new DateTime(2024, 1, 1, 10, 0, 0)),
                (new DateTime(2024, 1, 1, 11, 0, 0), new DateTime(2024, 1, 1, 12, 0, 0))
            };

            var result = _service.MergeIntervals(intervals);

            Assert.Equal(2, result.Count);
            Assert.Equal(intervals[0], result[0]);
            Assert.Equal(intervals[1], result[1]);
        }

        [Fact]
        public void MergeIntervals_Overlap_ReturnsMergedIntervals()
        {
            var intervals = new List<(DateTime Start, DateTime End)>
            {
                (new DateTime(2024, 1, 1, 9, 0, 0), new DateTime(2024, 1, 1, 11, 0, 0)),
                (new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 12, 0, 0))
            };

            var result = _service.MergeIntervals(intervals);

            Assert.Single(result);
            Assert.Equal(new DateTime(2024, 1, 1, 9, 0, 0), result[0].Start);
            Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0), result[0].End);
        }

        [Fact]
        public void MergeIntervals_ContainedIntervals_ReturnsMergedIntervals()
        {
            var intervals = new List<(DateTime Start, DateTime End)>
            {
                (new DateTime(2024, 1, 1, 9, 0, 0), new DateTime(2024, 1, 1, 13, 0, 0)),
                (new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 11, 0, 0))
            };

            var result = _service.MergeIntervals(intervals);

            Assert.Single(result);
            Assert.Equal(new DateTime(2024, 1, 1, 9, 0, 0), result[0].Start);
            Assert.Equal(new DateTime(2024, 1, 1, 13, 0, 0), result[0].End);
        }

        [Fact]
        public void MergeIntervals_AdjacentIntervals_ReturnsMergedIntervals()
        {
            var intervals = new List<(DateTime Start, DateTime End)>
            {
                (new DateTime(2024, 1, 1, 9, 0, 0), new DateTime(2024, 1, 1, 10, 0, 0)),
                (new DateTime(2024, 1, 1, 10, 0, 0), new DateTime(2024, 1, 1, 11, 0, 0))
            };

            var result = _service.MergeIntervals(intervals);

            Assert.Single(result);
            Assert.Equal(new DateTime(2024, 1, 1, 9, 0, 0), result[0].Start);
            Assert.Equal(new DateTime(2024, 1, 1, 11, 0, 0), result[0].End);
        }

        [Fact]
        public void MergeIntervals_EmptyList_ReturnsEmptyList()
        {
            var intervals = new List<(DateTime Start, DateTime End)>();

            var result = _service.MergeIntervals(intervals);

            Assert.Empty(result);
        }
        
        
        [Fact]
        public void PairIntervals_SingleStartFinishPair_ReturnsSingleInterval()
        {
            var workLogs = new List<WorkLogEntry>
            {
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 9, 0, 0), Status = (int)WorkLogStatus.Started },
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 17, 0, 0), Status = (int)WorkLogStatus.Finished }
            };

            var result = _service.PairIntervals(workLogs);

            Assert.Single(result);
            Assert.Equal(new DateTime(2024, 1, 1, 9, 0, 0), result[0].Start);
            Assert.Equal(new DateTime(2024, 1, 1, 17, 0, 0), result[0].End);
        }

        [Fact]
        public void PairIntervals_MultipleStartFinishPairs_ReturnsMultipleIntervals()
        {
            var workLogs = new List<WorkLogEntry>
            {
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 9, 0, 0), Status = (int)WorkLogStatus.Started },
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 9, 0, 0), Status = (int)WorkLogStatus.Started },
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 12, 0, 0), Status = (int)WorkLogStatus.Finished },
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 13, 0, 0), Status = (int)WorkLogStatus.Started },
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 17, 0, 0), Status = (int)WorkLogStatus.Finished }
            };

            var result = _service.PairIntervals(workLogs);

            Assert.Equal(2, result.Count);
            Assert.Equal(new DateTime(2024, 1, 1, 9, 0, 0), result[0].Start);
            Assert.Equal(new DateTime(2024, 1, 1, 12, 0, 0), result[0].End);
            Assert.Equal(new DateTime(2024, 1, 1, 9, 0, 0), result[1].Start);
            Assert.Equal(new DateTime(2024, 1, 1, 17, 0, 0), result[1].End);
        }

        [Fact]
        public void PairIntervals_OnlyStartLogs_ReturnsEmptyList()
        {
            var workLogs = new List<WorkLogEntry>
            {
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 9, 0, 0), Status = (int)WorkLogStatus.Started },
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 10, 0, 0), Status = (int)WorkLogStatus.Started }
            };

            var result = _service.PairIntervals(workLogs);

            Assert.Empty(result);
        }

        [Fact]
        public void PairIntervals_OnlyFinishLogs_ReturnsEmptyList()
        {
            var workLogs = new List<WorkLogEntry>
            {
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 17, 0, 0), Status = (int)WorkLogStatus.Finished },
                new WorkLogEntry { Created = new DateTime(2024, 1, 1, 18, 0, 0), Status = (int)WorkLogStatus.Finished }
            };

            var result = _service.PairIntervals(workLogs);

            Assert.Empty(result);
        }
}