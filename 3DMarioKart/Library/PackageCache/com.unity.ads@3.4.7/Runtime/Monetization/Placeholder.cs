using System;
using System.IO;
using System.Reflection;

namespace UnityEngine.Monetization
{
    [AddComponentMenu("")]
    sealed class Placeholder : MonoBehaviour
    {
        Texture2D m_LandscapeTexture;
        Texture2D m_PortraitTexture;
        Texture2D m_LandscapeAdTexture;
        Texture2D m_PortraitAdTexture;

        bool m_Showing;
        bool purchaseButtonIsClicked = false;

        string m_PlacementId;
        bool m_AllowSkip;

        internal event ShowAdStartCallback onStart;
        internal event ShowAdFinishCallback onFinish;

        static Texture2D TextureFromEmbeddedResource(string resourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream resourceStream = assembly.GetManifestResourceStream(resourceName);
                byte[] bytes = new byte[resourceStream.Length];
                resourceStream.Read(bytes, 0, (int)resourceStream.Length);

                var texture2D = new Texture2D(1, 1);

                var loadImage = typeof(Texture2D).GetMethod("LoadImage", new[] { typeof(byte[]) });
                if (loadImage != null)
                {
                    loadImage.Invoke(texture2D, new object[] { bytes });
                }
                else
                {
                    var imageConversion = Type.GetType("UnityEngine.ImageConversion, UnityEngine");
                    loadImage = imageConversion.GetMethod("LoadImage", new[] { typeof(Texture2D), typeof(byte[]), typeof(bool) });
                    loadImage.Invoke(texture2D, new object[] { texture2D, bytes, true });
                }
                return texture2D;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Awake()
        {
            m_LandscapeTexture = Resources.Load("LandscapeMon") as Texture2D;
            m_PortraitTexture = Resources.Load("PortraitMon") as Texture2D;
            m_LandscapeAdTexture = Resources.Load("Landscape") as Texture2D;
            m_PortraitAdTexture = Resources.Load("Portrait") as Texture2D;
        }

        public void Show(string placementId, bool allowSkip)
        {
            m_PlacementId = placementId;
            m_AllowSkip = allowSkip;
            m_Showing = true;
            onStart?.Invoke();
        }

        public void OnGUI()
        {
            if (!m_Showing)
            {
                return;
            }
            GUI.ModalWindow(0, new Rect(0, 0, Screen.width, Screen.height), ModalWindowFunction, "");
        }

        void OnApplicationQuit()
        {
            m_Showing = false;
        }

        void ModalWindowFunction(int id)
        {
            if (m_PlacementId == "ShowAdPlacement")
            {
                // show normal ads
                if (m_LandscapeAdTexture != null && m_PortraitAdTexture != null)
                {
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Screen.width > Screen.height ? m_LandscapeAdTexture : m_PortraitAdTexture, ScaleMode.ScaleAndCrop);
                }
                else
                {
                    GUIStyle myStyle = new GUIStyle(GUI.skin.label);
                    myStyle.alignment = TextAnchor.MiddleCenter;
                    myStyle.fontSize = 32;
                    GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "This screen would be your Ad Unit", myStyle);
                }
            }
            else
            {
                // show promo ads
                if (m_LandscapeTexture != null && m_PortraitTexture != null)
                {
                    GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Screen.width > Screen.height ? m_LandscapeTexture : m_PortraitTexture, ScaleMode.ScaleAndCrop);
                }
                else
                {
                    GUIStyle myStyle = new GUIStyle(GUI.skin.label);
                    myStyle.alignment = TextAnchor.MiddleCenter;
                    myStyle.fontSize = 32;
                    GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "This screen would be your Promo Ad Unit", myStyle);
                }

                GUIStyle purchaseStyle = new GUIStyle(GUI.skin.button);
                purchaseStyle.fontSize = 40;
                purchaseStyle.normal.textColor = Color.white;
                if (!purchaseButtonIsClicked)
                {
                    if (Screen.width > Screen.height)
                    {
                        if (GUI.Button(new Rect(Screen.width * 3 / 4 - 100, Screen.height * 3 / 4 - 100, 200, 100), "Purchase", purchaseStyle))
                        {
                            purchaseButtonIsClicked = true;
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height * 3 / 4, 200, 100), "Purchase", purchaseStyle))
                        {
                            purchaseButtonIsClicked = true;
                        }
                    }
                }
                else
                {
                    GUIStyle confirmStyle = new GUIStyle(GUI.skin.button);
                    confirmStyle.fontSize = 40;
                    confirmStyle.normal.textColor = Color.white;

                    GUIStyle cancelStyle = new GUIStyle(GUI.skin.button);
                    cancelStyle.fontSize = 40;
                    cancelStyle.normal.textColor = Color.white;

                    if (GUI.Button(new Rect(Screen.width * 3 / 4, Screen.height - 150, 200, 100), "Cancel", confirmStyle))
                    {
                        purchaseButtonIsClicked = false;
                        m_Showing = false;
                    }
                    if (GUI.Button(new Rect(Screen.width * 3 / 4 - 200, Screen.height - 150, 200, 100), "Confirm", cancelStyle))
                    {
                        purchaseButtonIsClicked = false;
                        m_Showing = false;
                    }
                }
            }

            if (m_AllowSkip && GUI.Button(new Rect(20, 20, 150, 50), "Skip"))
            {
                m_Showing = false;
                onFinish?.Invoke(ShowResult.Skipped);
            }

            if (GUI.Button(new Rect(Screen.width - 170, 20, 150, 50), "Close"))
            {
                m_Showing = false;
                onFinish?.Invoke(ShowResult.Finished);
            }
        }
    }
}
