using UnityEngine;

namespace Beautify.Universal {
    public class CameraAnimator : MonoBehaviour {
        void Update() {
            transform.Rotate(new Vector3(0, 0, Time.deltaTime * 10f));

        }
    }
}