using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NbpClient.Contracts
{
  public interface INbpClient
  {
    ExchangeRates<T> GetExchangeRates<T>(DateTime forDate) where T : IExchangeRate;

    Task<ExchangeRates<T>> GetExchangeRatesAsync<T>(DateTime forDate) where T : IExchangeRate;

    IEnumerable<DateTime> GetExchangeRateAvailableDates<T>() where T : IExchangeRate;

    Task<IEnumerable<DateTime>> GetExchangeRateAvailableDatesAsync<T>() where T : IExchangeRate;
  }
}