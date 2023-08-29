using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireTest : MonoBehaviour
{
    private Vector3 localPos;
    public LayerMask mask;

    public Transform wheelLookAt;

    public float hitDistance = 0.4f;

    float driftCooldown = 0;

    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        localPos = transform.GetChild(0).localPosition;

        StartCoroutine(getPos());
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.V))
        {
            driftCooldown = 1;
        }
        driftCooldown -= Time.deltaTime;

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().antiGravity)
        {
            transform.GetChild(0).localPosition = Vector3.LerpUnclamped(transform.GetChild(0).localPosition, pos, 10 * Time.deltaTime);
        }


        if (gameObject.name.IndexOf("Axel") < 0) //is a tire
        {
            if (!GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().antiGravity)
            {
                RaycastHit hit;
                if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().GLIDER_FLY) //this helps prevent the tires from simulating suspension on trigger colliders placed around the track when gliding because it would look awkward
                {
                    if (Physics.Raycast(transform.position, -transform.up, out hit, 0.75f, mask))
                    {

                        transform.GetChild(0).position = Vector3.LerpUnclamped(transform.GetChild(0).position, hit.point, 20 * Time.deltaTime);
                    }
                    else
                    {
                        transform.GetChild(0).localPosition = Vector3.LerpUnclamped(transform.GetChild(0).localPosition, new Vector3(0, -0.37f, 0), 20 * Time.deltaTime);
                    }
                }
                else
                {
                    //dont want the suspension effect to act too much when hopping
                    float lerpSpeed = 20;
                    if (driftCooldown > 0 && !GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetComponent<UtilityFunctions>().hopEnd)
                    {
                        lerpSpeed = 5;
                    }

                    if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().REALCURRENTSPEED < 20)
                    {
                        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, hitDistance, mask))
                        {

                            transform.GetChild(0).position = Vector3.LerpUnclamped(transform.GetChild(0).position, hit.point, lerpSpeed * Time.deltaTime);
                        }
                        else
                        {
                            transform.GetChild(0).localPosition = Vector3.LerpUnclamped(transform.GetChild(0).localPosition, new Vector3(0, -hitDistance, 0), lerpSpeed * Time.deltaTime);
                        }
                    }

                    //if speed is greater than 20, we dont apply the mask because when the speed is slow, the suspension effect tends to react very intense on tiny bumps or objects on the surface of the road, and this is not that noticeable when moving faster
                    else
                    {
                        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, hitDistance))
                        {

                            transform.GetChild(0).position = Vector3.LerpUnclamped(transform.GetChild(0).position, hit.point, lerpSpeed * Time.deltaTime);
                        }
                        else
                        {
                            transform.GetChild(0).localPosition = Vector3.LerpUnclamped(transform.GetChild(0).localPosition, new Vector3(0, -hitDistance, 0), lerpSpeed * Time.deltaTime);
                        }
                    }

                }
            }
            
        }
        else //is an axel
        {
            transform.LookAt(wheelLookAt.transform.GetChild(0));
        }
        
        
       


    }

    IEnumerator getPos()
    {
        yield return new WaitForSeconds(1);
        if (gameObject.name.IndexOf("Axel") < 0)
        {
            pos = transform.GetChild(0).localPosition;
        }
    }
}
