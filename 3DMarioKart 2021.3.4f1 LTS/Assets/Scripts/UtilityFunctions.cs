using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UtilityFunctions : MonoBehaviour
{
    private PlayerSounds playersounds;
    private Player playerscript;

    private bool drifting;
    private Vector3 offset;

    [HideInInspector]
    public bool hopEnd = false;

    private RACE_MANAGER rm;



    // Start is called before the first frame update
    void Start()
    {
        playersounds = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>();
        playerscript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //cow stuff
        if(gameObject.name.IndexOf("Cow") >= 0)
        {
            StartCoroutine(cowAnimDelay());
        }

        StartCoroutine(headBoneParent());
        StartCoroutine(headBoneParentOpponent());

        disableCam();

        if(gameObject.name.IndexOf("PalmTree") >= 0)
        {
            StartCoroutine(palmTreeAnim());
        }

        rm = GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>();

        StartCoroutine(trolleyAudioSource());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.V))
        {
            drifting = true;
        }
        else
        {
            drifting = false;
        }

        camPos();
        hairAnim();
        trail();

        playerAntiGravityConstantForce();

        if(gameObject.tag == "GliderPanel" || gameObject.tag == "ColliderInAir")
        {
            if (RACE_MANAGER.RACE_COMPLETED)
            {
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    private void LateUpdate()
    {
        moustacheAnim();

    }

    public void cam_shake()
    {
        if(transform.parent.tag == "Player")
        {

            if(!GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().FrontFPCam.activeSelf)
                Camera.main.GetComponent<Animator>().SetTrigger("Shake");
        }
    }
    public void playCoinSound()
    {
        GetComponent<AudioSource>().Play();
    }
    public void playHopSound()
    {
        if (drifting)
        {
            //removed sound from here
        }

        hopStart();
    }
    public void gliderOpenflapSound()
    {
        playersounds.effectSounds[12].Play();
    }

    public void is_drifting()
    {
       GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().drifting = true;
        hopEnd = true;
    }
    public void hopStart()
    {
        hopEnd = false;
    }

    public void camPos()
    {
        if(this.gameObject.name == "Main Camera Back")
        {
            transform.localPosition = new Vector3(0, -0.66f, 2.5f);
        }
    }

    public void moustacheAnim()
    {
        if(gameObject.name == "Moustache")
        {
            

            if (playerscript.REALCURRENTSPEED > 40)
            {
                gameObject.GetComponent<Animator>().SetBool("moustaceMove", true);
            }
            else
            {
                gameObject.GetComponent<Animator>().SetBool("moustaceMove", false);
            }

            

        }    
    }
    public void hairAnim()
    {
        if(gameObject.name == "Hair")
        {
            if(playerscript.REALCURRENTSPEED > 40)
            {
                gameObject.GetComponent<Animator>().SetBool("HairMove", true);
            }
            else
            {
                gameObject.GetComponent<Animator>().SetBool("HairMove", false);
            }
        }
    }



    public void countDownNoise()
    {
        GetComponent<AudioSource>().Play();
    }
    public void goSound()
    {
        transform.GetChild(3).GetComponent<AudioSource>().Play();
    }
    public void raceStarted()
    {
        RACE_MANAGER.RACE_STARTED = true;
        if(rm.trolleySystem != null)
        {
            rm.trolleySystem.InstantiateFirstTram();
        }
    }

    public void fadeOut()
    {
        GameObject.Find("FadeInOut").GetComponent<Animator>().SetTrigger("FadeOut");
    }

    void trail()
    {
        if(gameObject.name == "Trail")
        {
            //ground normal rotation
            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;
            if (Physics.Raycast(ground, out hit, 4) && hit.normal.y > 0.5f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
                Debug.DrawRay(hit.point, hit.normal, Color.white, 20f);
            }
        }
    }



    public void startCountDown()
    {
        StartCoroutine(GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().CountDownTImerPlay());
    }
    public void disableThisCam()
    {
        gameObject.GetComponent<Camera>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);    
    }
    public void disableCourseNameUI()
    {
        StartCoroutine(disableCourseNameUIFunc());
    }
    IEnumerator disableCourseNameUIFunc()
    {
        GameObject NameUI = GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().CourseNameUI;

        //make text transparent
        for (int i = 0; i < 20; i++)
        {
            for(int j = 0; j < NameUI.transform.GetChild(0).childCount; j++)
            {
                Transform text = NameUI.transform.GetChild(0);
                text.GetChild(j).GetComponent<Text>().color -= new Color(0, 0, 0, 0.2f);
            }
            yield return new WaitForSeconds(0.001f);

        }

        //make images transparent
        for (int i = 0; i < 200; i++)
        {
            for (int j = 0; j < NameUI.transform.GetChild(1).childCount; j++)
            {
                Transform images = NameUI.transform.GetChild(1);
                images.GetChild(j).GetComponent<Image>().color -= new Color(0, 0, 0, 0.01f);
            }
            NameUI.GetComponent<Image>().color -= new Color(0, 0, 0, 0.02f);
            yield return new WaitForSeconds(0.001f);

        }


    }

    public void happy()
    {
        StartCoroutine(happyFacePlayer());
    }
    public IEnumerator happyFacePlayer()
    {
        for(int i = 0; i < 40; i++)
        {
            playerscript.MarioFace.material = playerscript.faces[3];
            yield return new WaitForSeconds(0.016f);
        }
    }

    void updatePositionUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i + 1 != GameObject.FindGameObjectWithTag("Player").GetComponent<LapCounter>().Position)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    void explode()
    {
        transform.parent.GetComponent<BlueShell>().isactive = false;
        GameObject clone = Instantiate(transform.parent.GetComponent<BlueShell>().blueExplosion, transform.position + new Vector3(0, 1, 0), transform.parent.GetComponent<BlueShell>().blueExplosion.transform.rotation);
        if(transform.parent.GetComponent<BlueShell>().who_threw_shell != transform.parent.GetComponent<BlueShell>().chase_opponent.name)
        {
            if(transform.parent.GetComponent<BlueShell>().who_threw_shell == "Mario")
            {
                GameObject.Find("Mario").GetComponent<Player>().Driver.SetTrigger("HitItem");
                if(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().Check_if_playing())
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().effectSounds[18].Play();
            }
        }

        try
        {
            rm.FrontCam.GetComponent<Animator>().SetTrigger("Shake2");
        }
        catch (Exception e)
        {

        }
        clone.GetComponent<AudioSource>().Play();
        Instantiate(transform.parent.GetComponent<BlueShell>().smoke, clone.transform.GetChild(0).position, transform.parent.GetComponent<BlueShell>().smoke.transform.rotation);
        Destroy(transform.parent.gameObject);
    }
    public void playResultSound()
    {
        GetComponent<AudioSource>().Play();
    }


    IEnumerator cowAnimDelay()
    {
        GetComponent<Animator>().enabled = false;

        int x = UnityEngine.Random.Range(0, 4);

        yield return new WaitForSeconds(x);

        GetComponent<Animator>().enabled = true;

    }

    IEnumerator headBoneParent()
    {
        yield return new WaitForSeconds(1);
        if(gameObject.name == "Bone_017")
        {
            Transform newParent = transform.parent.GetChild(1);

            transform.parent = newParent;
        }
    }
    IEnumerator headBoneParentOpponent()
    {
        yield return new WaitForSeconds(1);
        if(gameObject.name == "Head")
        {
            Transform newParent = transform.parent.GetChild(3);

            transform.parent = newParent;
        }
    }



    public void disableSet1()
    {
        GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().DisableSet1();
    }
    public void disableSet2()
    {
        GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().DisableSet2();
    }
    public void disableSet3()
    {
        GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().DisableSet3();
    }
    public void enableSets()
    {
        GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().enableAllSets();
    }

    //to make the lens flare system detect that this camera is a camera to include in the post processing effects, but immediately disable after game starts when not using.
    public void disableCam()
    {
        if(this.gameObject.name == "Main Camera" || gameObject.name == "Main Camera Back" || gameObject.name == "FPCAM")
            StartCoroutine(disableMainCam());
    }
    IEnumerator disableMainCam()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }

    public IEnumerator palmTreeAnim()
    {
        float time = UnityEngine.Random.Range(0, 1.5f);
        yield return new WaitForSeconds(time);
        GetComponent<Animator>().enabled = true;
    }

    public IEnumerator trolleyAudioSource()
    {
        if(this.gameObject.name == "TrolleyAudioSourceEntry")
        {
            yield return new WaitForSeconds(3);
            GetComponent<AudioSource>().enabled = false;
        }
    }

    void playerAntiGravityConstantForce()
    {
        if(gameObject.tag == "Player")
        {
            if (GetComponent<Player>().antiGravity)
            {
                GetComponent<ConstantForce>().enabled = true;
            }
            else
            {
                GetComponent<ConstantForce>().enabled = false;
            }
        }
    }


}
