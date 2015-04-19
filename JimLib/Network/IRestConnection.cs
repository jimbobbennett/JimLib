using System.Collections.Generic;
using System.Threading.Tasks;

namespace JimBobBennett.JimLib.Network
{
    public interface IRestConnection
    {
        Task<RestResponse<T>> MakeRequestAsync<T, TData>(Method method, ResponseType responseType, string baseUrl,
            string resource = "/", string username = null, string password = null, int timeout = 10000,
            Dictionary<string, string> headers = null, TData postData = null)
            where T : class, new()
            where TData : class;

        Task<byte[]> MakeRawGetRequestAsync(string baseUrl, string resource = "/",
            string username = null, string password = null, int timeout = 10000,
            Dictionary<string, string> headers = null);
    }
}
