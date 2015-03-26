using System;

namespace NbpClient.Parsers
{
  public class AverageExchangeRateAvailabilityParser : ExchangeRateAvailabilityParser
  {
    public override string FileNamePrefix
    {
      get { return "a"; }
    }

    public override Type ParserFor
    {
      get { return typeof(AverageExchangeRate); }
    }
  }
}