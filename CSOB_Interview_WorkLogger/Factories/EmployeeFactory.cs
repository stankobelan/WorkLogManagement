using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Models;

namespace CSOB_Interview_WorkLogger.Factories;

public interface IEmployeeFactory
{
    Employee CreateEmployee(CreateEmployeeViewModel model);
}

public class EmployeeFactory : IEmployeeFactory
{
    public Employee CreateEmployee(CreateEmployeeViewModel model)
    {
        var employee = new Employee()
        {
            Email = model.Email,
            JobPosition = model.JobPosition,
            FirstName = model.FirstName,
            LastName = model.LastName,
            OnboardingDay = model.OnboardingDay
        };
        return employee;
    }
}