using Application.Interfaces;
using Application.Repository;
using Application.Services;
using Core;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            System.Console.WriteLine("Please provide a command.");
            return;
        }

        var serviceProvider = ConfigureServices();
        var employeeService = serviceProvider.GetService<IEmployeeService>();

        try
        {
            switch (args[0])
            {
                case "1":
                    await employeeService.InitializeDatabaseAsync();
                    System.Console.WriteLine("Table created successfully.");
                    break;

                case "2":
                    if (args.Length != 4)
                    {
                        System.Console.WriteLine("Usage: EmployeeDirectory.Console.exe 2 \"Full Name\" \"YYYY-MM-DD\" \"Gender\"");
                        return;
                    }
                    var employee = new Employee
                    {
                        FullName = args[1],
                        BirthDate = DateTime.Parse(args[2]),
                        Gender = args[3]
                    };
                    await employeeService.AddEmployeeAsync(employee);
                    System.Console.WriteLine($"Employee added successfully. Age: {employee.CalculateAge()}");
                    break;

                case "3":
                    var employees = await employeeService.GetAllUniqueEmployeesSortedByNameAsync();
                    foreach (var emp in employees)
                    {
                        System.Console.WriteLine($"{emp.FullName} | {emp.BirthDate:yyyy-MM-dd} | {emp.Gender} | {emp.CalculateAge()} years");
                    }
                    break;

                case "4":
                    System.Console.WriteLine("Generating sample data... This may take a while.");
                    await employeeService.GenerateSampleDataAsync();
                    System.Console.WriteLine("Sample data generated successfully.");
                    break;

                case "5":
                    var stopwatch = Stopwatch.StartNew();
                    var maleFEmployees = await employeeService.GetMaleEmployeesWithLastNameStartingWithFAsync();
                    stopwatch.Stop();
                    System.Console.WriteLine($"Found {maleFEmployees.Count()} male employees with last name starting with 'F'");
                    foreach (var emp in maleFEmployees)
                    {
                        System.Console.WriteLine($"{emp.FullName} | {emp.BirthDate:yyyy-MM-dd} | {emp.Gender} | {emp.CalculateAge()} years");
                    }
                    System.Console.WriteLine($"Query executed in {stopwatch.ElapsedMilliseconds} ms");
                    break;

                case "6":
                    System.Console.WriteLine("Optimizing database...");
                    stopwatch = Stopwatch.StartNew();
                    await employeeService.OptimizeDatabaseForQueryAsync();
                    stopwatch.Stop();
                    System.Console.WriteLine($"Database optimized in {stopwatch.ElapsedMilliseconds} ms");
                    System.Console.WriteLine("Run command 5 again to see performance improvement.");
                    break;

                default:
                    System.Console.WriteLine("Invalid command.");
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        var connectionString = "Host=localhost;Port=5432;Database=EmployeeDirectory;Username=user;Password=1234";
        services.AddSingleton<IDatabaseContext>(new DatabaseContext(connectionString));

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services.BuildServiceProvider();
    }
}
