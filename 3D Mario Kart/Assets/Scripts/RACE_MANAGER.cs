using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RACE_MANAGER : MonoBehaviour
{
    public GameObject FrontCam;
    public GameObject FrontFPCam;
    public GameObject BackCam;
    bool FPCam = false; //first person cam

    public AudioSource music;
    public AudioSource musicFast;
    bool lastLap;

    public GameObject CountDownTimer;
    public GameObject CoinCounter;
    public GameObject LapCounter;
    public GameObject MiniMap;
    public GameObject PositionCounter;
    public GameObject CourseNameUI;

    private float RaceTime = 0;


    public List<LapCounter> lapCounters = new List<LapCounter>();
    public List<LapCounter> sortedRacers = new List<LapCounter>();


    private float sortTime = 0;

    public static bool RACE_STARTED = false;
    public static bool RACE_COMPLETED = false;
    public static bool raceFinishStuff = false;

    private int lastPos;

    public GameObject spectatorSounds;
    public GameObject itemSystem;
    public ResultsUI resultsUI;
    public GameObject finishUI;

    public GameObject RedShellWarning;
    public GameObject BlueShellWarning;
    public GameObject StarWarning;
    public GameObject BulletWarning;

    private Transform player;
    public Transform Canvas;

    [Header("Sets to disable when doing the Scene Entry Camera")]
    public GameObject[] set1;
    public GameObject[] set2;
    public GameObject[] set3;


    public static Transform allPaths;

    public TrolleySystem trolleySystem;


    public static float countDownTime = 0;
    private bool startCountDownInternalTimer = false;

    [HideInInspector]
    public int currentBlueShellCount = 0;

    public int MAXLAPS = 3;

    void Awake()
    {
        Application.targetFrameRate = 60;
    }
    // Start is called before the first frame update
    void Start()
    {
        allPaths = GameObject.Find("AI PATHS").transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;


    }

    // Update is called once per frame
    void Update()
    {
        if (startCountDownInternalTimer)
        {
            countDownTime += Time.deltaTime;
        }

        if (RACE_STARTED && !RACE_COMPLETED)
        {
            RaceTime += Time.deltaTime;
            if (!music.isPlaying && !lastLap && RaceTime > 0.5f)
            {
                music.Play();
            }
            sortTime += Time.deltaTime;

            if(sortTime > 0.1f)
            {
                calculateRacerPosition();
                sortTime = 0;
            }

        }
        //camera stuff
        
        if (Input.GetKeyDown(KeyCode.Alpha1) && RACE_STARTED && !RACE_COMPLETED) //if pressed 1 and back cam is not enabled, disable front cam and enable FP cam
        {
            if (!Input.GetKey(KeyCode.B))
            {
                FPCam = true;
                FrontFPCam.SetActive(true);
                FrontCam.GetComponent<Camera>().enabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && RACE_STARTED && !RACE_COMPLETED) //if pressed 1 and back cam is not enabled, disable FrontFP cam and enable regular front cam
        {
            if (!Input.GetKey(KeyCode.B))
            {
                FPCam = false;
                FrontFPCam.SetActive(false);
                FrontCam.GetComponent<Camera>().enabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.B) && RACE_STARTED && !RACE_COMPLETED)
        {
            if (FPCam)
            {
                BackCam.SetActive(true);
                FrontFPCam.SetActive(false);
            }
            else
            {
                BackCam.SetActive(true);
                FrontCam.GetComponent<Camera>().enabled = false;
            }

        }
        if (Input.GetKeyUp(KeyCode.B) && RACE_STARTED && !RACE_COMPLETED)
        {
            if (FPCam)
            {
                FrontFPCam.SetActive(true);
                BackCam.SetActive(false);
            }
            else
            {
                BackCam.SetActive(false);
                FrontCam.GetComponent<Camera>().enabled = true;
            }

        }

        if (RACE_COMPLETED && !raceFinishStuff)
        {
            raceFinishStuff = true;
            StartCoroutine(FinishRace());
        }

        if(GameObject.FindGameObjectWithTag("Player").GetComponent<LapCounter>().LAPCOUNT == MAXLAPS && !lastLap)
        {
            lastLap = true;
            music.Stop();
            musicFast.Play();
        }
    }

    public IEnumerator CountDownTImerPlay()
    {
        GameObject.Find("FadeInOut").GetComponent<Animator>().SetTrigger("FadeIn");//fade in anim
        CoinCounter.SetActive(true);
        LapCounter.SetActive(true);
        MiniMap.SetActive(true);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().SceneEntryFinished = true; //start the engine sounds

        //make the main camera active, start it's entry animation, wait a bit before playing the audio, and then disable the sceneEntry camera, and start the countdown in 4.5 seconds
        FrontCam.SetActive(true);                                                                   
        FrontCam.GetComponent<Animator>().SetTrigger("Entry");
        yield return new WaitForSeconds(0.5f);
        FrontCam.GetComponent<AudioSource>().Play();
        GameObject.Find("SceneEntryCamera").GetComponent<Camera>().enabled = false;
        yield return new WaitForSeconds(4.5f);
        CountDownTimer.GetComponent<Animator>().SetTrigger("Timer");

        startCountDownInternalTimer = true;
    }

    public void calculateRacerPosition()
    {

        sortedRacers = new List<LapCounter>(lapCounters);
        sortedRacers.Sort(SortByScore);


        for(int i = 0; i < sortedRacers.Count; i++)
        {
            sortedRacers[i].Position = i + 1;
        }

    }
    int SortByScore(LapCounter p1, LapCounter p2)
    {
        if(p1.totalCheckpointVal != p2.totalCheckpointVal)
            return -p1.totalCheckpointVal.CompareTo(p2.totalCheckpointVal);
        else
        {
            return p1.distanceToNextCheckpoint.CompareTo(p2.distanceToNextCheckpoint);

        }
    }

    IEnumerator FinishRace()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().effectSounds[14].Play();

        if(spectatorSounds !=null)
            spectatorSounds.SetActive(false);
        itemSystem.SetActive(false);
        FrontCam.transform.parent.parent.GetComponent<AudioSource>().Stop();
        finishUI.GetComponent<Animator>().SetBool("Finish", true);
        BackCam.SetActive(false);
        FrontCam.SetActive(true);

        FrontCam.GetComponent<Camera>().enabled = true;
        FrontFPCam.SetActive(false);
        //end music based on position
        if (GameObject.FindGameObjectWithTag("Player").GetComponent<LapCounter>().Position == 1)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Driver.SetBool("FirstPlace", true);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().marioFirstPlace.Play();
            yield return new WaitForSeconds(1);

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().effectSounds[15].Play();
            yield return new WaitForSeconds(2.5f);
            FrontCam.GetComponent<Animator>().SetBool("RaceEndCam", true);
            yield return new WaitForSeconds(0.5f);
            resultsUI.createResults(sortedRacers);
            yield return new WaitForSeconds(3f);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().effectSounds[17].Play();

        }
        else if (GameObject.FindGameObjectWithTag("Player").GetComponent<LapCounter>().Position < 6)
        {
            //for now it is the same thing
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Driver.SetBool("FirstPlace", true);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().marioFirstPlace.Play();
            yield return new WaitForSeconds(1);

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().effectSounds[16].Play(); //except for this
            yield return new WaitForSeconds(2.5f);
            FrontCam.GetComponent<Animator>().SetBool("RaceEndCam", true);
            yield return new WaitForSeconds(0.5f);
            resultsUI.createResults(sortedRacers);
            yield return new WaitForSeconds(3f);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().effectSounds[17].Play();
        }
        else
        {
            //for now it is the same thing
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().Driver.SetBool("LoseAnim", true);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().MarioLose.Play();
            yield return new WaitForSeconds(1);

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().effectSounds[21].Play(); //except for this
            yield return new WaitForSeconds(2.5f);
            FrontCam.GetComponent<Animator>().SetBool("RaceEndCam", true);
            yield return new WaitForSeconds(0.5f);
            resultsUI.createResults(sortedRacers);
            yield return new WaitForSeconds(3f);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().effectSounds[22].Play(); //and this
        }

    }


    public IEnumerator warningRedShell(Transform redshell) //this function gets called by the incoming redshell once, and the function will handle that redshell alone. This way, if there are more than one, the function can be called again, referring to another redshell
    {
        GameObject warning = Instantiate(RedShellWarning, RedShellWarning.transform.position, RedShellWarning.transform.rotation);
        warning.SetActive(true);
        warning.transform.SetParent(Canvas);

        while(redshell.GetComponent<RedShell>().isactive && !RACE_COMPLETED && redshell.GetComponent<RedShell>().current_node <= player.GetComponent<ItemManager>().currentWayPoint)
        {
            Vector3 myangle = player.position - redshell.position;
            Vector3 angle = Vector3.Cross(-player.forward, myangle);
            float dir = Vector3.Dot(angle, player.up);


            Vector3 oldPos = warning.GetComponent<RectTransform>().localPosition;
            oldPos.x = 0 + dir * 10;
           

            warning.GetComponent<RectTransform>().localPosition = oldPos;


            


            yield return new WaitForSeconds(0.02f);
        }

        Destroy(warning);
    }
    public IEnumerator warningBlueShell(Transform blueshell) //this function gets called by the incoming redshell once, and the function will handle that redshell alone. This way, if there are more than one, the function can be called again, referring to another redshell
    {
        GameObject warning = Instantiate(BlueShellWarning, BlueShellWarning.transform.position, BlueShellWarning.transform.rotation);
        warning.SetActive(true);
        warning.transform.SetParent(Canvas);

        while (blueshell.GetComponent<BlueShell>().isactive == true && !RACE_COMPLETED && blueshell.GetComponent<BlueShell>().current_node <= player.GetComponent<ItemManager>().currentWayPoint && Vector3.Distance(player.position, blueshell.position) < 100)
        {
            Vector3 myangle = player.position - blueshell.position;
            Vector3 angle = Vector3.Cross(-player.forward, myangle);
            float dir = Vector3.Dot(angle, player.up);


            Vector3 oldPos = warning.GetComponent<RectTransform>().localPosition;

            oldPos.x = 0 + dir * 10;


            warning.GetComponent<RectTransform>().localPosition = oldPos;

            if (!blueshell.GetComponent<BlueShell>().isactive)
            {
                break;
            }


            yield return new WaitForSeconds(0.02f);
        }

        Destroy(warning);
    }
    public IEnumerator warningStar(Transform opponent) //this function gets called by the incoming redshell once, and the function will handle that redshell alone. This way, if there are more than one, the function can be called again, referring to another redshell
    {
        GameObject warning = Instantiate(StarWarning, StarWarning.transform.position, StarWarning.transform.rotation);
        warning.SetActive(true);
        warning.transform.SetParent(Canvas);

        while (opponent.GetComponent<OpponentItemManager>().StarPowerUp && !RACE_COMPLETED && opponent.GetComponent<LapCounter>().totalCheckpointVal <= player.GetComponent<LapCounter>().totalCheckpointVal)
        {
            Vector3 myangle = player.position - opponent.position;
            Vector3 angle = Vector3.Cross(-player.forward, myangle);
            float dir = Vector3.Dot(angle, player.up);


            Vector3 oldPos = warning.GetComponent<RectTransform>().localPosition;
            oldPos.x = 0 + dir * 10;


            warning.GetComponent<RectTransform>().localPosition = Vector3.Lerp(warning.GetComponent<RectTransform>().localPosition, oldPos, 3 * Time.deltaTime);

            if (!opponent.GetComponent<OpponentItemManager>().StarPowerUp)
            {
                break;
            }


            yield return new WaitForSeconds(0.02f);
        }

        Destroy(warning);
    }

    public IEnumerator warningBullet(Transform opponent) //this function gets called by the incoming redshell once, and the function will handle that redshell alone. This way, if there are more than one, the function can be called again, referring to another redshell
    {
        GameObject warning = Instantiate(BulletWarning, BulletWarning.transform.position, BulletWarning.transform.rotation);
        warning.SetActive(true);
        warning.transform.SetParent(Canvas);

        while (opponent.GetComponent<OpponentItemManager>().isBullet && !RACE_COMPLETED && opponent.GetComponent<LapCounter>().totalCheckpointVal <= player.GetComponent<LapCounter>().totalCheckpointVal)
        {
            Vector3 myangle = player.position - opponent.position;
            Vector3 angle = Vector3.Cross(-player.forward, myangle);
            float dir = Vector3.Dot(angle, player.up);


            Vector3 oldPos = warning.GetComponent<RectTransform>().localPosition;

            oldPos.x = 0 + dir * 10;


            warning.GetComponent<RectTransform>().localPosition = Vector3.Lerp(warning.GetComponent<RectTransform>().localPosition, oldPos, 3 * Time.deltaTime);

            if (!opponent.GetComponent<OpponentItemManager>().isBullet)
            {
                break;
            }


            yield return new WaitForSeconds(0.02f);
        }

        Destroy(warning);
    }


    public void DisableSet1()
    {
        for(int i = 0; i < set1.Length; i++)
        {
            if(set1[i].name.IndexOf("Cow") >= 0)
            {
                set1[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                set1[i].SetActive(false);
            }
        }
    }
    public void DisableSet2()
    {
        for (int i = 0; i < set1.Length; i++)
        {
            if (set1[i].name.IndexOf("Cow") >= 0)
            {
                set1[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                set1[i].SetActive(true);
            }
        }

        for (int i = 0; i < set2.Length; i++)
        {
            if (set2[i].name.IndexOf("Cow") >= 0)
            {
                set2[i].transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                set2[i].SetActive(false);
            }
        }
    }

    public void DisableSet3()
    {
        for (int i = 0; i < set2.Length; i++)
        {
            if (set2[i].name.IndexOf("Cow") >= 0)
            {
                set2[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                set2[i].SetActive(true);
            }
        }

        for (int i = 0; i < set3.Length; i++)
        {
            if (set3[i].name.IndexOf("Cow") >= 0)
            {
                set3[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                set3[i].SetActive(false);
            }
        }
    }
    public void enableAllSets()
    {
        for (int i = 0; i < set1.Length; i++)
        {
            if (set1[i].name.IndexOf("Cow") >= 0)
            {
                set1[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                set1[i].SetActive(true);
            }
        }
        for (int i = 0; i < set2.Length; i++)
        {
            if (set2[i].name.IndexOf("Cow") >= 0)
            {
                set2[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                set2[i].SetActive(true);
            }
        }
        for (int i = 0; i < set3.Length; i++)
        {
            if (set3[i].name.IndexOf("Cow") >= 0)
            {
                set3[i].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                set3[i].SetActive(true);
            }
        }
    }



}
