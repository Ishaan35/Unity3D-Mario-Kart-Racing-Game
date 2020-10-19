using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace UnityEngine.Advertisements.Tests
{
    [TestFixture("test")]
    [TestFixture("gamer")]
    [TestFixture("")]
    class MetaDataTests
    {
        private string m_Category;
        private MetaData m_Metadata;

        public MetaDataTests(string category)
        {
            m_Category = category;
        }

        [SetUp]
        public void SetUp()
        {
            m_Metadata = new MetaData(m_Category);
        }

        [Test]
        public void InitialState()
        {
            Assert.That(m_Metadata.category, Is.EqualTo(m_Category));
            Assert.That(m_Metadata.Values(), Is.Not.Null);
            Assert.That(m_Metadata.Values(), Is.Empty);
        }

        [Test]
        public void ToJSON_Empty()
        {
            Assert.That(m_Metadata.ToJSON(), Is.EqualTo("{}"));
        }

        [Test]
        public void ToJSON()
        {
            m_Metadata.Set("int", 25);
            m_Metadata.Set("bool", false);
            m_Metadata.Set("double", 1.34);
            m_Metadata.Set("string", "test");

            Assert.That(m_Metadata.ToJSON(), Is.EqualTo(@"{""int"":25,""bool"":false,""double"":1.34,""string"":""test""}"));
        }

        [TestCase("test", "value")]
        [TestCase("key", null)]
        [TestCase("bla", true)]
        [TestCase("not", false)]
        [TestCase("id", 10)]
        [TestCase("height", 3.14)]
        [TestCase("enum", ShowResult.Finished)]
        public void SetAndGet(string key, object value)
        {
            Assert.DoesNotThrow(() => m_Metadata.Set(key, value));

            object currentValue = null;
            Assert.DoesNotThrow(() => currentValue = m_Metadata.Get(key));

            Assert.That(currentValue, Is.SameAs(value));
        }

        [TestCase("")]
        [TestCase("key")]
        public void Get_ThrowKeyNotFound(string key)
        {
            Assert.Throws<KeyNotFoundException>(() => m_Metadata.Get(key));
        }

        [TestCase(null)]
        public void Get_ArgumentNullException(string key)
        {
            Assert.Throws<ArgumentNullException>(() => m_Metadata.Get(key));
        }

        [TestCase(null, "value")]
        public void Set_ArgumentNullException(string key, object value)
        {
            Assert.Throws<ArgumentNullException>(() => m_Metadata.Set(key, value));
        }

        [Test]
        public void Set_SameKey_Override()
        {
            m_Metadata.Set("key", "value");
            Assert.DoesNotThrow(() => m_Metadata.Set("key", "test"));

            Assert.That(m_Metadata.Get("key"), Is.EqualTo("test"));
        }

        [Test]
        public void ComplexObject()
        {
            m_Metadata.Set("version", "1.0");
            m_Metadata.Set("key", new TestJsonObject { value = 65 });

            Assert.That(m_Metadata.ToJSON(), Is.EqualTo(@"{""version"":""1.0"",""key"":""UnityEngine.Advertisements.Tests.MetaDataTests+TestJsonObject""}"));
        }

        class TestJsonObject
        {
            public long value;
        }
    }
}
