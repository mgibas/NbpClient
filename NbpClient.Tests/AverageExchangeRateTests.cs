using NUnit.Framework;

namespace NbpClient.Tests
{
  [TestFixture]
  public class AverageExchangeRateTests
  {
    [TestCase("0,123123", 0.123123)]
    [TestCase("0.123123", 0.123123)]
    public void Value_ValidNumber_ParseDecimalProperly(string stringValue, decimal expectedValue)
    {
      var result = new AverageExchangeRate { StringValue = stringValue }.Value;

      Assert.AreEqual(expectedValue, result);
    }
  }
}
