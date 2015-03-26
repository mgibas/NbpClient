using System.Configuration.Abstractions;
using System.Net;
using System.Net.Http;
using NbpClient.Contracts;
using NbpClient.Parsers;
using Ninject;
using ProxyFoo;

namespace NbpClient.Ninject
{
  public static class KernelExtensions
  {
    public static void BindNbpClient(this IKernel @this)
    {
      @this.Bind<IHttpClient>()
        .ToMethod(context => Duck.Cast<IHttpClient>(new HttpClient(new HttpClientHandler { Credentials = CredentialCache.DefaultNetworkCredentials })));

      @this.Bind<IAppSettings>()
        .ToMethod(context => ConfigurationManager.Instance.AppSettings);

      @this.Bind<ExchangeRateAvailabilityParser>()
        .To<AverageExchangeRateAvailabilityParser>();

      @this.Bind<ExchangeRateAvailabilityParser>()
        .To<AverageInconvertibleExchangeRateAvailabilityParser>();

      @this.Bind<ExchangeRateAvailabilityParser>()
        .To<BuyAndSellExchangeRateAvailabilityParser>();

      @this.Bind<ExchangeRateAvailabilityParser>()
        .To<UnitsExchangeRateAvailabilityParser>();
    }
  }
}
