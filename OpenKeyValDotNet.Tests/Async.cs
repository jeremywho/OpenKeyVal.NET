using System;
using NUnit.Framework;

namespace OpenKeyValDotNet.Tests
{
    class Async
    {
        private static string _guid = Guid.NewGuid().ToString();
        private static string _key = "OpenKeyValDotNetTests-" + _guid;

        [Test]
        public async void AsyncGetString_ShouldGetStringBack()
        {
            var openKeyVal = new OpenKeyVal();
            var result = await openKeyVal.GetStringAsync("location");            
            Assert.IsInstanceOf<string>(result);
        }

        [Test]
        public async void AsyncSaveString_ShouldGetStringBack()
        {
            var openKeyVal = new OpenKeyVal();
            var result = await openKeyVal.SaveStringAsync(_key, "Reno, NV, USA");

            Assert.IsInstanceOf<string>(result);
        }
    }
}
