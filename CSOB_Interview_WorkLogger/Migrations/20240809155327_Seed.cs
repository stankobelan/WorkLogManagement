using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSOB_Interview_WorkLogger.Migrations
{
    /// <inheritdoc />
    public partial class Seed : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO Employees (FirstName, LastName, Email, JobPosition, OnboardingDay)\nVALUES \n('Jan', 'Novak', 'jan.novak@example.com', 'Developer','2024-01-01 09:00:00'),\n('Petra', 'Sukova', 'petra.sukova@example.com', 'Manager','2024-02-01 09:00:00'),\n('Milan', 'Kovac', 'milan.kovac@example.com', 'Analyst','2024-04-01 09:00:00');");

            migrationBuilder.Sql(
                "INSERT INTO WorkLogs (EmployeeId, Created, Description, Status)\nVALUES \n-- Jan Novak, multiple logs in a day\n(1, '2024-08-01 09:00:00', 'Started working on feature X', 0), -- Started\n(1, '2024-08-01 13:00:00', 'Completed feature X', 1),          -- Finished\n(1, '2024-08-01 14:00:00', 'Started bug fixing', 0),            -- Started\n(1, '2024-08-01 18:00:00', 'Finished bug fixing', 1),           -- Finished\n\n-- Petra Sukova, multiple logs in a day\n(2, '2024-08-01 10:00:00', 'Started meeting with clients', 0), -- Started\n(2, '2024-08-01 12:00:00', 'Finished meeting with clients', 1), -- Finished\n(2, '2024-08-01 13:00:00', 'Started project management', 0),   -- Started\n(2, '2024-08-01 17:00:00', 'Finished project management', 1),  -- Finished\n\n-- Milan Kovac, multiple logs in a day\n(3, '2024-08-01 11:00:00', 'Started data analysis', 0),        -- Started\n(3, '2024-08-01 16:00:00', 'Finished data analysis', 1),       -- Finished\n(3, '2024-08-01 17:00:00', 'Started data reporting', 0),       -- Started\n(3, '2024-08-01 19:00:00', 'Finished data reporting', 1);      -- Finished");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
