using System;
using System.Globalization;
using System.Xml.Serialization;
using NbpClient.Contracts;

namespace NbpClient
{
  [Serializable]
  public class BuyAndSellExchangeRate : IExchangeRate
  {
    [XmlElement("nazwa_waluty")]
    public string CurrencyName { get; set; }

    [XmlElement("kod_waluty")]
    public string CurrencyCode { get; set; }

    [XmlElement("przelicznik")]
    public int Units { get; set; }

    [XmlElement("kurs_kupna")]
    public string StringBuyValue { get; set; }

    [XmlElement("kurs_sprzedazy")]
    public string StringSellValue { get; set; }

    public decimal BuyValue
    {
      get { return decimal.Parse(StringBuyValue.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture); }
    }

    public decimal SellValue
    {
      get { return decimal.Parse(StringSellValue.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture); }
    }
  }
}