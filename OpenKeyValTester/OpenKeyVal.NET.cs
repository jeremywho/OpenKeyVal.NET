using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenKeyValTester
{
    public class OpenKeyValDotNET
    {
        private string _address = "http://api.openkeyval.org/";        
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public async Task<string> SaveAsync<T>(string key, T value)
        {
            var valueAsJson = JsonConvert.SerializeObject(value);
            using (var client = new WebClient())
            {
                var result = await client.UploadValuesTaskAsync(Address, "POST", new NameValueCollection { { key, valueAsJson } });
                return Encoding.ASCII.GetString(result);
            }
        }

        public string Save<T>(string key, T value)
        {
            var valueAsJson = JsonConvert.SerializeObject(value);

            using (var client = new WebClient())
            {
                var result = client.UploadValues(Address, "POST", new NameValueCollection { { key, valueAsJson } });
                return Encoding.ASCII.GetString(result);
            }
        }

        public async Task<string> SaveStringAsync(string key, string value)
        {
            using (var client = new WebClient())
            {
                var result = await client.UploadValuesTaskAsync(Address, "POST", new NameValueCollection { { key, value } });
                return Encoding.ASCII.GetString(result);
            }
        }

        public string SaveString<T>(string key, T value)
        {
            var valueAsJson = JsonConvert.SerializeObject(value);
            using (var client = new WebClient())
            {
                var result = client.UploadValues(Address, "POST", new NameValueCollection { { key, valueAsJson } });
                return Encoding.ASCII.GetString(result);
            }
        }    

        /// <summary>
        /// Async method that returns the value for a key that has been deserialized into type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key)
        {
            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(Address + key);
                return JsonConvert.DeserializeObject<T>(response);
            }       
        }

        /// <summary>
        /// Synchronous method that returns the value for a key that has been deserialized into type T
        /// </summary>
        /// <typeparam name="T">The type to deserialize into.</typeparam>
        /// <param name="key">The key to retrieve</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            using (var client = new WebClient())
            {
                var response = client.DownloadString(Address + key);
                return JsonConvert.DeserializeObject<T>(response);
            }           
        }

        /// <summary>
        /// Synchronous method that returns the value for a key that has been deserialized into type T
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <returns></returns>
        public string GetString(string key)
        {
            using (var client = new WebClient())
            {
                var response = client.DownloadString(Address + key);
                return response;
            }
        }

        /// <summary>
        /// Synchronous method that returns the value for a key that has been deserialized into type T
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <returns></returns>
        public string Get(string key)
        {
            using (var client = new WebClient())
            {
                return client.DownloadString(Address + key);
            }
        }

        /// <summary>
        /// Async method that returns the value for a key as a JSON string.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetStringAsync(string key)
        {
            using (var client = new WebClient())
            {
                var response = await client.DownloadStringTaskAsync(Address + key);
                return response;
            }    
        }
    }
}