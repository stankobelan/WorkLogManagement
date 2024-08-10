using CSOB_Interview_WorkLogger.Data.Entities;

namespace CSOB_Interview_WorkLogger.Models;

public class CreateEmployeeViewModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string JobPosition { get; set; }
    public string Email { get; set; }
    
    public DateTime OnboardingDay { get; set; }

    protected CreateEmployeeViewModel(DateTime onboardingDay)
    {
        OnboardingDay = onboardingDay;
    }

    public CreateEmployeeViewModel()
    {
        OnboardingDay = DateTime.Now;
    }
    protected CreateEmployeeViewModel(string firstName, string lastName, string jobPosition, string email, DateTime onboardingDay)
    {
        FirstName = firstName;
        LastName = lastName;
        JobPosition = jobPosition;
        Email = email;
        OnboardingDay = onboardingDay;
    }
}

public class EditEmployeeViewModel : CreateEmployeeViewModel
{
    public int Id { get; set; }

    protected EditEmployeeViewModel() : base()
    {
    }

    public EditEmployeeViewModel(Employee employee) : base(employee.FirstName, employee.LastName, employee.JobPosition,
        employee.Email, employee.OnboardingDay)
    {
        Id = employee.ID;
    }
}

public class EmployeeViewModel : EditEmployeeViewModel
{
    public double TotalHoursWorked { get; set; } // This will store the calculated hours
}