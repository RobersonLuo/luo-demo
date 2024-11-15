namespace Luo.Web.Host.Extensions;

public static class RandomExtensions
{
    /// <summary>
    /// Get a random decimal number in [min, max].
    /// </summary>
    /// <param name="random"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static decimal NextDecimal(this Random random, int min, int max)
    {
        // 整数部分：[-min, max]之间的随机整数
        int integerPart = random.Next(-min, max + 1);

        // 小数部分: 0.00到0.99之间的随机小数
        decimal decimalPart = (decimal)random.Next(0, 100) / 100;

        // 随机生成的decimal数
        return integerPart + decimalPart;
    }
}
