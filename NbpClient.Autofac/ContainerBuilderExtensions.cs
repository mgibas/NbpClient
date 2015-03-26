using System.Configuration.Abstractions;
using System.Net;
using System.Net.Http;
using Autofac;
using NbpClient.Contracts;
using NbpClient.Parsers;
using ProxyFoo;

namespace NbpClient.Autofac
{
  public static class ContainerBuilderExtensions
  {
    public static void RegisterNbpClient(this ContainerBuilder @this)
    {
      @this.Register(context => Duck.Cast<IHttpClient>(new HttpClient(new HttpClientHandler { Credentials = CredentialCache.DefaultNetworkCredentials })))
        .As<IHttpClient>();

      @this.Register(context => ConfigurationManager.Instance.AppSettings)
        .As<IAppSettings>();

      @this.RegisterType<AverageExchangeRateAvailabilityParser>()
        .As<ExchangeRateAvailabilityParser>();

      @this.RegisterType<AverageInconvertibleExchangeRateAvailabilityParser>()
        .As<ExchangeRateAvailabilityParser>();

      @this.RegisterType<BuyAndSellExchangeRateAvailabilityParser>()
        .As<ExchangeRateAvailabilityParser>();

      @this.RegisterType<UnitsExchangeRateAvailabilityParser>()
        .As<ExchangeRateAvailabilityParser>();
    }
  }
}
