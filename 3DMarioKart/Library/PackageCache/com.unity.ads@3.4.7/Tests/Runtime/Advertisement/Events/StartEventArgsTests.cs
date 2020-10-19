using NUnit.Framework;
using UnityEngine.Advertisements.Events;

namespace UnityEngine.Advertisements.Tests
{
    [TestFixture]
    class StartEventArgsTests
    {
        [TestCase("ads")]
        [TestCase("intro")]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void Constructor(string placementId)
        {
            var eventArgs = new StartEventArgs(placementId);

            Assert.That(eventArgs.placementId, Is.EqualTo(placementId));
        }
    }
}
