using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace OpenKeyValDotNet.Tests
{
    class Synchronous
    {
        private static readonly string Guid = System.Guid.NewGuid().ToString();
        private static readonly string Key = "OpenKeyValDotNetTests-" + Guid;

        [Test]
        public void SaveAndGetString()
        {
            var testValue = "This is our test string. " + DateTime.Now;
            var openKeyValClient = new OpenKeyValClient();

            openKeyValClient.SaveString(Key, testValue);
            var actualValue = openKeyValClient.GetString(Key);

            Assert.AreEqual(testValue, actualValue);

            openKeyValClient.Delete(Key);
        }

        [Test]
        public void SaveAndGetType()
        {
            var openKeyValClient = new OpenKeyValClient();

            var testClass = new TestClass
            {
                NameOfTallestPerson = "Boburto",
                NumberOfScubaTanks = 7,
                DrawerContents = new List<string> { "Eights", "Eights", "Barbque chips" }
            };

            openKeyValClient.Save(Key, testClass);
            var actualClass = openKeyValClient.Get<TestClass>(Key);

            AssertEx.PropertyValuesAreEquals(testClass, actualClass);

            openKeyValClient.Delete(Key);
        }

        [Test]
        public void SaveAndGetStringWithCompression()
        {
            var testValue = "This is our test string. " + DateTime.Now;
            var openKeyValClient = new OpenKeyValClient();

            openKeyValClient.SaveString(Key, testValue, true);
            var actualValue = openKeyValClient.GetString(Key, true);

            Assert.AreEqual(testValue, actualValue);

            openKeyValClient.Delete(Key);
        }

        [Test]
        public void SaveAndGetTypeWithCompression()
        {
            var openKeyValClient = new OpenKeyValClient();

            var testClass = new TestClass
            {
                NameOfTallestPerson = "Boburto",
                NumberOfScubaTanks = 7,
                DrawerContents = new List<string> { "Eights", "Eights", "Barbque chips" }
            };

            openKeyValClient.Save(Key, testClass, true);
            var actualClass = openKeyValClient.Get<TestClass>(Key, true);

            AssertEx.PropertyValuesAreEquals(testClass, actualClass);

            openKeyValClient.Delete(Key);
        }

        [Test]
        public void DeleteDeletes()
        {
            var testValue = "This is our test string. " + DateTime.Now;
            var openKeyValClient = new OpenKeyValClient();

            openKeyValClient.SaveString(Key, testValue);
            openKeyValClient.Delete(Key);
            var result = openKeyValClient.GetString(Key);

            Assert.AreEqual(result, String.Empty);
        }

        [Test]
        public void PassingInBaseAddress()
        {
            var testValue = "This is our test string. " + DateTime.Now;
            var openKeyValClient = new OpenKeyValClient("http://api.openkeyval.org/");

            openKeyValClient.SaveString(Key, testValue);
            openKeyValClient.Delete(Key);
            var result = openKeyValClient.GetString(Key);

            Assert.AreEqual(result, String.Empty);
        }
    }
}
