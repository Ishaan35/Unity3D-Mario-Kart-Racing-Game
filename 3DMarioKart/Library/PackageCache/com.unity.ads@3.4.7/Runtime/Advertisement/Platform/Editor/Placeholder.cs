#if UNITY_EDITOR
using System;
using UnityEngine.Advertisements.Events;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityEngine.Advertisements
{
    internal sealed class Placeholder : MonoBehaviour
    {
        private Texture2D m_LandscapeTexture;
        private Texture2D m_PortraitTexture;

        private GameObject m_LandscapeCanvas;
        private GameObject m_PortraitCanvas;

        private bool m_Showing;
        private string m_PlacementId;
        private ScreenOrientation m_CurrentScreenOrientation;

        internal event EventHandler<FinishEventArgs> OnFinish;

        private void Awake()
        {
            m_Showing = false;
            m_PlacementId = "";
            m_CurrentScreenOrientation = ScreenOrientation.Portrait;

            m_LandscapeTexture = Resources.Load("Landscape") as Texture2D;
            m_PortraitTexture = Resources.Load("Portrait") as Texture2D;

            var placeholderGameObject = new GameObject("Placeholder") { hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy };
            DontDestroyOnLoad(placeholderGameObject);
            m_LandscapeCanvas = CreateCanvas(placeholderGameObject, "Canvas(Landscape)", 1600, 1200);
            m_PortraitCanvas = CreateCanvas(placeholderGameObject, "Canvas(Portrait)", 1200, 1600);
            CreatePlaceholder(m_LandscapeCanvas, m_LandscapeTexture, 1600, 1200);
            CreatePlaceholder(m_PortraitCanvas, m_PortraitTexture, 1200, 1600);
        }

        private void Update()
        {
            if (!m_Showing) return;

            //Only update orientation if we are showing
            if (Screen.width > Screen.height)
            {
                if (m_CurrentScreenOrientation != ScreenOrientation.Landscape)
                {
                    SwapCanvas(ScreenOrientation.Landscape);
                }

                m_CurrentScreenOrientation = ScreenOrientation.Landscape;
            }
            else
            {
                if (m_CurrentScreenOrientation != ScreenOrientation.Portrait)
                {
                    SwapCanvas(ScreenOrientation.Portrait);
                }

                m_CurrentScreenOrientation = ScreenOrientation.Portrait;
            }
        }

        public void Load(string placementId) {}

        public void Show(string placementId, bool allowSkip)
        {
            m_Showing = true;
            m_PlacementId = placementId;

            if (allowSkip)
            {
                ShowSkipButton(m_LandscapeCanvas);
                ShowSkipButton(m_PortraitCanvas);
            }
            else
            {
                HideSkipButton(m_LandscapeCanvas);
                HideSkipButton(m_PortraitCanvas);
            }

            if (m_CurrentScreenOrientation == ScreenOrientation.Landscape)
            {
                m_LandscapeCanvas.SetActive(true);
            }
            else
            {
                m_PortraitCanvas.SetActive(true);
            }
        }

        private static GameObject CreateCanvas(GameObject parentGameObject, string gameObjectName, int width, int height)
        {
            var canvasGameObject = new GameObject(gameObjectName) { hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy };
            var canvas = canvasGameObject.AddComponent<Canvas>();
            var canvasScaler = canvasGameObject.AddComponent<CanvasScaler>();
            canvasGameObject.AddComponent<GraphicRaycaster>();

            canvasGameObject.transform.SetParent(parentGameObject.transform);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = Int16.MaxValue;
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = width > height ? 0.0f : 1.0f;
            canvasScaler.referenceResolution = new Vector2(width, height);

            if (EventSystem.current == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            canvasGameObject.SetActive(false);
            return canvasGameObject;
        }

        private void CreatePlaceholder(GameObject canvasGameObject, Texture2D texture, int textureWidth, int textureHeight)
        {
            CreateImage(canvasGameObject, texture, textureWidth, textureHeight);

            CreateButton(canvasGameObject, -100, -50, 150, 50, Vector2.one, Vector2.one, "Close", () => {
                m_Showing = false;
                m_LandscapeCanvas.SetActive(false);
                m_PortraitCanvas.SetActive(false);
                OnFinish?.Invoke(this, new FinishEventArgs(m_PlacementId, ShowResult.Finished));
            });

            CreateButton(canvasGameObject, 100, -50, 150, 50, Vector2.up, Vector2.up, "Skip", () => {
                m_Showing = false;
                m_LandscapeCanvas.SetActive(false);
                m_PortraitCanvas.SetActive(false);
                OnFinish?.Invoke(this, new FinishEventArgs(m_PlacementId, ShowResult.Skipped));
            });
        }

        private static void CreateButton(GameObject canvasGameObject, int x, int y, int width, int height, Vector2 anchorMin, Vector2 anchorMax, string buttonText, UnityAction onButtonClick)
        {
            var closeButtonGameObject = new GameObject(buttonText);
            closeButtonGameObject.transform.SetParent(canvasGameObject.transform);
            closeButtonGameObject.transform.localPosition = new Vector3(x, y, 0);
            var rectTransform = closeButtonGameObject.AddComponent<RectTransform>();
            var buttonComponent = closeButtonGameObject.AddComponent<Button>();
            var image = closeButtonGameObject.AddComponent<Image>();
            rectTransform.sizeDelta = new Vector2(width, height);
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            buttonComponent.onClick.AddListener(onButtonClick);
            buttonComponent.image = image;
            image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            image.type = Image.Type.Sliced;

            var buttonTextGameObject = new GameObject("Text");
            buttonTextGameObject.transform.SetParent(closeButtonGameObject.transform);
            buttonTextGameObject.transform.localPosition = Vector3.zero;
            var textComponent = buttonTextGameObject.AddComponent<Text>();
            textComponent.text = buttonText;
            textComponent.fontSize = 20;
            textComponent.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.color = Color.black;
        }

        private static void CreateImage(GameObject canvasGameObject, Texture2D texture, int width, int height)
        {
            var imageGameObject = new GameObject("Image");

            imageGameObject.transform.SetParent(canvasGameObject.transform);
            imageGameObject.transform.localPosition = Vector3.zero;

            var image = imageGameObject.AddComponent<Image>();
            var sprite = Sprite.Create(texture, Rect.MinMaxRect(0, 0, width, height), Vector2.zero);
            image.sprite = sprite;
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        }

        private static void HideSkipButton(GameObject canvasGameObject)
        {
            canvasGameObject.transform.Find("Skip").gameObject.SetActive(false);
        }

        private static void ShowSkipButton(GameObject canvasGameObject)
        {
            canvasGameObject.transform.Find("Skip").gameObject.SetActive(true);
        }

        private void SwapCanvas(ScreenOrientation newOrientation)
        {
            if (newOrientation == ScreenOrientation.Landscape)
            {
                m_LandscapeCanvas.SetActive(true);
                m_PortraitCanvas.SetActive(false);
            }
            else
            {
                m_LandscapeCanvas.SetActive(false);
                m_PortraitCanvas.SetActive(true);
            }
        }

        private void OnApplicationQuit()
        {
            m_Showing = false;
        }
    }
}
#endif
