using Luo.Web.Host.Models;

namespace Luo.Web.Host.Services;

public interface ICustomerService
{
    /// <summary>
    /// add customer data for test
    /// </summary>
    bool AddCustomers4Test();

    /// <summary>
    /// update customer score
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="score"></param>
    /// <returns>the customer's current score</returns>
    Task<decimal> UpdateScoreAsync(long customerId, decimal score);

    /// <summary>
    /// Get leaderboards which the rank is between start rank and end rank
    /// </summary>
    /// <param name="start">start rank, included in response if exists</param>
    /// <param name="end">end rank, included in response if exists</param>
    /// <returns>the found leaderboards which the rank is between start rank and end rank</returns>
    Task<List<LeaderBoard>> GetLeaderBoardsByRankAsync(int start, int end);

    /// <summary>
    /// Get the customer's leaderboards and it's neighbours according to the low or high number.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="high"> number of neighbors whose rank is higher than the specified customer</param>
    /// <param name="low">number of neighbors whose rank is lower than the specified customer</param>
    /// <returns>the found customer's leaderboard and it's neighbours</returns>
    Task<List<LeaderBoard>> GetLeaderBoardsWithNeighboursByRankAsync(long customerId, int high = 0, int low = 0);
}
