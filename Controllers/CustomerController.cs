using Luo.Web.Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace Luo.Web.Host.Controllers;

/// <summary>
/// Customer Web Api
/// </summary>
/// <param name="customerService"></param>
[Route("customer")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    private readonly ICustomerService _customerService = customerService;

    /// <summary>
    /// add customer data for test
    /// </summary>
    [HttpPost("add-test-data")]
    public bool AddCustomers4Test()
    {
        return _customerService.AddCustomers4Test();
    }

    /// <summary>
    /// update customer score
    /// </summary>
    /// <param name="customerid"></param>
    /// <param name="score"></param>
    /// <returns></returns>
    [HttpPost("{customerid:long:min(1)}/score/{score:decimal}")]
    public Task<decimal> UpdateScore(long customerid, decimal score)
    {
        return _customerService.UpdateScoreAsync(customerid, score);
    }
}
