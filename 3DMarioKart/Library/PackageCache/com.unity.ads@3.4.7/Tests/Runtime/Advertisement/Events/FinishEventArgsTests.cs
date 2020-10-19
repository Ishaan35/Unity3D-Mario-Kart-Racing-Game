using NUnit.Framework;
using UnityEngine.Advertisements.Events;

namespace UnityEngine.Advertisements.Tests
{
    [TestFixture]
    class FinishEventArgsTests
    {
        [TestCase("ads", ShowResult.Failed)]
        [TestCase("intro", ShowResult.Finished)]
        [TestCase("test", ShowResult.Skipped)]
        [TestCase("", ShowResult.Finished)]
        [TestCase(null, ShowResult.Skipped)]
        public void Constructor(string placementId, ShowResult showResult)
        {
            var eventArgs = new FinishEventArgs(placementId, showResult);

            Assert.That(eventArgs.placementId, Is.EqualTo(placementId));
            Assert.That(eventArgs.showResult, Is.EqualTo(showResult));
        }
    }
}
