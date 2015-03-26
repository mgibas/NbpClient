using NUnit.Framework;
using NbpClient.Parsers;

namespace NbpClient.Tests.Parsers
{
  [TestFixture]
  public class AverageExchangeRateAvailabilityParserTests
  {
    private AverageExchangeRateAvailabilityParser _parser;

    [SetUp]
    public void SetUp()
    {
      _parser = new AverageExchangeRateAvailabilityParser();
    }

    [Test]
    public void FileNamePrefix_ReturnsPropperPrefix()
    {
      Assert.AreEqual("a", _parser.FileNamePrefix);
    }

    [Test]
    public void ParserFor_ReturnsPropperExchangeRateType()
    {
      Assert.AreEqual(typeof(AverageExchangeRate), _parser.ParserFor);
    }
  }
}
