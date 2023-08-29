using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Beautify.Universal {

    public delegate float OnBeforeFocusEvent(float currentFocusDistance);


    public class BeautifySettings : MonoBehaviour {

        [Header("Scene Settings")]
        public Transform sun;
        public Transform depthOfFieldTarget;

        public OnBeforeFocusEvent OnBeforeFocus;

        [NonSerialized]
        public static float depthOfFieldCurrentFocalPointDistance;

        [NonSerialized]
        public static int bloomExcludeMask = -1;

        static BeautifySettings _instance;
        static Volume _beautifyVolume;
        static Beautify _beautify;

        /// <summary>
        /// Returns a reference to the Beautify Settings component attached to the Post Processing Layer or camera
        /// </summary>
        /// <value>The instance.</value>
        public static BeautifySettings instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<BeautifySettings>();
                    if (_instance == null) {
                        // Check if there's a single volume component, then add BeautifySettings singleton to that gameobject
                        // otherwise create a new dedicated gameobject
                        _beautifyVolume = FindBeautifyVolume();
                        GameObject go;
                        if (_beautifyVolume == null) {
                            go = new GameObject("Beautify Scene Settings", typeof(BeautifySettings));
                        } else {
                            go = _beautifyVolume.gameObject;
                            _instance = go.GetComponent<BeautifySettings>();
                        }
                        if (_instance == null) {
                            _instance = go.AddComponent<BeautifySettings>();
                        }
                    }
                }
                return _instance;
            }
        }


        static Volume FindBeautifyVolume() {
            Volume[] vols = FindObjectsOfType<Volume>();
            foreach (Volume volume in vols) {
                if (volume.sharedProfile != null && volume.sharedProfile.Has<Beautify>()) {
                    _beautifyVolume = volume;
                    return volume;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a reference to the settings of Beautify in the Post Processing Profile
        /// </summary>
        /// <value>The shared settings.</value>
        public static Beautify sharedSettings {
            get {
                if (_beautify != null) return _beautify;
                if (_beautifyVolume == null) FindBeautifyVolume();
                if (_beautifyVolume == null) return null;

                bool foundEffectSettings = _beautifyVolume.sharedProfile.TryGet(out _beautify);
                if (!foundEffectSettings) {
                    Debug.Log("Cant load Beautify settings");
                    return null;
                }
                return _beautify;
            }
        }

        /// <summary>
        /// Returns a copy of the settings of Beautify in the Post Processing Profile
        /// </summary>
        /// <value>The settings.</value>
        public static Beautify settings {
            get {
                if (_beautify != null) return _beautify;
                if (_beautifyVolume == null) FindBeautifyVolume();
                if (_beautifyVolume == null) return null;

                bool foundEffectSettings = _beautifyVolume.profile.TryGet(out _beautify);
                if (!foundEffectSettings) {
                    Debug.Log("Cant load Beautify settings");
                    return null;
                }
                return _beautify;
            }
        }


        public static void Blink(float duration, float maxValue = 1) {
            if (duration <= 0)
                return;

            instance.StartCoroutine(instance.DoBlink(duration, maxValue));
        }

        IEnumerator DoBlink(float duration, float maxValue) {

            Beautify beautify = settings;
            float start = Time.time;
            WaitForEndOfFrame w = new WaitForEndOfFrame();
            beautify.vignettingBlink.overrideState = true;
            float t;
            // Close
            do {
                t = (Time.time - start) / duration;
                if (t > 1f)
                    t = 1f;
                float easeOut = t * (2f - t);
                beautify.vignettingBlink.value = easeOut * maxValue;
                yield return w;
            } while (t < 1f);

            // Open
            start = Time.time;
            do {
                t = (Time.time - start) / duration;
                if (t > 1f)
                    t = 1f;
                float easeIn = t * t;
                beautify.vignettingBlink.value = (1f - easeIn) * maxValue;
                yield return w;
            } while (t < 1f);
            beautify.vignettingBlink.overrideState = false;
        }

    }

}
