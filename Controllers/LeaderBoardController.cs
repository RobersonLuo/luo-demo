using Luo.Web.Host.Services;
using Luo.Web.Host.Models;
using Microsoft.AspNetCore.Mvc;

namespace Luo.Web.Host.Controllers;

/// <summary>
/// LeaderBoard Web Api
/// </summary>
/// <param name="customerService"></param>
[Route("leaderboard/")]
public class LeaderBoardController(ICustomerService customerService) : ControllerBase
{
    private readonly ICustomerService _customerService = customerService;

    /// <summary>
    /// Get leaderboards which the rank is between the start rank and end rank
    /// </summary>
    /// <param name="start">start rank, included in response if exists</param>
    /// <param name="end">end rank, included in response if exists</param>
    /// <returns>the found leaderboards which the rank is between start rank and end rank.</returns>
    [HttpGet]
    public Task<List<LeaderBoard>> GetLeaderBoardsByRank(int start, int end)
    {
        return _customerService.GetLeaderBoardsByRankAsync(start, end);
    }

    /// <summary>
    /// Get the customer's leaderboards and it's neighbours according to the low or high number.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="high"> number of neighbors whose rank is higher than the specified customer</param>
    /// <param name="low">number of neighbors whose rank is lower than the specified customer</param>
    /// <returns>the found customer's leaderboard and it's neighbours</returns>
    [HttpGet, Route("{customerId}")]
    public Task<List<LeaderBoard>> GetLeaderBoardsWithNeighboursByRank(long customerId, int high = 0, int low = 0)
    {
        return _customerService.GetLeaderBoardsWithNeighboursByRankAsync(customerId, high, low);
    }
}
