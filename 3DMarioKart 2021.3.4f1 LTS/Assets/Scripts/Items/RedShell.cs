using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedShell : MonoBehaviour
{
    [Header("Movement")]
    public Transform pathMain;

    public Transform pathOption1;
    public Transform pathOption2;


    public int current_node = 0;

    private Rigidbody rb;
    public LayerMask mask;
    public LayerMask maskAntiGravity;

    [HideInInspector]
    public float dir = 0;
    bool LockedOnTarget = false;
    public string who_threw_shell;
    public float lifetime;

    private SphereCollider sphereCollider;

    private GameObject[] opponents;

    public GameObject[] allplayers;
    private Transform chase_opponent = null;
    private bool grounded;

    private bool closeToPlayer = false;
    private RACE_MANAGER rm;
    private Transform player;

    [HideInInspector]
    public bool isactive = true;

    public bool AntiGravity = false;

    float y = 0;

    private float speed = 6000;

    public bool needsExtraDownForceAntigravity = false;
    private bool antiGravityGrounded = false;



    // Start is called before the first frame update
    void Start()
    {

        pathMain = pathOption1;

        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
       

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

        if (!GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().GLIDER_FLY)
        {
            if(!AntiGravity)
                rb.AddForce(Vector3.down * 15000 * Time.deltaTime, ForceMode.Acceleration);
           
        }
        if (AntiGravity)
        {
            rb.AddRelativeForce(Vector3.down * 30000 * Time.deltaTime, ForceMode.Acceleration);
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

        if(chase_opponent == null)
        {
            if (AntiGravity)
            {
                if (Physics.Raycast(ground, out hit, 10))//different mask
                {
                    transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 10 * Time.deltaTime);


                    //angle calc
                    Vector3 myangle = pathMain.GetChild(current_node).position - transform.position;
                    Vector3 angle = Vector3.Cross(transform.forward, myangle);
                    dir = Vector3.Dot(angle, transform.up);


                    float none = 0;

                    // maybe get dir, and make float y lerp to that dir value, and then rotate y axis (space.self) according to that y value or something

                    //float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 2.5f * Time.deltaTime);
                    y = Mathf.SmoothDamp(y, dir, ref none, 2.5f * Time.deltaTime);



                    transform.Rotate(0, y / 2, 0, Space.Self);
                }
            }
            else
            {
                //angle calc
                Vector3 myangle = pathMain.GetChild(current_node).position - transform.position;
                Vector3 angle = Vector3.Cross(transform.forward, myangle);
                dir = Vector3.Dot(angle, transform.up);
                float none = 0;

                // maybe get dir, and make float y lerp to that dir value, and then rotate y axis (space.self) according to that y value or something

                //float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 2.5f * Time.deltaTime);
                y = Mathf.SmoothDamp(y, dir, ref none, 2.5f * Time.deltaTime);
                transform.Rotate(0, y / 2, 0, Space.Self);

                if (Physics.Raycast(ground, out hit, 10, mask))
                {
                    Quaternion rot = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(rot.eulerAngles.x, transform.eulerAngles.y, rot.eulerAngles.z);
                }
            }
        }
        
        

    }
    void ChaseTarget()
    {
        Ray ground = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ground, out hit, 10, mask))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 6 * Time.deltaTime);


            //angle calc
            Vector3 myangle = chase_opponent.position - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);
            dir = Vector3.Dot(angle, transform.up);


            float none = 0;

            // maybe get dir, and make float y lerp to that dir value, and then rotate y axis (space.self) according to that y value or something

            //float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 2.5f * Time.deltaTime);
            y = dir;



            transform.Rotate(0, y*2, 0, Space.Self);

        }


    }
    void move()
    {
        Vector3 vel = transform.forward * speed * Time.deltaTime;

        if(!AntiGravity)
            vel.y = rb.velocity.y;
       

        rb.velocity = vel;

        if (needsExtraDownForceAntigravity && AntiGravity && !antiGravityGrounded)
        {
            rb.AddRelativeForce(Vector3.down * 20000 * Time.deltaTime, ForceMode.Acceleration);
        }
    }
    void DetectTarget()
    {
        if (!LockedOnTarget)
        {
            if(who_threw_shell == "Mario")
            {
                for (int i = 0; i < opponents.Length; i++)
                {
                    if (Vector3.Distance(transform.position, opponents[i].transform.position) < 30)
                    {
                        if(GameObject.Find(who_threw_shell).GetComponent<LapCounter>().totalCheckpointVal < opponents[i].GetComponent<LapCounter>().totalCheckpointVal)
                        {
                            chase_opponent = opponents[i].transform;
                            LockedOnTarget = true;
                            break;
                        }
                        else if(GameObject.Find(who_threw_shell).GetComponent<LapCounter>().totalCheckpointVal == opponents[i].GetComponent<LapCounter>().totalCheckpointVal)
                        {
                            if(GameObject.Find(who_threw_shell).GetComponent<LapCounter>().distanceToNextCheckpoint > opponents[i].GetComponent<LapCounter>().distanceToNextCheckpoint)
                            {
                                chase_opponent = opponents[i].transform;
                                LockedOnTarget = true;
                                break;
                            }
                        }
                        
                    }
                }
            }
            else
            {
                for (int i = 0; i < allplayers.Length; i++)
                {
                    if (Vector3.Distance(transform.position, allplayers[i].transform.position) < 30     &&     allplayers[i].name != who_threw_shell)
                    {
                        if (allplayers[i].GetComponent<LapCounter>().totalCheckpointVal > GameObject.Find(who_threw_shell).GetComponent<LapCounter>().totalCheckpointVal)
                        {
                            chase_opponent = allplayers[i].transform;
                            LockedOnTarget = true;
                            break;
                        }
                        else if(GameObject.Find(who_threw_shell).GetComponent<LapCounter>().totalCheckpointVal == allplayers[i].GetComponent<LapCounter>().totalCheckpointVal)
                        {
                            if(GameObject.Find(who_threw_shell).GetComponent<LapCounter>().distanceToNextCheckpoint > allplayers[i].GetComponent<LapCounter>().distanceToNextCheckpoint)
                            {
                                chase_opponent = allplayers[i].transform;
                                LockedOnTarget = true;
                                break;
                            }
                        }
                        
                    }

                }
            }
            

        }
    }



    private void OnTriggerEnter(Collider other)
    {
        //next waypoint
        if (other.transform == pathMain.GetChild(current_node))
        {
            if (current_node == pathMain.childCount - 1) //if last node, set the next node to first
            {
                current_node = 0;
            }
            else if(current_node != pathMain.childCount)
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Equals("ItemPathColliderPath1"))
        {
            pathMain = pathOption1;
        }
        if (other.gameObject.name.Equals("ItemPathColliderPath2"))
        {
            pathMain = pathOption2;
        }

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
            if (collision.gameObject.tag == "Opponent" && who_threw_shell != collision.gameObject.name)
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
            if(collision.gameObject.tag == "Player" && Vector3.Distance(transform.position, collision.transform.position) < 5)
            {
                if(who_threw_shell != collision.gameObject.name)
                {
                    if (!collision.gameObject.GetComponent<ItemManager>().StarPowerUp)
                    {                        StartCoroutine(collision.gameObject.GetComponent<Player>().hitByShell()); //the player has the function that does all this work

                        try
                        {
                            GameObject.Find("Main Camera").GetComponent<Animator>().SetTrigger("ShellHit");
                        }catch(System.Exception e)
                        {

                        }
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
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Dirt")
        {
            antiGravityGrounded = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Ground" || collision.gameObject.tag != "Dirt")
        {
            grounded = false;
        }
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Dirt")
        {
            antiGravityGrounded = false;
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
        try
        {
            sphereCollider.enabled = false;
            rb.isKinematic = true;
        }
        catch (System.Exception e)
        {

        }

        Destroy(gameObject, 3);
    }
}
