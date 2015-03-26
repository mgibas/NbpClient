using System;
using System.Globalization;
using System.Xml.Serialization;
using NbpClient.Contracts;

namespace NbpClient
{
  [Serializable]
  public class AverageExchangeRate : IExchangeRate
  {
    [XmlElement("nazwa_waluty")]
    public string CurrencyName { get; set; }

    [XmlElement("kod_waluty")]
    public string CurrencyCode { get; set; }

    [XmlElement("przelicznik")]
    public int Units { get; set; }

    [XmlElement("kurs_sredni")]
    public string StringValue { get; set; }

    public decimal Value
    {
      get { return decimal.Parse(StringValue.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture); }
    }
  }
}