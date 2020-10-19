#if NUGET_MOQ_AVAILABLE
using System;
using System.Collections;
using Moq;
using NUnit.Framework;
using UnityEngine.Advertisements.Utilities;
using UnityEngine.TestTools;

namespace UnityEngine.Advertisements.Tests
{
    public class BannerTests
    {
        private Mock<IUnityLifecycleManager> m_CoroutineExecutorMock;
        private Mock<INativeBanner> m_NativeBannerMock;

        [SetUp]
        public void Setup()
        {
            m_CoroutineExecutorMock = new Mock<IUnityLifecycleManager>(MockBehavior.Strict);
            m_NativeBannerMock = new Mock<INativeBanner>(MockBehavior.Default);

            m_CoroutineExecutorMock.Setup(x => x.Post(It.IsAny<Action>())).Callback<Action>((action) => { action?.Invoke(); });
            m_CoroutineExecutorMock.Setup(x => x.StartCoroutine(It.IsAny<IEnumerator>())).Callback<IEnumerator>(x => {
                while (x.MoveNext()) {}
            }).Returns<Coroutine>(null);
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public void GetIsLoaded(bool actual, bool expected)
        {
            m_NativeBannerMock.Setup(x => x.IsLoaded).Returns(actual);
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            Assert.That(banner.IsLoaded, Is.EqualTo(expected), "IsLoaded did not return the correct value");
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(true, true)]
        [TestCase(null, false)]
        public void GetAndSetShowAfterLoad(bool actual, bool expected)
        {
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.ShowAfterLoad = actual;
            Assert.That(banner.ShowAfterLoad, Is.EqualTo(expected), "ShowAfterLoad did not return the correct value");
        }

        [Test]
        [TestCase("loadBanner")]
        [TestCase("")]
        [TestCase(null)]
        public void NativeBannerLoad(string placementId)
        {
            m_NativeBannerMock.Setup(x => x.Load(It.IsAny<string>(), It.IsAny<BannerLoadOptions>()));
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.Load(placementId, new BannerLoadOptions());
            m_NativeBannerMock.Verify(x => x.Load(It.IsAny<string>(), It.IsAny<BannerLoadOptions>()), Times.Once(), "Banner.Load() was not called as expected");
        }

        [Test]
        [TestCase("loadBanner")]
        [TestCase("")]
        [TestCase(null)]
        public void NativeBannerShow(string placementId)
        {
            m_NativeBannerMock.Setup(x => x.Show(It.IsAny<string>(), It.IsAny<BannerOptions>()));
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.Show(placementId, new BannerOptions());
            m_NativeBannerMock.Verify(x => x.Show(It.IsAny<string>(), It.IsAny<BannerOptions>()), Times.Once(), "Banner.Show() was not called as expected");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void NativeBannerHide(bool destroy)
        {
            m_NativeBannerMock.Setup(x => x.Hide(It.IsAny<bool>()));
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.Hide();
            m_NativeBannerMock.Verify(x => x.Hide(It.IsAny<bool>()), Times.Once(), "Banner.Hide() was not called as expected");
        }

        [Test]
        [TestCase(BannerPosition.CENTER)]
        [TestCase(BannerPosition.TOP_LEFT)]
        [TestCase(BannerPosition.TOP_RIGHT)]
        [TestCase(BannerPosition.TOP_CENTER)]
        [TestCase(BannerPosition.BOTTOM_LEFT)]
        [TestCase(BannerPosition.BOTTOM_RIGHT)]
        [TestCase(BannerPosition.BOTTOM_CENTER)]
        public void NativePlatformSetPosition(BannerPosition bannerPosition)
        {
            m_NativeBannerMock.Setup(x => x.SetPosition(It.IsAny<BannerPosition>()));
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.SetPosition(bannerPosition);
            m_NativeBannerMock.Verify(x => x.SetPosition(It.IsAny<BannerPosition>()), Times.Once(), "Banner.SetPosition() was not called as expected");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerDidLoad(string expectedPlacementId)
        {
            var hasCalledListener = false;
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            var bannerLoadOptions = new BannerLoadOptions();
            bannerLoadOptions.loadCallback += () => {
                hasCalledListener = true;
            };
            banner.UnityAdsBannerDidLoad(expectedPlacementId, bannerLoadOptions);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The loadCallback should have been called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerDidLoadNullOptions(string expectedPlacementId)
        {
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.UnityAdsBannerDidLoad(expectedPlacementId, null);
            yield return null;
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Never(), "There should have been no calls to the coroutineExecutor");
        }

        [UnityTest]
        [TestCase("Some error message", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerDidError(string expectedErrorMessage)
        {
            var hasCalledListener = false;
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            var bannerLoadOptions = new BannerLoadOptions();
            bannerLoadOptions.errorCallback += (message) => {
                hasCalledListener = true;
                Assert.That(message, Is.EqualTo(expectedErrorMessage), "The error message should be: ");
            };
            banner.UnityAdsBannerDidError(expectedErrorMessage, bannerLoadOptions);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The errorCallback should have been called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("Some error message", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerDidErrorNullOptions(string expectedErrorMessage)
        {
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.UnityAdsBannerDidError(expectedErrorMessage, null);
            yield return null;
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Never(), "There should have been no calls to the coroutineExecutor");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerDidShow(string expectedPlacementId)
        {
            var hasCalledListener = false;
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            var bannerLoadOptions = new BannerOptions();
            bannerLoadOptions.showCallback += () => {
                hasCalledListener = true;
            };
            banner.UnityAdsBannerDidShow(expectedPlacementId, bannerLoadOptions);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The loadCallback should have been called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerDidShowNullOptions(string expectedPlacementId)
        {
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.UnityAdsBannerDidShow(expectedPlacementId, null);
            yield return null;
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Never(), "There should have been no calls to the coroutineExecutor");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerClick(string expectedPlacementId)
        {
            var hasCalledListener = false;
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            var bannerLoadOptions = new BannerOptions();
            bannerLoadOptions.clickCallback += () => {
                hasCalledListener = true;
            };
            banner.UnityAdsBannerClick(expectedPlacementId, bannerLoadOptions);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The clickCallback should have been called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerClickNullOptions(string expectedPlacementId)
        {
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.UnityAdsBannerClick(expectedPlacementId, null);
            yield return null;
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Never(), "There should have been no calls to the coroutineExecutor");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerDidHide(string expectedPlacementId)
        {
            var hasCalledListener = false;
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            var bannerLoadOptions = new BannerOptions();
            bannerLoadOptions.hideCallback += () => {
                hasCalledListener = true;
            };
            banner.UnityAdsBannerDidHide(expectedPlacementId, bannerLoadOptions);
            while (!hasCalledListener) yield return null;
            Assert.That(hasCalledListener, Is.True, "The hideCallback should have been called");
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Once(), "Calls should happen on the main thread and should all batched together as 1 call");
        }

        [UnityTest]
        [TestCase("randomPlacementId", ExpectedResult = null)]
        [Timeout(1000)]
        public IEnumerator UnityAdsBannerDidHideNullOptions(string expectedPlacementId)
        {
            var banner = new Banner(m_NativeBannerMock.Object, m_CoroutineExecutorMock.Object);
            banner.UnityAdsBannerDidHide(expectedPlacementId, null);
            yield return null;
            m_CoroutineExecutorMock.Verify(x => x.Post(It.IsAny<Action>()), Times.Never(), "There should have been no calls to the coroutineExecutor");
        }
    }
}
#endif
