using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueShell : MonoBehaviour
{
    [Header("Movement")]
    public Transform path;
    private List<Transform> nodes = new List<Transform>();
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



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rm = GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>();
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        Transform[] pathTransforms = path.GetComponentsInChildren<Transform>();
        //the paths
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != path.transform) //make sure the node is not the same as the path parent object
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        if (who_threw_shell.Equals("Player"))
        {
            current_node = GameObject.Find(who_threw_shell).GetComponent<ItemManager>().currentWayPoint;
        }



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

        if (!grounded && !GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().GLIDER_FLY)
        {
            rb.AddForce(Vector3.down * 15000 * Time.deltaTime, ForceMode.Acceleration);
        }

        if(Vector3.Distance(transform.position, player.position) < 100 && !initiateWarning)
        {
            StartCoroutine(rm.warningBlueShell(transform));
            initiateWarning = true;
        }

    }






    void RotateTowards()
    {


        Ray ground = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ground, out hit, 10, mask))
        {
            Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);


            //angle calc
            Vector3 myangle = nodes[current_node].position - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);
            dir = Vector3.Dot(angle, transform.up);


            float none = 0;

            float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 2.5f * Time.deltaTime);



            transform.eulerAngles = new Vector3(rot.eulerAngles.x, y, rot.eulerAngles.z);
        }

    }
    void ChaseTarget()
    {
        Ray ground = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ground, out hit, 10, mask))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);


            Vector3 direction = chase_opponent.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.deltaTime);

        }
    }

    void move()
    {
       Vector3 vel = transform.forward * 5500 * Time.deltaTime;
        vel.y = rb.velocity.y/2;
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
            LockedOnTarget = true;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit, Mathf.Infinity, groundmask))
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, hit.point.y + 8f, transform.position.z), 5 * Time.deltaTime);
            }
            transform.GetChild(0).GetComponent<Animator>().SetBool("LockedOn", true);

            Vector3 vectorToTarget = transform.position - chase_opponent.position;
            vectorToTarget.y = 0;
            float distanceToTarget = vectorToTarget.magnitude;
            if(distanceToTarget <= 2)
            {
                attackingTarget = true;
                transform.parent = chase_opponent;
                GetComponent<Rigidbody>().isKinematic = true;
                transform.GetChild(0).GetComponent<Animator>().SetBool("Attack", true);
                isactive = false;

            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        //next waypoint
        if (other.transform == nodes[current_node])
        {
            if (current_node == nodes.Count - 1) //if last node, set the next node to first
            {
                current_node = 0;
            }
            else if (current_node != nodes.Count)
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
            if (collision.gameObject.tag == "Player")
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
    
}
