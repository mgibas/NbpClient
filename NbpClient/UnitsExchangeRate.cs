using System;
using System.Globalization;
using System.Xml.Serialization;
using NbpClient.Contracts;

namespace NbpClient
{
  [Serializable]
  public class UnitsExchangeRate : IExchangeRate
  {
    [XmlElement("nazwa_kraju")]
    public string CountryName { get; set; }

    [XmlElement("symbol_waluty")]
    public string CurrencySymbol { get; set; }

    [XmlElement("nazwa_waluty")]
    public string CurrencyName { get; set; }

    [XmlElement("przelicznik")]
    public int Units { get; set; }

    [XmlElement("odnosnik")]
    public string Reference { get; set; }

    [XmlElement("kurs_sredni")]
    public string StringAverageValue { get; set; }

    [XmlElement("kurs_sprzedazy")]
    public string StringSellValue { get; set; }

    [XmlElement("kurs_kupna")]
    public string StringBuyValue { get; set; }

    public decimal AverageValue
    {
      get { return decimal.Parse(StringAverageValue.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture); }
    }

    public decimal SellValue
    {
      get { return decimal.Parse(StringSellValue.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture); }
    }

    public decimal BuyValue
    {
      get { return decimal.Parse(StringBuyValue.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture); }
    }
  }
}