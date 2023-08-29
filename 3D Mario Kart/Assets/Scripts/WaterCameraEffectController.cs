using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class WaterCameraEffectController : MonoBehaviour
{

    public UnityEngine.Rendering.VolumeProfile volumeProfile;


    public Image waterScreen;
    public float waterScreenAlpha;

    public Material playerHat;
    public float hatSmoothness = 0;
    private float origSmoothness;

    [Header("Audio")]
    public AudioClip regAudio;
    public AudioClip underWaterAudio;
    public AudioSource audioSource;
    public float regularAudioVolume = 0;
    public float underWaterAudioVolume = 0;

    [Header("UnderwaterMarioVoiceEffects")]
    public AudioMixerGroup underWaterVoice;
    public Transform allMarioVoiceAudioSources;
    public AudioSource kartSound;
    public AudioMixerGroup kartSoundUnderwater;
    public Transform effectSounds;
    public AudioMixerGroup effectSoundUnderwater;

    [Header("Bubbles")]
    public ParticleSystem bubbleBurst;
    public ParticleSystem propellerBubbles;
    public ParticleSystem regularBubbles;


    // Start is called before the first frame update
    void Start()
    {
        playerHat.SetFloat("_Smoothness", hatSmoothness);
        origSmoothness = playerHat.GetFloat("_Smoothness");
        
    }

    // Update is called once per frame
    void Update()
    {
        playerHat.SetFloat("_Smoothness", Mathf.Lerp(playerHat.GetFloat("_Smoothness"), origSmoothness, Time.deltaTime/2));
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name == "DEPTHWATER")
        {
            UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments;
            if (!volumeProfile.TryGet(out colorAdjustments)) throw new System.NullReferenceException(nameof(colorAdjustments));
            {
                colorAdjustments.active = true;         
                Color c = waterScreen.color;
                c.a = waterScreenAlpha;
                waterScreen.color = c;
                if (!propellerBubbles.isPlaying)
                {
                    propellerBubbles.Play();
                    regularBubbles.Play();
                }


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

                Color c = waterScreen.color;
                c.a = 0;
                waterScreen.color = c;
                playerHat.SetFloat("_Smoothness", 1f);

                float time = audioSource.time;
                audioSource.clip = regAudio;
                audioSource.time = time;

                if (regularAudioVolume != 0)
                {
                    audioSource.volume = regularAudioVolume;
                }
            }
            revertMarioUnderwaterVoice();
            propellerBubbles.Stop();
            regularBubbles.Stop();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "DEPTHWATER")
        {
            float time = audioSource.time;
            audioSource.clip = underWaterAudio;
            audioSource.time = time;
            if(underWaterAudioVolume != 0)
            {
                audioSource.volume = underWaterAudioVolume;
            }
            setMarioUnderwaterVoice();
            bubbleBurst.Play();

        }
    }

    void setMarioUnderwaterVoice()
    {
        for(int i = 0; i < allMarioVoiceAudioSources.childCount; i++)
        {
            allMarioVoiceAudioSources.GetChild(i).GetComponent<AudioSource>().outputAudioMixerGroup = underWaterVoice;
        }
        kartSound.outputAudioMixerGroup = kartSoundUnderwater;
        for (int i = 0; i < effectSounds.childCount; i++)
        {
            effectSounds.GetChild(i).GetComponent<AudioSource>().outputAudioMixerGroup = effectSoundUnderwater;
        }
    }

    void revertMarioUnderwaterVoice()
    {
        for (int i = 0; i < allMarioVoiceAudioSources.childCount; i++)
        {
            allMarioVoiceAudioSources.GetChild(i).GetComponent<AudioSource>().outputAudioMixerGroup = null;
        }
        kartSound.outputAudioMixerGroup = null;
        for (int i = 0; i < effectSounds.childCount; i++)
        {
            effectSounds.GetChild(i).GetComponent<AudioSource>().outputAudioMixerGroup = null;
        }

    }


    public void SceneEntryUnderWaterView()
    {
        UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments;
        if (!volumeProfile.TryGet(out colorAdjustments)) throw new System.NullReferenceException(nameof(colorAdjustments));
        {
            colorAdjustments.active = true;
            Color c = waterScreen.color;
            c.a = waterScreenAlpha;
            waterScreen.color = c;
        }

    }

    public void SceneEntryCamExitWater()
    {
        UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments;

        if (!volumeProfile.TryGet(out colorAdjustments)) throw new System.NullReferenceException(nameof(colorAdjustments));
        {
            colorAdjustments.active = false;

            Color c = waterScreen.color;
            c.a = 0;
            waterScreen.color = c;
        }
    }
}
