using CSOB_Interview_WorkLogger.Data.Entities;
using CSOB_Interview_WorkLogger.Models;

namespace CSOB_Interview_WorkLogger.Factories
{
    /// <summary>
    /// Interface defining a factory for creating Employee entities.
    /// </summary>
    public interface IEmployeeFactory
    {
        /// <summary>
        /// Creates a new Employee entity based on the provided view model.
        /// </summary>
        /// <param name="model">The view model containing the data for the new employee.</param>
        /// <returns>A new Employee entity.</returns>
        Employee CreateEmployee(CreateEmployeeViewModel model);
    }

    /// <summary>
    /// Factory implementation for creating Employee entities.
    /// </summary>
    public class EmployeeFactory : IEmployeeFactory
    {
        /// <summary>
        /// Creates a new Employee entity based on the provided view model.
        /// </summary>
        /// <param name="model">The view model containing the data for the new employee.</param>
        /// <returns>A new Employee entity.</returns>
        public Employee CreateEmployee(CreateEmployeeViewModel model)
        {
            // Create a new Employee entity and map the data from the view model.
            var employee = new Employee()
            {
                Email = model.Email,
                JobPosition = model.JobPosition,
                FirstName = model.FirstName,
                LastName = model.LastName,
                OnboardingDay = model.OnboardingDay
            };

            // Return the newly created Employee entity.
            return employee;
        }
    }
}