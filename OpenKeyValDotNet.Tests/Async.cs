using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace OpenKeyValDotNet.Tests
{
    class Async
    {
        private static readonly string Guid = System.Guid.NewGuid().ToString();
        private static readonly string Key = "OpenKeyValDotNetTests-" + Guid;

        [Test]
        public async void AsyncSaveAndGetString()
        {
            var testValue = "This is our test string. " + DateTime.Now;            
            var openKeyValClient = new OpenKeyValClient();

            await openKeyValClient.SaveStringAsync(Key, testValue);
            var actualValue = await openKeyValClient.GetStringAsync(Key);

            Assert.AreEqual(testValue, actualValue);

            openKeyValClient.Delete(Key);
        }

        [Test]
        public async void AsyncSaveAndGetType()
        {            
            var openKeyValClient = new OpenKeyValClient();

            var testClass = new TestClass
                                {
                                    NameOfTallestPerson = "Boburto",
                                    NumberOfScubaTanks = 7,
                                    DrawerContents = new List<string> {"Eights", "Eights", "Barbque chips"}
                                };

            await openKeyValClient.SaveAsync(Key, testClass);
            var actualClass = await openKeyValClient.GetAsync<TestClass>(Key);

            AssertEx.PropertyValuesAreEquals(testClass, actualClass);

            openKeyValClient.Delete(Key);
        }

        [Test]
        public async void AsyncSaveAndGetStringWithCompression()
        {
            var testValue = "This is our test string. " + DateTime.Now;
            var openKeyValClient = new OpenKeyValClient();

            await openKeyValClient.SaveStringAsync(Key, testValue, true);
            var actualValue = await openKeyValClient.GetStringAsync(Key, true);

            Assert.AreEqual(testValue, actualValue);

            openKeyValClient.Delete(Key);
        }

        [Test]
        public async void AsyncSaveAndGetTypeWithCompression()
        {
            var openKeyValClient = new OpenKeyValClient();

            var testClass = new TestClass
            {
                NameOfTallestPerson = "Boburto",
                NumberOfScubaTanks = 7,
                DrawerContents = new List<string> { "Eights", "Eights", "Barbque chips" }
            };

            await openKeyValClient.SaveAsync(Key, testClass, true);
            var actualClass = await openKeyValClient.GetAsync<TestClass>(Key, true);

            AssertEx.PropertyValuesAreEquals(testClass, actualClass);

            openKeyValClient.Delete(Key);
        }

        [Test]
        public async void DeleteAsyncDeletes()
        {
            var testValue = "This is our test string. " + DateTime.Now;
            var openKeyValClient = new OpenKeyValClient();

            await openKeyValClient.SaveStringAsync(Key, testValue);
            await openKeyValClient.DeleteAsync(Key);
            var result = await openKeyValClient.GetStringAsync(Key);


            Assert.AreEqual(result, String.Empty);
        }
    }

    internal class TestClass
    {
        public string NameOfTallestPerson { get; set; }
        public int NumberOfScubaTanks { get; set; }
        public List<string> DrawerContents { get; set; }
    }

    public static class AssertEx
    {
        //http://stackoverflow.com/a/318238/613575

        public static void PropertyValuesAreEquals(object actual, object expected)
        {
            var properties = expected.GetType().GetProperties();
            foreach (var property in properties)
            {
                var expectedValue = property.GetValue(expected, null);
                var actualValue = property.GetValue(actual, null);

                var list = actualValue as IList;
                if (list != null)
                    AssertListsAreEquals(property, list, (IList)expectedValue);
                else if (!Equals(expectedValue, actualValue))
                    Assert.Fail("Property {0}.{1} does not match. Expected: {2} but was: {3}", (property.DeclaringType != null) ? property.DeclaringType.Name : "property.DeclaringType s", property.Name, expectedValue, actualValue);
            }
        }

        private static void AssertListsAreEquals(PropertyInfo property, IList actualList, IList expectedList)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (actualList.Count != expectedList.Count)
                Assert.Fail("Property {0}.{1} does not match. Expected IList containing {2} elements but was IList containing {3} elements", property.PropertyType.Name, property.Name, expectedList.Count, actualList.Count);

            for (int i = 0; i < actualList.Count; i++)
                if (!Equals(actualList[i], expectedList[i]))
                    Assert.Fail("Property {0}.{1} does not match. Expected IList with element {1} equals to {2} but was IList with element {1} equals to {3}", property.PropertyType.Name, property.Name, expectedList[i], actualList[i]);
        }
    }
}
