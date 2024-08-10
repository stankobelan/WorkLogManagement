using System.ComponentModel.DataAnnotations;
using CSOB_Interview_WorkLogger.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CSOB_Interview_WorkLogger.Models;

public class WorkLogViewModel
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string? EmployeeName { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime Created { get; set; }
    public string Description { get; set; }
        
    public WorkLogStatus Status { get; set; }  // Using enum type for the status
    public string StatusText => Status.ToString();
    
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]    
    public SelectList? EmployeeSelectList { get; set; }

    public WorkLogViewModel(WorkLog workLog)
    {
        Id = workLog.Id;
        EmployeeId = workLog.EmployeeId;
        Created = workLog.Created;
        Description = workLog.Description;
        Status = workLog.Status == 0 ? WorkLogStatus.Started : WorkLogStatus.Finished;
        EmployeeName = $" {workLog.Employee.FirstName} {workLog.Employee.LastName}";
    }

    public WorkLogViewModel()
    {
        Created = DateTime.Now;
    }
}
