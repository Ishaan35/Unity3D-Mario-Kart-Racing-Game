using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedShell : MonoBehaviour
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
    public string who_threw_shell;
    public float lifetime;

    private SphereCollider sphereCollider;

    private GameObject[] opponents;

    public GameObject[] allplayers;
    private Transform chase_opponent;
    private bool grounded;

    private bool closeToPlayer = false;
    private RACE_MANAGER rm;
    private Transform player;

    [HideInInspector]
    public bool isactive = true;



    // Start is called before the first frame update
    void Start()
    {
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

        opponents = GameObject.FindGameObjectsWithTag("Opponent");

        rm = GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>();
        player = GameObject.FindGameObjectWithTag("Player").transform;


        
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        lifetime += Time.deltaTime;
        DetectTarget();

        if(!LockedOnTarget)
            RotateTowards();
        else
        {
            ChaseTarget();
        }
        
        move();

        if (!grounded && !GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().GLIDER_FLY)
        {
            rb.AddForce(Vector3.down * 15000 * Time.deltaTime, ForceMode.Acceleration);
        }

        if(!closeToPlayer)
        {
            
                if(Vector3.Distance(player.position, transform.position) < 100 && who_threw_shell != player.name)
                {
                    closeToPlayer = true;
                    StartCoroutine(rm.warningRedShell(transform));
                }
            
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
        Vector3 vel = transform.forward * 5000 * Time.deltaTime;
        vel.y = rb.velocity.y / 1.5f;
        rb.velocity = vel;
    }
    void DetectTarget()
    {
        if (!LockedOnTarget)
        {
            if(who_threw_shell == "Mario")
            {
                for (int i = 0; i < opponents.Length; i++)
                {
                    if (Vector3.Distance(transform.position, opponents[i].transform.position) < 20)
                    {
                        chase_opponent = opponents[i].transform;
                        LockedOnTarget = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < allplayers.Length; i++)
                {
                    if (Vector3.Distance(transform.position, allplayers[i].transform.position) < 20     &&     allplayers[i].name != who_threw_shell     &&      allplayers[i].GetComponent<LapCounter>().totalCheckpointVal > GameObject.Find(who_threw_shell).GetComponent<LapCounter>().totalCheckpointVal)
                    {
                        chase_opponent = allplayers[i].transform;
                        LockedOnTarget = true;
                        break;
                    }
                }
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
            else if(current_node != nodes.Count)
            {
                current_node++;
            }

        }

        if(other.gameObject.tag == "TrailingItem")
        {
            if(other.gameObject.name == "TrailingBanana")
            {
                other.transform.parent.parent.parent.GetComponent<ItemManager>().CurrentTrailingItem.SetActive(false);
                other.transform.parent.parent.parent.GetComponent<ItemManager>().CurrentTrailingItem = null;
                other.transform.parent.parent.parent.GetComponent<ItemManager>().current_Item = "";
                other.transform.parent.parent.parent.GetComponent<ItemManager>().used_Item_Done();
                destroyShell();
            }
            else
            {
                int x = other.transform.GetChild(0).childCount; //particle systems
                for (int i = 0; i < x; i++)
                {
                    other.transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>().Play();
                }
                other.transform.parent.parent.parent.GetComponent<ItemManager>().CurrentTrailingItem.SetActive(false);
                other.transform.parent.parent.parent.GetComponent<ItemManager>().CurrentTrailingItem = null;
                other.transform.parent.parent.parent.GetComponent<ItemManager>().current_Item = "";
                other.transform.parent.parent.parent.GetComponent<ItemManager>().used_Item_Done();
                destroyShell();
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ground" && collision.gameObject.tag != "Dirt")
        {

            if (collision.gameObject.name.Equals(who_threw_shell)) //wait 0.5 second before detecting collisions with person 
            {
                if (lifetime > 0.5f)
                {
                    destroyShell();

                }
            }
            
            //hitting wach other
            if(collision.gameObject.tag == "Shell")
            {
                destroyShell();
                
            }
            if(collision.gameObject.tag == "Banana" || collision.gameObject.tag == "Cow")
            {
                destroyShell();
                if(collision.gameObject.tag != "Cow")
                {
                    Destroy(collision.gameObject);
                }
            }

            //HITTING OPPONENTS
            if (collision.gameObject.tag == "Opponent")
            {
                if (!collision.gameObject.GetComponent<OpponentItemManager>().StarPowerUp)
                {
                    collision.gameObject.GetComponent<OpponentItemManager>().hitByShell(); //the opponent has the function that does all this work
                    if (who_threw_shell == "Mario")
                    {
                        GameObject.Find("Mario").GetComponent<Player>().Driver.SetTrigger("HitItem");
                        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().Check_if_playing())
                            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSounds>().effectSounds[18].Play();
                    }
                    destroyShell();
                    if (who_threw_shell == "Mario")
                    {
                        //for loop with player face
                    }
                }
                else
                {
                    destroyShell();
                }
                
            }
            if(collision.gameObject.tag == "Player")
            {
                if(who_threw_shell != collision.gameObject.name)
                {
                    if (!collision.gameObject.GetComponent<ItemManager>().StarPowerUp)
                    {
                        StartCoroutine(collision.gameObject.GetComponent<Player>().hitByShell()); //the player has the function that does all this work

                        if(rm.FrontCam.activeSelf)
                            GameObject.Find("Main Camera").GetComponent<Animator>().SetTrigger("ShellHit");
                    }
                    destroyShell();
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

    public void destroyShell()
    {
        isactive = false;
        int x = transform.GetChild(0).childCount; //particle systems
        for (int i = 0; i < x; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>().Play();
        }
        transform.GetChild(1).GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
        sphereCollider.enabled = false;
        rb.isKinematic = true;

        Destroy(gameObject, 3);
    }
}
