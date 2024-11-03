namespace Luo.Web.Host.Models;

/// <summary>
/// Customer Entity
/// </summary>
public class Customer
{
    /// <summary>
    /// gets or sets the CustomerId
    /// </summary>
    public long CustomerId { get; set; }
    /// <summary>
    /// gets or sets the Score
    /// </summary>
    public decimal Score { get; set; } = 0;
}
