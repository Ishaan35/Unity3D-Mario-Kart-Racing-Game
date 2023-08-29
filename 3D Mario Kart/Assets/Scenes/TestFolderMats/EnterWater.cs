using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterWater : MonoBehaviour
{
    public UnityEngine.Rendering.VolumeProfile volumeProfile;


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.name == "DEPTHWATER")
        {
            UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments;
            if (!volumeProfile.TryGet(out colorAdjustments)) throw new System.NullReferenceException(nameof(colorAdjustments));
            {
                colorAdjustments.active = true;
                RenderSettings.fog = true;
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "DEPTHWATER")
        {
            UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments;
            if (!volumeProfile.TryGet(out colorAdjustments)) throw new System.NullReferenceException(nameof(colorAdjustments));
            {
                colorAdjustments.active = false;
                RenderSettings.fog = false;
            }

        }
    }
}
