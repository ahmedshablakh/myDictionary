using Microsoft.EntityFrameworkCore;
using MyDictionary.Core.Entities;
using MyDictionary.Core.Interfaces;
using MyDictionary.Infrastructure.Data;

namespace MyDictionary.Infrastructure.Repositories;

public class TestResultRepository : ITestResultRepository
{
    private readonly ApplicationDbContext _context;

    public TestResultRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TestResult>> GetAllAsync()
    {
        return await _context.TestResults
            .OrderByDescending(t => t.TestDate)
            .ToListAsync();
    }

    public async Task<TestResult?> GetByIdAsync(int id)
    {
        return await _context.TestResults.FindAsync(id);
    }

    public async Task<TestResult> AddAsync(TestResult testResult)
    {
        _context.TestResults.Add(testResult);
        await _context.SaveChangesAsync();
        return testResult;
    }

    public async Task<IEnumerable<TestResult>> GetRecentResultsAsync(int count)
    {
        return await _context.TestResults
            .OrderByDescending(t => t.TestDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<Dictionary<TestType, double>> GetAverageScoresByTypeAsync()
    {
        return await _context.TestResults
            .GroupBy(t => t.TestType)
            .Select(g => new { TestType = g.Key, AvgScore = g.Average(t => t.Score) })
            .ToDictionaryAsync(x => x.TestType, x => x.AvgScore);
    }

    public async Task<IEnumerable<TestResult>> GetResultsByDateRangeAsync(DateTime from, DateTime to)
    {
        return await _context.TestResults
            .Where(t => t.TestDate >= from && t.TestDate <= to)
            .OrderByDescending(t => t.TestDate)
            .ToListAsync();
    }
}
