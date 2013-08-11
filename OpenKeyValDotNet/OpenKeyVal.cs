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
    public class OpenKeyVal
    {
        private string _address = "http://api.openkeyval.org/";
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public async Task<string> SaveAsync<T>(IDictionary<string,T> values, bool useCompression = false)
        {
            var nameValueCollection = new NameValueCollection();

            foreach(var entry in values)
                nameValueCollection.Add(entry.Key, GetJsonFromValue(entry.Value, useCompression));

            using (var client = new WebClient())
            {
                var result = await client.UploadValuesTaskAsync(Address, "POST", nameValueCollection);
                return Encoding.ASCII.GetString(result);
            }
        }

        public async Task<string> SaveAsync<T>(string key, T value, bool useCompression = false)
        {
            var valueAsJson = GetJsonFromValue(value, useCompression);
            using (var client = new WebClient())
            {
                var result = await client.UploadValuesTaskAsync(Address, "POST", new NameValueCollection { { key, valueAsJson } });
                return Encoding.ASCII.GetString(result);
            }
        }

        public string Save<T>(string key, T value, bool useCompression = false)
        {
            var valueAsJson = GetJsonFromValue(value, useCompression);
            using (var client = new WebClient())
            {
                var result = client.UploadValues(Address, "POST", new NameValueCollection {{key, valueAsJson}});
                return Encoding.ASCII.GetString(result);
            }
        }

        public async Task<string> SaveStringAsync(string key, string value, bool useCompression = false)
        {
            return await SaveAsync(key, value, useCompression);
        }

        /// <summary>
        /// Synchronous method that saves a string value for a given key.
        /// </summary>
        /// <param name="key">String containing the key</param>
        /// <param name="value">String containing the value to save.</param>
        /// <param name="useCompression">Boolean value indicating we</param>
        /// <returns></returns>
        public string SaveString(string key, string value, bool useCompression = false)
        {
            return Save(key, value, useCompression);
        }

        /// <summary>
        /// Async method that returns the value for a key that has been deserialized into type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="useCompression"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, bool useCompression = false)
        {
            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(Address + key);
                return GetValueFromJson<T>(response, useCompression);
            }
        }

        /// <summary>
        /// Synchronous method that returns the value for a key that has been deserialized into type T
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="key">The key to retrieve</param>
        /// <param name="useCompression"></param>
        /// <returns></returns>
        public T Get<T>(string key, bool useCompression = false)
        {
            using (var client = new WebClient())
            {
                var response = client.DownloadString(Address + key);
                return GetValueFromJson<T>(response, useCompression);
            }
        }

        /// <summary>
        /// Synchronous method that returns the value for a key that has been deserialized into type T
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <param name="useCompression"></param>
        /// <returns></returns>
        public string GetString(string key, bool useCompression = false)
        {
            using (var client = new WebClient())
            {
                var response = client.DownloadString(Address + key);
                return GetValueFromJson<string>(response, useCompression);
            }
        }

        /// <summary>
        /// Async method that returns the value for a key as a JSON string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="useCompression"></param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string key, bool useCompression = false)
        {
            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(Address + key);
                return GetValueFromJson<string>(response, useCompression);
            }
        }

        private static string GetJsonFromValue<T>(T value, bool useCompression)
        {
            return (useCompression)
                       ? Compress(JsonConvert.SerializeObject(value))
                       : JsonConvert.SerializeObject(value);
        }

        private static T GetValueFromJson<T>(string response, bool useCompression)
        {
            return (useCompression)
                       ? JsonConvert.DeserializeObject<T>(Decompress(response))
                       : JsonConvert.DeserializeObject<T>(response);
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
    }
}
