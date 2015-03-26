using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace NbpClient.Parsers
{
  public abstract class ExchangeRateAvailabilityParser
  {
    public abstract string FileNamePrefix { get; }

    public abstract Type ParserFor { get; }

    public virtual bool CanParse(string fileName)
    {
      return fileName.StartsWith(FileNamePrefix);
    }

    public virtual ExchangeRateAvailability Parse(string fileNameToParse)
    {
      var match = Regex.Match(fileNameToParse, "(?<type>[a-z])[0-9]{3}z(?<date>[0-9]{6})");
      var result = new ExchangeRateAvailability();
      result.FileName = fileNameToParse;
      result.ForDate = DateTime.ParseExact(match.Groups["date"].Value, "yyMMdd", CultureInfo.InvariantCulture);

      return result;
    }
  }
}