using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector]
    public bool Boost = false;

    //movements
    public float boost_speed = 90;
    public float drift_speed;
    public float desiredMaxSpeed = 75;
    public float max_speed;
    [HideInInspector]
    public float currentspeed = 0;
    [HideInInspector]
    public float REALCURRENTSPEED;

    private Rigidbody rb;

    public float desired_rotate_strength;
    private float rotateStrengthWithStar;
    float rotate_strength;
    public Transform raycastPos;

    [HideInInspector]
    public bool grounded = false;

    //steer and direction of drift
    float direction;
    int drift_direction; //-1 is left and 1 is right

    bool drift_right = false;
    bool drift_left = false;

    // what boost will I get
    float Drift_time = 0;
    [HideInInspector]
    public float Boost_time = 0;

    //before start boost
    private float beforeStartAccelTime;

    public GameObject Boost_PS;
    public GameObject BoostBurstPS;
    public GameObject DriftPS;
    GameObject Right_Wheel_Drift_PS;
    GameObject Left_Wheel_Drift_PS;

    public LayerMask mask;

    [Header("Dust")]
    public Transform dustParticles;
    public Transform DriftDustLeft;
    public Transform DriftDustRight;
    public Transform exhaustDust;
    public Transform AccelBeforeStartDust;


    [Header("Particles and Tires")]

    //particle colors
    public Color drift1;
    public Color drift2;
    public Color drift3;
    public GameObject trails;

    //drift force stuff
    [HideInInspector]
    public bool drifting = false;

    //kart gameobjects
    public GameObject Tires;
    GameObject FrontLeftTire;
    GameObject FrontRightTire;
    float max_tire_rotation = 20;
    public GameObject[] tires;
    public GameObject steeringwheel;

    //glider stuff
    public GameObject glider;
    [HideInInspector]
    public bool GLIDER_FLY = false;
    private float glideAngleZ=0;
    private float glideAngleX = 0;
    private bool cancelAddforceDown = false; //in some areas, we want not too strong gravity
    bool glider_close_confirm = false; //to prevent  from closing to early
    float glidingTime = 0;
    bool glideTrick = false;
    float inputHorizontalSmooth = 0;

    //jump panel
    [HideInInspector]////////////////////////////////
    public bool JUMP_PANEL = false;
    private float jumpPanelUpForce = -250000;
    private float jumpPanelDownForce = 0;


    public Animator Driver;

    private PlayerSounds playersounds;

    [HideInInspector]
    public bool hasitem = false; //true when player hits itembox
    [HideInInspector]
    public bool has_item_hold = false;
    public GameObject ItemBox;
    private ItemManager item_manager;
    private LapCounter lapCounter;

    //faces
    [Header("Faces")]
    public Material[] faces;
    public Material current_face_material;
    public Renderer MarioFace;

    //shell and banana
    public bool HitByBanana_ = false;
    public bool HitByShell_ = false;
    [HideInInspector]
    public bool playSpinAnim = true;
    [HideInInspector]
    public bool invincible = false;

    //collision cooldown
    float collideCooldown = 0;

    //position variable before change
    int lastPos = 2;
    public GameObject positionUI;
    private bool showUI;

    public Transform RaceEndPath;
    [HideInInspector]
    public int currentWayPoint = 0;
    float raceEndTime = 0;

    public Transform trickParticles;

    [Header("Kart animators")]
    public RuntimeAnimatorController kartReg;

    [Header("Player Head")]
    public Transform headBone;
    [HideInInspector]
    public Transform opponentLook;
    private float lookAtTime = 2;
    private bool lookAtOpponentEyes = false;
    private GameObject[] allOpponents;


    [HideInInspector]
    public bool SpecialFace = false;
    private bool reverseSkid = false;
    private bool reversing;
    float reversingTime = 0;
    float blinkingTimeCounter = 0;



    //tricks
    [HideInInspector]
    public bool trickAvailable = false;
    [HideInInspector]
    public bool trickBoostPending = false;
    private int trickAnimCounter = 0;
    private float groundRayDist;

    public bool antiGravity = false;
    float y; //this is for gathering data for turning along the resative y axis

    public ParticleSystem groundLandParticles;

    [Header("Anti Gravity Tire effects and animation")]
    public Color antiGravityTireColor;
    public ParticleSystem[] antiGravSpin;
    private float particleSystemAntigravSpinTimer = 0;
    public Renderer[] tireRenderers;
    public Transform axels;
    public Transform[] tireArms;
    public GameObject[] TireParents;
    private Vector3[] tireLocalPositions = new Vector3[4];
    public Vector3[] antiGravityTirePositions = new Vector3[4];
    public Material kartMat;
    public Color lightDecalColor;
    public Color dirtColor;
    public bool Is_Dirt_Track;
    public bool isRainbowRoad = false;

    [Header("Water Stuff")]
    private bool IN_WATER = false;
    public Transform propeller;


    public Camerafollow Cam;



    // Start is called before the first frame update
    void Start()
    {
        kartMat.SetVector("Vector4_70BBF882", new Vector4(0, 0, 0, 0));

        rb = GetComponent<Rigidbody>();
        Right_Wheel_Drift_PS = DriftPS.transform.GetChild(0).gameObject;
        Left_Wheel_Drift_PS = DriftPS.transform.GetChild(1).gameObject;

        FrontLeftTire = Tires.transform.GetChild(0).GetChild(0).gameObject;
        FrontRightTire = Tires.transform.GetChild(1).GetChild(0).gameObject;

        playersounds = GetComponent<PlayerSounds>();
        item_manager = GetComponent<ItemManager>();
        lapCounter = GetComponent<LapCounter>();

        rotateStrengthWithStar = desired_rotate_strength + 15;

        allOpponents = GameObject.FindGameObjectsWithTag("Opponent");

        for (int i = 0; i < TireParents.Length; i++)
        {
            tireLocalPositions[i] = TireParents[i].transform.localPosition;
        }

    }

    private void Update()
    {
        if (trickAvailable && Input.GetKeyDown(KeyCode.V) && REALCURRENTSPEED > 40 && !GLIDER_FLY)
        {

            trickAvailable = false;
            trickBoostPending = true;
            StartCoroutine(trickJump());
        }
        //drift input
        if(RACE_MANAGER.RACE_STARTED && !item_manager.isBullet && !RACE_MANAGER.RACE_COMPLETED)
        {
            Drift();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (GLIDER_FLY )
        {
            groundRayDist = 3;
        }
        else if (JUMP_PANEL || antiGravity)
        {
            groundRayDist = 2;

        }
        else
        {
            groundRayDist = 2;
        }

        mario_face();

        //if the race has started, but not completed
        if (RACE_MANAGER.RACE_STARTED && !item_manager.isBullet && !RACE_MANAGER.RACE_COMPLETED)
        {
            AccelBeforeStartDust.GetChild(0).GetComponent<ParticleSystem>().Stop();
            AccelBeforeStartDust.GetChild(1).GetComponent<ParticleSystem>().Stop();
            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("StartTurbo", false);
            playersounds.effectSounds[11].Stop();

            //before start boost
            if(beforeStartAccelTime > 1 && beforeStartAccelTime < 2)
            {
                Boost_time = 1;
                playersounds.effectSounds[13].Play();
                beforeStartAccelTime = 0;
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("StartBoostTilt");
                playersounds.Mario_Boost_Sounds[3].Play();
                for (int i = 0; i < BoostBurstPS.transform.childCount; i++)
                {
                    BoostBurstPS.transform.GetChild(i).GetComponent<ParticleSystem>().Play(); //left and right included
                }
                for(int i = 0; i < 60; i++)
                {
                    if (Input.GetKey(KeyCode.Space))
                    {
                        currentspeed = Mathf.SmoothStep(currentspeed, boost_speed, 2f * Time.deltaTime);
                    }
                }
            }

            if (!GetComponent<OutOfBounds>().FellInWater && !GetComponent<OutOfBounds>().outOfBounds)
            {
                Move();
                Steer();

            }

            player_animations();
            boostEffects();
            movingCarParts();
            GroundNormalRotation();
            tireDirtEffect();



            //dust particles
            if (REALCURRENTSPEED > 30 && !drift_left && !drift_right && !GLIDER_FLY && !JUMP_PANEL && !Input.GetKey(KeyCode.S) && grounded && !item_manager.isBullet && Is_Dirt_Track)
            {
                dustParticles.GetChild(0).GetComponent<ParticleSystem>().Play();
                dustParticles.GetChild(1).GetComponent<ParticleSystem>().Play();
            }
            else
            {
                dustParticles.GetChild(0).GetComponent<ParticleSystem>().Stop();
                dustParticles.GetChild(1).GetComponent<ParticleSystem>().Stop();

            }
            
            

            exhaustDust.GetChild(0).GetComponent<ParticleSystem>().Stop();
            exhaustDust.GetChild(1).GetComponent<ParticleSystem>().Stop();


            //constant subtract of boost time
            if (Boost_time > 0)
            {
                Boost_time -= Time.deltaTime;
                max_speed = boost_speed;
                Boost = true;
                if (Boost_time <= 0)
                {
                    currentspeed = Mathf.Lerp(currentspeed, max_speed, 5 * Time.deltaTime);
                }
            }
            if (Boost_time <= 0)
            {
                Boost_time = 0;
                max_speed = desiredMaxSpeed;
                Boost = false;

            }
        }
        //if you are a bullet
        else if (item_manager.isBullet)
        {
            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            Vector3 lookat = item_manager.path.GetChild(item_manager.currentWayPoint).position;

            float dir = 0;
            if(antiGravity)
                rb.AddRelativeForce(Vector3.down * 5000 * Time.deltaTime, ForceMode.Acceleration);
            else
            {
                rb.AddForce(Vector3.down * 5000 * Time.deltaTime, ForceMode.Acceleration);
            }
            if (Physics.Raycast(ground, out hit, 10, mask))
            {
                //ground normal rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);


                //MOVE FORWARD
                currentspeed = 130;
                rb.velocity = transform.forward * currentspeed;
            }
            //angle calc
            Vector3 myangle = lookat - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);
            dir = Vector3.Dot(angle, transform.up);


            float none = 0;

            // maybe get dir, and make float y lerp to that dir value, and then rotate y axis (space.self) according to that y value or something

            //float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 2.5f * Time.deltaTime);
            y = Mathf.SmoothDamp(y, dir, ref none, 2.5f * Time.deltaTime);



            transform.Rotate(0, y / 6, 0, Space.Self);
        }
        else if(!RACE_MANAGER.RACE_STARTED && !RACE_MANAGER.RACE_COMPLETED) //before race starts
        {
            GroundNormalRotation();
            if (Input.GetKey(KeyCode.Space))
            {
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("StartTurbo", true);
                tires[2].transform.Rotate(-90 * Time.deltaTime * 75 / 5.5f, 0, 0);
                tires[3].transform.Rotate(-90 * Time.deltaTime * 75 / 5.5f, 0, 0);

                AccelBeforeStartDust.GetChild(0).GetComponent<ParticleSystem>().Play();
                AccelBeforeStartDust.GetChild(1).GetComponent<ParticleSystem>().Play();

                if (!playersounds.effectSounds[11].isPlaying)
                {
                    playersounds.effectSounds[11].Play();
                }
                beforeStartAccelTime += Time.deltaTime;

            }
            else
            {
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("StartTurbo", false);
                playersounds.effectSounds[11].Stop();
                AccelBeforeStartDust.GetChild(0).GetComponent<ParticleSystem>().Stop();
                AccelBeforeStartDust.GetChild(1).GetComponent<ParticleSystem>().Stop();
                beforeStartAccelTime = 0;
            }
        }

        if (RACE_MANAGER.RACE_STARTED && !RACE_MANAGER.RACE_COMPLETED)
            PositionUI();

        if (RACE_MANAGER.RACE_COMPLETED)
        {

            raceEndTime += Time.deltaTime;
            steerOnPath();
            moveOnPath();
            raceEndCarMoveParts();
            {
                Boost_time = 0;
                Boost = false;
                for (int i = 0; i < 2; i++)
                {
                    ParticleSystem currentboost = Boost_PS.transform.GetChild(i).GetComponent<ParticleSystem>();
                    currentboost.Stop();                         
                }
            }
            if(lapCounter.endPosition < 6)
            {
                if(raceEndTime % 4 < 0.2f)
                {
                    MarioFace.material = faces[2];
                }
                else
                {
                    MarioFace.material = faces[3];
                }

            }
            else
            {
                AnimatorStateInfo animationState = Driver.GetCurrentAnimatorStateInfo(0);
                AnimatorClipInfo[] myAnimatorClip = Driver.GetCurrentAnimatorClipInfo(0);

                float time = animationState.length;
                float progress = animationState.normalizedTime % 1;

                float currTime = time * progress;

                if(currTime < 3.25 || currTime > 4.1)
                {
                    MarioFace.material = faces[7];
                }
                else
                {
                    MarioFace.material = faces[4];
                }

            }
        }



        kartAnims();
        blinking();
        lookAtOpponent();


        if (trickBoostPending && !JUMP_PANEL)
        {
            transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z), 1f * Time.deltaTime);
        }

        


    }


    private void OnCollisionStay(Collision other)
    {
        if(other.gameObject.tag == "Ground" || other.gameObject.tag == "AntiGravity")
        {
            grounded = true;
            if (other.gameObject.tag == "Ground" || other.gameObject.tag == "Dirt")
            {
                if (JUMP_PANEL)
                {
                    JUMP_PANEL = false;
                }
            }
        }
        if(other.gameObject.tag == "Dirt")
        {
            max_speed = 30;
            grounded = false;
        }
        if(other.gameObject.tag == "Boost")
        {
            Boost_time = 2;
            grounded = true;

            Boost = true;
            max_speed = boost_speed;
            if (playersounds.Check_if_playing())
            {
                playersounds.Mario_Boost_Sounds[playersounds.sound_count].Play();
                playersounds.sound_count++;
            }

        }

    }
    private IEnumerator OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Dirt")
        {
         

            //when I hit the ground after jumping off a jump panel
            if (JUMP_PANEL)
            {
                JUMP_PANEL = false;
                playersounds.effectSounds[7].Play();
                playersounds.effectSounds[4].Play();
                playersounds.effectSounds[5].Play();
            }

            if (trickBoostPending)
            {
                if(Boost_time < 0.9f)
                {
                    Boost_time = 0.9f;
                }
                trickBoostPending = false;
                groundLandParticles.Play();
                playersounds.effectSounds[4].Play();
                playersounds.effectSounds[7].Play();
            }
        }

        if((collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Dirt") && glider_close_confirm)
        {
            if (GLIDER_FLY && !item_manager.isBullet)
            {
                playersounds.effectSounds[4].Play();
                playersounds.effectSounds[5].Play();

                

            }

            GLIDER_FLY = false;
            glideTrick = false;
            glidingTime = 0;
          
            playersounds.effectSounds[3].Stop();
            glider.GetComponent<Animator>().SetBool("GliderOpen", false);
            glider.GetComponent<Animator>().SetBool("GliderClose", true);

            for(int i = 0; i < 60; i++)
            {
                groundRayDist = 6;
                yield return new WaitForSeconds(0.01f);
            }
            groundRayDist = 1;




        }
        if (collision.gameObject.tag == "Banana")//regular banana
        {
            if(collision.gameObject.GetComponent<Banana>().lifetime > 0.2f && !item_manager.StarPowerUp && !item_manager.isBullet)
            {
                StartCoroutine(hitByBanana());
                Destroy(collision.gameObject);
            }
            if (item_manager.StarPowerUp)
            {
                Destroy(collision.gameObject);
            }
            if (item_manager.isBullet)
            {
                Destroy(gameObject);
            }
        }
       
        if(collision.gameObject.tag == "Opponent" && Vector3.Distance(transform.position, collision.transform.position) < 5)
        {
            if(collideCooldown <= 0)
            {
                collideCooldown = 0.75f;
                //angle calc
                Vector3 myangle = collision.transform.position - transform.position;
                Vector3 angle = Vector3.Cross(transform.forward, myangle);
                float dir = Vector3.Dot(angle, transform.up);

                float force;
                if(currentspeed > 50 && !item_manager.StarPowerUp){
                    force = 50000;
                }
                else
                {
                    force = 35000;
                }

                playersounds.effectSounds[10].Play();
                if (dir < 0)
                {
                    if (currentspeed > 50)
                    {
                        if (!antiGravity)
                        {
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("HitRight");
                        }
                        else
                        {
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AntigravSpinLeft");
                            playersounds.effectSounds[25].Play(); //anti gravity spin
                        }
                    }
                    if (item_manager.StarPowerUp)
                    {
                        collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.transform.right * 100000 * Time.deltaTime, ForceMode.Impulse);
                        collision.gameObject.GetComponent<OpponentItemManager>().hitByShell();
                    }
                    if (item_manager.isBullet)
                    {
                        collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.transform.right * 75000 * Time.deltaTime, ForceMode.Impulse);
                        gameObject.GetComponent<OpponentItemManager>().hitByShell();
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        force = Mathf.Lerp(force, 0, 3 * Time.deltaTime);
                        rb.AddForce(transform.right * force * Time.deltaTime, ForceMode.Impulse);
                        yield return new WaitForSeconds(0.01f);
                    }

                    
                }
                else
                {
                    if (currentspeed > 50)
                    {
                        if(!antiGravity)
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("HitLeft");
                        else
                        {
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AntigravSpinRight");
                            playersounds.effectSounds[25].Play(); //anti gravity spin
                        }
                    }
                    if (item_manager.StarPowerUp)
                    {
                        collision.gameObject.GetComponent<Rigidbody>().AddForce(collision.transform.right * 75000 * Time.deltaTime, ForceMode.Impulse);
                        collision.gameObject.GetComponent<OpponentItemManager>().hitByShell();
                    }
                    if (item_manager.isBullet)
                    {
                        collision.gameObject.GetComponent<Rigidbody>().AddForce(collision.transform.right * 75000 * Time.deltaTime, ForceMode.Impulse);
                        collision.gameObject.GetComponent<OpponentItemManager>().hitByShell();
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        force = Mathf.Lerp(force, 0, 3 * Time.deltaTime);
                        rb.AddForce(-transform.right * force * Time.deltaTime, ForceMode.Impulse);
                        yield return new WaitForSeconds(0.01f);
                    }
                    
                    
                }
                if (antiGravity)
                {
                    yield return new WaitForSeconds(0.3f);
                    if(Boost_time < 1)
                        Boost_time = 1.25f;
                }
            }
            
        }
        if (collision.gameObject.tag == "GliderPanel")
        {
            if (GLIDER_FLY)
            {
                playersounds.effectSounds[7].Play();
            }
        }
        if(collision.gameObject.tag == "fence")
        {
            
            if (!item_manager.isBullet)
            {
                rb.velocity = Vector3.zero;
                if (currentspeed > 50)
                    currentspeed -= 10;
                Vector3 oldvel = collision.contacts[0].normal;
                oldvel = oldvel.normalized;

                Vector3 newvel = oldvel * 25000;

                newvel.y = -100000 * Time.deltaTime;

                for (int i = 0; i < 60; i++)
                {
                    rb.AddForce(newvel * Time.deltaTime, ForceMode.Impulse);
                    rb.AddForce(Vector3.down * 5000 * Time.deltaTime, ForceMode.Acceleration);
                    newvel = Vector3.Lerp(newvel, Vector3.zero, 3 * Time.deltaTime);
                    yield return new WaitForSeconds(0.01f);
                }

            }
            




        }
        if((collision.gameObject.tag == "Cow" || collision.gameObject.tag == "ChainChomp") && Vector3.Distance(transform.position, collision.transform.position) < 6)
        {
            if(collideCooldown <= 0)
            {
                collideCooldown = 0.75f;
                //angle calc
                Vector3 myangle = collision.transform.position - transform.position;
                Vector3 angle = Vector3.Cross(transform.forward, myangle);
                float dir = Vector3.Dot(angle, transform.up);

                float force = 50000;

                if(dir < 0)
                {
                    force *= -1;
                }

                if(REALCURRENTSPEED > 35)
                {
                    if(!item_manager.StarPowerUp && !item_manager.isBullet)
                    {
                        if (!RACE_MANAGER.RACE_COMPLETED)
                        {
                            playersounds.effectSounds[19].Play();
                        }
                        StartCoroutine(hitByBanana());
                        for (int i = 0; i < 30; i++)
                        {
                            force = Mathf.Lerp(force, 0, 3 * Time.deltaTime);
                            rb.AddForce(transform.right * force * Time.deltaTime, ForceMode.Impulse);
                            yield return new WaitForSeconds(0.01f);
                        }
                    }
                    
                }
            }
        }
        if (collision.gameObject.tag == "Trolley" && Vector3.Distance(transform.position, collision.transform.position) < 25)
        {
            if (collideCooldown <= 0)
            {
                collideCooldown = 0.75f;

                //angle calc
                Vector3 myangle = collision.transform.position - transform.position;
                Vector3 angle = Vector3.Cross(transform.forward, myangle);
                float dir = Vector3.Dot(angle, transform.up);

                float force = 50000;

                if (dir < 0)
                {
                    force *= -1;
                }

                if (REALCURRENTSPEED > 35)
                {
                    if (!item_manager.StarPowerUp && !item_manager.isBullet)
                    {
                        if(item_manager.StarPowerUp || item_manager.isBullet)
                        {
                            StartCoroutine(collision.gameObject.GetComponent<Trolley>().hitByPowerup());
                        }
                        else
                        {
                            StartCoroutine(hitByShell());
                        }
                        if(!item_manager.StarPowerUp || item_manager.isBullet)
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                force = Mathf.Lerp(force, 0, 3 * Time.deltaTime);
                                rb.AddForce(transform.right * force * Time.deltaTime, ForceMode.Impulse);
                                yield return new WaitForSeconds(0.01f);
                            }
                        }
                        
                    }

                }
            }

        }

        if(collision.gameObject.tag == "Crate")
        {
            if(currentspeed > 5 && Boost_time <= 0) { currentspeed = 5;}

            StartCoroutine(collision.gameObject.GetComponent<Crates>().destroy());
        }

        if(collision.gameObject.tag == "Submarine")
        {

            if (collideCooldown <= 0)
            {
                collideCooldown = 0.75f;
                //angle calc
                Vector3 myangle = collision.transform.position - transform.position;
                Vector3 angle = Vector3.Cross(transform.forward, myangle);
                float dir = Vector3.Dot(angle, transform.up);

                float force;
                if (currentspeed > 50 && !item_manager.StarPowerUp)
                {
                    force = 50000;
                }
                else
                {
                    force = 35000;
                }

                if (dir < 0)
                {
                    if (currentspeed > 50)
                    {
                        if (!antiGravity)
                        {
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("HitRight");
                        }
                        else
                        {
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AntigravSpinLeft");
                            playersounds.effectSounds[25].Play(); //anti gravity spin
                        }
                    }
                    if (item_manager.StarPowerUp)
                    {
                        collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.transform.right * 100000 * Time.deltaTime, ForceMode.Impulse);
                        collision.gameObject.GetComponent<OpponentItemManager>().hitByShell();
                    }
                    if (item_manager.isBullet)
                    {
                        collision.gameObject.GetComponent<Rigidbody>().AddForce(-collision.transform.right * 75000 * Time.deltaTime, ForceMode.Impulse);
                        gameObject.GetComponent<OpponentItemManager>().hitByShell();
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        force = Mathf.Lerp(force, 0, 3 * Time.deltaTime);
                        rb.AddForce(transform.right * force * Time.deltaTime, ForceMode.Impulse);
                        yield return new WaitForSeconds(0.01f);
                    }


                }
                else
                {
                    if (currentspeed > 50)
                    {
                        if (!antiGravity)
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("HitLeft");
                        else
                        {
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AntigravSpinRight");
                            playersounds.effectSounds[25].Play(); //anti gravity spin
                        }
                    }
                    if (item_manager.StarPowerUp)
                    {
                        collision.gameObject.GetComponent<Rigidbody>().AddForce(collision.transform.right * 75000 * Time.deltaTime, ForceMode.Impulse);
                        collision.gameObject.GetComponent<OpponentItemManager>().hitByShell();
                    }
                    if (item_manager.isBullet)
                    {
                        collision.gameObject.GetComponent<Rigidbody>().AddForce(collision.transform.right * 75000 * Time.deltaTime, ForceMode.Impulse);
                        collision.gameObject.GetComponent<OpponentItemManager>().hitByShell();
                    }
                    for (int i = 0; i < 30; i++)
                    {
                        force = Mathf.Lerp(force, 0, 3 * Time.deltaTime);
                        rb.AddForce(-transform.right * force * Time.deltaTime, ForceMode.Impulse);
                        yield return new WaitForSeconds(0.01f);
                    }


                }
                if (antiGravity)
                {
                    yield return new WaitForSeconds(0.3f);
                    if (Boost_time < 1)
                        Boost_time = 1.25f;
                }
            }
        }

        
    }
    IEnumerator OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "ItemBox" && !RACE_MANAGER.RACE_COMPLETED)
        {
            item_manager.PlaySelectsound.Play();
            hasitem = true; //will trigger a method in the item manager script
            for(int i = 0; i < 5; i++)
            {

                other.transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>().Play();

            }

            //start hiding stuff
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
            for(int i = 1; i < 3; i++)
            {
                other.transform.GetChild(2).GetChild(i).GetComponent<SkinnedMeshRenderer>().enabled = false; //box
                other.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false; //question mark
            }
            other.gameObject.GetComponent<Animator>().SetBool("Enlarge", false); //reset to start process

            //re-enable
            yield return new WaitForSeconds(1);
            for (int i = 1; i < 3; i++)
            {
                other.transform.GetChild(2).GetChild(i).GetComponent<SkinnedMeshRenderer>().enabled = true; //box
                other.transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = true; //question mark
            }
            other.gameObject.GetComponent<Animator>().SetBool("Enlarge", true);  //show the item box spawning with animation, even though it was already there
            other.gameObject.GetComponent<BoxCollider>().enabled = true;


        }
        if((other.gameObject.tag == "Explosion" || other.gameObject.tag == "ChainChomp") && !item_manager.isBullet && !item_manager.StarPowerUp)
        {
            StartCoroutine(hitByBanana());
            currentspeed = 0;
        }
        if (other.gameObject.tag == "JumpPanel" && !JUMP_PANEL && !item_manager.isBullet)
        {
            jumpPanelUpForce = other.gameObject.GetComponent<JumpPanelScript>().upforce;
            jumpPanelDownForce = other.gameObject.GetComponent<JumpPanelScript>().downforce;
            JUMP_PANEL = true;
            
            Boost_time = 2;
            Boost = true;
            playersounds.effectSounds[8].Play();

            max_speed = boost_speed;
            for (int i = 0; i < BoostBurstPS.transform.childCount; i++)
            {
                BoostBurstPS.transform.GetChild(i).GetComponent<ParticleSystem>().Play(); //left and right included
            } //burst boost



            if (playersounds.Check_if_playing())
            {
                playersounds.Mario_Boost_Sounds[playersounds.sound_count].Play();
                playersounds.sound_count++;
            }



        }

        if(other.gameObject.tag == "CancelDownForce")
        {
            cancelAddforceDown = true;
        }

        if(other.gameObject.tag == "Coin")
        {
            other.GetComponent<SphereCollider>().enabled = false;
            other.transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
            other.gameObject.GetComponent<Animator>().SetBool("Spawn", false); //reset to start process
            GetComponent<ScoreCount>().COINCOUNT++;
            item_manager.coinSparkle.Play();
            playersounds.effectSounds[9].Play();

            yield return new WaitForSeconds(1.5f);

            other.GetComponent<SphereCollider>().enabled = true;
            other.transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = true;
            other.gameObject.GetComponent<Animator>().SetBool("Spawn", true); //reset to start process


        }

        //next waypoint
        if (other.transform == RaceEndPath.GetChild(currentWayPoint) && RACE_MANAGER.RACE_COMPLETED)
        {
            if (currentWayPoint == RaceEndPath.childCount - 1) //if last node, set the next node to first
            {
                currentWayPoint = 0;
            }
            else
                currentWayPoint++;


        }


        if(other.gameObject.tag == "Cow" && (item_manager.StarPowerUp || item_manager.isBullet))
        {
 
                StartCoroutine(other.gameObject.GetComponent<Cow>().hitByPowerup());
            
        }
       

        //trick collider
        if (other.gameObject.tag == "TrickCollider")
        {
            trickAvailable = true;
        }


        if (other.gameObject.tag == "AntiGravity")
        {
            StartCoroutine(antiGravityColor());

            if (other.GetComponent<CameraRotateAntigravity>().rotateCam)
            {
                Cam.rotateAmountAntigravityX = other.GetComponent<CameraRotateAntigravity>().rotAmountX;
                Cam.rotateAmountAntigravityZ = other.GetComponent<CameraRotateAntigravity>().rotAmountZ;
                item_manager.canUseBulletAntigravity = other.GetComponent<CameraRotateAntigravity>().canUseBullet;
                Cam.rotateCamAntiGravity = true;

            }
        }
        if (other.gameObject.tag == "AntiGravityFalse")
        {
                StartCoroutine(exitAntiGravity());

            if(other.gameObject.GetComponent<AntiGravityExitRotate>().rotateX || other.gameObject.GetComponent<AntiGravityExitRotate>().rotateY || other.gameObject.GetComponent<AntiGravityExitRotate>().rotateZ)
               Cam.antiGravityTimeAgo = 0;

            Cam.rotateAmountAntigravityX = 2;
            Cam.rotateAmountAntigravityZ = 0;
            Cam.rotateCamAntiGravity = false;

        }

        if (other.gameObject.name == "MarioKartSignBoard")
        {
            other.gameObject.GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(0.05f);

            other.gameObject.GetComponent<MeshCollider>().enabled = false;

            yield return new WaitForSeconds(1);
            other.gameObject.GetComponent<MeshCollider>().enabled = true;
        }
    }
    private IEnumerator OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "GliderPanel")
        {
            if(gameObject.tag == "Player")
            {
                Boost_time = 2;
                Boost = true;
                playersounds.effectSounds[8].Play();

                max_speed = boost_speed;
                for (int i = 0; i < BoostBurstPS.transform.childCount; i++)
                {
                    BoostBurstPS.transform.GetChild(i).GetComponent<ParticleSystem>().Play(); //left and right included
                } //burst boost



                if (playersounds.Check_if_playing())
                {
                    playersounds.Mario_Boost_Sounds[playersounds.sound_count].Play();
                    playersounds.sound_count++;
                }

                if (GLIDER_FLY)
                {
                    float force = 20000;
                    for (int i = 0; i < 60; i++)
                    {
                        rb.AddForce(Vector3.up * force * Time.deltaTime, ForceMode.Impulse);
                        force -= 300;
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
     

        }
        if(other.gameObject.tag == "GliderPanelFly")
        {
            glideAngleZ = other.GetComponent<GetGlideAngle>().glideAngle;
            glideAngleX = other.GetComponent<GetGlideAngle>().glideAngleX;
            
            Cam.glideAngleZ = other.GetComponent<GetGlideAngle>().glideAngle;
            Cam.glideAngleX = other.GetComponent<GetGlideAngle>().glideAngleX;

            //we can change camera rotations during gliding, but only do the initiation once
            if (!GLIDER_FLY && other.gameObject.name != "GlidingTriggers")
            {
                if (currentspeed <= 60)
                {
                    glider.GetComponent<Animator>().SetBool("GliderOpen", true);
                    glider.GetComponent<Animator>().SetBool("GliderClose", false);

                    GLIDER_FLY = true;
                    glider_close_confirm = false;
                     playersounds.effectSounds[2].Play();
                }



                drift_direction = 0;
                drift_left = false;
                drift_right = false;


                //reset everything
                Drift_time = 0;
                //stop particles
                for (int i = 0; i < 5; i++)
                {
                    ParticleSystem DriftPS = Right_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //right wheel particles
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;

                    ParticleSystem DriftPS2 = Left_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //left wheel particles
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

                    DriftPS.Stop();
                    DriftPS2.Stop();
                }
                Drift_time = 0;
                drift_right = false;
                drift_left = false;

                //trick
                if (currentspeed > 60)
                {
                    GLIDER_FLY = true;
                    glideTrick = true;
                    int rand = Random.Range(0, 2);
                    if (rand == 0)
                    {
                        transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("Glide1");
                    }
                    else
                    {
                        transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("Glide2");
                    }
                    playersounds.effectSounds[2].Play();

                    yield return new WaitForSeconds(0.45f);

                    glider.GetComponent<Animator>().SetBool("GliderOpen", true);
                    glider.GetComponent<Animator>().SetBool("GliderClose", false);
                    glider_close_confirm = false;

                    yield return new WaitForSeconds(0.35f);
                    Boost_time = 0.5f;
                    Boost = true;
                    max_speed = boost_speed;
                    Driver.SetTrigger("GliderTrick");
                    for (int i = 0; i < trickParticles.childCount; i++)
                    {
                        trickParticles.GetChild(i).GetComponent<ParticleSystem>().Play();
                    }
                    for (int i = 0; i < BoostBurstPS.transform.childCount; i++)
                    {
                        BoostBurstPS.transform.GetChild(i).GetComponent<ParticleSystem>().Play(); //left and right included
                    } //burst boost


                    //sounds
                    if (playersounds.Check_if_playing() && !playersounds.Mario_Glider.isPlaying && !RACE_MANAGER.RACE_COMPLETED)
                    {
                        playersounds.Mario_Glider.Play();
                    }
                    //happy face
                    for (int i = 0; i < 45; i++)
                    {
                        SpecialFace = true;
                        current_face_material = faces[3];
                        yield return new WaitForSeconds(0.016f);
                    }
                    current_face_material = faces[0];
                    SpecialFace = false;
                }

                yield return new WaitForSeconds(1f);
                glider_close_confirm = true; //ensures glider does not instantly close in any scenario a glitch might happen
                if (GLIDER_FLY)
                    playersounds.effectSounds[3].Play();
            }

        }
        if (other.gameObject.tag == "CancelDownForce")
        {
            cancelAddforceDown = false;
        }
        if (other.gameObject.tag == "TrickCollider")
        {
            trickAvailable = false;
        }

        if (other.gameObject.name == "DEPTHWATER")
        {
            IN_WATER = false;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "ColliderInAir")
        {
            if((!RACE_MANAGER.RACE_COMPLETED) || (RACE_MANAGER.RACE_COMPLETED && other.gameObject.GetComponent<colliderInAir>().isForRaceEnd))
            {
                if (GLIDER_FLY && !other.GetComponent<colliderInAir>().isONLYforRaceEnd)
                {
                    if (other.GetComponent<colliderInAir>().relativeForce)
                    {
                        rb.AddRelativeForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.Acceleration);
                    }
                    else
                    {
                        rb.AddForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.Acceleration);
                    }
                }
                else if (antiGravity)
                {
                    rb.AddRelativeForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.Acceleration);
                }
            }
            if (RACE_MANAGER.RACE_COMPLETED)
            {
                if (GLIDER_FLY)
                {
                    if (other.GetComponent<colliderInAir>().isONLYforRaceEnd)
                    {
                        if (other.GetComponent<colliderInAir>().relativeForce)
                        {
                            rb.AddRelativeForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.VelocityChange);
                        }
                        else
                        {
                            rb.AddForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.VelocityChange);
                        }
                    }
                }
            }
            
            

        }
        if(other.gameObject.tag == "CancelDownForce")
        {
            Vector3 oldvel = rb.velocity;
            oldvel.y *= 0.98f;
            rb.velocity = oldvel;
        }

        if (other.gameObject.tag == "AntiGravity")
        {
            antiGravity = true;
        }
        if (other.gameObject.tag == "AntiGravityFalse")
        {
            antiGravity = false;

            if (other.gameObject.GetComponent<AntiGravityExitRotate>().rotateX){
                transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(other.gameObject.GetComponent<AntiGravityExitRotate>().newRotation.x, transform.eulerAngles.y, transform.eulerAngles.z), 1 * Time.deltaTime);
            }
            if (other.gameObject.GetComponent<AntiGravityExitRotate>().rotateZ)
            {
                transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, other.gameObject.GetComponent<AntiGravityExitRotate>().newRotation.z), 3 * Time.deltaTime);
            }
            if (other.gameObject.GetComponent<AntiGravityExitRotate>().rotateY)
            {
                transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, other.gameObject.GetComponent<AntiGravityExitRotate>().newRotation.y, transform.eulerAngles.z), 3 * Time.deltaTime);
            }
        }

        if(other.gameObject.name == "DEPTHWATER")
        {
            IN_WATER = true;

            //velocity decrease gravity
            if (!antiGravity)
            {
                Vector3 vel = transform.InverseTransformDirection(rb.velocity);
                vel.y *= 0.91f;
                rb.velocity = transform.TransformDirection(vel);
            }
            
        }

    }

    void Move()
    {
        REALCURRENTSPEED = transform.InverseTransformDirection(rb.velocity).z;

        collideCooldown -= Time.deltaTime;
        if(!GLIDER_FLY && !JUMP_PANEL && !cancelAddforceDown)
        {
            if(antiGravity)
                rb.AddRelativeForce(Vector3.down * 5000 * Time.deltaTime, ForceMode.Acceleration);
            else
            {
                rb.AddForce(Vector3.down * 5000 * Time.deltaTime, ForceMode.Acceleration);
            }
        }
        //input speed into velocity
        Vector3 velocity = transform.forward * currentspeed;
        if(velocity.y > rb.velocity.y )
        {
            if(!antiGravity)
                velocity.y = rb.velocity.y;

        }
        rb.velocity = velocity;

        if (antiGravity)
        {
            rb.AddRelativeForce(Physics.gravity * 5, ForceMode.Acceleration);
        }

        if(GLIDER_FLY)
        {
            Vector3 newVel = rb.velocity;
            if (!Input.GetKey(KeyCode.DownArrow))
            {
                newVel.y *= 0.75f;
            }
            else
            {
                newVel.y *= 0.45f;
            }
            rb.velocity = newVel;
        }
        if (GLIDER_FLY)
        {
            if (!Input.GetKey(KeyCode.Space))
            {
                currentspeed = Mathf.Lerp(currentspeed, 65, 2 * Time.deltaTime);
            }
            else
            {
                currentspeed = Mathf.Lerp(currentspeed, max_speed, 2 * Time.deltaTime);
            }
        }

        //accelerate
        if (Input.GetKey(KeyCode.Space))
        {
            currentspeed = Mathf.Lerp(currentspeed, max_speed, 0.5f * Time.deltaTime);
            if(!drift_right && !drift_left && (!item_manager.StarPowerUp || drift_left || drift_right))
                rotate_strength = desired_rotate_strength;
            if (item_manager.StarPowerUp && !drift_right && !drift_left)
            {
                rotate_strength = rotateStrengthWithStar;
            }
        }
        //reverse
        if (Input.GetKey(KeyCode.S) && !GLIDER_FLY)
        {
            currentspeed = Mathf.Lerp(currentspeed, -max_speed / 1.6f, 0.03f);
            if(REALCURRENTSPEED <= 0)
            {
                rotate_strength = 120;
            }
            
        }
        //slowdown by itself
        if (!Input.GetKey(KeyCode.Space))
        {
            currentspeed = Mathf.Lerp(currentspeed, 0, 0.01f);
            drift_right = false;
            drift_left = false;
            drift_direction = 0;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0, 0), 0.4f);
        }

        //speed on track vs offtrack
        if (!grounded && !Boost && !item_manager.StarPowerUp)
        {
            max_speed = 30;
            if (Input.GetKey(KeyCode.Space))
            {
                currentspeed = Mathf.Lerp(currentspeed, max_speed, 3 * Time.deltaTime);
            }

        }
        if (!grounded && Boost)
        {
            max_speed = boost_speed;
            currentspeed = boost_speed;
        }
        if (grounded && !Boost && !item_manager.StarPowerUp)
        {
            max_speed = desiredMaxSpeed;
        }
        if (grounded && Boost)
        {
            max_speed = boost_speed;
        }
        if (item_manager.StarPowerUp)
        {
            max_speed = boost_speed - 5;
            if (Input.GetKey(KeyCode.Space))
            {
                currentspeed = Mathf.Lerp(currentspeed, max_speed, 3 * Time.deltaTime);
            }
        }

        //glider movements
        if (GLIDER_FLY)
        {
            gliderMovements();
        }
        if (JUMP_PANEL)
        {
            rb.velocity = transform.forward * currentspeed;
            jumpPanelUpForce = Mathf.Lerp(jumpPanelUpForce, jumpPanelDownForce, 2.5f * Time.deltaTime);
            rb.AddRelativeForce(Vector3.down * jumpPanelUpForce * Time.deltaTime, ForceMode.Acceleration);
            rb.AddForce(transform.forward * 60000 * Time.deltaTime, ForceMode.Acceleration);


            //Camera.main.transform.parent.parent.localEulerAngles += new Vector3(0.5f, 0, 0);
        }


        if((Input.GetKey(KeyCode.Space) && REALCURRENTSPEED < 0) || (currentspeed > 40 && REALCURRENTSPEED <= 5 && Input.GetKey(KeyCode.Space)))//skid effect
        {
            reverseSkid = true;    
        }

        if (reverseSkid && REALCURRENTSPEED < 20 && Input.GetKey(KeyCode.Space) && !IN_WATER)
        {
            AccelBeforeStartDust.GetChild(0).GetComponent<ParticleSystem>().Play();
            AccelBeforeStartDust.GetChild(1).GetComponent<ParticleSystem>().Play();
            if (!playersounds.effectSounds[20].isPlaying)
            {
                playersounds.effectSounds[20].Play();
            }
            if(GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().FrontCam.activeSelf)
                GameObject.Find("Main Camera").GetComponent<Animator>().SetBool("Vibrate", true);
        }
        else
        {
            reverseSkid = false;
            AccelBeforeStartDust.GetChild(0).GetComponent<ParticleSystem>().Stop();
            AccelBeforeStartDust.GetChild(1).GetComponent<ParticleSystem>().Stop();
            playersounds.effectSounds[20].Stop();
            if(!Input.GetKey(KeyCode.B) && GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().FrontCam.activeSelf)
                GameObject.Find("Main Camera").GetComponent<Animator>().SetBool("Vibrate", false);

        }

        //reverse face and animation
        if(Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.Space) && REALCURRENTSPEED < 0 && !SpecialFace)
        {
            reversing = true;
            Driver.SetBool("Reverse", true);
            reversingTime += Time.deltaTime;
        }
        else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyDown(KeyCode.Space)){
            current_face_material = faces[0];
            Driver.SetBool("Reverse", false);
            reversing = false;
            reversingTime = 0;
        }
        if(reversing && !SpecialFace)
        {
            if(reversingTime % 2 > 1.9)
            {
                current_face_material = faces[2];
            }
            else
            {
                current_face_material = faces[1];
            }
        }

 

        if (HitByBanana_)
        {
            if (playSpinAnim && !invincible)
            {
                invincible = true;
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("BananaHit");
                playSpinAnim = false;
            }
            currentspeed = Mathf.Lerp(currentspeed, 0, 4.5f * Time.deltaTime);
        }
        if (HitByShell_)
        {
            if (playSpinAnim && !invincible)
            {
                invincible = true;
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("ShellHit");
                playSpinAnim = false;
            }
            currentspeed = Mathf.Lerp(currentspeed, 0, 4.5f * Time.deltaTime);
        }

        if(transform.GetChild(0).GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(2).IsName("SpinLeft") || transform.GetChild(0).GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(2).IsName("SpinRight") && particleSystemAntigravSpinTimer < 0.95f){
            particleSystemAntigravSpinTimer += Time.deltaTime; //this timer thing is limited to 0.95 seconds because when the animation returns to original state, the trail gets weird
            if (!antiGravSpin[0].isPlaying && antiGravSpin.Length > 0)
            {
                antiGravSpin[0].Play();
                antiGravSpin[1].Play();
            }
        }
        else
        {
            if(antiGravSpin.Length > 0)
            {
                antiGravSpin[0].Stop();
                antiGravSpin[1].Stop();
            }

        }
        if(particleSystemAntigravSpinTimer > 1.01f)
        {
            particleSystemAntigravSpinTimer = 0;
        }
    }
    void Steer()
    {
        float force = 50000;

        //steer
        if (Input.GetAxis("Horizontal") != 0)
        {
            

            //turning mechanism if drifting
            if (drift_right && !drift_left)
            {
                direction = Input.GetAxis("Horizontal") > 0 ? 2.1f : 0.5f;
                transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 20f, 0), 8f * Time.deltaTime);
                max_speed = desiredMaxSpeed - 10;

                //force
                if (drifting)
                {
                    rb.AddForce(-transform.right * force * Time.deltaTime, ForceMode.Acceleration);
                }
                if(IN_WATER && !antiGravity)
                {
                    transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("UnderWaterDriftRight", true);
                    transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("UnderWaterDriftLeft", false);
                }

            }
            if (drift_left && !drift_right)
            {
                direction = Input.GetAxis("Horizontal") < 0 ? -2.1f : -0.5f;
                transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, -20f, 0), 8f * Time.deltaTime);
                max_speed = desiredMaxSpeed - 10;

                if (drifting)
                {
                    rb.AddForce(transform.right * force * Time.deltaTime, ForceMode.Acceleration);
                }
                if (IN_WATER && !antiGravity)
                {
                    transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("UnderWaterDriftRight", false);
                    transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("UnderWaterDriftLeft", true);
                }

            }


            
            //how strong the player can turn
            float speed_rotate_rate = 0;
            if (drift_left || drift_right)
                speed_rotate_rate = 1.2f;

            if (REALCURRENTSPEED > 10 && REALCURRENTSPEED < 40 && !drift_right && !drift_left)
                speed_rotate_rate = 1.3f;

            if(REALCURRENTSPEED < 10 && REALCURRENTSPEED > 3 && !drift_right && !drift_left)
            {
                speed_rotate_rate = 0.5f;
            }

            if (REALCURRENTSPEED >= 40 && !drift_right && !drift_left)
                speed_rotate_rate = 1.75f;

            if (REALCURRENTSPEED < -5 && !Input.GetKey(KeyCode.Space)) //reverse
                speed_rotate_rate = -0.5f;


            //final rotations
            //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + rotate_strength * direction * speed_rotate_rate, transform.localEulerAngles.z), 1f * Time.deltaTime); //which direction to rotate kart 

            if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
                transform.Rotate(new Vector3(0, rotate_strength * direction * speed_rotate_rate * 0.025f, 0), Space.Self); //changed to suit anti-gravity contitions from the euler angle method
        }

        if (!drift_right && !drift_left) //no drift
        {
            direction = Input.GetAxis("Horizontal") > 0 ? 1 : -1; //-1 = left, 1 = right
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0, 0), 8f * Time.deltaTime);
            max_speed = desiredMaxSpeed;


                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("UnderWaterDriftRight", false);
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("UnderWaterDriftLeft", false);
        }
        if (!IN_WATER || antiGravity)
        {
            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("UnderWaterDriftRight", false);
            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("UnderWaterDriftLeft", false);
        }
    }
    void Drift()
    {

        Ray ground = new Ray(raycastPos.position, -transform.up);
        RaycastHit hit;

        bool onGround = Physics.Raycast(ground, out hit, 1, mask) && (hit.normal.y > 0.5f || antiGravity);
        

        //drift hop
        if (Input.GetKeyDown(KeyCode.V) && ! GLIDER_FLY && ! JUMP_PANEL && onGround)
        {
            transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Drift");

            if (!transform.GetChild(0).GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Shake"))
            {
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("Shake");
            }

            playersounds.effectSounds[6].Play();
            if (direction > 0)
            {
                drift_direction = 1;
            }
            if (direction < 0)
            {
                drift_direction = -1;
            }
            rotate_strength = 5;

        }

        //while v is pressed, you are drifting
        if (Input.GetKey(KeyCode.V) && grounded && currentspeed > 40 && Input.GetAxis("Horizontal") != 0 && ! GLIDER_FLY && !JUMP_PANEL && !HitByBanana_ && !HitByShell_)
        {
            rotate_strength = Mathf.Lerp(rotate_strength, desired_rotate_strength, 3 * Time.deltaTime);
            Drift_time += Time.deltaTime;


            if (drift_direction == -1)
            {
                drift_right = false;
                drift_left = true;
                if (!playersounds.effectSounds[0].isPlaying)
                {
                    playersounds.effectSounds[0].PlayDelayed(0.25f); //drift sound steering
                    if (isRainbowRoad)
                    {
                        if (!playersounds.effectSounds[26].isPlaying)
                        {
                            playersounds.effectSounds[26].PlayDelayed(0.25f); //drift on rainbow road
                        }
                    }
                }
            }
            if (drift_direction == 1)
            {
                drift_right = true;
                drift_left = false;
                if (!playersounds.effectSounds[0].isPlaying)
                {
                    playersounds.effectSounds[0].PlayDelayed(0.25f); //drift sound steering
                    if (isRainbowRoad)
                    {
                        if (!playersounds.effectSounds[26].isPlaying)
                            playersounds.effectSounds[26].PlayDelayed(0.25f); //drift on rainbow road
                    }
                }
            }

            //drift dust particles
            if(Drift_time < 1f && drifting && !IN_WATER) //drifting bool starts from animator when player hop touches ground
            {
                if (drift_left)
                {
                    DriftDustLeft.GetChild(0).GetComponent<ParticleSystem>().Play();
                    DriftDustLeft.GetChild(1).GetComponent<ParticleSystem>().Play();
                }
                else if (drift_right)
                {
                    DriftDustRight.GetChild(0).GetComponent<ParticleSystem>().Play();
                    DriftDustRight.GetChild(1).GetComponent<ParticleSystem>().Play();
                }
            }
            else
            {
                DriftDustLeft.GetChild(0).GetComponent<ParticleSystem>().Stop();
                DriftDustLeft.GetChild(1).GetComponent<ParticleSystem>().Stop();
                DriftDustRight.GetChild(0).GetComponent<ParticleSystem>().Stop();
                DriftDustRight.GetChild(1).GetComponent<ParticleSystem>().Stop();
            }

            //particles and tire colour
            if (Drift_time >= 1.5 && Drift_time < 4)
            {

                for (int i = 0; i < 5; i++)
                {
                    ParticleSystem DriftPS = Right_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //right wheel particles
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;

                    ParticleSystem DriftPS2 = Left_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //left wheel particles
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;
                    PSMAIN.startColor = drift1;
                    PSMAIN2.startColor = drift1;

                    if (!DriftPS.isPlaying && !DriftPS2.isPlaying)
                    {
                        DriftPS.Play();
                        DriftPS2.Play();
                    }

                }
                if (!playersounds.effectSounds[1].isPlaying)
                    playersounds.effectSounds[1].Play();
            }
            if (Drift_time >= 4 && Drift_time < 7)
            {
                if (Drift_time >= 4 && Drift_time <= 4.05f)
                    playersounds.effectSounds[1].Stop();

                //drift color particles
                for (int i = 0; i < 5; i++)
                {
                    ParticleSystem DriftPS = Right_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;
                    ParticleSystem DriftPS2 = Left_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;
                    PSMAIN.startColor = drift2;
                    PSMAIN2.startColor = drift2;


                }
                if (!playersounds.effectSounds[1].isPlaying)
                    playersounds.effectSounds[1].Play();

            }
            if (Drift_time >= 7)
            {
                if (Drift_time >= 7 && Drift_time <= 7.05f)
                    playersounds.effectSounds[1].Stop();

                for (int i = 0; i < 5; i++)
                {

                    ParticleSystem DriftPS = Right_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;
                    ParticleSystem DriftPS2 = Left_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;
                    PSMAIN.startColor = drift3;
                    PSMAIN2.startColor = drift3;

                }
                if (!playersounds.effectSounds[1].isPlaying)
                    playersounds.effectSounds[1].Play();
            }



        }
        else if(currentspeed < 40)
        {
            drift_left = false;
            drift_right = false;
            Drift_time = 0;
        }


        //no more drift
        if (!Input.GetKey(KeyCode.V)) //if not drifting, or drifting without direction
        {

            drifting = false;
            playersounds.effectSounds[0].Stop();
            playersounds.effectSounds[1].Stop();
            if (isRainbowRoad)
            {
                playersounds.effectSounds[25].Stop(); //drift on rainbow road
            }

            drift_direction = 0;
            drift_left = false;
            drift_right = false;

            //different boost times to get different boost durations
            if (Drift_time > 1.5 && Drift_time < 4)
            {
                Boost = true;
                Boost_time = 0.75f;
                if (playersounds.Check_if_playing())
                {
                    playersounds.Mario_Boost_Sounds[playersounds.sound_count].Play();
                    playersounds.sound_count++;
                }
                for(int i = 0; i < BoostBurstPS.transform.childCount; i++)
                {
                    BoostBurstPS.transform.GetChild(i).GetComponent<ParticleSystem>().Play(); //left and right included
                }

            }
            if (Drift_time >= 4 && Drift_time < 7)
            {
                Boost = true;
                Boost_time = 1.5f;
                if (playersounds.Check_if_playing())
                {
                    playersounds.Mario_Boost_Sounds[playersounds.sound_count].Play();
                    playersounds.sound_count++;
                }
                for (int i = 0; i < BoostBurstPS.transform.childCount; i++)
                {
                    BoostBurstPS.transform.GetChild(i).GetComponent<ParticleSystem>().Play(); //left and right included
                }
            }
            if (Drift_time >= 7)
            {
                Boost = true;
                Boost_time = 2.5f;
                if (playersounds.Check_if_playing())
                {
                    playersounds.Mario_Boost_Sounds[playersounds.sound_count].Play();
                    playersounds.sound_count++;
                }
                for (int i = 0; i < BoostBurstPS.transform.childCount; i++)
                {
                    BoostBurstPS.transform.GetChild(i).GetComponent<ParticleSystem>().Play(); //left and right included
                }
            }



            //reset everything
            Drift_time = 0;
            //stop particles
            for (int i = 0; i < 5; i++)
            {
                ParticleSystem DriftPS = Right_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //right wheel particles
                ParticleSystem.MainModule PSMAIN = DriftPS.main;

                ParticleSystem DriftPS2 = Left_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //left wheel particles
                ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

                DriftPS.Stop();
                DriftPS2.Stop();

            }
            DriftDustLeft.GetChild(0).GetComponent<ParticleSystem>().Stop();
            DriftDustLeft.GetChild(1).GetComponent<ParticleSystem>().Stop();
            DriftDustRight.GetChild(0).GetComponent<ParticleSystem>().Stop();
            DriftDustRight.GetChild(1).GetComponent<ParticleSystem>().Stop();
        }
    }
    void player_animations()
    {
        if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            Driver.SetBool("TurnLeft", false);
            Driver.SetBool("TurnRight", false);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Driver.SetBool("TurnLeft", false);
            Driver.SetBool("TurnRight", true);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            Driver.SetBool("TurnLeft", true);
            Driver.SetBool("TurnRight", false);
        }
        
    }
    void boostEffects()
    {
        //boost particle systems
        if (Boost)
        {
            for (int i = 0; i < 2; i++)
            {
                ParticleSystem currentboost = Boost_PS.transform.GetChild(i).GetComponent<ParticleSystem>();
                if (!currentboost.isPlaying)
                    currentboost.Play();
            }
        }
        if (!Boost)
        {
            for (int i = 0; i < 2; i++)
            {
                ParticleSystem currentboost = Boost_PS.transform.GetChild(i).GetComponent<ParticleSystem>();
                if (currentboost.isPlaying)
                    currentboost.Stop();
            }
        }
    }
    void movingCarParts()
    {
        if (!antiGravity)
        {
            //tire gameObject steer and rotate and steeringwheel rotate
            float x = Input.GetAxis("Horizontal"); //direction 
            float rotate_speed = 12f;
            if (x >= 0.1)
            {
                FrontRightTire.transform.localEulerAngles = Vector3.LerpUnclamped(FrontRightTire.transform.localEulerAngles, new Vector3(0, 205, 0), rotate_speed * Time.deltaTime);
                FrontLeftTire.transform.localEulerAngles = Vector3.LerpUnclamped(FrontLeftTire.transform.localEulerAngles, new Vector3(0, 205, 0), rotate_speed * Time.deltaTime);

                steeringwheel.transform.localEulerAngles = Vector3.Lerp(steeringwheel.transform.localEulerAngles, new Vector3(0, 205, 0), rotate_speed * Time.deltaTime);


            } //right
            if (x <= -0.1)
            {
                FrontRightTire.transform.localEulerAngles = Vector3.LerpUnclamped(FrontRightTire.transform.localEulerAngles, new Vector3(0, 155, 0), rotate_speed * Time.deltaTime);
                FrontLeftTire.transform.localEulerAngles = Vector3.LerpUnclamped(FrontLeftTire.transform.localEulerAngles, new Vector3(0, 155, 0), rotate_speed * Time.deltaTime);

                steeringwheel.transform.localEulerAngles = Vector3.Lerp(steeringwheel.transform.localEulerAngles, new Vector3(0, 155, 0), rotate_speed * Time.deltaTime);



            } //left
            if (x == 0)
            {
                FrontRightTire.transform.localEulerAngles = Vector3.Lerp(FrontRightTire.transform.localEulerAngles, new Vector3(0, 180, 0), rotate_speed * Time.deltaTime);
                FrontLeftTire.transform.localEulerAngles = Vector3.Lerp(FrontLeftTire.transform.localEulerAngles, new Vector3(0, 180, 0), rotate_speed * Time.deltaTime);

                steeringwheel.transform.localEulerAngles = Vector3.Lerp(steeringwheel.transform.localEulerAngles, new Vector3(0, 180, 0), rotate_speed * Time.deltaTime);

            } //0
        }
        else
        {
            FrontRightTire.transform.localEulerAngles = Vector3.Lerp(FrontRightTire.transform.localEulerAngles, new Vector3(0, 180, 0), 10 * Time.deltaTime);
            FrontLeftTire.transform.localEulerAngles = Vector3.Lerp(FrontLeftTire.transform.localEulerAngles, new Vector3(0, 180, 0), 10 * Time.deltaTime);
        }
        
          //tire spinning
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKey(KeyCode.Space) && REALCURRENTSPEED < 0)
            {

                tires[0].transform.Rotate(-90 * Time.deltaTime * REALCURRENTSPEED * 0.5f, 0, 0);
                tires[1].transform.Rotate(-90 * Time.deltaTime * REALCURRENTSPEED * 0.5f, 0, 0);
                tires[2].transform.Rotate(-90 * Time.deltaTime * 5, 0, 0);
                tires[3].transform.Rotate(-90 * Time.deltaTime * 5, 0, 0);
            }
            else
            {
                if (currentspeed < 6.5 && currentspeed > -6.5)
                {
                    tires[i].transform.Rotate(-90 * Time.deltaTime * REALCURRENTSPEED * 0.5f, 0, 0);
                }
                else
                {
                    tires[i].transform.Rotate(-90 * Time.deltaTime * currentspeed / 4f, 0, 0);
                }
            }

            
        }

        //propeller
        
        if(propeller != null)
        {
            if (IN_WATER)
            {
                propeller.gameObject.SetActive(true);
                float speed = REALCURRENTSPEED+3;
                speed = Mathf.Clamp(speed, 0, 40);
                propeller.Rotate(0, 0, speed, Space.Self);
            }
            else
            {
                propeller.gameObject.SetActive(false);
            }
        }




    }
    void GroundNormalRotation()
    {
        if (!antiGravity)
        {
            if (!JUMP_PANEL && glidingTime < 3f)
            {
                //ground normal rotation
                Ray ground = new Ray(raycastPos.position, -transform.up);
                RaycastHit hit;


                if (Physics.Raycast(ground, out hit, groundRayDist, mask) && (hit.normal.y > 0.5f || antiGravity))
                {
                    if(antiGravity)
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 1f * Time.deltaTime);
                    else
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
                    }
                }


            }
            if (glidingTime > 3f)
            {
                //ground normal rotation
                Ray ground = new Ray(raycastPos.position, -transform.up);
                RaycastHit hit;
                if (Physics.Raycast(ground, out hit, 4, mask) && hit.normal.y > 0.5f && hit.transform.tag == "GliderPanel")
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
                }
            }
        }
        else
        {
            if (!JUMP_PANEL && glidingTime < 3f)
            {
                //ground normal rotation
                Ray ground = new Ray(raycastPos.position, -transform.up);
                RaycastHit hit;

                if (Physics.Raycast(ground, out hit, groundRayDist, mask) && (hit.normal.y > 0.5f || antiGravity))
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
                }


            }
            if (glidingTime > 3f)
            {
                //ground normal rotation
                Ray ground = new Ray(raycastPos.position, -transform.up);
                RaycastHit hit;
                if (Physics.Raycast(ground, out hit, 4, mask) && hit.normal.y > 0.5f && hit.transform.tag == "GliderPanel")
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
                }
            }
        }
        
      

    }
    void gliderMovements()
    {
        glidingTime += Time.deltaTime;

        float none = 0;
        inputHorizontalSmooth = Mathf.SmoothDamp(inputHorizontalSmooth, Input.GetAxis("Horizontal") * -1, ref none, 5 * Time.deltaTime);
        if (RACE_MANAGER.RACE_COMPLETED)
        {
            inputHorizontalSmooth = 0;
        }

        if (glideTrick) //if trick happens, do this, else do regular
        {
            if (glidingTime > 0.4)
            {
                glider.GetComponent<Animator>().SetBool("GliderOpen", true);
                glider.GetComponent<Animator>().SetBool("GliderClose", false);
            }
        }
        else
        {
            glider.GetComponent<Animator>().SetBool("GliderOpen", true);
            glider.GetComponent<Animator>().SetBool("GliderClose", false);
        }


        Transform kart = transform;

        //turn left and right, up and down
        
        {
            if (Input.GetKey(KeyCode.UpArrow) && !RACE_MANAGER.RACE_COMPLETED)
            {
                kart.rotation = Quaternion.SlerpUnclamped(kart.rotation, Quaternion.Euler(25 + glideAngleX, kart.eulerAngles.y, kart.eulerAngles.z), 1.5f * Time.deltaTime);
                rb.AddForce(Vector3.down * 2000 * Time.deltaTime, ForceMode.Acceleration);
            }
            else if (Input.GetKey(KeyCode.DownArrow) && !RACE_MANAGER.RACE_COMPLETED)
            {
                float angle = transform.localEulerAngles.x;
                angle = (angle > 180) ? angle - 360 : angle;

                if (angle > -20 + glideAngleX)
                {
                    kart.rotation = Quaternion.SlerpUnclamped(kart.rotation, Quaternion.Euler(-25 + glideAngleX, kart.eulerAngles.y, kart.eulerAngles.z), 1.5f * Time.deltaTime);
                }
                if (glidingTime < 6)
                    rb.AddRelativeForce(Vector3.up * Time.deltaTime * 1500, ForceMode.Acceleration);
            }
            else
            {
                kart.rotation = Quaternion.SlerpUnclamped(kart.rotation, Quaternion.Euler(0 + glideAngleX, kart.eulerAngles.y, kart.eulerAngles.z), 1.5f * Time.deltaTime);
            }
        }
       
        {
            //ground normal rotation
            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;
            if (!Physics.Raycast(ground, out hit, 0, mask))
            {
                rotate_strength = 15;

                Quaternion zeroRot = Quaternion.identity;



                Quaternion targetRot = Quaternion.Euler(kart.eulerAngles.x, kart.eulerAngles.y, glideAngleZ + 40 * inputHorizontalSmooth);
                kart.rotation = Quaternion.SlerpUnclamped(kart.rotation, targetRot, 3 * Time.deltaTime);
            }
            else
                rotate_strength = 25;
        }
        

    }


    void PositionUI()
    {
        if(!showUI && lapCounter.totalCheckpointVal > 1)
        {
            positionUI.GetComponent<Animator>().SetTrigger("Change");
            showUI = true;
        }
        if(lastPos != lapCounter.Position && lapCounter.totalCheckpointVal > 1)
        {
            lastPos = lapCounter.Position;
            positionUI.GetComponent<Animator>().SetTrigger("Change");
        }
    }
    
    void mario_face()
    {
        if(!item_manager.StarPowerUp)
            MarioFace.sharedMaterial = current_face_material;
        else
        {
            MarioFace.sharedMaterial = item_manager.starMat;
        }
    }

    public IEnumerator hitByBanana()
    {
        GetComponent<Minimap>().playerInMap.GetComponent<Animator>().SetTrigger("Spin");
        playersounds.playHurtSound();
        HitByBanana_ = true;
        Boost_time = 0;
        stopDrift();

        if (!RACE_MANAGER.RACE_COMPLETED)
        {
            current_face_material = faces[4];
            SpecialFace = true;
            Driver.SetBool("Hurt", true);
        }

        yield return new WaitForSeconds(1.5f);


        HitByBanana_ = false;
        playSpinAnim = true;
        invincible = false;

        if (!RACE_MANAGER.RACE_COMPLETED)
        {
            current_face_material = faces[0];
            SpecialFace = false;
            Driver.SetBool("Hurt", false);
        }
    }
    public IEnumerator hitByShell()
    {

        GetComponent<Minimap>().playerInMap.GetComponent<Animator>().SetTrigger("Spin");
        playersounds.playHurtSound();
        HitByShell_ = true;
        Boost_time = 0;
        stopDrift();
        if (!RACE_MANAGER.RACE_COMPLETED)
        {
            current_face_material = faces[4];
            Driver.SetBool("Hurt", true);
            SpecialFace = true;
        }

        yield return new WaitForSeconds(1.5f);

        HitByShell_ = false;
        playSpinAnim = true;
        invincible = false;

        if (!RACE_MANAGER.RACE_COMPLETED)
        {
            current_face_material = faces[0];
            Driver.SetBool("Hurt", false);
            SpecialFace = false;
        }

    }

    public void kartAnims()
    {
        

        //to prevent going in the air when drifting
        if((drift_left || drift_right) && !GLIDER_FLY)
        {
            rb.AddForce(-transform.up * 10000 * Time.deltaTime, ForceMode.Acceleration);
        }

    }

    void steerOnPath()
    {
        Vector3 lookat = RaceEndPath.GetChild(currentWayPoint).position;

        Ray ground = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        

        if (Physics.Raycast(ground, out hit, 10, mask))//different mask
        {
            Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);
            GroundNormalRotation();
        }
        //angle calc
        Vector3 myangle = lookat - transform.position;
        Vector3 angle = Vector3.Cross(transform.forward, myangle);


        float dir = Vector3.Dot(angle, transform.up);
        float none = 0;

        // maybe get dir, and make float y lerp to that dir value, and then rotate y axis (space.self) according to that y value or something

        //float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 2.5f * Time.deltaTime);
        y = Mathf.SmoothDamp(y, dir, ref none, 2.5f * Time.deltaTime);
        transform.Rotate(0, y / 16, 0, Space.Self);

    }
    void moveOnPath()
    {
        currentspeed = Mathf.Lerp(currentspeed, desiredMaxSpeed, 1.5f * Time.deltaTime);
        Vector3 vel = transform.forward * currentspeed;

        if(!antiGravity)
            vel.y = rb.velocity.y;

        rb.velocity = vel;

            rb.AddRelativeForce(Vector3.down * 5000 * Time.deltaTime, ForceMode.Acceleration);

        if (GLIDER_FLY)
        {
            gliderMovements();
        }

        if (transform.GetChild(0).GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(2).IsName("SpinLeft") || transform.GetChild(0).GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(2).IsName("SpinRight") && particleSystemAntigravSpinTimer < 0.95f)
        {
            particleSystemAntigravSpinTimer += Time.deltaTime; //this timer thing is limited to 0.95 seconds because when the animation returns to original state, the trail gets weird
            if (!antiGravSpin[0].isPlaying)
            {
                if (antiGravSpin.Length > 0)
                {
                    antiGravSpin[0].Play();
                    antiGravSpin[1].Play();
                }
                    
            }
        }
        else
        {
            if(antiGravSpin.Length > 0)
            {
                antiGravSpin[0].Stop();
                antiGravSpin[1].Stop();
            }
            
        }
        if (particleSystemAntigravSpinTimer > 1.01f)
        {
            particleSystemAntigravSpinTimer = 0;
        }


    }
    void raceEndCarMoveParts()
    {
        //spin
        for (int i = 0; i < 4; i++)
        {

            if (currentspeed < 6.5 && currentspeed > -6.5)
            {
                tires[i].transform.Rotate(-90 * Time.deltaTime * currentspeed * 0.015f, 0, 0);
            }
            else
            {
                tires[i].transform.Rotate(-90 * Time.deltaTime * currentspeed / 5f, 0, 0);
            }
        }

        //tire turn calc
        Ray ground = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        float dir = 0;
        if (Physics.Raycast(ground, out hit, 10, mask))
        {
            Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);
            //angle calc
            Vector3 myangle = RaceEndPath.GetChild(currentWayPoint).position - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);
            dir = Vector3.Dot(angle, transform.up);
        }

        if (!antiGravity)
        {
            if (dir > 2)
            {
                FrontRightTire.transform.localEulerAngles = Vector3.Lerp(FrontRightTire.transform.localEulerAngles, new Vector3(0, 205, 0), 8 * Time.deltaTime);
                FrontLeftTire.transform.localEulerAngles = Vector3.Lerp(FrontLeftTire.transform.localEulerAngles, new Vector3(0, 205, 0), 8 * Time.deltaTime);
                steeringwheel.transform.localEulerAngles = Vector3.Lerp(steeringwheel.transform.localEulerAngles, new Vector3(0, 205, 0), 8 * Time.deltaTime);
            }
            if (dir >= -2 && dir <= 2)
            {
                FrontRightTire.transform.localEulerAngles = Vector3.Lerp(FrontRightTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
                FrontLeftTire.transform.localEulerAngles = Vector3.Lerp(FrontLeftTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
                steeringwheel.transform.localEulerAngles = Vector3.Lerp(steeringwheel.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
            }
            if (dir < -2)
            {
                FrontRightTire.transform.localEulerAngles = Vector3.Lerp(FrontRightTire.transform.localEulerAngles, new Vector3(0, 155, 0), 8 * Time.deltaTime);
                FrontLeftTire.transform.localEulerAngles = Vector3.Lerp(FrontLeftTire.transform.localEulerAngles, new Vector3(0, 155, 0), 8 * Time.deltaTime);
                steeringwheel.transform.localEulerAngles = Vector3.Lerp(steeringwheel.transform.localEulerAngles, new Vector3(0, 155, 0), 8 * Time.deltaTime);
            }
        }
        else
        {
            FrontRightTire.transform.localEulerAngles = Vector3.Lerp(FrontRightTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
            FrontLeftTire.transform.localEulerAngles = Vector3.Lerp(FrontLeftTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
        }

        //propeller

        if (propeller != null)
        {
            if (IN_WATER)
            {
                propeller.gameObject.SetActive(true);
                float speed = currentspeed+3;
                speed = Mathf.Clamp(speed, -20, 20);
                propeller.Rotate(0, 0, speed, Space.Self);
            }
            else
            {
                propeller.gameObject.SetActive(false);
            }
        }

    }
    void disableItems()
    {
        item_manager.used_Item_Done();
        item_manager.current_Item = "";
    }
    public void stopDrift()
    {
        drifting = false;
        playersounds.effectSounds[0].Stop();
        playersounds.effectSounds[1].Stop();

        drift_direction = 0;
        drift_left = false;
        drift_right = false;

        //reset everything
        Drift_time = 0;
        //stop particles
        for (int i = 0; i < 5; i++)
        {
            ParticleSystem DriftPS = Right_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //right wheel particles
            ParticleSystem.MainModule PSMAIN = DriftPS.main;

            ParticleSystem DriftPS2 = Left_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //left wheel particles
            ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

            DriftPS.Stop();
            DriftPS2.Stop();

        }
        DriftDustLeft.GetChild(0).GetComponent<ParticleSystem>().Stop();
        DriftDustLeft.GetChild(1).GetComponent<ParticleSystem>().Stop();
        DriftDustRight.GetChild(0).GetComponent<ParticleSystem>().Stop();
        DriftDustRight.GetChild(1).GetComponent<ParticleSystem>().Stop();

        
    }


    void lookAtOpponent()
    {
        if (RACE_MANAGER.RACE_STARTED)
        {
            lookAtTime += Time.deltaTime;
            float dist = 99999;
            for (int i = 0; i < allOpponents.Length; i++)
            {
                if (Vector3.Distance(transform.position, allOpponents[i].transform.position) < dist)
                {
                    opponentLook = allOpponents[i].transform;
                    dist = Vector3.Distance(transform.position, allOpponents[i].transform.position);
                }
            }

            if (lookAtTime > 5)
            {
                lookAtTime = 0;
            }

            //animatiom booleans
            bool isIdleOrSteering = (Driver.GetCurrentAnimatorStateInfo(0).IsName("New State") || Driver.GetCurrentAnimatorStateInfo(0).IsName("TurnLeft") || Driver.GetCurrentAnimatorStateInfo(0).IsName("TurnRight") || Driver.GetCurrentAnimatorStateInfo(0).IsName("NormalWithItem") || Driver.GetCurrentAnimatorStateInfo(0).IsName("TurnLeftWithItem") || Driver.GetCurrentAnimatorStateInfo(0).IsName("TurnRightWithItem")) && currentspeed > -5;

            if (!RACE_MANAGER.RACE_COMPLETED && isIdleOrSteering)
            {
                if (Vector3.Distance(opponentLook.position, transform.position) < 20 && lookAtTime > 1)
                {
                    headBone.rotation = Quaternion.SlerpUnclamped(headBone.rotation, Quaternion.LookRotation(opponentLook.position - transform.position), 5 * Time.deltaTime);

                    Quaternion localRot = headBone.localRotation;

                    localRot.eulerAngles = new Vector3(headBone.localEulerAngles.x, 0, 0);

                    headBone.localRotation = localRot;

                    float dir = Vector3.Angle(transform.forward, opponentLook.position - transform.position);

                    if (!SpecialFace && !reversing)
                    {
                        float rot = headBone.localEulerAngles.x;
                        rot = (rot > 180) ? rot - 360 : rot;

                        if (dir > 90 && rot < 0) // look right
                        {
                            current_face_material = faces[6];
                            lookAtOpponentEyes = true;
                        }
                        else if (dir > 90 && rot > 0) //look left
                        {
                            current_face_material = faces[1];
                            lookAtOpponentEyes = true;

                        }



                    }
                    //headbone rotation + x rotation is look left and -  x rotation is look right
                    //use dir which is always positive (absolute value) and multiply it by -1 or 1 based on if the head rotation is - or +
                    //then apply a special look left or look right texture on mario face
                }
                else
                {
                    lookAtOpponentEyes = false;
                    headBone.localRotation = Quaternion.SlerpUnclamped(headBone.localRotation, Quaternion.Euler(0, 0, 0), 5 * Time.deltaTime);
                }
            }
            else if (!isIdleOrSteering && !RACE_MANAGER.RACE_COMPLETED)
            {
                headBone.localRotation = Quaternion.SlerpUnclamped(headBone.localRotation, Quaternion.Euler(0, 0, 0), 8 * Time.deltaTime);
            }
            else
            {
                headBone.localRotation = Quaternion.SlerpUnclamped(headBone.localRotation, Quaternion.Euler(0, 0, 0), 5 * Time.deltaTime);
            }
        }
        






    }
    void blinking()
    {
        blinkingTimeCounter += Time.deltaTime;
        if(!SpecialFace && reversingTime <= 0 && !lookAtOpponentEyes)
        {
            if(blinkingTimeCounter % 3 < 0.15 && blinkingTimeCounter % 3 > 0.1)
            {
                current_face_material = faces[2];
            }
            else if(blinkingTimeCounter % 3 <= 0.1 && blinkingTimeCounter % 3 >= 0.075)
            {
                current_face_material = faces[5];
            }
            else
            {
                current_face_material = faces[0];
            }
        }
    }

    IEnumerator happyFaceTrickJump()
    {
        for(int i = 0; i < 60; i++)
        {
            yield return new WaitForSeconds(0.01f);
            current_face_material = faces[3];
        }
        current_face_material = faces[0];
    }

    IEnumerator trickJump()
    {
        {
            //do your trick
            for (int i = 0; i < trickParticles.childCount; i++)
            {
                trickParticles.GetChild(i).GetComponent<ParticleSystem>().Play();
            }


            if (trickAnimCounter == 0)
            {
                Driver.SetTrigger("JumpTrick1");
                StartCoroutine(happyFaceTrickJump());
                if (playersounds.Check_if_playing())
                {
                    playersounds.MarioJumpTrickSounds[0].Play();
                }
            }
            else if (trickAnimCounter == 2)
            {
                Driver.SetTrigger("GliderTrick");
                StartCoroutine(happyFaceTrickJump());
                if (playersounds.Check_if_playing())
                {
                    playersounds.MarioJumpTrickSounds[1].Play();
                }
            }
            else if (trickAnimCounter == 1)
            {
                Driver.SetTrigger("JumpTrick2");
                StartCoroutine(happyFaceTrickJump());
                if (playersounds.Check_if_playing())
                {
                    playersounds.MarioJumpTrickSounds[2].Play();
                }
            }

            trickAnimCounter++;
            if (trickAnimCounter > 2)
            {
                trickAnimCounter = 0;
            }

            if (!JUMP_PANEL)
            {
                float force = 6500;
                for (int i = 0; i < 30 && !JUMP_PANEL; i++)
                {
                    rb.AddRelativeForce(Vector3.up * force * Time.deltaTime, ForceMode.Acceleration);
                    if (force >= 300)
                        force -= 300;
                    yield return new WaitForSeconds(0.01f);
                }
            }
            
        }

    }

    IEnumerator antiGravityColor()
    {
        if (!antiGravity)
        {
            playersounds.effectSounds[23].Play();
            for (int i = 0; i < TireParents.Length; i++)
            {

                transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AntiGravity");
            }

            if (!item_manager.StarPowerUp && !item_manager.isBullet)
            {
                for(int i = 0; i < tireRenderers.Length; i++)
                {
                    tireRenderers[i].material.SetColor("Color_B652FD79", Color.white);
                    tireRenderers[i].material.SetColor("Color_89B4C299", antiGravityTireColor);
                    tireRenderers[i].material.SetVector("Vector4_6C68F82F", new Vector4(5, 5, 5, 5));
                }
                kartMat.SetVector("Vector4_70BBF882", new Vector4(5, 5, 5, 5));


                for (int i = 0; i < 30; i++)
                {
                    yield return new WaitForSeconds(0.005f);

                    for (int j = 0; j < tireRenderers.Length; j++)
                    {
                        TireParents[j].transform.localPosition = Vector3.MoveTowards(TireParents[j].transform.localPosition, antiGravityTirePositions[j], 5f * Time.deltaTime);
                        axels.gameObject.SetActive(false);
                        if (j == 0 || j == 1) { tireArms[j].localScale = Vector3.LerpUnclamped(tireArms[j].localScale, new Vector3(1.5f, tireArms[j].localScale.y, tireArms[j].localScale.z), 20 * Time.deltaTime); }
                        if (j == 2 || j == 3) { tireArms[j].localScale = Vector3.LerpUnclamped(tireArms[j].localScale, new Vector3(1.2f, tireArms[j].localScale.y, tireArms[j].localScale.z), 20 * Time.deltaTime); }

                    }

                }
            }
            



            //rotate 90 degrees
            for (int i = 0; i < 30; i++)
            {
                
                TireParents[0].transform.localRotation = Quaternion.LerpUnclamped(TireParents[0].transform.localRotation, Quaternion.Euler(0, 0, 90), 12 * Time.deltaTime);
                TireParents[1].transform.localRotation = Quaternion.LerpUnclamped(TireParents[1].transform.localRotation, Quaternion.Euler(0, 180, -90), 12 * Time.deltaTime);
                TireParents[2].transform.localRotation = Quaternion.LerpUnclamped(TireParents[2].transform.localRotation, Quaternion.Euler(0, 0, -90), 12 * Time.deltaTime);
                TireParents[3].transform.localRotation = Quaternion.LerpUnclamped(TireParents[3].transform.localRotation, Quaternion.Euler(0, 180, 90), 12 * Time.deltaTime);
                yield return new WaitForSeconds(0.001f);

                for(int j = 0; j < 4; j++)
                {
                    TireParents[j].transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(TireParents[j].transform.GetChild(1).GetComponent<Renderer>().material.GetColor("_Color"), lightDecalColor, 12 * Time.deltaTime));
                }


            }

            if(!item_manager.StarPowerUp && !item_manager.isBullet)
            {
                for (int i = 0; i < 60; i++)
                {
                    yield return new WaitForSeconds(0.01f);
                    for (int j = 0; j < tireRenderers.Length; j++)
                    {
                        //glow color
                        tireRenderers[j].material.SetColor("Color_B652FD79", Color.LerpUnclamped(tireRenderers[j].material.GetColor("Color_B652FD79"), Color.black, 7 * Time.deltaTime));
                        //center color
                        tireRenderers[j].material.SetColor("Color_89B4C299", Color.LerpUnclamped(tireRenderers[j].material.GetColor("Color_89B4C299"), Color.black, 7 * Time.deltaTime));
                    }
                }
            }
            
        }
        
    }
    IEnumerator exitAntiGravity()
    {
        if (antiGravity)
        {
            playersounds.effectSounds[24].Play();

            

            for (int i = 0; i < 25; i++)
            {
                TireParents[0].transform.localRotation = Quaternion.LerpUnclamped(TireParents[0].transform.localRotation, Quaternion.Euler(0, 0, 0), 12 * Time.deltaTime);
                TireParents[1].transform.localRotation = Quaternion.LerpUnclamped(TireParents[1].transform.localRotation, Quaternion.Euler(0, 180, 0), 12 * Time.deltaTime);
                TireParents[2].transform.localRotation = Quaternion.LerpUnclamped(TireParents[2].transform.localRotation, Quaternion.Euler(0, 0, 0), 12 * Time.deltaTime);
                TireParents[3].transform.localRotation = Quaternion.LerpUnclamped(TireParents[3].transform.localRotation, Quaternion.Euler(0, 180, 0), 12 * Time.deltaTime);
                yield return new WaitForSeconds(0.001f);

                if (!item_manager.StarPowerUp && !item_manager.isBullet)
                {
                    tireRenderers[0].material.SetVector("Vector4_6C68F82F", Vector4.LerpUnclamped(tireRenderers[0].material.GetVector("Vector4_6C68F82F"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));
                    tireRenderers[1].material.SetVector("Vector4_6C68F82F", Vector4.LerpUnclamped(tireRenderers[1].material.GetVector("Vector4_6C68F82F"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));
                    tireRenderers[2].material.SetVector("Vector4_6C68F82F", Vector4.LerpUnclamped(tireRenderers[2].material.GetVector("Vector4_6C68F82F"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));
                    tireRenderers[3].material.SetVector("Vector4_6C68F82F", Vector4.LerpUnclamped(tireRenderers[3].material.GetVector("Vector4_6C68F82F"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));

                    kartMat.SetVector("Vector4_70BBF882", Vector4.LerpUnclamped(kartMat.GetVector("Vector4_70BBF882"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));

                    for (int j = 0; j < 4; j++)
                    {
                        TireParents[j].transform.GetChild(1).GetComponent<Renderer>().material.SetColor("_Color", Color.Lerp(TireParents[j].transform.GetChild(1).GetComponent<Renderer>().material.GetColor("_Color"), new Color(0, 0, 0, 0), 12 * Time.deltaTime));
                    }
                }

                 

            }
            for (int i = 0; i < TireParents.Length; i++)
            {
                TireParents[i].GetComponent<TireTest>().enabled = true;

            }
            for (int i = 0; i < 30; i++)
            {
                yield return new WaitForSeconds(0.001f);
                for (int j = 0; j < tireRenderers.Length; j++)
                {
                    axels.gameObject.SetActive(true);

                    tireArms[j].localScale = Vector3.LerpUnclamped(tireArms[j].localScale, new Vector3(0.8f, tireArms[j].localScale.y, tireArms[j].localScale.z), 20 * Time.deltaTime); 


                    TireParents[j].transform.localPosition = Vector3.LerpUnclamped(TireParents[j].transform.localPosition, tireLocalPositions[j], 12 * Time.deltaTime);

                }
            }
        }
       
    }

    void tireDirtEffect()
    {
        if (Is_Dirt_Track)
        {
            if (Input.GetKey(KeyCode.Space) && !item_manager.isBullet && !item_manager.StarPowerUp)
            {
                for(int i = 0; i < tireRenderers.Length; i++)
                {
                    tireRenderers[i].material.SetColor("Color_7E73E7E8", Color.LerpUnclamped(tireRenderers[i].material.GetColor("Color_7E73E7E8"), dirtColor, 0.8f * Time.deltaTime));
                }
            }
        }
    }
}
