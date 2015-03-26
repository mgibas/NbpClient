using NUnit.Framework;
using NbpClient.Parsers;

namespace NbpClient.Tests.Parsers
{
  [TestFixture]
  public class AverageInconvertibleExchangeRateAvailabilityParserTests
  {
    private AverageInconvertibleExchangeRateAvailabilityParser _parser;

    [SetUp]
    public void SetUp()
    {
      _parser = new AverageInconvertibleExchangeRateAvailabilityParser();
    }

    [Test]
    public void FileNamePrefix_ReturnsPropperPrefix()
    {
      Assert.AreEqual("b", _parser.FileNamePrefix);
    }

    [Test]
    public void ParserFor_ReturnsPropperExchangeRateType()
    {
      Assert.AreEqual(typeof(AverageInconvertibleExchangeRate), _parser.ParserFor);
    }
  }
}
