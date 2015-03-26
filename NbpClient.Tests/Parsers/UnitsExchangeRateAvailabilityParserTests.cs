using NUnit.Framework;
using NbpClient.Parsers;

namespace NbpClient.Tests.Parsers
{
  [TestFixture]
  public class UnitsExchangeRateAvailabilityParserTests
  {
    private UnitsExchangeRateAvailabilityParser _parser;

    [SetUp]
    public void SetUp()
    {
      _parser = new UnitsExchangeRateAvailabilityParser();
    }

    [Test]
    public void FileNamePrefix_ReturnsPropperPrefix()
    {
      Assert.AreEqual("h", _parser.FileNamePrefix);
    }

    [Test]
    public void ParserFor_ReturnsPropperExchangeRateType()
    {
      Assert.AreEqual(typeof(UnitsExchangeRate), _parser.ParserFor);
    }
  }
}
