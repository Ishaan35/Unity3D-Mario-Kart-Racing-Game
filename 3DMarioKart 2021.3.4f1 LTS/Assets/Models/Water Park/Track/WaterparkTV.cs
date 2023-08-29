using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterparkTV : MonoBehaviour
{
    Renderer Renderer;
    public Texture[] tex;

    int currTex = 0;

    float time = 0;
    public float interval;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<Renderer>();

        Renderer.material.EnableKeyword("_EMISSION");

        Renderer.material.SetTexture("_MainTex", tex[currTex]);
        Renderer.material.mainTexture = tex[currTex];


    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(time >= interval)
        {
            time = 0;
            currTex++;
            if(currTex >= tex.Length)
            {
                currTex = 0;
            }
            Renderer.material.mainTexture = tex[currTex];
            Renderer.material.SetTexture("_EmissionMap", tex[currTex]);

        }
    }
}
