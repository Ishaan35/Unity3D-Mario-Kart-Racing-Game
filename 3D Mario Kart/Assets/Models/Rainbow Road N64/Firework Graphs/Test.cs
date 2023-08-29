using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Test : MonoBehaviour
{
    private VisualEffect vfx;
    float particleNum;

    public AudioClip explode2;
    public AudioClip explode;
    private AudioSource sound;

    private float explodeTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        vfx = GetComponent<VisualEffect>();
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        particleNum = vfx.aliveParticleCount;


        if(particleNum > 50)
        {
            explodeTime += Time.deltaTime;
            int rand = Random.Range(0, 2);

            if (!sound.isPlaying && explodeTime < 0.8f)
            {
                sound.clip = rand > 0 ? explode : explode2;
                sound.Play();
            }
        }
        else
        {
            explodeTime = 0;
        }
    }
}
