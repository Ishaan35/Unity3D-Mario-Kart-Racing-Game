#if NUGET_MOQ_AVAILABLE
using Moq;
using NUnit.Framework;
using UnityEngine.Advertisements.Platform;
using UnityEngine.Advertisements.Platform.Unsupported;

namespace UnityEngine.Advertisements.Tests
{
    [TestFixture]
    public class UnsupportedPlatformTests
    {
        [Test]
        public void InitialState()
        {
            var platform = new UnsupportedPlatform();
            Assert.That(platform.GetDebugMode, Is.False, "DebugMode should be set to false by default");
            Assert.That(platform.IsInitialized, Is.False, "IsInitialized should be set to false by default");
            Assert.That(platform.GetVersion(), Is.EqualTo("UnsupportedPlatformVersion"));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("test")]
        [TestCase("ads")]
        [TestCase("1")]
        public void IsReadyAndGetPlacementState(string placementId)
        {
            var platform = new UnsupportedPlatform();
            Assert.That(platform.IsReady(placementId), Is.False);
            Assert.That(platform.GetPlacementState(placementId), Is.EqualTo(PlacementState.NotAvailable));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("test")]
        [TestCase("ads")]
        [TestCase("1")]
        public void Load(string placementId)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.Load(placementId));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("test")]
        [TestCase("ads")]
        [TestCase("1")]
        public void Show(string placementId)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.Show(placementId));
        }

        [TestCase(null, false, false)]
        [TestCase("", false, false)]
        [TestCase("test", false, false)]
        [TestCase("123435", false, false)]
        [TestCase(null, true, false)]
        [TestCase("", true, false)]
        [TestCase("test", true, false)]
        [TestCase("123435", true, false)]
        [TestCase(null, false, true)]
        [TestCase("", false, true)]
        [TestCase("test", false, true)]
        [TestCase("123435", false, true)]
        [TestCase(null, true, true)]
        [TestCase("", true, true)]
        [TestCase("test", true, true)]
        [TestCase("123435", true, true)]
        public void Initialize(string gameId, bool testMode, bool enablePerPlacementLoad)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.Initialize(gameId, testMode, enablePerPlacementLoad));
        }

        [Test]
        [TestCase("test")]
        [TestCase("")]
        [TestCase(null)]
        public void SetMetaData(string metaDataCategory)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.SetMetaData(new MetaData(metaDataCategory)));
        }

        [Test]
        public void SetupPlatform()
        {
            var unsupportedPlatform = new UnsupportedPlatform();
            var platform = new Mock<IPlatform>();

            Assert.DoesNotThrow(() => unsupportedPlatform.SetupPlatform(platform.Object));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void SetDebugMode(bool debugMode)
        {
            var platform = new UnsupportedPlatform();
            Assert.DoesNotThrow(() => platform.SetDebugMode(debugMode));
        }
    }
}
#endif
