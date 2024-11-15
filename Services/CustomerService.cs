using Luo.Web.Host.Extensions;
using Luo.Web.Host.Models;
using System.Diagnostics;

namespace Luo.Web.Host.Services;

/// <summary>
/// Customer Service
/// </summary>
/// <param name="cache"></param>
public class CustomerService() : ICustomerService
{
    private static readonly SkipList _data = new(15);
    //for test: allow a lost of  requests for updating customer score
    private static readonly List<long> _dataIds = [];


    /// <summary>
    /// add customer data for test
    /// </summary>
    public async Task AddCustomers4TestAsync(int count)
    {
        Console.WriteLine();
        Console.WriteLine("---------------begin create customers------------------------------");

        //---Test 1: simple case
        //_data.Insert(new Customer { CustomerId = 1, Score = 124 });
        //_data.Insert(new Customer { CustomerId = 2, Score = 113 });
        //_data.Insert(new Customer { CustomerId = 3, Score = 100 });
        //_data.Insert(new Customer { CustomerId = 4, Score = 100 });
        //_data.Insert(new Customer { CustomerId = 5, Score = 96 });
        //_data.Insert(new Customer { CustomerId = 6, Score = 95 });
        //_data.Insert(new Customer { CustomerId = 7, Score = 93 });
        //_data.Insert(new Customer { CustomerId = 8, Score = 93 });
        //_data.Insert(new Customer { CustomerId = 9, Score = 93 });
        //_data.Insert(new Customer { CustomerId = 10, Score = 92 });

        //Test 2: handle lots of customers (add) and a lot of simultaneous requests (update score)
        Random rnd = new();
        int numCount = count;
        int numRange = 500;

        long start = Stopwatch.GetTimestamp();
        for (int i = 0; i < numCount; i++)
        {
            int customerId = rnd.Next();
            int score = rnd.Next(1, numRange);
            _data.Insert(new Customer { CustomerId = customerId, Score = score });
            _dataIds.Add(customerId);
        }
        long end = Stopwatch.GetTimestamp();

        Console.WriteLine($"{numCount} customers created and elapsed: {Stopwatch.GetElapsedTime(start, end).TotalMilliseconds} ms");
        Console.WriteLine();

        _data.PrintList();

        Console.WriteLine("---------------begin update customers's score------------------------------");
        start = Stopwatch.GetTimestamp();

        //requests all in parallel at the same time.
        //request of update score
        var tasks = _dataIds.Select(e => UpdateScoreAsync(e, rnd.NextDecimal(-1000, 1000)));
        var newScores = await Task.WhenAll(tasks);

        end = Stopwatch.GetTimestamp();
        Console.WriteLine();

        Console.WriteLine($"--------{_dataIds.Count} customers's score updated and elapsed: {Stopwatch.GetElapsedTime(start, end).TotalMilliseconds} ms");
        Console.WriteLine();
    }

    /// <summary>
    /// update the customer's score
    /// </summary>
    /// <param name="cusomerId"></param>
    /// <param name="score"></param>
    /// <returns>the updated score of the customer</returns>
    public async Task<decimal> UpdateScoreAsync(long cusomerId, decimal score)
    {
        //use 'task.delay' to simulating asynchronous opterations
        await Task.Delay(1);
        return _data.UpdateScore(cusomerId, score);
    }

    /// <summary>
    /// Get leaderboards which the rank is between the start rank and end rank
    /// </summary>
    /// <param name="start">start rank, included in response if exists</param>
    /// <param name="end">end rank, included in response if exists</param>
    /// <returns>the found leaderboards which the rank is between start rank and end rank.</returns>
    public async Task<List<LeaderBoard>> GetLeaderBoardsByRankAsync(int start, int end)
    {
        //use 'task.delay' to simulating asynchronous opterations
        await Task.Delay(1);

        if (start <= 0) { start = 0; }
        if (end <= 0) { end = 0; }

        // if the parameters 'start' and 'end' not exist at the same time, return empty;
        // 'start' should not greater than 'end'
        if ((start <= 0 && end <= 0) || (start > end))
        {
            return [];
        }

        return _data.GetNodesByRankingRegion(start, end);
    }

    /// <summary>
    /// Get the customer's leaderboards and it's neighbours according to the low or high number.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="high"> number of neighbors whose rank is higher than the specified customer</param>
    /// <param name="low">number of neighbors whose rank is lower than the specified customer</param>
    /// <returns>the found customer's leaderboard and it's neighbours</returns>
    public async Task<List<LeaderBoard>> GetLeaderBoardsWithNeighboursByRankAsync(long customerId, int high = 0, int low = 0)
    {
        //use 'task.delay' to simulating asynchronous opterations
        await Task.Delay(1);

        if (high <= 0) { high = 0; }
        if (low <= 0) { low = 0; }

        int currentRank = _data.GetRanking(customerId);
        if (currentRank == 0) { return []; }

        int start = currentRank - high;
        int end = currentRank + low;

        return _data.GetNodesByRankingRegion(start, end);
    }
}
