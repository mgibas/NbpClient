using System;

namespace NbpClient.Parsers
{
  public class AverageInconvertibleExchangeRateAvailabilityParser : ExchangeRateAvailabilityParser
  {
    public override string FileNamePrefix
    {
      get { return "b"; }
    }

    public override Type ParserFor
    {
      get { return typeof(AverageInconvertibleExchangeRate); }
    }
  }
}