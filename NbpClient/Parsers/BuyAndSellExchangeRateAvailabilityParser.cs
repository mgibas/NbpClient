using System;

namespace NbpClient.Parsers
{
  public class BuyAndSellExchangeRateAvailabilityParser : ExchangeRateAvailabilityParser
  {
    public override string FileNamePrefix
    {
      get { return "c"; }
    }

    public override Type ParserFor
    {
      get { return typeof(BuyAndSellExchangeRate); }
    }
  }
}