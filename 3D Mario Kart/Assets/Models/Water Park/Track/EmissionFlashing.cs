using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionFlashing : MonoBehaviour
{
    public Material mat;

    public float maxIntensity;
    public float step;
    private float intensity;
    bool intensityIncrease = true;

    public float delay;
    float time = 0;

    private int rate;


    // Start is called before the first frame update
    void Start()
    {
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.white * 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if(time >= delay)
        {
            rate++;
            if (rate > 2)
            {
                rate = 1;
            }
            if (rate == 2) //every 3 frames
            {
                if (intensity >= maxIntensity - 0.1f)
                {
                    intensityIncrease = false;
                }
                if (intensity <= 0.1)
                {
                    intensityIncrease = true;
                }

                if (intensityIncrease)
                {
                    intensity = Mathf.Lerp(intensity, maxIntensity, step * Time.deltaTime);
                }
                else
                {
                    intensity = Mathf.Lerp(intensity, 0, step * Time.deltaTime);
                }


                mat.SetColor("_EmissionColor", Color.white * intensity);
            }
        }
        

        
    }
}
