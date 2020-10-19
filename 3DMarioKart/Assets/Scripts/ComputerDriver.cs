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
    public List<Transform> nodes = new List<Transform>();
    [HideInInspector]
    public int current_node = 0;

    public float max_steer_angle;
    public float max_speed;
    [HideInInspector]
    public float current_speed;
    [HideInInspector]
    public float REALSPEED;
    public float Desired_Max_Speed;
    bool grounded = false;

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
    public GameObject GLider;
    

    public Animator DriverAnim;

    [Header("Tires Steering")]
    public Transform frontLeftTire;
    public Transform frontRightTire;

    [HideInInspector]
    public bool lookAtBool = false;

    


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        item_manage = GetComponent<OpponentItemManager>();

        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        //the paths
        for(int i = 0; i < pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != path.transform) //make sure the node is not the same as the path parent object
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        Right_Wheel_Drift_PS = DriftPS.transform.GetChild(0).gameObject;
        Left_Wheel_Drift_PS = DriftPS.transform.GetChild(1).gameObject;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        carTires();
        REALSPEED = transform.InverseTransformDirection(rb.velocity).z;
        if (RACE_MANAGER.RACE_STARTED && !item_manage.isBullet)
        {
            steer();
            Move();
            drift_func();
            animations();

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

            if(REALSPEED > 40 && !driftleft && !driftright && !GLIDER_FLY)
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
            rb.AddForce(Vector3.down * 10000 * Time.deltaTime, ForceMode.Acceleration);
            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;
            if (Physics.Raycast(ground, out hit, 10, mask))
            {
                //DIRECTION TO FACE
                Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);
                //angle calc
                Debug.DrawRay(hit.point, hit.normal, Color.white, 30f);
                Vector3 myangle = item_manage.path.GetChild(item_manage.currentWayPoint).position - transform.position;
                Vector3 angle = Vector3.Cross(transform.forward, myangle);
                dir = Vector3.Dot(angle, transform.up);

                float none = 0;

                float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 3f * Time.deltaTime);
                transform.eulerAngles = new Vector3(rot.eulerAngles.x, y, rot.eulerAngles.z);

                //MOVE FORWARD
                current_speed = 115;
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

        Vector3 lookat = nodes[current_node].position;

        Ray ground = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ground, out hit, 10, mask))
        {
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);
            }


            //angle calc
            Vector3 myangle = nodes[current_node].position - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);
            dir = Vector3.Dot(angle, transform.up);

            float none = 0;
            float y;

            if (slowDown)
            {
                y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir * 2, ref none, 6.5f * Time.deltaTime);
            }
            else
            {
                y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 6.5f * Time.deltaTime);
            }




            transform.eulerAngles =new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
        }

    }
    void Move()
    {
        item_manage.item_select_time += Time.deltaTime;
        if(!item_manage.HitByShell_)
            current_speed = Mathf.Lerp(current_speed, max_speed, 0.5f * Time.deltaTime);


        Vector3 vel = transform.forward * current_speed;
        vel.y = rb.velocity.y;
        rb.velocity = vel;
        if(!GLIDER_FLY)
            rb.AddForce(Vector3.down * 8000 * Time.deltaTime, ForceMode.Acceleration);
        else
        {
            rb.AddForce(Vector3.down * 5000 * Time.deltaTime, ForceMode.Acceleration);
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
        }

        if (item_manage.StarPowerUp)
        {
            current_speed = Boost_Speed - 5;
        }




    }
    void drift_func()
    {
        
        

        if(nodes[current_node].tag == "DriftLeft" && transform.InverseTransformDirection(rb.velocity).z > 40) //left
        {
            if(hop_anim)
            {
                transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Drift");
                hop_anim = false;
            }
            driftleft = true;
            driftright = false;
            
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, -20, 0), 8f * Time.deltaTime);
            
        }
        if (nodes[current_node].tag == "DriftRight" && transform.InverseTransformDirection(rb.velocity).z > 40) //right
        {
            if (hop_anim)
            {
                transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Drift");
                hop_anim = false;
            }
            driftleft = false;
            driftright = true;
            transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 20, 0), 8 * Time.deltaTime);

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

        if ((nodes[current_node].tag != "DriftLeft" && nodes[current_node].tag != "DriftRight") || transform.InverseTransformDirection(rb.velocity).z <= 40 || !grounded) //stop drifting
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
        //steer
        if (dir > 2)
        {
            frontRightTire.transform.localEulerAngles = Vector3.Lerp(frontRightTire.transform.localEulerAngles, new Vector3(0, 205, 0), 8 * Time.deltaTime);
            frontLeftTire.transform.localEulerAngles = Vector3.Lerp(frontLeftTire.transform.localEulerAngles, new Vector3(0, 205, 0), 8 * Time.deltaTime);
        }
        if (dir >= -2 && dir <= 2)
        {
            frontRightTire.transform.localEulerAngles = Vector3.Lerp(frontRightTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
            frontLeftTire.transform.localEulerAngles = Vector3.Lerp(frontLeftTire.transform.localEulerAngles, new Vector3(0, 180, 0), 8 * Time.deltaTime);
        }
        if (dir < -2)
        {
            frontRightTire.transform.localEulerAngles = Vector3.Lerp(frontRightTire.transform.localEulerAngles, new Vector3(0, 155, 0), 8 * Time.deltaTime);
            frontLeftTire.transform.localEulerAngles = Vector3.Lerp(frontLeftTire.transform.localEulerAngles, new Vector3(0, 155, 0), 8 * Time.deltaTime);
        }

        //spin
        for (int i = 0; i < 4; i++)
        {

            if (current_speed < 6.5 && current_speed > -6.5)
            {
                tires[i].transform.Rotate(-90 * Time.deltaTime * current_speed * 0.015f, 0, 0);
            }
            else
            {
                tires[i].transform.Rotate(-90 * Time.deltaTime * current_speed / 5f, 0, 0);
            }
        }
    }
    void lookAtOthers()
    {

    }


    

    

    

    private void OnTriggerEnter(Collider other)
    {
        //next waypoint
        if(other.transform == nodes[current_node])
        {
            if (current_node == nodes.Count - 1) //if last node, set the next node to first
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
            if (!GLIDER_FLY)
            {
                transform.GetChild(0).GetChild(0).GetComponent<Animator>().SetTrigger("Glide1");
            }
            GLIDER_FLY = true;
            GLider.GetComponent<Animator>().SetBool("GliderOpen", true);
            GLider.GetComponent<Animator>().SetBool("GliderClose", false);

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
        }

        if(collision.gameObject.tag == "Player")
        {
            if (item_manage.StarPowerUp && !collision.gameObject.GetComponent<ItemManager>().StarPowerUp)
            {
                StartCoroutine(collision.gameObject.GetComponent<Player>().hitByShell()); //the player has the function that does all this work
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


}
