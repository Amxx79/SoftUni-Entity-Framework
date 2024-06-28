using SoftUni.Data;
using SoftUni.Models;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new SoftUniContext();
            Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));
        }

        //01
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            List<Employee> employees = context.Employees
                .ToList();

            var sb = new StringBuilder();
            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} {employee.MiddleName} " +
                    $"{employee.JobTitle} {employee.Salary:F2}");
            }
            return sb.ToString().Trim();
        }

        //02
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Salary > 50_000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary,
                })
                .OrderBy(e => e.FirstName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} – {employee.Salary:F2}");
            }


            return sb.ToString().TrimEnd();
        }

        //03
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .Select(e => new
                {
                    FullName = $"{e.FirstName} {e.LastName}",
                    e.Department,
                    e.Salary,
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FullName} from {e.Department.Name} ${e.Salary:F2}");
            }
            return sb.ToString().TrimEnd();
        }

        //04
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAdress = new Address()
            {
                AddressText = "Vitoshka 15",
                AddressId = 4,
            };

            var employeeNakov = context.Employees
                .FirstOrDefault(e => e.LastName == "Nakov");

            if (employeeNakov != null)
            {
                employeeNakov.Address = newAdress;
                context.SaveChanges();
            }


            List<string> allEmployees = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                .ToList();

            return String.Join(Environment.NewLine, allEmployees);
        }

        //05
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerInfo = $"{e.Manager.FirstName} {e.Manager.LastName}",
                    Projects = e.EmployeesProjects
                        .Where(ep =>
                            ep.Project.StartDate.Year >= 2001 &&
                            ep.Project.StartDate.Year <= 2003)
                        .Select(ep => new
                        {
                            ProjectName = ep.Project.Name,
                            ep.Project.StartDate,
                            EndDate = ep.Project.EndDate.HasValue ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") :
                            "not finished"
                        })
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var r in employees)
            {
                sb.AppendLine($"{r.FirstName} - Manager: {r.ManagerInfo}");
                if (r.Projects.Any())
                {
                    foreach (var p in r.Projects)
                    {
                        sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                    }
                }
            }

            return sb.ToString();
        }

        //06

        public static string GetAddressesByTown(SoftUniContext context)
        {
            string[] addresses = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(e => e.Town.Name)
                .ThenBy(e => e.AddressText)
                .Select(e => $"{e.AddressText}, {e.Town.Name} - {e.Employees.Count} employees")
                .Take(10)
                .ToArray();

            return String.Join(Environment.NewLine, addresses);
        }

        //09

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects.Select(p => new { p.Project.Name }).OrderBy(p => p.Name).ToArray(),
                })
                .FirstOrDefault();

            var sb = new StringBuilder();

            if (employee != null)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");

                sb.AppendLine(String.Join(Environment.NewLine, employee.Projects.Select(p => p.Name)));
            }

            return sb.ToString().Trim();
        }

        //7

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(e => new
                {
                    e.Name,
                    ManagerNames = $"{e.Manager.FirstName} {e.Manager.LastName}",
                    Employees = e.Employees
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .Select(e => new
                    {
                        EmployeeData = $"{e.FirstName} {e.LastName} - {e.JobTitle}"
                    })
                    .ToArray()
                })
                .ToArray();

            var sb = new StringBuilder();

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.Name} - {department.ManagerNames}");
                sb.AppendLine(String.Join(Environment.NewLine, department.Employees.Select(e => e.EmployeeData)));
            }

            return sb.ToString().Trim();
        }

        //8

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                })
                .OrderBy(p => p.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var p in projects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate.ToString());
            }

            return sb.ToString();
        }

        //09

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employeesUpdate = context.Employees
                .Where(e => e.Department.Name == "Engineering" || e.Department.Name == "Tool Design"
                || e.Department.Name == "Marketing" || e.Department.Name == "Information Services")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employeesUpdate)
            {
                employee.Salary = employee.Salary * 1.12m;
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:F2})");
            }

            return sb.ToString();
        }

        //10

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.FirstName.StartsWith("Sa"))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salary:F2})");
            }

            return sb.ToString().Trim();
        }
    }
}