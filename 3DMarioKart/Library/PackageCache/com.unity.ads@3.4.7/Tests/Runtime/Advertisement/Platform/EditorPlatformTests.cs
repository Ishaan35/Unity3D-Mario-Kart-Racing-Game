//using System;
//using System.Collections;
//using System.Net;
//using NUnit.Framework;
//using UnityEngine.TestTools;
//using UnityEngine.Advertisements.Enums;
//
//namespace UnityEngine.Advertisements.Tests
//{
//   class EditorPlatformTest
//   {
//      string BaseUrl = "http://127.0.0.1:8080";
//      HttpListener listener;
//
//      [SetUp]
//      public void Initialize()
//      {
//          listener = new HttpListener();
//          //Platform.s_BaseUrl = BaseUrl;
//      }
//
//
//      [Test]
//      public void PreInitializedEditorPlatformFieldsTest()
//      {
//          var platform = new Platform();
//          Assert.That(platform.isSupported, Is.True);
//          Assert.That(platform.isInitialized, Is.False);
//          Assert.That(platform.IsReady(null), Is.False);
//      }
//
//      [UnityTest]
//      [TestCase("12345", false, PlacementState.NotAvailable, true, ExpectedResult = null, TestName = "not valid placement when enable load")]
//      [TestCase("12345-default", false, PlacementState.NotAvailable, true, ExpectedResult = null, TestName = "valid placement when enable load")]
//      [TestCase(null, true, PlacementState.Ready, false, ExpectedResult = null, TestName = "null placement when disable load")]
//      [TestCase("12345-default", true, PlacementState.Ready, false, ExpectedResult = null, TestName = "valid placement when disable load")]
//      [Timeout(15000)]
//      public IEnumerator PlacementStatusTestsLoadEnabled(string placementId, bool isReadyResult, PlacementState stateResult, bool enableLoad)
//      {
//          var platform = new Platform();
//          MockRequest(BaseUrl, StaticResponse);
//          platform.Initialize("12345", true, enableLoad);
//          while (!(platform.isInitialized && platform.m_PlacementMap.Count == 3)) yield return null;
//          Assert.That(platform.IsReady(placementId), Is.EqualTo(isReadyResult));
//          Assert.That(platform.GetPlacementState(placementId), Is.EqualTo(stateResult));
//          listener.Stop();
//          listener.Close();
//      }
//
//      [UnityTest]
//      [TestCase("12345-default", true, ExpectedResult = null, TestName = "load valid placementId")]
//      [TestCase("12345", false, ExpectedResult = null, TestName = "load invalid placementId")]
//      [Timeout(15000)]
//      public IEnumerator LoadFunctionTest(string placementId, bool expectedResult)
//      {
//          var platform = new Platform();
//          MockRequest(BaseUrl, StaticResponse);
//          platform.Initialize("12345", true, true);
//          while (!(platform.isInitialized && platform.m_PlacementMap.Count == 3)) yield return null;
//          platform.Load(placementId);
//          Assert.That(platform.IsReady(placementId), Is.EqualTo(expectedResult));
//          listener.Stop();
//          listener.Close();
//      }
//
//      [UnityTest]
//      [TestCase("12345-default", false, ExpectedResult = null, TestName = "load and show valid placementId")]
//      [TestCase(null, true, ExpectedResult = null, TestName = "load and show null placementId")]
//      [TestCase("12345", false, ExpectedResult = null, TestName = "load and show invalid placementId")]
//      public IEnumerator ShowFunctionTest(string placementId, bool expectedResult)
//      {
//          var platform = new Platform();
//          MockRequest(BaseUrl, StaticResponse);
//          platform.Initialize("12345", true, true);
//          while (!(platform.isInitialized && platform.m_PlacementMap.Count == 3)) yield return null;
//          platform.Load(placementId);
//          platform.Show(placementId);
//          Assert.That(platform.IsReady(placementId), Is.EqualTo(expectedResult));
//          listener.Stop();
//          listener.Close();
//      }
//
//      [UnityTest]
//      [TestCase("12345-default", true, false, false, ExpectedResult = null, TestName = "show valid placementId without load when load enabled")]
//      [TestCase("12345-default", false, true, true, ExpectedResult = null, TestName = "show valid placementId without load when load disabled")]
//      [TestCase(null, false, true, true, ExpectedResult = null, TestName = "show null placementId without load when load disabled")]
//      [TestCase(null, true, true, true, ExpectedResult = null, TestName = "show null placementId without load when load enabled")]
//      public IEnumerator LoadShowFunctionTest(string placementId, bool enableLoad, bool isReadyResultBeforeShow, bool IsReadyAfterShow)
//      {
//          var platform = new Platform();
//          MockRequest(BaseUrl, StaticResponse);
//          platform.Initialize("12345", true, enableLoad);
//          while (!(platform.isInitialized && platform.m_PlacementMap.Count == 3)) yield return null;
//          Assert.That(platform.IsReady(placementId), Is.EqualTo(isReadyResultBeforeShow));
//          platform.Show(placementId);
//          Assert.That(platform.IsReady(placementId), Is.EqualTo(IsReadyAfterShow));
//          listener.Stop();
//          listener.Close();
//      }
//
//      protected static void StaticResponse(IAsyncResult result)
//      {
//          var listener = (HttpListener)result.AsyncState;
//          var context = listener.EndGetContext(result);
//          SendResponse(context, "{\"enabled\":true,\"coppaCompliant\":false,\"assetCaching\":\"forced\",\"projectId\":\"e26b9cd5-4fac-4635-8259-92b75fc1f241\",\"placements\":[{\"id\":\"12345-default\",\"name\":\"100_Coins\",\"default\":true,\"allowSkip\":false,\"skipEndCardOnClose\":false,\"disableVideoControlsFade\":false,\"disableBackButton\":true,\"muteVideo\":false,\"useDeviceOrientationForVideo\":false,\"adTypes\":[\"MRAID\",\"VIDEO\"],\"auctionType\":\"cpm\",\"banner\":{\"refreshRate\":30},\"useCloseIconInsteadOfSkipIcon\":false},{\"id\":\"video\",\"name\":\"Video\",\"default\":false,\"allowSkip\":true,\"skipEndCardOnClose\":false,\"disableVideoControlsFade\":false,\"disableBackButton\":true,\"muteVideo\":false,\"useDeviceOrientationForVideo\":false,\"adTypes\":[\"MRAID\",\"VIDEO\"],\"auctionType\":\"cpm\",\"banner\":{\"refreshRate\":30},\"useCloseIconInsteadOfSkipIcon\":false,\"skipInSeconds\":5},{\"id\":\"rewardedVideo\",\"name\":\"Rewarded Video\",\"default\":false,\"allowSkip\":true,\"skipEndCardOnClose\":false,\"disableVideoControlsFade\":false,\"disableBackButton\":true,\"muteVideo\":false,\"useDeviceOrientationForVideo\":false,\"adTypes\":[\"MRAID\",\"VIDEO\"],\"auctionType\":\"cpm\",\"banner\":{\"refreshRate\":30},\"useCloseIconInsteadOfSkipIcon\":false,\"skipInSeconds\":5}],\"properties\":\"h4MxeMovIhAc7/mdl10h+A/mq6DRx5EqYIu/2JZDpdJ42gJAvIQ/P2wAKvCr1L0=\",\"analytics\":false,\"organizationId\":\"2495213\",\"country\":\"US\",\"gameSessionId\":\"937bec9f-f5c0-4d4b-8413-016e8177e420\"}");
//      }
//
//      static void SendResponse(HttpListenerContext context, string responseString)
//      {
//          var response = context.Response;
//          var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
//          response.ContentLength64 = buffer.Length;
//          var output = response.OutputStream;
//          output.Write(buffer, 0, buffer.Length);
//          output.Close();
//      }
//
//      protected void MockRequest(string url, AsyncCallback callback)
//      {
//          if (!url.EndsWith("/"))
//          {
//              url += "/";
//          }
//
//          if (listener.IsListening)
//          {
//              listener.Stop();
//          }
//          listener.Prefixes.Add(url);
//          listener.Start();
//          listener.BeginGetContext(callback, listener);
//      }
//   }
//}
