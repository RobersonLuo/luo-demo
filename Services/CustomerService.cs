using Luo.Web.Host.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Luo.Web.Host.Services;

/// <summary>
/// Customer Service
/// </summary>
/// <param name="cache"></param>
public class CustomerService(IMemoryCache cache) : ICustomerService
{
    private static readonly Dictionary<long, Customer> _customers = [];

    const string CACHE_KEY = "leaderboard";
    private readonly IMemoryCache _cache = cache;

    /// <summary>
    /// add customer data for test
    /// </summary>
    public bool AddCustomers4Test()
    {
        if (_customers.Count == 0)
        {
            _customers.Add(15514665, new Customer { CustomerId = 15514665, Score = 124 });
            _customers.Add(81546541, new Customer { CustomerId = 81546541, Score = 113 });
            _customers.Add(1745431, new Customer { CustomerId = 1745431, Score = 100 });
            _customers.Add(76786448, new Customer { CustomerId = 76786448, Score = 100 });
            _customers.Add(254814111, new Customer { CustomerId = 254814111, Score = 96 });
            _customers.Add(53274324, new Customer { CustomerId = 53274324, Score = 95 });
            _customers.Add(6144320, new Customer { CustomerId = 6144320, Score = 93 });
            _customers.Add(8009471, new Customer { CustomerId = 8009471, Score = 93 });
            _customers.Add(11028481, new Customer { CustomerId = 11028481, Score = 93 });
            _customers.Add(38819, new Customer { CustomerId = 38819, Score = 92 });
        }

        return true;
    }

    /// <summary>
    /// update customer score
    /// </summary>
    /// <param name="cusomerId"></param>
    /// <param name="score"></param>
    /// <returns>the customer's current score</returns>
    public async Task<decimal> UpdateScoreAsync(long cusomerId, decimal score)
    {
        decimal currentScore;

        //Add customer first if it not found; otherwise, update it's score
        if (!_customers.TryGetValue(cusomerId, out Customer? value))
        {
            _customers.Add(cusomerId, new Customer() { CustomerId = cusomerId, Score = score });
            currentScore = score;
        }
        else
        {
            value.Score += score;
            currentScore = value.Score;
        }

        //do ranking after score changed
        await DoRankingAsync();

        return currentScore;
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

        //if the parameters 'start' and 'end' not exist at the same time, return empty;
        if (start <= 0 && end <= 0)
        {
            return [];
        }

        List<LeaderBoard> data = [];
        if (_cache.TryGetValue(CACHE_KEY, out List<LeaderBoard>? value))
        {
            data = value ?? [];
        }

        //ignore the 'end' condition if parameter 'end' not exist (no value inputed).
        if (end == 0)
        {
            return data.Where(e => e.Rank >= start).ToList();
        }

        return data.Where(e => e.Rank >= start && e.Rank <= end).ToList();
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

        List<LeaderBoard> data = [];
        if (_cache.TryGetValue(CACHE_KEY, out List<LeaderBoard>? value))
        {
            data = value ?? [];
        }

        var find = data.Find(e => e.CustomerId == customerId);
        if (find == null) { return []; }

        var list = new List<LeaderBoard>();
        if (high > 0)
        {
            //add the upper rank neighours
            list.AddRange(data.Where(e => e.Rank >= (find.Rank - high) && e.Rank < find.Rank));
        }
        //add itself
        list.Add(find);
        if (low > 0)
        {
            //add the lower rank neighours if exist
            list.AddRange(data.Where(e => e.Rank > find.Rank && e.Rank <= (find.Rank + low)));
        }

        return list;
    }

    /// <summary>
    /// Do ranking by the updated customer score.
    /// Note: All customers whose score is greater than zero participate in a competition.
    /// </summary>
    private async Task DoRankingAsync()
    {
        //use 'task.delay' to simulating asynchronous opterations
        await Task.Delay(1);

        //do ranking
        var data = _customers.Values.Where(e => e.Score > 0)
            .OrderByDescending(e => e.Score)
            .ThenBy(e => e.CustomerId)
            .Select((c, idx) => new LeaderBoard
            {
                CustomerId = c.CustomerId,
                Score = c.Score,
                Rank = idx + 1
            }).ToList();

        //cache leaderboard data
        if (_cache.TryGetValue(CACHE_KEY, out List<LeaderBoard>? value))
        {
            //clear cache data if exsit
            _cache.Remove(CACHE_KEY);
        }
        //var cacheOptions = new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromMinutes(10) };
        _cache.Set(CACHE_KEY, data);
    }
}
