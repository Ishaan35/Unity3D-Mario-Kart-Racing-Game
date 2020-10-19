using NUnit.Framework;

namespace UnityEngine.Advertisements.Tests
{
    public class ConfigurationTests
    {
        [Test]
        [TestCase("{\"enabled\":true,\"coppaCompliant\":true,\"gdprEnabled\":false,\"assetCaching\":\"forced\",\"projectId\":\"a635b135-8caa-4d48-8e2d-f146889f21d5\",\"placements\":[{\"id\":\"video\",\"name\":\"Video\",\"default\":true,\"allowSkip\":true,\"disableBackButton\":true,\"muteVideo\":false,\"useDeviceOrientationForVideo\":false,\"adTypes\":[\"MRAID\",\"VIDEO\",\"DISPLAY\"],\"skipInSeconds\":5,\"skipEndCardOnClose\":false,\"disableVideoControlsFade\":false,\"auctionType\":\"cpm\",\"useCloseIconInsteadOfSkipIcon\":false,\"banner\":{\"refreshRate\":30}},{\"id\":\"rewardedVideo\",\"name\":\"Rewarded Video\",\"default\":false,\"allowSkip\":false,\"disableBackButton\":true,\"muteVideo\":false,\"useDeviceOrientationForVideo\":false,\"adTypes\":[\"MRAID\",\"VIDEO\"],\"skipEndCardOnClose\":false,\"disableVideoControlsFade\":false,\"auctionType\":\"cpm\",\"useCloseIconInsteadOfSkipIcon\":false,\"banner\":{\"refreshRate\":30}}],\"organizationId\":\"2473932314656\",\"developerId\":1107051,\"properties\":\"\",\"analytics\":false,\"gamePrivacy\":{\"method\":\"legitimate_interest\"},\"ageGateLimit\":0,\"legalFramework\":\"\",\"abGroup\":0,\"optOutRecorded\":false,\"optOutEnabled\":false,\"token\":\"\",\"country\":\"\",\"gameSessionId\":\"\"}", true, "video", "rewardedVideo", false, TestName = "Basic Config")]
        public void TestValidConfiguration(string json, bool enabled, string defaultPlacement, string placementKey, bool placementValue)
        {
            var configuration = new Configuration(json);
            Assert.That(configuration, Is.Not.Null, "Configuration should not be null");
            Assert.That(configuration.enabled, Is.EqualTo(enabled), "configuration.enabled was not set properly");
            Assert.That(configuration.defaultPlacement, Is.EqualTo(defaultPlacement), "configuration.defaultPlacement was not set properly");
            Assert.That(configuration.placements.ContainsKey(placementKey), Is.EqualTo(true), "placementKey was not found in the list of placements");
            Assert.That(configuration.placements.ContainsValue(placementValue), Is.EqualTo(true), "placementValue was not found in the list of placements");
            Assert.That(configuration.placements[placementKey], Is.EqualTo(placementValue), "configuration.defaultPlacement was not set properly");
        }
    }
}
