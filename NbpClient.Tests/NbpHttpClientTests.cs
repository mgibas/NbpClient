using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Asserts.Compare;
using NUnit.Framework;
using NbpClient.Contracts;
using NbpClient.Parsers;

namespace NbpClient.Tests
{
  [TestFixture]
  public class NbpHttpClientTests
  {
    private IAppSettings _appSettings;
    private IHttpClient _httpClient;
    private List<ExchangeRateAvailabilityParser> parsers;
    private NbpHttpClient _client;

    [SetUp]
    public void Setup()
    {
      _httpClient = A.Fake<IHttpClient>();
      _appSettings = A.Fake<IAppSettings>();
      parsers = new List<ExchangeRateAvailabilityParser>();
      _client = A.Fake<NbpHttpClient>(o => o.WithArgumentsForConstructor(() => new NbpHttpClient(_httpClient, _appSettings, parsers)).CallsBaseMethods());
    }

    [Test]
    public async Task GetExchangeRateAvailabilityAsyncOfType_ParserExists_GetsStreamFromConfiguredUrl()
    {
      var dirUrl = "http://im.valid.url.pl/";
      parsers.Add(A.Fake<ExchangeRateAvailabilityParser>());
      A.CallTo(() => parsers[0].ParserFor).Returns(typeof(AverageExchangeRate));
      A.CallTo(() => _appSettings.AppSetting("nbp.dir.url", null)).Returns(dirUrl);
      A.CallTo(() => _httpClient.GetStreamAsync(A<string>._)).Returns(Task.FromResult(CreateStream(new string[0])));

      await _client.GetExchangeRateAvailabilityAsync<AverageExchangeRate>();

      A.CallTo(() => _httpClient.GetStreamAsync(dirUrl)).MustHaveHappened();
    }

    [Test]
    public async Task GetExchangeRateAvailabilityAsyncOfType_MultipleParsersExists_ParseOnlyWithParserForProvidedType()
    {
      var fileNames = new[] { "parseMe" };
      var stream = CreateStream(fileNames);
      parsers.Add(A.Fake<ExchangeRateAvailabilityParser>());
      parsers.Add(A.Fake<ExchangeRateAvailabilityParser>());
      parsers.Add(A.Fake<ExchangeRateAvailabilityParser>());
      A.CallTo(() => parsers[0].ParserFor).Returns(typeof(AverageExchangeRate));
      A.CallTo(() => parsers[0].CanParse(A<string>._)).Returns(true);
      A.CallTo(() => parsers[1].ParserFor).Returns(typeof(BuyAndSellExchangeRate));
      A.CallTo(() => parsers[2].ParserFor).Returns(typeof(UnitsExchangeRate));
      A.CallTo(() => _httpClient.GetStreamAsync(A<string>._)).Returns(Task.FromResult(stream));

      await _client.GetExchangeRateAvailabilityAsync<AverageExchangeRate>();

      A.CallTo(() => parsers[0].Parse(A<string>._)).MustHaveHappened();
      A.CallTo(() => parsers[1].Parse(A<string>._)).MustNotHaveHappened();
      A.CallTo(() => parsers[2].Parse(A<string>._)).MustNotHaveHappened();
    }

    [Test]
    public async Task GetExchangeRateAvailabilityAsyncOfType_MultipleFileNames_ParseOnlyWithMatchingLinePrefix()
    {
      var fileNames = new[] { "c001z020102", "parseMe", "b001z020102" };
      var stream = CreateStream(fileNames);
      parsers.Add(A.Fake<ExchangeRateAvailabilityParser>());
      A.CallTo(() => parsers[0].ParserFor).Returns(typeof(AverageExchangeRate));
      A.CallTo(() => parsers[0].CanParse(fileNames[1])).Returns(true);
      A.CallTo(() => _httpClient.GetStreamAsync(A<string>._)).Returns(Task.FromResult(stream));

      await _client.GetExchangeRateAvailabilityAsync<AverageExchangeRate>();

      A.CallTo(() => parsers[0].Parse(fileNames[1])).MustHaveHappened();
      A.CallTo(() => parsers[0].Parse(fileNames[0])).MustNotHaveHappened();
      A.CallTo(() => parsers[0].Parse(fileNames[2])).MustNotHaveHappened();
    }

    [Test]
    public async Task GetExchangeRateAvailableDatesAsyncOfType_MultipleAvailabilities_ReturnsDatesOnly()
    {
      var availabilities = new List<ExchangeRateAvailability>
      {
        new ExchangeRateAvailability {ForDate = DateTime.UtcNow.Date.AddDays(99)},
        new ExchangeRateAvailability {ForDate = DateTime.UtcNow.Date.AddDays(1)},
        new ExchangeRateAvailability {ForDate = DateTime.UtcNow.Date.AddDays(-1)}
      };
      var expectedResult = new[] { availabilities[0].ForDate, availabilities[1].ForDate, availabilities[2].ForDate };
      A.CallTo(() => _client.GetExchangeRateAvailabilityAsync<AverageExchangeRate>()).Returns(Task.FromResult<IEnumerable<ExchangeRateAvailability>>(availabilities));

      var result = await _client.GetExchangeRateAvailableDatesAsync<AverageExchangeRate>();

      CollectionAssert.AreEquivalent(expectedResult, result);
    }

    [Test]
    public void GetExchangeRateAvailableDatesOfType_ReturnsAwaitedAsyncResult()
    {
      var asyncResult = new[] { DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date.AddDays(-1) };
      A.CallTo(() => _client.GetExchangeRateAvailableDatesAsync<AverageExchangeRate>()).Returns(Task.FromResult<IEnumerable<DateTime>>(asyncResult));

      var result = _client.GetExchangeRateAvailableDates<AverageExchangeRate>();

      CollectionAssert.AreEquivalent(asyncResult, result);
    }

    [Test]
    public void GetExchangeRatesOfType_ReturnsAwaitedAsyncResult()
    {
      var asyncResult = new ExchangeRates<AverageExchangeRate>();
      var date = DateTime.UtcNow.Date;
      A.CallTo(() => _client.GetExchangeRatesAsync<AverageExchangeRate>(date)).Returns(Task.FromResult(asyncResult));

      var result = _client.GetExchangeRates<AverageExchangeRate>(date);

      Assert.AreEqual(asyncResult, result);
    }

    [Test]
    public async Task GetExchangeRatesAsyncOfType_RateIsAvaible_GetsStreamUsingConfiguredUrlAndFileNameFromAvailability()
    {
      var xmlUrl = "http://im.valid.url.pl/";
      var availabilities = new List<ExchangeRateAvailability>
      {
        new ExchangeRateAvailability{ForDate = new DateTime(2000,1,1),FileName = "testFile"}
      };
      var expectedUrl = xmlUrl + availabilities[0].FileName + ".xml";
      A.CallTo(() => _appSettings.AppSetting("nbp.xml.url", null)).Returns(xmlUrl);
      A.CallTo(() => _httpClient.GetStreamAsync(A<string>._)).Returns(Task.FromResult(CreateStream(@"<tabela_kursow />")));
      A.CallTo(() => _client.GetExchangeRateAvailabilityAsync<AverageExchangeRate>()).Returns(availabilities);

      await _client.GetExchangeRatesAsync<AverageExchangeRate>(availabilities[0].ForDate);

      A.CallTo(() => _httpClient.GetStreamAsync(expectedUrl)).MustHaveHappened();
    }

    [Test]
    public async Task GetExchangeRatesAsyncOfType_RateIsAvailable_DeserializeExchangeRates()
    {
      var availabilities = new List<ExchangeRateAvailability>
      {
        new ExchangeRateAvailability {ForDate = DateTime.UtcNow.Date}
      };
      var expectedRate = new ExchangeRates<AverageExchangeRate> { PublicationDate = new DateTime(2013, 1, 2), Number = "001/A/NBP/2013" };
      var xml = @"<tabela_kursow typ=""A"" uid=""13a001""> 
                        <numer_tabeli>001/A/NBP/2013</numer_tabeli>
                        <data_publikacji>2013-01-02</data_publikacji>
                      </tabela_kursow>";
      A.CallTo(() => _httpClient.GetStreamAsync(A<string>._)).Returns(Task.FromResult(CreateStream(xml)));
      A.CallTo(() => _client.GetExchangeRateAvailabilityAsync<AverageExchangeRate>()).Returns(Task.FromResult<IEnumerable<ExchangeRateAvailability>>(availabilities));

      var result = await _client.GetExchangeRatesAsync<AverageExchangeRate>(availabilities[0].ForDate);

      CompareAssert.AreEqual(expectedRate, result, ignore => ignore.Rates);
    }

    [Test]
    public async Task GetExchangeRatesAsyncOfType_RateIsAvaible_DeserializeConcreteExchangeRates()
    {
      var availabilities = new List<ExchangeRateAvailability>
      {
        new ExchangeRateAvailability {ForDate = DateTime.UtcNow.Date}
      };
      var expectedRates = new ExchangeRates<AverageExchangeRate>();
      expectedRates.Rates = new List<AverageExchangeRate>
      {
        new AverageExchangeRate{CurrencyCode = "THB", StringValue = "0,1009" , Units = 100, CurrencyName = "bat (Tajlandia)"},
        new AverageExchangeRate{CurrencyCode = "USD", StringValue = "3,0660" , Units = 1, CurrencyName = "dolar amerykański"}
      };
      var xml = @"<tabela_kursow typ=""A"" uid=""13a001""> 
                    <numer_tabeli>001/A/NBP/2013</numer_tabeli>
                    <data_publikacji>2013-01-02</data_publikacji>
                    <pozycja>
                      <nazwa_waluty>bat (Tajlandia)</nazwa_waluty>
                      <przelicznik>100</przelicznik>
                      <kod_waluty>THB</kod_waluty>
                      <kurs_sredni>0,1009</kurs_sredni>
                    </pozycja>
                    <pozycja>
                      <nazwa_waluty>dolar amerykański</nazwa_waluty>
                      <przelicznik>1</przelicznik>
                      <kod_waluty>USD</kod_waluty>
                      <kurs_sredni>3,0660</kurs_sredni>
                    </pozycja>
                  </tabela_kursow>";
      A.CallTo(() => _httpClient.GetStreamAsync(A<string>._)).Returns(Task.FromResult(CreateStream(xml)));
      A.CallTo(() => _client.GetExchangeRateAvailabilityAsync<AverageExchangeRate>()).Returns(Task.FromResult<IEnumerable<ExchangeRateAvailability>>(availabilities));

      var result = await _client.GetExchangeRatesAsync<AverageExchangeRate>(availabilities[0].ForDate);

      CompareAssert.AreEqual(expectedRates.Rates.ElementAt(0), result.Rates.ElementAt(0));
      CompareAssert.AreEqual(expectedRates.Rates.ElementAt(1), result.Rates.ElementAt(1));
    }

    [Test, TestCaseSource("GetExchangeRatesAsyncOfType2_RateIsAvaible_DeserializeConcreteExchangeRates_TestCaseSource")]
    public async Task GetExchangeRatesAsyncOfType2_RateIsAvaible_DeserializeConcreteExchangeRates<T>(T ignoreMe, List<T> expectedRates, string xml) where T : IExchangeRate
    {
      var availabilities = new List<ExchangeRateAvailability>
      {
        new ExchangeRateAvailability {ForDate = DateTime.UtcNow.Date}
      };
      A.CallTo(() => _httpClient.GetStreamAsync(A<string>._)).Returns(Task.FromResult(CreateStream(xml)));
      A.CallTo(() => _client.GetExchangeRateAvailabilityAsync<T>()).Returns(Task.FromResult<IEnumerable<ExchangeRateAvailability>>(availabilities));

      var result = await _client.GetExchangeRatesAsync<T>(availabilities[0].ForDate);

      CompareAssert.AreEqual(expectedRates.ElementAt(0), result.Rates.ElementAt(0));
      CompareAssert.AreEqual(expectedRates.ElementAt(1), result.Rates.ElementAt(1));
    }

    [Test, TestCaseSource("GetExchangeRatesAsync2_RateIsAvailable_DeserializeExchangeRates_TestCasesSource")]
    public async Task GetExchangeRatesAsync2_RateIsAvailable_DeserializeExchangeRates<T>(T nullArg) where T : IExchangeRate
    {
      var availabilities = new List<ExchangeRateAvailability>
      {
        new ExchangeRateAvailability {ForDate = DateTime.UtcNow.Date}
      };
      var expectedRate = new ExchangeRates<T> { PublicationDate = new DateTime(2013, 1, 2), Number = "001/A/NBP/2013" };
      var xml = @"<tabela_kursow typ=""A"" uid=""13a001""> 
                        <numer_tabeli>001/A/NBP/2013</numer_tabeli>
                        <data_publikacji>2013-01-02</data_publikacji>
                      </tabela_kursow>";
      A.CallTo(() => _httpClient.GetStreamAsync(A<string>._)).Returns(Task.FromResult(CreateStream(xml)));
      A.CallTo(() => _client.GetExchangeRateAvailabilityAsync<T>()).Returns(Task.FromResult<IEnumerable<ExchangeRateAvailability>>(availabilities));

      var result = await _client.GetExchangeRatesAsync<T>(availabilities[0].ForDate);

      CompareAssert.AreEqual(expectedRate, result, ignore => ignore.Rates);
    }

    static object[] GetExchangeRatesAsync2_RateIsAvailable_DeserializeExchangeRates_TestCasesSource =
    {
        new object[] { new AverageExchangeRate() },
        new object[] { new AverageInconvertibleExchangeRate() },
        new object[] { new UnitsExchangeRate() },
        new object[] { new BuyAndSellExchangeRate() }
    };

    static object[] GetExchangeRatesAsyncOfType2_RateIsAvaible_DeserializeConcreteExchangeRates_TestCaseSource =
    {
        new object[]
        {
          new AverageExchangeRate(),
          new List<AverageExchangeRate>
          {
            new AverageExchangeRate{CurrencyCode = "THB", StringValue = "0,1009" , Units = 100, CurrencyName = "bat (Tajlandia)"},
            new AverageExchangeRate{CurrencyCode = "USD", StringValue = "3,0660" , Units = 1, CurrencyName = "dolar amerykański"}
          },
          @"<tabela_kursow typ=""A"" uid=""13a001""> 
                    <numer_tabeli>001/A/NBP/2013</numer_tabeli>
                    <data_publikacji>2013-01-02</data_publikacji>
                    <pozycja>
                      <nazwa_waluty>bat (Tajlandia)</nazwa_waluty>
                      <przelicznik>100</przelicznik>
                      <kod_waluty>THB</kod_waluty>
                      <kurs_sredni>0,1009</kurs_sredni>
                    </pozycja>
                    <pozycja>
                      <nazwa_waluty>dolar amerykański</nazwa_waluty>
                      <przelicznik>1</przelicznik>
                      <kod_waluty>USD</kod_waluty>
                      <kurs_sredni>3,0660</kurs_sredni>
                    </pozycja>
                  </tabela_kursow>"
        },
        new object[] 
        { 
          new AverageInconvertibleExchangeRate(),
          new List<AverageInconvertibleExchangeRate>
          {
            new AverageInconvertibleExchangeRate{CountryName = "Afganistan", CurrencyCode = "AFN", StringValue = "6,3103" , Units = 100, CurrencyName = "afgani"},
            new AverageInconvertibleExchangeRate{CountryName = "Albania", CurrencyCode = "ALL", StringValue = "3,3700" , Units = 100, CurrencyName = "lek"}
          },
          @"<tabela_kursow typ=""B"" uid=""08b053"">
                    <numer_tabeli>53/B/NBP/2008</numer_tabeli>
                    <data_publikacji>2008-12-31</data_publikacji>
                    <pozycja>
                      <nazwa_kraju>Afganistan</nazwa_kraju>
                      <nazwa_waluty>afgani</nazwa_waluty>
                      <przelicznik>100</przelicznik>
                      <kod_waluty>AFN</kod_waluty>
                      <kurs_sredni>6,3103</kurs_sredni>
                    </pozycja>
                    <pozycja>
                      <nazwa_kraju>Albania</nazwa_kraju>
                      <nazwa_waluty>lek</nazwa_waluty>
                      <przelicznik>100</przelicznik>
                      <kod_waluty>ALL</kod_waluty>
                      <kurs_sredni>3,3700</kurs_sredni>
                    </pozycja>
                  </tabela_kursow>"
        },
        new object[] 
        { 
          new UnitsExchangeRate(),
          new List<UnitsExchangeRate>
          {
            new UnitsExchangeRate{CountryName = "Kraje b. RWPG", CurrencySymbol = "101", CurrencyName = "rubel transf.", Units = 1, Reference ="*)", StringBuyValue = "0,2090", StringSellValue = "0,2110", StringAverageValue = "0,2100"},
            new UnitsExchangeRate{CountryName = "Albania", CurrencySymbol = "315", CurrencyName = "rubel clear.", Units = 1, Reference = "",StringBuyValue = "0,299", StringSellValue = "2,214", StringAverageValue = "7,54"},
          },
          @"<tabela_kursow typ=""H"" uid=""10h255"">
                    <numer_tabeli>255/2010/BGK</numer_tabeli>
                    <data_notowania>2010-12-30</data_notowania>
                    <data_publikacji>2010-12-31</data_publikacji>
                    <pozycja>
                      <nazwa_kraju>Kraje b. RWPG</nazwa_kraju>
                      <symbol_waluty>101</symbol_waluty>
                      <nazwa_waluty>rubel transf.</nazwa_waluty>
                      <przelicznik>1</przelicznik>
                      <odnosnik>*)</odnosnik>
                      <kurs_kupna>0,2090</kurs_kupna>
                      <kurs_sprzedazy>0,2110</kurs_sprzedazy>
                      <kurs_sredni>0,2100</kurs_sredni>
                    </pozycja>
                    <pozycja>
                      <nazwa_kraju>Albania</nazwa_kraju>
                      <symbol_waluty>315</symbol_waluty>
                      <nazwa_waluty>rubel clear.</nazwa_waluty>
                      <przelicznik>1</przelicznik>
                      <odnosnik/>
                      <kurs_kupna>0,299</kurs_kupna>
                      <kurs_sprzedazy>2,214</kurs_sprzedazy>
                      <kurs_sredni>7,54</kurs_sredni>
                    </pozycja>
                  </tabela_kursow>"
        },
        new object[] 
        { 
          new BuyAndSellExchangeRate(),
          new List<BuyAndSellExchangeRate>
          {
            new BuyAndSellExchangeRate{CurrencyCode = "USD", StringBuyValue = "2,9469" ,StringSellValue = "3,0065" , Units = 1, CurrencyName = "dolar amerykański"},
            new BuyAndSellExchangeRate{CurrencyCode = "AUD", StringBuyValue = "2,9888" ,StringSellValue = "3,0492" , Units = 1, CurrencyName = "dolar australijski"}
          },
          @"<tabela_kursow typ=""C"" uid=""10c255"">
                    <numer_tabeli>255/C/NBP/2010</numer_tabeli>
                    <data_notowania>2010-12-30</data_notowania>
                    <data_publikacji>2010-12-31</data_publikacji>
                    <pozycja>
                      <nazwa_waluty>dolar amerykański</nazwa_waluty>
                      <przelicznik>1</przelicznik>
                      <kod_waluty>USD</kod_waluty>
                      <kurs_kupna>2,9469</kurs_kupna>
                      <kurs_sprzedazy>3,0065</kurs_sprzedazy>
                    </pozycja>
                    <pozycja>
                      <nazwa_waluty>dolar australijski</nazwa_waluty>
                      <przelicznik>1</przelicznik>
                      <kod_waluty>AUD</kod_waluty>
                      <kurs_kupna>2,9888</kurs_kupna>
                      <kurs_sprzedazy>3,0492</kurs_sprzedazy>
                    </pozycja>
                  </tabela_kursow>"
        }
    };

    private Stream CreateStream(string xml)
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);
      writer.Write(xml);
      writer.Flush();
      stream.Position = 0;
      return stream;
    }

    private Stream CreateStream(IEnumerable<string> content)
    {
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);

      foreach (var line in content)
      {
        writer.WriteLine(line);
      }
      writer.Flush();
      stream.Position = 0;
      return stream;
    }
  }
}
