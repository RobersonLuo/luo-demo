using Luo.Web.Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace Luo.Web.Host.Controllers;

/// <summary>
/// Customer Web Api
/// </summary>
/// <param name="customerService"></param>
[ApiController]
[Route("customer/")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    private readonly ICustomerService _customerService = customerService;

    /// <summary>
    /// add customer data for test
    /// </summary>
    [HttpPost, Route("add-test-data")]
    public bool AddCustomers4Test()
    {
        return _customerService.AddCustomers4Test();
    }

    /// <summary>
    /// update customer score
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="score"></param>
    /// <returns></returns>
    [HttpPost, Route("{customerId}/score/{score}")]
    public Task<decimal> UpdateScore(long customerId, decimal score)
    {
        return _customerService.UpdateScoreAsync(customerId, score);
    }
}
