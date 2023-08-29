using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerDriver : MonoBehaviour
{
    public Player playerscript; //some parts of this script are the same constants defined in player script, so just access them directly

    private OpponentItemManager item_manage;

    private Rigidbody rb;
    public LayerMask mask;

    

    [Header("Movement")]
    public Transform path;
    [HideInInspector]
    public int current_node = 0;

    public float max_steer_angle;
    public float max_speed;
    float y; //this is used to gather the direction to rotate on y axis
    [HideInInspector]
    public float current_speed;
    [HideInInspector]
    public float REALSPEED;
    public float Desired_Max_Speed;
    bool grounded = false;

    public Transform RayCastPos;

    float dir;
    [HideInInspector]
    public bool driftright;
    [HideInInspector]
    public bool driftleft;
    bool hop_anim = true;
    float drift_time = 0;
    public GameObject DriftPS;
    GameObject Right_Wheel_Drift_PS;
    GameObject Left_Wheel_Drift_PS;

    [Header("BOOST")]
    public float Boost_Speed;
    public GameObject BoostPS;
    [HideInInspector]
    public bool boost;
    [HideInInspector]
    public float boost_time = 0;
    public GameObject BoostBurstPS;
    public GameObject dustPS;
    public Transform[] tires;

    public GameObject exhaustParticles;

    bool slowDown;

    [HideInInspector]
    public bool GLIDER_FLY = false;
    private bool aboutToFly;

    //will get shot up in the air with a constantly changing force
    [HideInInspector]
    public bool JUMP_PANEL = false;
    private float jumpPanelUpForce = -250000;
    private float jumpPanelDownForce = 0;

    public GameObject GLider;
    

    public Animator DriverAnim;

    [Header("Tires Steering")]
    public Transform frontLeftTire;
    public Transform frontRightTire;

    public Transform head;
    [HideInInspector]
    public float lookAtTime = 0;
    [HideInInspector]
    public Transform personToLookAt = null;

    private RACE_MANAGER raceManager;
    private List<Transform> allPlayers = new List<Transform>();

    bool StartBoost = false;
    float startBoostTime = 0;
    public Transform startBoostDust;

    [HideInInspector]
    public bool AntiGravity;
    Quaternion rot;


    [Header("Anti Gravity Tire effects and animation")]
    public Color antiGravityTireColor;
    public Renderer[] tireRenderers;
    public Transform[] axels;
    private Vector3[] axelScales = new Vector3[4];
    public GameObject[] TireParents;
    private Vector3[] tireLocalPositions = new Vector3[4];
    public Vector3[] antiGravityTirePositions = new Vector3[4];
    public Material kartMat;

    public bool isDirtTrack;

    private float collideCooldown = 0;
    public float turnSpeedDivider = 10;

    private float glideAngleZ = 0;
    private float glideAngleX = 0;


    // Start is called before the first frame update
    void Start()
    {

        kartMat.SetVector("Vector4_70BBF882", new Vector4(0, 0, 0, 0));


        rb = GetComponent<Rigidbody>();
        item_manage = GetComponent<OpponentItemManager>();



        Right_Wheel_Drift_PS = DriftPS.transform.GetChild(0).gameObject;
        Left_Wheel_Drift_PS = DriftPS.transform.GetChild(1).gameObject;

        raceManager = GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>();
        //get all players except yourself
        for(int i = 0; i < raceManager.lapCounters.Count; i++)
        {
            if(raceManager.lapCounters[i] != GetComponent<LapCounter>())
            {
                allPlayers.Add(raceManager.lapCounters[i].transform);
            }
        }
        

        int strtBoost = Random.Range(0, 2);
        StartBoost = strtBoost == 0 ? true : false;

        for (int i = 0; i < TireParents.Length; i++)
        {
            tireLocalPositions[i] = TireParents[i].transform.localPosition;
        }


    }

    // Update is called once per frame
    void Update()
    {
        groundNormalRotation();

        if (!RACE_MANAGER.RACE_COMPLETED)
        {

            if(GetComponent<LapCounter>().totalCheckpointVal > (GameObject.FindGameObjectWithTag("Player").GetComponent<LapCounter>().totalCheckpointVal + 15))
            {
                Desired_Max_Speed = 55;
            }
            else
            {
                Desired_Max_Speed = 90;
            }
        }

        if(!RACE_MANAGER.RACE_STARTED && !RACE_MANAGER.RACE_COMPLETED)
        {
            if (StartBoost)
            {
                if(RACE_MANAGER.countDownTime > 1.2f)
                {
                    transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("StartTurbo", true);
                    tires[2].transform.Rotate(-90 * Time.deltaTime * 75 / 5.5f, 0, 0);
                    tires[3].transform.Rotate(-90 * Time.deltaTime * 75 / 5.5f, 0, 0);
                    startBoostTime += Time.deltaTime;

                    for(int i = 0; i < startBoostDust.childCount && isDirtTrack; i++)
                    {
                        if(!startBoostDust.GetChild(i).GetComponent<ParticleSystem>().isPlaying)
                            startBoostDust.GetChild(i).GetComponent<ParticleSystem>().Play();
                    }
                }
            }
        }

        REALSPEED = transform.InverseTransformDirection(rb.velocity).z;
        if (RACE_MANAGER.RACE_STARTED && !item_manage.isBullet)
        {
            startBoostTime -= Time.deltaTime;
            if(startBoostTime > 0)
            {
                current_speed = Mathf.Lerp(current_speed, Boost_Speed, 0.5f* Time.deltaTime);
                for (int i = 0; i < startBoostDust.childCount; i++)
                {
                    if (startBoostDust.GetChild(i).GetComponent<ParticleSystem>().isPlaying)
                        startBoostDust.GetChild(i).GetComponent<ParticleSystem>().Stop();
                }
            }
            else
            {
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetBool("StartTurbo", false);
            }
            

            if (StartBoost)
            {
                StartBoost = false;
                boost_time = 0.8f;
            }

            carTires();
            steer();
            if (!GetComponent<OutOfBounds>().FellInWater && !GetComponent<OutOfBounds>().outOfBounds)
            {
                Move();
            }

            drift_func();
            animations();
            lookAtOthers();

            for(int i = 0; i < exhaustParticles.transform.childCount; i++)
            {
                exhaustParticles.transform.GetChild(i).GetComponent<ParticleSystem>().Stop();
            }

            //
            if (boost_time > 0)
            {
                boost_time -= Time.deltaTime; //always decrease no matter what
                max_speed = Boost_Speed;
                boost = true;

                for (int i = 0; i < 2; i++)
                {
                    ParticleSystem currentboost = BoostPS.transform.GetChild(i).GetComponent<ParticleSystem>();
                    if (!currentboost.isPlaying)
                        currentboost.Play();
                }
            }
            if (boost_time <= 0)
            {
                boost = false;
                max_speed = Desired_Max_Speed;
                boost_time = 0;

                for (int i = 0; i < 2; i++)
                {
                    ParticleSystem currentboost = BoostPS.transform.GetChild(i).GetComponent<ParticleSystem>();
                    if (currentboost.isPlaying)
                        currentboost.Stop();
                }
            }

            if(REALSPEED > 40 && !driftleft && !driftright && !GLIDER_FLY && isDirtTrack)
            {
                for(int i = 0; i < dustPS.transform.childCount; i++)
                {
                    dustPS.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
                }
            }
            else
            {
                for (int i = 0; i < dustPS.transform.childCount; i++)
                {
                    dustPS.transform.GetChild(i).GetComponent<ParticleSystem>().Stop();
                }
            }
            
        }
        else if (item_manage.isBullet)
        {
            Vector3 lookat = item_manage.path.GetChild(item_manage.currentWayPoint).position;

            float dir = 0;
            rb.AddRelativeForce(Vector3.down * 10000 * Time.deltaTime, ForceMode.Acceleration);
            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;
            if (Physics.Raycast(ground, out hit, 10, mask))
            {
                //DIRECTION TO FACE
                Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);
                //angle calc
                Vector3 myangle = item_manage.path.GetChild(item_manage.currentWayPoint).position - transform.position;
                Vector3 angle = Vector3.Cross(transform.forward, myangle);
                dir = Vector3.Dot(angle, transform.up);

                float none = 0;

                float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 3f * Time.deltaTime);
                transform.eulerAngles = new Vector3(rot.eulerAngles.x, y, rot.eulerAngles.z);

                //MOVE FORWARD
                current_speed = 125;
                rb.velocity = transform.forward * current_speed;
            }
        }
        if (RACE_MANAGER.RACE_STARTED)
        {
            for(int i = 0; i < exhaustParticles.transform.childCount; i++)
            {
                exhaustParticles.transform.GetChild(i).GetComponent<ParticleSystem>().Stop();
            }
        }
        else
        {
            for (int i = 0; i < exhaustParticles.transform.childCount; i++)
            {
                exhaustParticles.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
            }
        }
    }




    void steer()
    {

        Vector3 lookat = path.GetChild(current_node).position;

        
        //angle calc
        Vector3 myangle = lookat - transform.position;
        Vector3 angle = Vector3.Cross(transform.forward, myangle);
        dir = Vector3.Dot(angle, transform.up);


        float none = 0;



        y = Mathf.SmoothDamp(y, dir, ref none, 2.5f * Time.deltaTime);
        transform.Rotate(0, y/turnSpeedDivider, 0, Space.Self);


    }
    void Move()
    {
        collideCooldown -= Time.deltaTime;

        item_manage.item_select_time += Time.deltaTime;
        if(!item_manage.HitByShell_)
            current_speed = Mathf.Lerp(current_speed, max_speed, 0.5f * Time.deltaTime);


        Vector3 vel = transform.forward * current_speed;
        if(!AntiGravity)
            vel.y = rb.velocity.y;
        rb.velocity = vel;

        rb.AddRelativeForce(Vector3.down * 5000 * Time.deltaTime, ForceMode.Acceleration);

        if (AntiGravity)
        {
            if (AntiGravity)
            {
                rb.AddRelativeForce(Physics.gravity * 5, ForceMode.Acceleration);
            }
        }


        if (!grounded && !boost)
        {
            max_speed = 40;
            //current_speed = Mathf.Lerp(current_speed, max_speed, 4);
        }
        else
            max_speed = Desired_Max_Speed;

        if(item_manage.HitByShell_)
        {
            current_speed = Mathf.Lerp(current_speed, 0, 2 * Time.deltaTime);
        }
        if (item_manage.HitByBanana_)
        {
            
            current_speed = Mathf.Lerp(current_speed, 0, 3 * Time.deltaTime);

        }

        if(GLIDER_FLY || aboutToFly)
        {
            Vector3 GlideVel = rb.velocity;
            GlideVel.y *= 0.5f;
            rb.velocity = GlideVel;

            if (GLIDER_FLY)
            {
                //steering
                if(y > 5)
                {
                    Quaternion targetRot = Quaternion.Euler(glideAngleX, transform.eulerAngles.y, glideAngleZ - 30);
                    transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, targetRot, 3 * Time.deltaTime);
                }
                else if(y <= -5)
                {
                    Quaternion targetRot = Quaternion.Euler(glideAngleX, transform.eulerAngles.y, glideAngleZ + 30);
                    transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, targetRot, 3 * Time.deltaTime);
                }
                else
                {
                    Quaternion targetRot = Quaternion.Euler(glideAngleX, transform.eulerAngles.y, glideAngleZ + 0);
                    transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, targetRot, 3 * Time.deltaTime);
                }
            }
        }
        

        if (item_manage.StarPowerUp)
        {
            max_speed = Boost_Speed;
        }

        if (JUMP_PANEL)
        {
            rb.velocity = transform.forward * current_speed;
            jumpPanelUpForce = Mathf.Lerp(jumpPanelUpForce, jumpPanelDownForce, 2.5f * Time.deltaTime);
            rb.AddRelativeForce(Vector3.down * jumpPanelUpForce * Time.deltaTime, ForceMode.Acceleration);
            rb.AddForce(transform.forward * 80000 * Time.deltaTime, ForceMode.Acceleration);

            if (AntiGravity) { }
                //transform.Rotate(5 * Time.deltaTime, 0, 0, Space.Self);

            //Camera.main.transform.parent.parent.localEulerAngles += new Vector3(0.5f, 0, 0);
        }


    }
    void drift_func()
    {
        
        

        if(path.GetChild(current_node).tag == "DriftLeft" && transform.InverseTransformDirection(rb.velocity).z > 40) //left
        {
            if(hop_anim)
            {
                transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Drift");
                hop_anim = false;
            }
            driftleft = true;
            driftright = false;
            
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, -30, 0), 8f * Time.deltaTime);
            
        }
        if (path.GetChild(current_node).tag == "DriftRight" && transform.InverseTransformDirection(rb.velocity).z > 40) //right
        {
            if (hop_anim)
            {
                transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Drift");
                hop_anim = false;
            }
            driftleft = false;
            driftright = true;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 30, 0), 8 * Time.deltaTime);

        }

        if (driftright || driftleft)
        {
            drift_time += Time.deltaTime;
            max_speed = Desired_Max_Speed - 15; //a little slower

            //particle effects
            if (drift_time >= 1 && drift_time < 3)
            {
                
                for (int i = 0; i < 5; i++)
                {
                    //the two particle systems and their mai modules for drift effects
                    ParticleSystem DriftPS = Right_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //right wheel particles
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;

                    ParticleSystem DriftPS2 = Left_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>(); //left wheel particles
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;

                    PSMAIN.startColor = playerscript.drift1; //accessing them from playerscript because they are constants common to everyone
                    PSMAIN2.startColor = playerscript.drift1;

                    if (!DriftPS.isPlaying && !DriftPS2.isPlaying)
                    {
                        DriftPS.Play();
                        DriftPS2.Play();
                    }

                }
                
            }
            if (drift_time >= 3 && drift_time < 6)
            {
                for (int i = 0; i < 5; i++)
                {
                    ParticleSystem DriftPS = Right_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;
                    ParticleSystem DriftPS2 = Left_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;
                    PSMAIN.startColor = playerscript.drift2;
                    PSMAIN2.startColor = playerscript.drift2;
            }

            }
            if (drift_time >= 6)
            {
                for (int i = 0; i < 5; i++)
                {

                    ParticleSystem DriftPS = Right_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN = DriftPS.main;
                    ParticleSystem DriftPS2 = Left_Wheel_Drift_PS.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
                    ParticleSystem.MainModule PSMAIN2 = DriftPS2.main;
                    PSMAIN.startColor = playerscript.drift3;
                    PSMAIN2.startColor = playerscript.drift3;

                }
;
            }
        }

        if ((path.GetChild(current_node).tag != "DriftLeft" && path.GetChild(current_node).tag != "DriftRight") || transform.InverseTransformDirection(rb.velocity).z <= 40 || !grounded) //stop drifting
        {
            if(drift_time >= 1 && drift_time < 3)
            {
                boost = true;
                boost_time = 0.5f;
            }
            if(drift_time >= 3 && drift_time < 6)
            {
                boost = true;
                boost_time = 1.5f;
            }
            if (drift_time >= 6)
            {
                boost = true;
                boost_time = 2.5f;
            }

            //reset
            driftleft = false;
            driftright = false;
            hop_anim = true;
            drift_time = 0;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0, 0), 8f * Time.deltaTime);
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
        } 
    }
    void animations()
    {
        if(dir <= -5)
        {
            DriverAnim.SetBool("TurnLeft", true);
            DriverAnim.SetBool("TurnRight", false);
        }
        else if (dir >= 5)
        {
            DriverAnim.SetBool("TurnLeft", false);
            DriverAnim.SetBool("TurnRight", true);
        }
        else
        {
            DriverAnim.SetBool("TurnLeft", false);
            DriverAnim.SetBool("TurnRight", false);
        }
    }
    void carTires()
    {
        if (!AntiGravity)
        {
            //steer
            if (dir > 2)
            {
                frontRightTire.transform.localEulerAngles = Vector3.Lerp(frontRightTire.transform.localEulerAngles, new Vector3(0, 210, 0), 8 * Time.deltaTime);
                frontLeftTire.transform.localEulerAngles = Vector3.Lerp(frontLeftTire.transform.localEulerAngles, new Vector3(0, 205, 0), 8 * Time.deltaTime);
            }
            if (dir >= -2 && dir <= 2)
            {
                frontRightTire.transform.localEulerAngles = Vector3.Lerp(frontRightTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
                frontLeftTire.transform.localEulerAngles = Vector3.Lerp(frontLeftTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
            }
            if (dir < -2)
            {
                frontRightTire.transform.localEulerAngles = Vector3.Lerp(frontRightTire.transform.localEulerAngles, new Vector3(0, 150, 0), 8 * Time.deltaTime);
                frontLeftTire.transform.localEulerAngles = Vector3.Lerp(frontLeftTire.transform.localEulerAngles, new Vector3(0, 155, 0), 8 * Time.deltaTime);
            }
        }
        else
        {
            frontRightTire.transform.localEulerAngles = Vector3.Lerp(frontRightTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
            frontLeftTire.transform.localEulerAngles = Vector3.Lerp(frontLeftTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
        }
        

        //spin
        for (int i = 0; i < 4; i++)
        {

            if (current_speed < 6.5 && current_speed > -6.5)
            {
                tires[i].transform.Rotate(-90 * Time.deltaTime * current_speed * 0.035f, 0, 0);
            }
            else
            {
                tires[i].transform.Rotate(-90 * Time.deltaTime * current_speed / 4f, 0, 0);
            }
        }
    }
    void lookAtOthers()
    {
        lookAtTime += Time.deltaTime;

        if(lookAtTime > 5)
        {
            lookAtTime = 0;
        }

        if(head != null)
        {

            
            //We also have to establish the condition to look at someone when a specific animation is not playing
            if(lookAtTime > 2)
            {
                float dist = 9999;
                //find closest player
                for(int i = 0; i < allPlayers.Count; i++)
                {
                    if(Vector3.Distance(transform.position, allPlayers[i].position) < dist)
                    {
                        dist = Vector3.Distance(transform.position, allPlayers[i].position);
                        personToLookAt = allPlayers[i];

                        Vector3 Target = transform.InverseTransformPoint(personToLookAt.position);
                        float targetAngle = Mathf.Atan2(Target.x, Target.z) * Mathf.Rad2Deg;
                        targetAngle = Mathf.Clamp(targetAngle, -90, 90);

                        if(Vector3.Distance(personToLookAt.position, transform.position) < 15)
                        {
                            head.localRotation = Quaternion.SlerpUnclamped(head.localRotation, Quaternion.Euler(new Vector3(0, -targetAngle, 0)), 5 * Time.deltaTime);
                        }
                    }
                }
            }
            else
            {
                head.localRotation = Quaternion.SlerpUnclamped(head.localRotation ,Quaternion.Euler(new Vector3(0, 0, 3)), 5 * Time.deltaTime);

            }
        }
        
    }

    void groundNormalRotation()
    {
        Ray ground = new Ray(RayCastPos.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ground, out hit, 3f, mask) && !JUMP_PANEL)
        {
            if (hit.normal.y > 0.5f || AntiGravity)
            {
                transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 7.5f * Time.deltaTime);
            }



        }
    }



    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "AntiGravity")
        {
            AntiGravity = true;
        }
        if (other.gameObject.tag == "AntiGravityFalse")
        {
            if (other.gameObject.tag == "AntiGravityFalse")
            {
                AntiGravity = false;

                if (other.gameObject.GetComponent<AntiGravityExitRotate>().rotateX)
                {
                    transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(other.gameObject.GetComponent<AntiGravityExitRotate>().newRotation.x, transform.eulerAngles.y, transform.eulerAngles.z), 3 * Time.deltaTime);
                }
                if (other.gameObject.GetComponent<AntiGravityExitRotate>().rotateZ)
                {
                    transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, other.gameObject.GetComponent<AntiGravityExitRotate>().newRotation.z), 9 * Time.deltaTime);
                }
                if (other.gameObject.GetComponent<AntiGravityExitRotate>().rotateY)
                {
                    transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, other.gameObject.GetComponent<AntiGravityExitRotate>().newRotation.y, transform.eulerAngles.z), 9 * Time.deltaTime);
                }
            }
        }

        if(other.gameObject.tag == "ColliderInAir")
        {
            /*
            if (AntiGravity)
            {
                rb.AddRelativeForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.Acceleration);
            }
            if (GLIDER_FLY)
            {
                if (!AntiGravity)
                    rb.AddForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.Acceleration);
                else
                {
                    rb.AddRelativeForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.Acceleration);
                }
            }
            */

            if (GLIDER_FLY)
            {
                if (!AntiGravity)
                    rb.AddForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.Acceleration);
                else
                {
                    rb.AddRelativeForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.Acceleration);
                }
            }
            else if (AntiGravity)
            {
                rb.AddRelativeForce(Vector3.down * other.GetComponent<colliderInAir>().force * Time.deltaTime, ForceMode.Acceleration);
            }
            


        }
    }



    private void OnTriggerEnter(Collider other)
    {
        //next waypoint
        if(other.transform == path.GetChild(current_node))
        {
            if (current_node == path.childCount - 1) //if last node, set the next node to first
            {
                current_node = 0;
            }
            else
                current_node++;


        }
        if(other.gameObject.tag == "SlowDownComputer")
        {
            if (boost_time < 1)
            {
                Desired_Max_Speed = 50;
                current_speed = Desired_Max_Speed;
                slowDown = true;
            }

        }
        if (other.gameObject.tag == "GliderPanelFly")
        {
            aboutToFly = true;
        }

        if (other.gameObject.tag == "AntiGravity")
        {
            if (!item_manage.StarPowerUp)
                StartCoroutine(antiGravityColor());
        }
        if (other.gameObject.tag == "AntiGravityFalse")
        {
            if (!item_manage.StarPowerUp)
                StartCoroutine(exitAntiGravity());
        }

        if (other.gameObject.tag == "JumpPanel" && !JUMP_PANEL)
        {
            jumpPanelUpForce = other.gameObject.GetComponent<JumpPanelScript>().upforce;
            jumpPanelDownForce = other.gameObject.GetComponent<JumpPanelScript>().downforce;
            JUMP_PANEL = true;

            boost_time = 2;
            boost = true;

            max_speed = Boost_Speed;
            for (int i = 0; i < BoostBurstPS.transform.childCount; i++)
            {
                BoostBurstPS.transform.GetChild(i).GetComponent<ParticleSystem>().Play(); //left and right included
            } //burst boost

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "SlowDownComputer")
        {
            Desired_Max_Speed = 70;
            slowDown = false;
        }

        if (other.gameObject.tag == "GliderPanelFly")
        {
            glideAngleZ = other.GetComponent<GetGlideAngle>().glideAngle;
            glideAngleX = other.GetComponent<GetGlideAngle>().glideAngleXOpponent;

            if (!GLIDER_FLY && other.gameObject.name != "GlidingTriggers")
            {
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("Glide1");
                GLIDER_FLY = true;
                GLider.GetComponent<Animator>().SetBool("GliderOpen", true);
                GLider.GetComponent<Animator>().SetBool("GliderClose", false);
            }


        }
        if(other.gameObject.tag == "GliderPanel")
        {
            boost_time = 2;
        }

        
    }
    private IEnumerator OnCollisionEnter(Collision collision)
    {


        if(collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Dirt")
        {
            GLIDER_FLY = false;
            aboutToFly = false;
            GLider.GetComponent<Animator>().SetBool("GliderOpen", false);
            GLider.GetComponent<Animator>().SetBool("GliderClose", true);
            

            if (JUMP_PANEL)
            {
                JUMP_PANEL = false;
            }
        }

        if(collision.gameObject.tag == "Player")
        {
            if ((item_manage.StarPowerUp || item_manage.isBullet) && !collision.gameObject.GetComponent<ItemManager>().StarPowerUp )
            {
                StartCoroutine(collision.gameObject.GetComponent<Player>().hitByShell()); //the player has the function that does all this work
                if(!raceManager.FrontFPCam.activeSelf)
                    GameObject.Find("Main Camera").GetComponent<Animator>().SetTrigger("ShellHit");
            }
        }
        if(collision.gameObject.tag == "Opponent" && !collision.gameObject.GetComponent<OpponentItemManager>().StarPowerUp && item_manage.StarPowerUp)
        {
            collision.gameObject.GetComponent<OpponentItemManager>().hitByShell(); //the opponent has the function that does all this work
        }

        if(collision.gameObject.tag == "Cow")
        {
            //angle calc
            Vector3 myangle = collision.transform.position - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);
            float dir = Vector3.Dot(angle, transform.up);

            float force = 50000;

            if (dir < 0)
            {
                force *= -1;
            }

            if (REALSPEED > 35 && !item_manage.StarPowerUp && !item_manage.isBullet)
            {
                if (!item_manage.StarPowerUp && !item_manage.isBullet)
                {
                   
                    item_manage.hitByBanana();
                    for (int i = 0; i < 30; i++)
                    {
                        force = Mathf.Lerp(force, 0, 3 * Time.deltaTime);
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
            else if(item_manage.StarPowerUp || item_manage.isBullet)
            {
            }
        }
        if (collision.gameObject.tag == "Trolley")
        {
            //angle calc
            Vector3 myangle = collision.transform.position - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);
            float dir = Vector3.Dot(angle, transform.up);

            float force = 50000;

            if (dir < 0)
            {
                force *= -1;
            }

            if (REALSPEED > 60 && !item_manage.StarPowerUp && !item_manage.isBullet && collideCooldown > 10)
            {
                if (!item_manage.StarPowerUp && !item_manage.isBullet)
                {
                    collideCooldown = 0;
                    item_manage.hitByShell();
                    for (int i = 0; i < 30; i++)
                    {
                        force = Mathf.Lerp(force, 0, 3 * Time.deltaTime);
                        rb.AddForce(transform.right * force * Time.deltaTime, ForceMode.Impulse);
                        yield return new WaitForSeconds(0.01f);
                    }
                }
            }
            else if (item_manage.StarPowerUp || item_manage.isBullet)
            {
            }
        }

        if (collision.gameObject.tag == "Crate")
        {
            if (current_speed > 5 && boost_time <= 0) {  current_speed = 5; }

            StartCoroutine(collision.gameObject.GetComponent<Crates>().destroy());
        }
        if ((collision.gameObject.tag == "Opponent" || collision.gameObject.tag == "Player") && Vector3.Distance(transform.position, collision.transform.position) < 5 && AntiGravity)
        {
            if (collideCooldown <= 0)
            {
                collideCooldown = 0.75f;
                //angle calc
                Vector3 myangle = collision.transform.position - transform.position;
                Vector3 angle = Vector3.Cross(transform.forward, myangle);
                float dir = Vector3.Dot(angle, transform.up);

                float force;
                if (current_speed > 50 && !item_manage.StarPowerUp)
                {
                    force = 20000;
                }
                else
                {
                    force = 5000;
                }

                if (dir < 0)
                {
                    if (current_speed > 50)
                    {

                        {
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AntigravSpinLeft");
                        }
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
                    if (current_speed > 50)
                    {
                        {
                            transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AntigravSpinRight");
                        }
                    }                 
                    for (int i = 0; i < 30; i++)
                    {
                        force = Mathf.Lerp(force, 0, 3 * Time.deltaTime);
                        rb.AddForce(-transform.right * force * Time.deltaTime, ForceMode.Impulse);
                        yield return new WaitForSeconds(0.01f);
                    }

                }
            }

        }
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            grounded = true;

        }
        if (other.gameObject.tag == "Dirt")
        {
            grounded = false;
            driftleft = false;
            driftright = false;
            drift_time = 0;
        }
    }

    IEnumerator antiGravityColor()
    {
        if (!AntiGravity)
        {
            for (int i = 0; i < axels.Length; i++)
            {
                axelScales[i] = axels[i].localScale;

                transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("AntiGravity");
            }

            for (int i = 0; i < 30; i++)
            {
                yield return new WaitForSeconds(0.005f);
                for (int j = 0; j < tireRenderers.Length; j++)
                {
                    //glow color
                    tireRenderers[j].material.SetColor("Color_B652FD79", Color.LerpUnclamped(tireRenderers[j].material.GetColor("Color_B652FD79"), Color.white, 12 * Time.deltaTime));
                    //center color
                    tireRenderers[j].material.SetColor("Color_89B4C299", Color.LerpUnclamped(tireRenderers[j].material.GetColor("Color_89B4C299"), antiGravityTireColor, 12 * Time.deltaTime));

                    if(j < 3)
                        axels[j].localScale = Vector3.LerpUnclamped(axels[j].localScale, new Vector3(axels[j].localScale.x, 0, axels[j].localScale.z), 8 * Time.deltaTime);

                    TireParents[j].transform.localPosition = Vector3.Lerp(TireParents[j].transform.localPosition, antiGravityTirePositions[j], 12 * Time.deltaTime);

                    tireRenderers[j].material.SetVector("Vector4_6C68F82F", Vector4.LerpUnclamped(tireRenderers[j].material.GetVector("Vector4_6C68F82F"), new Vector4(5, 5, 5, 5), 12 * Time.deltaTime));


                }
                kartMat.SetVector("Vector4_70BBF882", Vector4.LerpUnclamped(kartMat.GetVector("Vector4_70BBF882"), new Vector4(5, 5, 5, 5), 12 * Time.deltaTime));

            }



            //rotate 90 degrees
            for (int i = 0; i < 30; i++)
            {

                TireParents[0].transform.localRotation = Quaternion.LerpUnclamped(TireParents[0].transform.localRotation, Quaternion.Euler(0, 0, 90), 12 * Time.deltaTime);
                TireParents[1].transform.localRotation = Quaternion.LerpUnclamped(TireParents[1].transform.localRotation, Quaternion.Euler(0, 180, -90), 12 * Time.deltaTime);
                TireParents[2].transform.localRotation = Quaternion.LerpUnclamped(TireParents[2].transform.localRotation, Quaternion.Euler(0, 0, -90), 12 * Time.deltaTime);
                TireParents[3].transform.localRotation = Quaternion.LerpUnclamped(TireParents[3].transform.localRotation, Quaternion.Euler(0, 180, 90), 12 * Time.deltaTime);
                yield return new WaitForSeconds(0.001f);


            }

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
    IEnumerator exitAntiGravity()
    {
        if (AntiGravity)
        {


            for (int i = 0; i < 25; i++)
            {
                TireParents[0].transform.localRotation = Quaternion.LerpUnclamped(TireParents[0].transform.localRotation, Quaternion.Euler(0, 0, 0), 12 * Time.deltaTime);
                TireParents[1].transform.localRotation = Quaternion.LerpUnclamped(TireParents[1].transform.localRotation, Quaternion.Euler(0, 180, 0), 12 * Time.deltaTime);
                TireParents[2].transform.localRotation = Quaternion.LerpUnclamped(TireParents[2].transform.localRotation, Quaternion.Euler(0, 0, 0), 12 * Time.deltaTime);
                TireParents[3].transform.localRotation = Quaternion.LerpUnclamped(TireParents[3].transform.localRotation, Quaternion.Euler(0, 180, 0), 12 * Time.deltaTime);
                yield return new WaitForSeconds(0.001f);

                tireRenderers[0].material.SetVector("Vector4_6C68F82F", Vector4.LerpUnclamped(tireRenderers[0].material.GetVector("Vector4_6C68F82F"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));
                tireRenderers[1].material.SetVector("Vector4_6C68F82F", Vector4.LerpUnclamped(tireRenderers[1].material.GetVector("Vector4_6C68F82F"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));
                tireRenderers[2].material.SetVector("Vector4_6C68F82F", Vector4.LerpUnclamped(tireRenderers[2].material.GetVector("Vector4_6C68F82F"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));
                tireRenderers[3].material.SetVector("Vector4_6C68F82F", Vector4.LerpUnclamped(tireRenderers[3].material.GetVector("Vector4_6C68F82F"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));

                kartMat.SetVector("Vector4_70BBF882", Vector4.LerpUnclamped(kartMat.GetVector("Vector4_70BBF882"), new Vector4(0, 0, 0, 0), 12 * Time.deltaTime));

            }

            for (int i = 0; i < 30; i++)
            {
                yield return new WaitForSeconds(0.001f);
                for (int j = 0; j < tireRenderers.Length; j++)
                {
                    if(j < 3)
                        axels[j].localScale = Vector3.LerpUnclamped(axels[j].localScale, axelScales[j], 5 * Time.deltaTime);

                    TireParents[j].transform.localPosition = Vector3.LerpUnclamped(TireParents[j].transform.localPosition, tireLocalPositions[j], 12 * Time.deltaTime);

                }
            }
        }

    }

}
