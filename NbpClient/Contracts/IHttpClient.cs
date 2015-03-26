using System.IO;
using System.Threading.Tasks;

namespace NbpClient.Contracts
{
  public interface IHttpClient
  {
    Task<Stream> GetStreamAsync(string requestUri);
  }
}