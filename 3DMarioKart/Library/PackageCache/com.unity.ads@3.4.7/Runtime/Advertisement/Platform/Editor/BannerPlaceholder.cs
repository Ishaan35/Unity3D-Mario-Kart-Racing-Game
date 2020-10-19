using System;
using UnityEngine;

namespace UnityEngine.Advertisements.Platform.Editor
{
    public class BannerPlaceholder : MonoBehaviour
    {
        public Texture2D aTexture;

        public BannerPosition BannerPosition;
        public BannerOptions BannerOptions;
        public bool IsShowing;

        private void Awake()
        {
            IsShowing = false;
            aTexture = BackgroundTexture(320, 50, Color.grey);
        }

        private void OnGUI()
        {
            if (!IsShowing) return;

            var myStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter, fontSize = 20 };

            if (GUI.Button(GetBannerRect(BannerPosition), aTexture))
            {
                BannerOptions?.clickCallback();
            }

            if (aTexture)
            {
                var bannerRect = GetBannerRect(BannerPosition);
                GUI.DrawTexture(bannerRect, aTexture, ScaleMode.ScaleToFit);
                GUI.Box(bannerRect, "This would be your banner", myStyle);
            }
        }

        public void ShowBanner(BannerPosition bannerPosition, BannerOptions bannerOptions)
        {
            BannerPosition = bannerPosition;
            BannerOptions = bannerOptions;
            IsShowing = true;
        }

        public void HideBanner()
        {
            IsShowing = false;
        }

        private static Texture2D BackgroundTexture(int width, int height, Color color)
        {
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; i++)
            {
                pix[i] = color;
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private static Rect GetBannerRect(BannerPosition position)
        {
            switch (position)
            {
                case BannerPosition.TOP_CENTER:
                    return new Rect(Screen.width / 2 - 160, 0, 320, 50);
                case BannerPosition.TOP_LEFT:
                    return new Rect(0, 0, 320, 50);
                case BannerPosition.TOP_RIGHT:
                    return new Rect(Screen.width - 320, 0, 320, 50);
                case BannerPosition.CENTER:
                    return new Rect(Screen.width / 2 - 160, Screen.height / 2 - 25, 320, 50);
                case BannerPosition.BOTTOM_CENTER:
                    return new Rect(Screen.width / 2 - 160, Screen.height - 50, 320, 50);
                case BannerPosition.BOTTOM_LEFT:
                    return new Rect(0, Screen.height - 50, 320, 50);
                case BannerPosition.BOTTOM_RIGHT:
                    return new Rect(Screen.width - 320, Screen.height - 50, 320, 50);
                default:
                    return new Rect(Screen.width / 2 - 160, Screen.height - 50, 320, 50);
            }
        }
    }
}
