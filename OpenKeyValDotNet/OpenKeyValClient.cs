using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenKeyValDotNet
{
    public class OpenKeyValClient
    {
        private readonly string _baseUrl = "http://api.openkeyval.org/";

        public OpenKeyValClient()
        {            
        }

        public OpenKeyValClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        /// <summary>
        /// Asynchronous method to save an IDictionary.
        /// </summary>
        /// <typeparam name="T">Type of keyValuePairs contained in the Dictionary</typeparam>
        /// <param name="keyValuePairs">The Dictionary containg keyValuePairs.</param>
        /// <param name="useCompression">Boolean value enabling compression. Default is false. Warning: Compress large objects only. Small strings will end up being larger after compression. </param>
        /// <returns>Task with the string returned from OpenKeyVal</returns>
        public async Task<string> SaveAsync<T>(IDictionary<string,T> keyValuePairs, bool useCompression = false)
        {
            var nameValueCollection = new NameValueCollection();

            foreach(var entry in keyValuePairs)
                nameValueCollection.Add(entry.Key, await GetJsonFromValueAsync(entry.Value, useCompression));

            using (var client = new WebClient())
            {
                var result = await client.UploadValuesTaskAsync(_baseUrl, "POST", nameValueCollection);
                return Encoding.ASCII.GetString(result);
            }
        }

        /// <summary>
        /// Asynchronous method to save a value of type T for the given key.
        /// </summary>
        /// <typeparam name="T">The type for value.</typeparam>
        /// <param name="key">The string repreenting the key to save.</param>
        /// <param name="value">The object to save into OpenKeyVal</param>
        /// <param name="useCompression">Boolean value enabling compression. Default is false. Warning: Compress large objects only. Small strings will end up being larger after compression. </param>
        /// <returns>Task with the string returned from OpenKeyVal</returns>
        public async Task<string> SaveAsync<T>(string key, T value, bool useCompression = false)
        {
            return await SaveAsync(new Dictionary<string, T> {{key, value}}, useCompression);
        }

        /// <summary>
        /// Asynchronous method that saves a string value for a given key.
        /// </summary>
        /// <param name="key">String containing the key</param>
        /// <param name="value">String containing the value to save.</param>
        /// <param name="useCompression">Boolean value enabling compression. Default is false. Warning: Compress large objects only. Small strings will end up being larger after compression. </param>
        /// <returns>Json string returned by OpenKeyVal</returns>
        public async Task<string> SaveStringAsync(string key, string value, bool useCompression = false)
        {
            return await SaveAsync(key, value, useCompression);
        }

        /// <summary>
        /// Async method that returns the value for a key that has been deserialized into type T.
        /// </summary>
        /// <typeparam name="T">Type to deserialize value into.</typeparam>
        /// <param name="key">The key for which you want to get the value</param>
        /// <param name="useCompression">Boolean value enabling compression. Default is false. Warning: Compress large objects only. Small strings will end up being larger after compression. </param>
        /// <returns>Task of T where T is the deserialized value for the given key.</returns>
        public async Task<T> GetAsync<T>(string key, bool useCompression = false)
        {
            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(_baseUrl + key);
                return await GetValueFromJsonAsync<T>(response, useCompression);
            }
        }

        /// <summary>
        /// Async method that returns the value for a key as a JSON string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="useCompression">Boolean value enabling compression. Default is false. Warning: Compress large objects only. Small strings will end up being larger after compression. </param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string key, bool useCompression = false)
        {
            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(_baseUrl + key);
                return await GetValueFromJsonAsync<string>(response, useCompression);
            }
        }

        /// <summary>
        /// Asynchronous method that deletes a given key.
        /// </summary>
        /// <param name="key">String containing the key</param>
        /// <returns>Json string returned by OpenKeyVal</returns>
        public async Task<string> DeleteAsync(string key)
        {
            return await SaveAsync(key, String.Empty);
        }

        /// <summary>
        /// Synchronous method to save a value of type T for the given key.
        /// </summary>
        /// <typeparam name="T">The type for value.</typeparam>
        /// <param name="key">The string repreenting the key to save.</param>
        /// <param name="value">The object to save into OpenKeyVal</param>
        /// <param name="useCompression">Boolean value enabling compression. Default is false. Warning: Compress large objects only. Small strings will end up being larger after compression. </param>
        /// <returns>Task with the Json string returned from OpenKeyVal</returns>
        public string Save<T>(string key, T value, bool useCompression = false)
        {
            var valueAsJson = GetJsonFromValue(value, useCompression);
            using (var client = new WebClient())
            {
                var result = client.UploadValues(_baseUrl, "POST", new NameValueCollection { { key, valueAsJson } });
                return Encoding.ASCII.GetString(result);
            }
        }        

        /// <summary>
        /// Synchronous method that saves a string value for a given key.
        /// </summary>
        /// <param name="key">String containing the key</param>
        /// <param name="value">String containing the value to save.</param>
        /// <param name="useCompression">Boolean value enabling compression. Default is false. Warning: Compress large objects only. Small strings will end up being larger after compression. </param>
        /// <returns>Json string returned by OpenKeyVal</returns>
        public string SaveString(string key, string value, bool useCompression = false)
        {
            return Save(key, value, useCompression);
        }

        /// <summary>
        /// Synchronous method that returns the value for a key that has been deserialized into type T
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="key">The key to retrieve</param>
        /// <param name="useCompression">Boolean value enabling compression. Default is false. Warning: Compress large objects only. Small strings will end up being larger after compression. </param>
        /// <returns>The value deserialized into type type for the given key.</returns>
        public T Get<T>(string key, bool useCompression = false)
        {
            using (var client = new WebClient())
            {
                var response = client.DownloadString(_baseUrl + key);
                return GetValueFromJson<T>(response, useCompression);
            }
        }

        /// <summary>
        /// Synchronous method that returns the value for a key that has been deserialized into type T
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <param name="useCompression">Boolean value enabling compression. Default is false. Warning: Compress large objects only. Small strings will end up being larger after compression. </param>
        /// <returns>The string stored for the given key.</returns>
        public string GetString(string key, bool useCompression = false)
        {
            using (var client = new WebClient())
            {
                var response = client.DownloadString(_baseUrl + key);
                return GetValueFromJson<string>(response, useCompression);
            }
        }

        /// <summary>
        /// Synchronous method that deletes a given key.
        /// </summary>
        /// <param name="key">String containing the key</param>
        /// <returns>Json string returned by OpenKeyVal</returns>
        public string Delete(string key)
        {
            return Save(key, String.Empty);
        }

        private static string GetJsonFromValue<T>(T value, bool useCompression)
        {
            return (useCompression)
                       ? Compress(JsonConvert.SerializeObject(value))
                       : JsonConvert.SerializeObject(value);
        }

        private static async Task<string> GetJsonFromValueAsync<T>(T value, bool useCompression)
        {
            return (useCompression)
                       ? await CompressAsync(await JsonConvert.SerializeObjectAsync(value))
                       : await JsonConvert.SerializeObjectAsync(value);
        }

        private static T GetValueFromJson<T>(string response, bool useCompression)
        {
            return (useCompression)
                       ? JsonConvert.DeserializeObject<T>(Decompress(response))
                       : JsonConvert.DeserializeObject<T>(response);
        }

        private static async Task<T> GetValueFromJsonAsync<T>(string response, bool useCompression)
        {
            return (useCompression)
                       ? await JsonConvert.DeserializeObjectAsync<T>(await DecompressAsync(response))
                       : await JsonConvert.DeserializeObjectAsync<T>(response);
        }

        /// <summary>
        /// Compresses a string using System.IO.Compression.GZipStream
        /// </summary>
        /// <param name="s">String to compress</param>
        /// <returns>Compressed string</returns>
        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return Convert.ToBase64String(mso.ToArray());
            }
        }

        /// <summary>
        /// Asynchronously compresses a string using System.IO.Compression.GZipStream
        /// </summary>
        /// <param name="s">String to compress</param>
        /// <returns>Compressed string</returns>
        public static async Task<string> CompressAsync(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    await msi.CopyToAsync(gs);
                }
                
                return Convert.ToBase64String(mso.ToArray());
            }
        }

        /// <summary>
        /// Decompresses a string using System.IO.Compression.GZipStream
        /// </summary>
        /// <param name="s">String to decompress</param>
        /// <returns>Decompressed string</returns>
        public static string Decompress(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    gs.CopyTo(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }

        /// <summary>
        /// Asychronously Decompresses a string using System.IO.Compression.GZipStream
        /// </summary>
        /// <param name="s">String to decompress</param>
        /// <returns>Decompressed string</returns>
        public static async Task<string> DecompressAsync(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                {
                    await gs.CopyToAsync(mso);
                }
                return Encoding.Unicode.GetString(mso.ToArray());
            }
        }
    }
}
