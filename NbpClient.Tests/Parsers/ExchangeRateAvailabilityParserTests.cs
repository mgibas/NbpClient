using System;
using FakeItEasy;
using NUnit.Asserts.Compare;
using NUnit.Framework;
using NbpClient.Parsers;

namespace NbpClient.Tests.Parsers
{
  [TestFixture]
  public class ExchangeRateAvailabilityParserTests
  {
    private ExchangeRateAvailabilityParser _parser;

    [SetUp]
    public void SetUp()
    {
      _parser = A.Fake<ExchangeRateAvailabilityParser>(o => o.CallsBaseMethods());
    }

    [TestCase("a123z000101", 2000, 1, 1)]
    [TestCase("a123z100304", 2010, 3, 4)]
    public void Parse_ValidFileName_ReturnsParsedAvailability(string fileName, int year, int month, int day)
    {
      var expectedDate = new DateTime(year, month, day);
      var expectedAvailability = new ExchangeRateAvailability { FileName = fileName, ForDate = expectedDate };

      var result = _parser.Parse(fileName);

      CompareAssert.AreEqual(expectedAvailability, result);
    }

    [Test]
    public void CanParse_FileNameDoesNotStartFromPrefix_ReturnsFalse()
    {
      A.CallTo(() => _parser.FileNamePrefix).Returns("1");

      var result = _parser.CanParse("222asd1sda111");

      Assert.IsFalse(result);
    }

    [Test]
    public void CanParse_FileNameStartsFromPrefix_ReturnsTrue()
    {
      A.CallTo(() => _parser.FileNamePrefix).Returns("1");

      var result = _parser.CanParse("1222asd1sda111");

      Assert.IsTrue(result);
    }
  }
}
