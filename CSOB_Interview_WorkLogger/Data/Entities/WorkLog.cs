
namespace CSOB_Interview_WorkLogger.Data.Entities;

public class WorkLog
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Created { get; set; }
    public string Description { get; set; }
    
    public int Status { get; set; }
    public virtual Employee Employee { get; set; }
}
