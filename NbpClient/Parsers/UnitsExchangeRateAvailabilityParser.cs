using System;

namespace NbpClient.Parsers
{
  public class UnitsExchangeRateAvailabilityParser : ExchangeRateAvailabilityParser
  {
    public override string FileNamePrefix
    {
      get { return "h"; }
    }

    public override Type ParserFor
    {
      get { return typeof(UnitsExchangeRate); }
    }
  }
}