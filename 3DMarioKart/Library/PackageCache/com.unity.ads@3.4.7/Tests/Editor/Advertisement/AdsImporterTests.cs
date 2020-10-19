#if NUGET_MOQ_AVAILABLE
using UnityEditor;
using UnityEditor.TestTools;
using NUnit.Framework;

namespace UnityEngine.Advertisements.Editor.Tests
{
    [TestFixture]
    [RequirePlatformSupport(BuildTarget.StandaloneOSX, BuildTarget.Android, BuildTarget.iOS)]
    class AdsImporterTests
    {
        private bool _isAdsEnabled;
        private BuildTarget _buildTarget;
        private BuildTargetGroup _buildTargetGroup;

        [SetUp]
        public void Init()
        {
            _isAdsEnabled = UnityEditor.Advertisements.AdvertisementSettings.enabled;
            if (!_isAdsEnabled)
            {
                Debug.Log("Temporarily Enabling Ads for tests");
                UnityEditor.Advertisements.AdvertisementSettings.enabled = true;
            }

            //Store the current test group
            _buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            _buildTarget = EditorUserBuildSettings.activeBuildTarget;
        }

        [TearDown]
        public void Cleanup()
        {
            if (!_isAdsEnabled)
            {
                Debug.Log("Ads tests complete.  Ads will now be disabled");
                UnityEditor.Advertisements.AdvertisementSettings.enabled = _isAdsEnabled;
            }

            //Restore active build target
            EditorUserBuildSettings.SwitchActiveBuildTarget(_buildTargetGroup, _buildTarget);
        }

        [Test]
        public void PlaformImportTest()
        {
            //Verify Ads IS NOT enabled for Standalone
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);

            CollectionAssert.DoesNotContain(EditorUserBuildSettings.activeScriptCompilationDefines, "UNITY_ADS");

            //Verify Ads IS enabled for iOS
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);

            CollectionAssert.Contains(EditorUserBuildSettings.activeScriptCompilationDefines, "UNITY_ADS");

            //Verify Ads IS enabled for Android
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            CollectionAssert.Contains(EditorUserBuildSettings.activeScriptCompilationDefines, "UNITY_ADS");
        }
    }
}
#endif
