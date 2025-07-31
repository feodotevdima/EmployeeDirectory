using Application.Interfaces;
using Core;
using Dapper;
using Persistence;

namespace Application.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDatabaseContext _dbContext;

        public EmployeeRepository(IDatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateTableAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                CREATE TABLE IF NOT EXISTS Employees (
                    Id SERIAL PRIMARY KEY,
                    FullName VARCHAR(100) NOT NULL,
                    BirthDate DATE NOT NULL,
                    Gender VARCHAR(10) NOT NULL
                )";
            await connection.ExecuteAsync(sql);
        }

        public async Task AddAsync(Employee employee)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO Employees (FullName, BirthDate, Gender)
                VALUES (@FullName, @BirthDate, @Gender)";
            await connection.ExecuteAsync(sql, employee);
        }

        public async Task AddBatchAsync(IEnumerable<Employee> employees)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();
            var transaction = connection.BeginTransaction();

            try
            {
                var sql = @"
                    INSERT INTO Employees (FullName, BirthDate, Gender)
                    VALUES (@FullName, @BirthDate, @Gender)";

                await connection.ExecuteAsync(sql, employees, transaction: transaction);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetAllUniqueSortedByNameAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT DISTINCT ON (FullName, BirthDate) 
                    FullName, BirthDate, Gender
                FROM Employees
                ORDER BY FullName, BirthDate";
            return await connection.QueryAsync<Employee>(sql);
        }

        public async Task<IEnumerable<Employee>> GetMaleWithLastNameStartingWithFAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT FullName, BirthDate, Gender
                FROM Employees
                WHERE Gender = 'Male' AND FullName LIKE 'F%'";
            return await connection.QueryAsync<Employee>(sql);
        }

        public async Task CreateIndexOnFullNameAndGenderAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                CREATE INDEX IF NOT EXISTS idx_employees_male_f_names
                ON Employees (FullName)
                WHERE Gender = 'Male' AND FullName LIKE 'F%'";
            await connection.ExecuteAsync(sql);
        }
    }
}
