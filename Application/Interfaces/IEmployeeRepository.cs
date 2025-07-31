using Core;

namespace Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task CreateTableAsync();
        Task AddAsync(Employee employee);
        Task AddBatchAsync(IEnumerable<Employee> employees);
        Task<IEnumerable<Employee>> GetAllUniqueSortedByNameAsync();
        Task<IEnumerable<Employee>> GetMaleWithLastNameStartingWithFAsync();
        Task CreateIndexOnFullNameAndGenderAsync();
    }
}
