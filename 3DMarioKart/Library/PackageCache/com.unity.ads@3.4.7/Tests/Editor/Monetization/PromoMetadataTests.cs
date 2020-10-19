using System;
using NUnit.Framework;

namespace UnityEngine.Monetization.Editor.Tests
{
    [TestFixture]
    public class PromoMetadataTests
    {
        public struct IsExpiredTestData
        {
            public DateTime firstImpressionDate;
            public TimeSpan offerDuration;
            public bool isExpired;
        }
        private IsExpiredTestData[] isExpiredTestData;

        [OneTimeSetUp]
        public void SetUp()
        {
            this.isExpiredTestData = new IsExpiredTestData[]
            {
                new IsExpiredTestData
                {
                    firstImpressionDate = DateTime.Now.Add(-TimeSpan.FromSeconds(100)),
                    offerDuration = TimeSpan.FromSeconds(60),
                    isExpired = true
                },
                new IsExpiredTestData
                {
                    offerDuration = TimeSpan.FromSeconds(100),
                    isExpired = false
                },
                new IsExpiredTestData
                {
                    firstImpressionDate = DateTime.Now.Add(-TimeSpan.FromSeconds(20)),
                    offerDuration = TimeSpan.FromSeconds(60),
                    isExpired = false
                }
            };
        }

        [Test]
        public void TestIsExpired()
        {
            foreach (var tt in isExpiredTestData)
            {
                var metadata = new PromoMetadata
                {
                    impressionDate = tt.firstImpressionDate,
                    offerDuration = tt.offerDuration
                };
                Assert.That(metadata.isExpired, Is.EqualTo(tt.isExpired));
            }
        }

        [TestCase(100, 60, -40)]
        [TestCase(20, 60, 40)]
        [TestCase(60, 60, 0)]
        public void TestTimeRemaining(int impressionTimeSpan, int offerDuratonTimeSpan, int remainingTime)
        {
            var metadata = new PromoMetadata
            {
                impressionDate = DateTime.Now.Add(-TimeSpan.FromSeconds(impressionTimeSpan)),
                offerDuration = TimeSpan.FromSeconds(offerDuratonTimeSpan)
            };
            Assert.That(metadata.timeRemaining, Is.EqualTo(TimeSpan.FromSeconds(remainingTime)).Within(1).Seconds);
        }

        [TestCase("100.gold.coins", true)]
        [TestCase(null, false)]
        public void TestIsPremium(string productId, bool isPremium)
        {
            var metadata = new PromoMetadata
            {
                premiumProduct =
                {
                    productId = productId
                }
            };
            Assert.That(metadata.isPremium, Is.EqualTo(isPremium));
        }
    }
}
