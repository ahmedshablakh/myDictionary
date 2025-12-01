using MyDictionary.Core.Entities;

namespace MyDictionary.Core.Interfaces;

public interface ITestResultRepository
{
    Task<IEnumerable<TestResult>> GetAllAsync();
    Task<TestResult?> GetByIdAsync(int id);
    Task<TestResult> AddAsync(TestResult testResult);
    Task<IEnumerable<TestResult>> GetRecentResultsAsync(int count);
    Task<Dictionary<TestType, double>> GetAverageScoresByTypeAsync();
    Task<IEnumerable<TestResult>> GetResultsByDateRangeAsync(DateTime from, DateTime to);
}
