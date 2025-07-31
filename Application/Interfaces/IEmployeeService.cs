using Core;

namespace Application.Interfaces
{
    public interface IEmployeeService
    {
        Task InitializeDatabaseAsync();
        Task AddEmployeeAsync(Employee employee);
        Task<IEnumerable<Employee>> GetAllUniqueEmployeesSortedByNameAsync();
        Task<IEnumerable<Employee>> GetMaleEmployeesWithLastNameStartingWithFAsync();
        Task GenerateSampleDataAsync();
        Task OptimizeDatabaseForQueryAsync();
    }
}
