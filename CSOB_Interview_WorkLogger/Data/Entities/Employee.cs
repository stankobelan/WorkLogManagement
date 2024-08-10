using System.ComponentModel.DataAnnotations;

namespace CSOB_Interview_WorkLogger.Data.Entities;

public class Employee
{
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string JobPosition { get; set; }
    public DateTime OnboardingDay { get; set; }
    public virtual ICollection<WorkLog> WorkLogs { get; set; }
}
