using System.Collections.Generic;
using NUnit.Framework;

namespace UnityEngine.Monetization.Editor.Tests
{
    [TestFixture]
    public class IPurchasingAdapterTests
    {
        [Test]
        public void TestTransactionDetailsToJson()
        {
            var details = new TransactionDetails
            {
                productId = "testProductId",
                transactionId = "testTransactionId",
                price = 90m,
                currency = "USD",
                receipt = "testReceipt",
                extras = new Dictionary<string, object>
                {
                    { "testKey", "testValue" },
                    { "testNumber", 56 }
                }
            };

            var jsonDictionary = details.ToJsonDictionary();
            Assert.AreEqual(new Dictionary<string, object>
            {
                { "productId", "testProductId" },
                { "transactionId", "testTransactionId" },
                { "receipt", "testReceipt" },
                { "price", 90m },
                { "currency", "USD" },
                { "extras", new Dictionary<string, object>
                  {
                      { "testKey", "testValue" },
                      { "testNumber", 56 }
                  } }
            }, jsonDictionary);
        }

        [Test]
        public void TestTransactionDetailsValidJson()
        {
            var details = new TransactionDetails
            {
                productId = "testProductId",
                transactionId = "testTransactionId",
                price = 90m,
                currency = "USD",
                receipt = "testReceipt",
                extras = new Dictionary<string, object>
                {
                    { "testKey", "testValue" },
                    { "testNumber", 56 }
                }
            };

            var jsonDictionary = details.ToJsonDictionary();
            var json = MiniJSON.Json.Serialize(jsonDictionary);
            Assert.AreEqual("{\"productId\":\"testProductId\",\"transactionId\":\"testTransactionId\",\"receipt\":\"testReceipt\",\"price\":90,\"currency\":\"USD\",\"extras\":{\"testKey\":\"testValue\",\"testNumber\":56}}", json);
        }
    }
}
