using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using NbpClient.Contracts;

namespace NbpClient
{
  [Serializable]
  [XmlRoot("tabela_kursow")]
  public class ExchangeRates<T> where T : IExchangeRate
  {
    [XmlElement("data_publikacji")]
    public DateTime PublicationDate { get; set; }

    [XmlElement("numer_tabeli")]
    public string Number { get; set; }

    [XmlElement("pozycja")]
    public List<T> Rates { get; set; }
  }
}