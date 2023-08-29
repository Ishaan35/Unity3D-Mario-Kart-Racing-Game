using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueShell : MonoBehaviour
{
    [Header("Movement")]
    public Transform path;

    public Transform pathOption1;
    public Transform pathOption2;

    public int current_node = 0;

    private Rigidbody rb;
    public LayerMask mask;

    [HideInInspector]
    public float dir = 0;
    bool LockedOnTarget = false;
    bool attackingTarget = false;
    public string who_threw_shell;
    public float lifetime;

    private SphereCollider sphereCollider;

    public GameObject[] allplayers;
    [HideInInspector]
    public Transform chase_opponent;
    private bool grounded;

    public LayerMask groundmask;

    public GameObject blueExplosion;
    public GameObject smoke;

    private Transform player;

    public bool isactive = true;
    [HideInInspector]
    public bool initiateWarning = false;

    private RACE_MANAGER rm;

    [HideInInspector]
    public bool AntiGravity = false;
    private float smoothTime = 2.5f;
    private float lerpspeed = 10;

    float y = 0;


    private float speed = 7500;


    // Start is called before the first frame update
    void Start()
    {

        path = pathOption1;


        player = GameObject.FindGameObjectWithTag("Player").transform;
        rm = GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>();
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        

        if (who_threw_shell.Equals("Player"))
        {
            current_node = GameObject.Find(who_threw_shell).GetComponent<ItemManager>().currentWayPoint;
        }
        speed = 7500;


    }



    // Update is called once per frame
    void FixedUpdate()
    {
        lifetime += Time.deltaTime;
        DetectTarget();

        if (!LockedOnTarget)
            RotateTowards();
        else if(!attackingTarget)
        {
            ChaseTarget();
        }

        if(!attackingTarget)
            move();

        if (!grounded && !GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().GLIDER_FLY && !AntiGravity)
        {
            rb.AddForce(Vector3.down * 15000 * Time.deltaTime, ForceMode.Acceleration);
        }
        else if(AntiGravity && !grounded)
        {
            rb.AddRelativeForce(Vector3.down * 15000 * Time.deltaTime, ForceMode.Acceleration);
        }

        if (Vector3.Distance(transform.position, player.position) < 100 && !initiateWarning)
        {
            StartCoroutine(rm.warningBlueShell(transform));
            initiateWarning = true;
        }

    }






    void RotateTowards()
    {
        if (!AntiGravity)
        {
            rb.AddRelativeForce(Vector3.down * 500, ForceMode.Acceleration);


            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            //angle calc
            Vector3 myangle = path.GetChild(current_node).position - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);
            dir = Vector3.Dot(angle, transform.up);

            float none = 0;
            float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, smoothTime * Time.deltaTime);




            if (Physics.Raycast(ground, out hit, 10, mask))
            {
                Quaternion rot = Quaternion.LerpUnclamped(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);



                transform.eulerAngles = new Vector3(rot.eulerAngles.x, y, rot.eulerAngles.z);
            }
            else
            {
                transform.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, y, transform.rotation.eulerAngles.z);

            }
        }
        else if(!LockedOnTarget)
        {
            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            if (Physics.Raycast(ground, out hit, 10, mask))
            {
                Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);


                //angle calc
                Vector3 myangle = path.GetChild(current_node).position - transform.position;
                Vector3 angle = Vector3.Cross(transform.forward, myangle);
                dir = Vector3.Dot(angle, transform.up);


                float none = 0;

                // maybe get dir, and make float y lerp to that dir value, and then rotate y axis (space.self) according to that y value or something

                //float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 2.5f * Time.deltaTime);
                y = Mathf.SmoothDamp(y, dir, ref none, 2.5f * Time.deltaTime);



                transform.rotation = Quaternion.Euler(rot.eulerAngles.x, transform.eulerAngles.y, rot.eulerAngles.z);
                transform.Rotate(0, y / 2, 0, Space.Self);
            }
        }

    }
    void ChaseTarget()
    {
        if (!AntiGravity)
        {
            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;
            if (Physics.Raycast(ground, out hit, 10, mask))
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);


                Vector3 direction = chase_opponent.transform.position - transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction);

            }
        }
        else
        {
            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;


            //angle calc
            Vector3 myangle = chase_opponent.position - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);
            dir = Vector3.Dot(angle, transform.up);



            // maybe get dir, and make float y lerp to that dir value, and then rotate y axis (space.self) according to that y value or something

            //float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 2.5f * Time.deltaTime);
            y = dir;
            transform.Rotate(0, y, 0, Space.Self);

            if (Physics.Raycast(ground, out hit, 10, mask))
            {
                Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);

                transform.rotation = Quaternion.Euler(rot.eulerAngles.x, transform.eulerAngles.y, rot.eulerAngles.z);
            }
        }
    }

    void move()
    {
       Vector3 vel = transform.forward * speed * Time.deltaTime;

        if(!AntiGravity)
            vel.y = rb.velocity.y/1.5f;
        rb.velocity = vel;
    }

    void DetectTarget()
    {
        for(int i = 0; i < allplayers.Length; i++)
        {
            if(allplayers[i].GetComponent<LapCounter>().Position == 1)
            {
                chase_opponent = allplayers[i].transform;
            }
        }
        if(Vector3.Distance(transform.position, chase_opponent.position) < 40)
        {
            smoothTime = 1f;
            LockedOnTarget = true;
            RaycastHit hit;
            if (!AntiGravity)
            {
                if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, groundmask))
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, chase_opponent.position.y + 7, transform.position.z), 10 * Time.deltaTime);
                }
            }
            else
            {
                if (Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, groundmask))
                {
                    Vector3 pos = hit.point;

                    Ray upRay = new Ray(pos, transform.position - pos);


                    Vector3 upDist = upRay.GetPoint(8);
                    transform.position = Vector3.Lerp(transform.position, upDist, 5 * Time.deltaTime);
                }
            }
            
            transform.GetChild(0).GetComponent<Animator>().SetBool("LockedOn", true);


            //fix magnitude thing
            float distanceToTarget = 0;

            distanceToTarget = Vector3.Distance(transform.position, chase_opponent.GetComponent<PlayerBlueShellRaycast>().upRay.GetPoint(8));


            if (distanceToTarget <= 2.5f)
            {
                attackingTarget = true;
                transform.parent = chase_opponent;
                GetComponent<Rigidbody>().isKinematic = true;
                transform.GetChild(0).GetComponent<Animator>().SetBool("Attack", true);
                isactive = false;

            }
        }
        else
        {
            smoothTime = 2.5f;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        //next waypoint
        if (other.transform == path.GetChild(current_node))
        {
            if (current_node == path.childCount - 1) //if last node, set the next node to first
            {
                current_node = 0;
            }
            else if (current_node != path.childCount)
            {
                current_node++;
            }

        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ground" && collision.gameObject.tag != "Dirt")
        {


            if (collision.gameObject.tag == "Banana")
            {
                Destroy(collision.gameObject);
            }

            //HITTING OPPONENTS
            if (collision.gameObject.tag == "Opponent" && who_threw_shell != collision.gameObject.name)
            {
                collision.gameObject.GetComponent<OpponentItemManager>().hitByShell(); //the opponent has the function that does all this work

            }
            if (collision.gameObject.tag == "Player" && Vector3.Distance(transform.position, collision.transform.position) < 5)
            {
                if (who_threw_shell != collision.gameObject.name)
                {
                    if (!collision.gameObject.GetComponent<ItemManager>().StarPowerUp)
                    {
                        StartCoroutine(collision.gameObject.GetComponent<Player>().hitByShell()); //the player has the function that does all this work
                        GameObject.Find("Main Camera").GetComponent<Animator>().SetTrigger("ShellHit");
                    }
                }
            }


        }



    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "Ground" || collision.gameObject.tag != "Dirt")
        {
            grounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Ground" || collision.gameObject.tag != "Dirt")
        {
            grounded = false;
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
            AntiGravity = false;

            if (other.gameObject.GetComponent<AntiGravityExitRotate>().rotateX)
            {
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

        if (other.gameObject.name.Equals("ItemPathColliderPath1"))
        {
            path = pathOption1;
        }
        if (other.gameObject.name.Equals("ItemPathColliderPath2"))
        {
            path = pathOption2;
        }
    }

}
