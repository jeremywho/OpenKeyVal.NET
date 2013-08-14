using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using OpenKeyValDotNet;

namespace OpenKeyValTester
{
    class Program
    {
        static void Main()
        {
            try
            {
                var tester = new OpenKeyValTester();
                var t = new Task(async () =>
                    {
                        await tester.RunTests();
                    });
                t.Start();
                t.Wait();
                
                Console.WriteLine("Done.");
                Console.ReadLine();
            }
            catch (WebException we)
            {
                Console.WriteLine("WebException: Message");
                Console.WriteLine(we.Message);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }

    public class OpenKeyValTester
    {
        private readonly List<string> _keys = new List<string>();
        private readonly OpenKeyValClient _okv = new OpenKeyValClient();

        public async Task RunTests()
        {
            await DoPostAsync();

            DoGetString();
            DoGetWithType();
        }

        public void DoPost()
        {
            var guid = ConfigurationManager.AppSettings.Get("Guid");
            if (String.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
                ConfigurationManager.AppSettings.Set("Guid", guid);
            }

            var keyPrefix = String.Format("weebu-{0}", guid);

            var key1 = String.Format("{0}-{1}", keyPrefix, "1");
            var value1 = new PostInfo
            {
                PostID = 3,
                DateTime = DateTime.Now,
                Data = "API is coming along!!"
            };            

            _keys.Add(key1);

            _okv.Save(key1, value1);
        }

        public async Task DoPostAsync()
        {
            var guid = ConfigurationManager.AppSettings.Get("Guid");
            if (String.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
                ConfigurationManager.AppSettings.Set("Guid", guid);
            }

            var keyPrefix = String.Format("weebu-{0}", guid);

            var key1 = String.Format("{0}-{1}", keyPrefix, "1");
            var value1 = new PostInfo
            {
                PostID = 3,
                DateTime = DateTime.Now,
                Data = "Here is our test."
            };

            _keys.Add(key1);

            await _okv.SaveAsync(key1, value1);
        }

        public async Task DoGetStringAsync()
        {
            foreach (var key in _keys)
            {
                var r = await _okv.GetAsync<PostInfo>(key);
                var output = String.Format("PostID: '{0}', DateTime: '{1}', Data: '{2}'", r.PostID, r.DateTime, r.Data);
                Console.WriteLine(output);
            }
        }

        public void DoGetString()
        {
            foreach (var key in _keys)
            {
                var r = _okv.Get<PostInfo>(key);
                var output = String.Format("PostID: '{0}', DateTime: '{1}', Data: '{2}'", r.PostID, r.DateTime, r.Data);
                Console.WriteLine(output);
            }
        }

        public async Task DoGetWithTypeAsync()
        {
            foreach (var key in _keys)
            {
                var r = await _okv.GetAsync<PostInfo>(key);
                var output = String.Format("PostID: '{0}', DateTime: '{1}', Data: '{2}'", r.PostID, r.DateTime, r.Data);
                Console.WriteLine(output);
            }
        }

        public void DoGetWithType()
        {
            foreach (var key in _keys)
            {
                var r = _okv.Get<PostInfo>(key);
                var output = String.Format("PostID: '{0}', DateTime: '{1}', Data: '{2}'", r.PostID, r.DateTime, r.Data);
                Console.WriteLine(output);
            }
        }
    }

    public class PostInfo
    {
        public int PostID { get; set; }
        public DateTime DateTime { get; set; }
        public string Data { get; set; }
    }

}
