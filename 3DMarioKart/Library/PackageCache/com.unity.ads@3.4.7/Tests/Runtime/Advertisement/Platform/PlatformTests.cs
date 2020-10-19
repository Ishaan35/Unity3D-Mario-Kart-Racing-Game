#if NUGET_MOQ_AVAILABLE
using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine.Advertisements.Utilities;
using UnityEngine.TestTools;

namespace UnityEngine.Advertisements.Tests
{
    public class PlatformTests
    {
        private Mock<IUnityLifecycleManager> m_CoroutineExecutorMock;
        private Mock<INativePlatform> m_NativePlatformMock;
        private Mock<IBanner> m_BannerMock;

        [SetUp]
        public void Setup()
        {
            m_CoroutineExecutorMock = new Mock<IUnityLifecycleManager>(MockBehavior.Strict);
            m_NativePlatformMock = new Mock<INativePlatform>(MockBehavior.Default);
            m_BannerMock = new Mock<IBanner>(MockBehavior.Default);

            m_CoroutineExecutorMock.Setup(x => x.Post(It.IsAny<Action>())).Callback<Action>((action) => { action?.Invoke(); });
            m_CoroutineExecutorMock.Setup(x => x.StartCoroutine(It.IsAny<IEnumerator>())).Callback<IEnumerator>(x => {
                while (x.MoveNext()) {}
            }).Returns<Coroutine>(null);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public void IsInitialize(bool actual, bool expected)
        {
            m_NativePlatformMock.Setup(x => x.IsInitialized()).Returns(actual);
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            Assert.That(platform.IsInitialized, Is.EqualTo(expected), "IsInitialized did not return the correct value");
        }

        [Test]
        [TestCase("3.4.0", "3.4.0", TestName = "GetVersion(Valid String)")]
        [TestCase("", "", TestName = "GetVersion(Empty String)")]
        [TestCase(null, "UnknownVersion", TestName = "GetVersion(null String)")]
        public void GetVersion(string actual, string expected)
        {
            m_NativePlatformMock.Setup(x => x.GetVersion()).Returns(actual);
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            Assert.That(platform.Version, Is.EqualTo(expected), "GetVersion failed to return the correct value");
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public void GetDebugMode(bool actual, bool expected)
        {
            m_NativePlatformMock.Setup(x => x.GetDebugMode()).Returns(actual);
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            Assert.That(platform.DebugMode, Is.EqualTo(expected), "GetDebugMode did not return the correct value");
            m_NativePlatformMock.Verify(x => x.GetDebugMode(), Times.Once(), "NativePlatform.GetDebugMode() was not called as expected");
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public void SetDebugMode(bool actual, bool expected)
        {
            m_NativePlatformMock.Setup(x => x.SetDebugMode(It.IsAny<bool>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.DebugMode = actual;
            m_NativePlatformMock.Verify(x => x.SetDebugMode(It.IsAny<bool>()), Times.Once(), "NativePlatform.SetDebugMode(bool) was not called as expected");
        }

        [Test]
        [TestCase("1234567", false, false)]
        [TestCase("", false, false)]
        [TestCase(null, false, false)]
        [TestCase("1234567", true, false)]
        [TestCase("1234567", null, false)]
        [TestCase("1234567", false, true)]
        [TestCase("1234567", false, null)]
        [TestCase("1234567", null, null)]
        [TestCase(null, null, null)]
        public void NativePlatformInitialize(string gameId, bool testMode, bool loadEnabled)
        {
            m_NativePlatformMock.Setup(x => x.Initialize(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Initialize(gameId, testMode, loadEnabled);
            m_NativePlatformMock.Verify(x => x.Initialize(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once(), "NativePlatform.Initialize() was not called as expected");
        }

        [Test]
        [TestCase("loadAd")]
        public void NativePlatformLoad(string placementId)
        {
            m_NativePlatformMock.Setup(x => x.Load(It.IsAny<string>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Load(placementId);
            m_NativePlatformMock.Verify(x => x.Load(It.IsAny<string>()), Times.Once(), "NativePlatform.Load() was not called as expected");
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void NativePlatformLoadError(string placementId)
        {
            m_NativePlatformMock.Setup(x => x.Load(It.IsAny<string>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Load(placementId);
            LogAssert.Expect(LogType.Error, "placementId cannot be nil or empty");
            m_NativePlatformMock.Verify(x => x.Load(It.IsAny<string>()), Times.Never(), "NativePlatform.Load() was called when it should not have been");
        }

        [Test]
        [TestCase("showAd")]
        [TestCase("")]
        [TestCase(null)]
        public void NativePlatformShow(string placementId)
        {
            m_NativePlatformMock.Setup(x => x.Show(It.IsAny<string>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Show(placementId, null);
            m_NativePlatformMock.Verify(x => x.Show(It.IsAny<string>()), Times.Once(), "NativePlatform.Show() was not called as expected");
        }

        [Test]
        [TestCase("adIsReady")]
        [TestCase("")]
        [TestCase(null)]
        public void NativePlatformIsReady(string placementId)
        {
            m_NativePlatformMock.Setup(x => x.IsReady(It.IsAny<string>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.IsReady(placementId);
            m_NativePlatformMock.Verify(x => x.IsReady(It.IsAny<string>()), Times.Once(), "NativePlatform.IsReady() was not called as expected");
        }

        [Test]
        [TestCase("adIsReady")]
        [TestCase("")]
        [TestCase(null)]
        public void NativePlatformGetPlacementState(string placementId)
        {
            m_NativePlatformMock.Setup(x => x.GetPlacementState(It.IsAny<string>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.GetPlacementState(placementId);
            m_NativePlatformMock.Verify(x => x.GetPlacementState(It.IsAny<string>()), Times.Once(), "NativePlatform.GetPlacementState() was not called as expected");
        }

        [Test]
        public void NativePlatformAddAndRemoveListener()
        {
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            var testListener = new UnityAdsTestListener(null, null, null, null);

            Assert.That(platform.Listeners, Is.Not.Null, "The list of Listeners should not be null if the platform has been created");

            platform.AddListener(testListener);
            Assert.That(platform.Listeners.Count, Is.EqualTo(1), "Incorrect number of listeners");

            platform.RemoveListener(testListener);
            Assert.That(platform.Listeners.Count, Is.EqualTo(0), "Incorrect number of listeners");
        }

        [Test]
        public void NativePlatformSetMetaData()
        {
            var metaData = new MetaData("TestMeta");
            metaData.Set("key1", "value1");

            m_NativePlatformMock.Setup(x => x.SetMetaData(It.IsAny<MetaData>()));
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.SetMetaData(metaData);
            m_NativePlatformMock.Verify(x => x.SetMetaData(It.IsAny<MetaData>()), Times.Once(), "NativePlatform.SetMetaData() was not called as expected");
        }

        [Test]
        public void GetClonedHashSet()
        {
            var testListener = new UnityAdsTestListener(null, null, null, null);
            var listeners = new HashSet<IUnityAdsListener> { testListener };

            var listenersCopy = Platform.Platform.GetClonedHashSet(listeners);
            Assert.That(listenersCopy.Count, Is.EqualTo(listeners.Count), "Both listener HashSets counts should be equal");

            var iterator1 = listeners.GetEnumerator();
            var iterator2 = listenersCopy.GetEnumerator();
            for (var x = 0; x < listeners.Count; x++)
            {
                Assert.That(iterator1.Current, Is.EqualTo(iterator2.Current), "Cloned HashSet object does not match original");
            }

            iterator1.Dispose();
            iterator2.Dispose();
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [TestCase(null, ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsReady(string testPlacementId)
        {
            var hasCalledListener = false;
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.AddListener(new UnityAdsTestListener((placementId) => {
                Assert.That(placementId, Is.EqualTo(testPlacementId), "The placementId under test should have been the placement id that is returned Ready");
                hasCalledListener = true;
            }, null, null, null));
            platform.UnityAdsReady(testPlacementId);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The OnUnityAdsReady callback should have called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [TestCase(null, ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsDidStart(string testPlacementId)
        {
            var hasCalledListener = false;
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.AddListener(new UnityAdsTestListener(null, null, (placementId) => {
                Assert.That(placementId, Is.EqualTo(testPlacementId), "The placementId under test should have been the placement id that is returned Starting");
                hasCalledListener = true;
            }, null));
            platform.UnityAdsDidStart(testPlacementId);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The OnUnityAdsDidStart callback should have called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ShowResult.Failed, ExpectedResult = null)]
        [TestCase(null, ShowResult.Failed, ExpectedResult = null)]
        [TestCase("randomPlacementId", ShowResult.Finished, ExpectedResult = null)]
        [TestCase(null, ShowResult.Finished, ExpectedResult = null)]
        [TestCase("randomPlacementId", ShowResult.Skipped, ExpectedResult = null)]
        [TestCase(null, ShowResult.Skipped, ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsDidFinish(string testPlacementId, ShowResult testShowResult)
        {
            var hasCalledListener = false;
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.AddListener(new UnityAdsTestListener(null, null, null, (placementId, showResult) => {
                Assert.That(placementId, Is.EqualTo(testPlacementId), "The placementId under test should have been the placement id that is returned Finishing");
                Assert.That(showResult, Is.EqualTo(testShowResult), "The showResult under test should have been the showResult that is returned Finishing");
                hasCalledListener = true;
            }));
            platform.UnityAdsDidFinish(testPlacementId, testShowResult);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The OnUnityAdsDidFinish callback should have called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [TestCase(null, ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsDidError(string testPlacementId)
        {
            var hasCalledListener = false;
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.AddListener(new UnityAdsTestListener(null, (placementId) => {
                Assert.That(placementId, Is.EqualTo(testPlacementId), "The placementId under test should have been the placement id that is returned in Error");
                hasCalledListener = true;
            }, null, null));
            platform.UnityAdsDidError(testPlacementId);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The OnUnityAdsDidError callback should have called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [TestCase(null, ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator OnStartCallbackSetsIsShowingOnAdStart(string testPlacementId)
        {
            var hasCalledListener = false;
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.AddListener(new UnityAdsTestListener(null, null, (placementId) => {
                Assert.That(placementId, Is.EqualTo(testPlacementId), "The placementId under test should have been the placement id that is returned Starting");
                hasCalledListener = true;
            }, null));
            platform.Initialize("1234567", false, false);
            platform.UnityAdsDidStart(testPlacementId);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The OnUnityAdsDidStart callback should have called");
            Assert.That(platform.IsShowing, Is.True, "IsShowing should be set to true after starting to show an ad");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ShowResult.Failed, ExpectedResult = null)]
        [TestCase(null, ShowResult.Failed, ExpectedResult = null)]
        [TestCase("randomPlacementId", ShowResult.Finished, ExpectedResult = null)]
        [TestCase(null, ShowResult.Finished, ExpectedResult = null)]
        [TestCase("randomPlacementId", ShowResult.Skipped, ExpectedResult = null)]
        [TestCase(null, ShowResult.Skipped, ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator OnFinishCallbackSetsIsShowingOnAdFinish(string testPlacementId, ShowResult testShowResult)
        {
            var hasCalledListener = false;
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            platform.Initialize("1234567", false, false);
            platform.AddListener(new UnityAdsTestListener(null, null, null, (placementId, showResult) => {
                Assert.That(placementId, Is.EqualTo(testPlacementId), "The placementId under test should have been the placement id that is returned Finishing");
                Assert.That(showResult, Is.EqualTo(testShowResult), "The showResult under test should have been the showResult that is returned Finishing");
                hasCalledListener = true;
            }));
            platform.UnityAdsDidFinish(testPlacementId, testShowResult);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The OnUnityAdsDidFinish callback should have called");
            Assert.That(platform.IsShowing, Is.False, "IsShowing should be set to false after finishing showing an ad");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ShowResult.Failed, ExpectedResult = null)]
        [TestCase("randomPlacementId", ShowResult.Skipped, ExpectedResult = null)]
        [TestCase("randomPlacementId", ShowResult.Finished, ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsDidFinishShowOptionsInvoked(string expectedPlacementId, ShowResult expectedShowResult)
        {
            var hasCalledListener = false;
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            m_NativePlatformMock.Setup(x => x.Show(It.IsAny<string>())).Callback(() => {
                platform.UnityAdsDidFinish(expectedPlacementId, expectedShowResult);
            });

            var showOptions = new ShowOptions();
            showOptions.resultCallback += result => {
                Assert.That(result, Is.EqualTo(expectedShowResult), "resultCallback showOption should match the expected value.");
                hasCalledListener = true;
            };
            platform.Show(expectedPlacementId, showOptions);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The hideCallback should have been called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomGamerSid", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator ShowOptionsGamerSid(string expectedGamerSid)
        {
            var setMetaDataCalled = false;
            var platform = new Platform.Platform(m_NativePlatformMock.Object, m_BannerMock.Object, m_CoroutineExecutorMock.Object);
            m_NativePlatformMock.Setup(x => x.SetMetaData(It.IsAny<MetaData>())).Callback<MetaData>(result => {
                Assert.That(result.category, Is.EqualTo("player"), "The category player should exist if gamerSid was stored properly");
                Assert.That(result.Get("server_id"), Is.EqualTo(expectedGamerSid), "GamerSid was not stored properly in MetaData");
                setMetaDataCalled = true;
            });

            var showOptions = new ShowOptions();
            showOptions.gamerSid += expectedGamerSid;
            platform.Show("placementId", showOptions);
            m_NativePlatformMock.Verify(x => x.SetMetaData(It.IsAny<MetaData>()), Times.Once(), "Set MetaData should have been called with the gamerSid");
            while (!setMetaDataCalled) yield return null;
            Assert.That(setMetaDataCalled, Is.True, "The SetMetaData function should have been called");
        }
    }
}
#endif
