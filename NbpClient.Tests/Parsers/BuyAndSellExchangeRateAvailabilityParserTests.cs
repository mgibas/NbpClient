using NUnit.Framework;
using NbpClient.Parsers;

namespace NbpClient.Tests.Parsers
{
  [TestFixture]
  public class BuyAndSellExchangeRateAvailabilityParserTests
  {
    private BuyAndSellExchangeRateAvailabilityParser _parser;

    [SetUp]
    public void SetUp()
    {
      _parser = new BuyAndSellExchangeRateAvailabilityParser();
    }

    [Test]
    public void FileNamePrefix_ReturnsPropperPrefix()
    {
      Assert.AreEqual("c", _parser.FileNamePrefix);
    }

    [Test]
    public void ParserFor_ReturnsPropperExchangeRateType()
    {
      Assert.AreEqual(typeof(BuyAndSellExchangeRate), _parser.ParserFor);
    }
  }
}
