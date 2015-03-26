using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using NbpClient.Contracts;
using NbpClient.Parsers;

namespace NbpClient
{
  public class NbpHttpClient : INbpClient
  {
    private readonly IHttpClient _httpClient;
    private readonly IAppSettings _appSettings;
    private readonly IEnumerable<ExchangeRateAvailabilityParser> _parsers;

    public NbpHttpClient(IHttpClient httpClient, IAppSettings appSettings, IEnumerable<ExchangeRateAvailabilityParser> parsers)
    {
      _httpClient = httpClient;
      _appSettings = appSettings;
      _parsers = parsers;
    }

    public ExchangeRates<T> GetExchangeRates<T>(DateTime forDate) where T : IExchangeRate
    {
      var task = GetExchangeRatesAsync<T>(forDate);
      task.Wait();
      return task.Result;
    }

    public async virtual Task<ExchangeRates<T>> GetExchangeRatesAsync<T>(DateTime forDate) where T : IExchangeRate
    {
      var availability = await GetExchangeRateAvailabilityAsync<T>();

      var baseUrl = _appSettings.AppSetting("nbp.xml.url");
      var url = string.Format("{0}{1}.xml", baseUrl, availability.Single(a => a.ForDate == forDate).FileName);
      
      var file = await _httpClient.GetStreamAsync(url);

      ExchangeRates<T> result;
      var serializer = new XmlSerializer(typeof(ExchangeRates<T>));
      using (var reader = new StreamReader(file))
      {
        result = (ExchangeRates<T>)serializer.Deserialize(reader);
      }

      return result;
    }

    public IEnumerable<DateTime> GetExchangeRateAvailableDates<T>() where T : IExchangeRate
    {
      var task = GetExchangeRateAvailableDatesAsync<T>();
      task.Wait();
      return task.Result;
    }

    public async virtual Task<IEnumerable<DateTime>> GetExchangeRateAvailableDatesAsync<T>() where T : IExchangeRate
    {
      var availability = await GetExchangeRateAvailabilityAsync<T>();
      return availability.Select(m => m.ForDate);
    }

    protected internal virtual async Task<IEnumerable<ExchangeRateAvailability>> GetExchangeRateAvailabilityAsync<T>()
    {
      var url = _appSettings.AppSetting("nbp.dir.url");

      var file = await _httpClient.GetStreamAsync(url);
      var parser = _parsers.Single(p => p.ParserFor == typeof(T));

      var result = new List<ExchangeRateAvailability>();
      using (var reader = new StreamReader(file))
      {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
          if (parser.CanParse(line))
          {
            result.Add(parser.Parse(line));
          }
        }
      }

      return result;
    }
  }
}