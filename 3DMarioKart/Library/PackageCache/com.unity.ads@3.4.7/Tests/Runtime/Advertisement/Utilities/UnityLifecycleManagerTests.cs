using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements.Utilities;

namespace UnityEngine.Advertisements.Tests
{
    public class UnityLifecycleManagerTests
    {
        private const string k_CoroutineExecutorGameObjectName = UnityLifecycleManager.gameObjectName;
        private IUnityLifecycleManager m_UnityLifecycleManager;

        private bool m_CoroutineVerification;

        [SetUp]
        public void Setup()
        {
            m_UnityLifecycleManager = new UnityLifecycleManager();
        }

        [TearDown]
        public void TearDown()
        {
            m_UnityLifecycleManager?.Dispose();
            m_UnityLifecycleManager = null;
        }

        [Test]
        public void TestReferenceCount()
        {
            using (new UnityLifecycleManager())
            {
                using (new UnityLifecycleManager())
                {
                    m_UnityLifecycleManager.Dispose();
                    Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Not.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to exists");
                }

                Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Not.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to exists");
            }
        }

        [Test]
        public void GameObjectExists()
        {
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Not.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to exists");
        }

        [Test]
        public void GameObjectDestroyed()
        {
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Not.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to exists");

            m_UnityLifecycleManager.Dispose();

            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to be destroyed");
        }

        [Test]
        public void CheckGameObject()
        {
            var gameObject = GameObject.Find(k_CoroutineExecutorGameObjectName);

            Assert.That(gameObject, Is.Not.Null);

            Assert.That(gameObject.hideFlags, Is.EqualTo(HideFlags.HideInHierarchy | HideFlags.HideInInspector));

            var components = gameObject.GetComponents<MonoBehaviour>();
            Assert.That(components.Length, Is.EqualTo(2), "Game object should have two MonoBehaviour components");

            var applicationQuitMonoBehavior = gameObject.GetComponent<CoroutineExecutor>();
            Assert.That(applicationQuitMonoBehavior, Is.Not.Null, "Unable to find ApplicationQuit MonoBehavior");
            Assert.That(applicationQuitMonoBehavior.hideFlags, Is.EqualTo(HideFlags.HideInHierarchy | HideFlags.HideInInspector));

            var coroutineExecutorMonoBehavior = gameObject.GetComponent<CoroutineExecutor>();
            Assert.That(coroutineExecutorMonoBehavior, Is.Not.Null, "Unable to find CoroutineExecutor MonoBehavior");
            Assert.That(coroutineExecutorMonoBehavior.hideFlags, Is.EqualTo(HideFlags.HideInHierarchy | HideFlags.HideInInspector));
        }

        [Test]
        public void OnlyOwnerShouldDestroyObject()
        {
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Not.Null);

            using (new UnityLifecycleManager())
            {
                Assert.That(Resources.FindObjectsOfTypeAll<CoroutineExecutor>().Length, Is.EqualTo(1));
            }

            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator GameObjectExistsWithNoComponent()
        {
            m_UnityLifecycleManager?.Dispose();
            yield return null;    //Make sure the above disposed object gets cleaned up properly
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to be destroyed");
            GameObject dummyCoroutineExecutor = new GameObject(k_CoroutineExecutorGameObjectName);
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Not.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to exists");
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName).GetComponent<CoroutineExecutor>(), Is.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to be an empty GameObject");
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName).GetComponent<ApplicationQuit>(), Is.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to be an empty GameObject");
            m_UnityLifecycleManager = new UnityLifecycleManager();
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName), Is.Not.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to exists");
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName).GetComponent<CoroutineExecutor>(), Is.Not.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to exists with a CoroutineExecutor MonoBehavior attached to the GameObject");
            Assert.That(GameObject.Find(k_CoroutineExecutorGameObjectName).GetComponent<ApplicationQuit>(), Is.Not.Null, "Expected " + k_CoroutineExecutorGameObjectName + " to exists with a ApplicationQuit MonoBehavior attached to the GameObject");
        }

        [UnityTest]
        public IEnumerator KeepsObjectWhenNewSceneLoaded()
        {
            m_UnityLifecycleManager.Dispose();
            m_UnityLifecycleManager = null;

            var scene = SceneManager.CreateScene("test");
            while (!scene.isLoaded) yield return null;

            SceneManager.SetActiveScene(scene);

            m_UnityLifecycleManager = new UnityLifecycleManager();

            yield return SceneManager.UnloadSceneAsync(scene);

            Assert.NotNull(GameObject.Find(k_CoroutineExecutorGameObjectName), "Expected " + k_CoroutineExecutorGameObjectName + " to stay when scene unloaded");
        }

        [UnityTest]
        public IEnumerator CoroutineExecutorRunsCoroutines()
        {
            m_CoroutineVerification = false;

            yield return m_UnityLifecycleManager.StartCoroutine(VerifyExecution());

            Assert.True(m_CoroutineVerification, "This variable was not properly set in the VerifyExecution coroutine");
        }

        [UnityTest]
        public IEnumerator CoroutineExecutorRunsCoroutinesAfterBeingDestroyed()
        {
            var coroutineExecutorGameObject = GameObject.Find(k_CoroutineExecutorGameObjectName);
            GameObject.DestroyImmediate(coroutineExecutorGameObject);

            m_CoroutineVerification = false;

            yield return m_UnityLifecycleManager.StartCoroutine(VerifyExecution());

            Assert.True(m_CoroutineVerification, "This variable was not properly set in the VerifyExecution coroutine");
        }

        [UnityTest]
        [Timeout(10000)]
        public IEnumerator CoroutineExecutorCanRunAction()
        {
            var hasPostedSuccessfully = false;
            m_UnityLifecycleManager.Post(() => {
                hasPostedSuccessfully = true;
            });

            while (!hasPostedSuccessfully) yield return null;
            Assert.That(hasPostedSuccessfully, Is.True, "Post was not executed successfully");
        }

        [UnityTest]
        public IEnumerator CoroutineExecutorRunsActionsAfterBeingDestroyed() {
            var coroutineExecutorGameObject = GameObject.Find(k_CoroutineExecutorGameObjectName);
            GameObject.DestroyImmediate(coroutineExecutorGameObject);

            m_CoroutineVerification = false;

            m_UnityLifecycleManager.Post(() => {
                m_CoroutineVerification = true;
            });

            yield return null;

            Assert.True(m_CoroutineVerification, "This variable was not properly set by the action run via Post");
        }

        [UnityTest]
        public IEnumerator JungleRunnerRegression()
        {
            //Destroy all GameObjects in the scene the try and run a coroutine.  Keep the PlaymodeTestsController GameObject only.
            var gameObjectList = GameObject.FindObjectsOfType<GameObject>();
            for (int i = 0; i < gameObjectList.Length; i++)
            {
                var go = gameObjectList[i];
                if (go.GetComponent("UnityEngine.TestTools.TestRunner.PlaymodeTestsController") != null)
                {
                    continue;
                }
                GameObject.DestroyImmediate(go);
            }

            m_CoroutineVerification = false;

            yield return m_UnityLifecycleManager.StartCoroutine(VerifyExecution());

            Assert.True(m_CoroutineVerification, "This variable was not properly set in the VerifyExecution coroutine");
        }

        [UnityTest]
        public IEnumerator JungleRunnerRegression_Post()
        {
            //Destroy all GameObjects in the scene the try and run a coroutine.  Keep the PlaymodeTestsController GameObject only.
            var gameObjectList = GameObject.FindObjectsOfType<GameObject>();
            for (int i = 0; i < gameObjectList.Length; i++)
            {
                var go = gameObjectList[i];
                if (go.GetComponent("UnityEngine.TestTools.TestRunner.PlaymodeTestsController") != null)
                {
                    continue;
                }
                GameObject.DestroyImmediate(go);
            }

            m_CoroutineVerification = false;

            m_UnityLifecycleManager.Post(() => {
                m_CoroutineVerification = true;
            });

            yield return null;

            Assert.True(m_CoroutineVerification, "This variable was not properly set in the VerifyExecution coroutine");
        }

        [UnityTest]
        public IEnumerator SetOnApplicationQuitCallback()
        {
            bool callbackWasCalled = false;
            m_UnityLifecycleManager.SetOnApplicationQuitCallback(() => {
                callbackWasCalled = true;
            });

            GameObject.Find(k_CoroutineExecutorGameObjectName).SendMessage("OnApplicationQuit");
            yield return null;
            Assert.That(callbackWasCalled, Is.True, "Callback was not called as expected.");
        }

        private IEnumerator VerifyExecution()
        {
            yield return null;
            m_CoroutineVerification = true;
        }
    }
}
