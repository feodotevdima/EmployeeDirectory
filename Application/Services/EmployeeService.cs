using Application.Interfaces;
using Core;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task InitializeDatabaseAsync()
        {
            await _employeeRepository.CreateTableAsync();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _employeeRepository.AddAsync(employee);
        }

        public async Task<IEnumerable<Employee>> GetAllUniqueEmployeesSortedByNameAsync()
        {
            return await _employeeRepository.GetAllUniqueSortedByNameAsync();
        }

        public async Task<IEnumerable<Employee>> GetMaleEmployeesWithLastNameStartingWithFAsync()
        {
            return await _employeeRepository.GetMaleWithLastNameStartingWithFAsync();
        }

        public async Task GenerateSampleDataAsync()
        {
            var random = new Random();
            var employees = new List<Employee>();
            var genders = new[] { "Male", "Female" };

            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones",
                          "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
                          "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
                          "Thomas", "Taylor", "Moore", "Jackson", "Lee" };

            var maleFirstNames = new[] { "James", "John", "Robert", "Michael", "William",
                               "David", "Richard", "Joseph", "Thomas", "Charles" };

            var femaleFirstNames = new[] { "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth",
                                 "Barbara", "Susan", "Jessica", "Sarah", "Karen" };

            var middleNames = new[] { "Edward", "George", "Henry", "Alexander", "Louis",
                            "Marie", "Anne", "Rose", "Grace", "Victoria" };

            for (int i = 0; i < 1000000; i++)
            {
                var gender = genders[random.Next(0, 2)];
                var firstName = gender == "Male"
                    ? maleFirstNames[random.Next(maleFirstNames.Length)]
                    : femaleFirstNames[random.Next(femaleFirstNames.Length)];

                employees.Add(new Employee
                {
                    FullName = $"{lastNames[random.Next(lastNames.Length)]} {firstName} {middleNames[random.Next(middleNames.Length)]}",
                    BirthDate = new DateTime(random.Next(1950, 2010), random.Next(1, 13), random.Next(1, 29)),
                    Gender = gender
                });

                if (employees.Count % 1000 == 0)
                {
                    await _employeeRepository.AddBatchAsync(employees);
                    employees.Clear();
                }
            }

            if (employees.Any())
            {
                await _employeeRepository.AddBatchAsync(employees);
                employees.Clear();
            }

            var fLastNames = lastNames.Where(name => name.StartsWith("F")).ToArray();
            if (fLastNames.Length == 0)
            {
                fLastNames = new[] { "Fisher", "Foster", "Ford", "Fletcher", "Fox" };
            }

            for (int i = 0; i < 1000; i++)
            {
                employees.Add(new Employee
                {
                    FullName = $"{fLastNames[random.Next(fLastNames.Length)]} {maleFirstNames[random.Next(maleFirstNames.Length)]} {middleNames[random.Next(middleNames.Length)]}",
                    BirthDate = new DateTime(random.Next(1950, 2010), random.Next(1, 13), random.Next(1, 29)),
                    Gender = "Male"
                });
            }

            await _employeeRepository.AddBatchAsync(employees);
        }

        public async Task OptimizeDatabaseForQueryAsync()
        {
            await _employeeRepository.CreateIndexOnFullNameAndGenderAsync();
        }
    }
}
