using UnityEngine;
using UnityEngine.UI;
using Beautify.Universal;

namespace Beautify.Demos {

    public class Demo : MonoBehaviour {

        private void Start() {
            UpdateText();
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.J)) {
                BeautifySettings.settings.bloomIntensity.value += 0.1f;
            }
            if (Input.GetKeyDown(KeyCode.T) || Input.GetMouseButtonDown(0)) {
                BeautifySettings.settings.disabled.value = !BeautifySettings.settings.disabled.value;
                UpdateText();
            }
            if (Input.GetKeyDown(KeyCode.B)) BeautifySettings.Blink(0.2f);

            if (Input.GetKeyDown(KeyCode.C)) {
                BeautifySettings.settings.compareMode.value = !BeautifySettings.settings.compareMode.value;
            }

        }

        void UpdateText() {

            if (BeautifySettings.settings.disabled.value) {
                GameObject.Find("Beautify").GetComponent<Text>().text = "Beautify OFF";
            } else {
                GameObject.Find("Beautify").GetComponent<Text>().text = "Beautify ON";
            }

        }


    }
}
