using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenKeyValDotNet
{
    internal static class JsonHelper
    {
        internal static string GetJsonFromValue<T>(T value, bool useCompression)
        {
            return (useCompression)
                       ? Compression.Compress(JsonConvert.SerializeObject(value))
                       : JsonConvert.SerializeObject(value);
        }

        internal static async Task<string> GetJsonFromValueAsync<T>(T value, bool useCompression)
        {
            return (useCompression)
                       ? await Compression.CompressAsync(await JsonConvert.SerializeObjectAsync(value))
                       : await JsonConvert.SerializeObjectAsync(value);
        }

        internal static T GetValueFromJson<T>(string response, bool useCompression)
        {
            return (useCompression)
                       ? JsonConvert.DeserializeObject<T>(Compression.Decompress(response))
                       : JsonConvert.DeserializeObject<T>(response);
        }

        internal static async Task<T> GetValueFromJsonAsync<T>(string response, bool useCompression)
        {
            return (useCompression)
                       ? await JsonConvert.DeserializeObjectAsync<T>(await Compression.DecompressAsync(response))
                       : await JsonConvert.DeserializeObjectAsync<T>(response);
        }
    }
}