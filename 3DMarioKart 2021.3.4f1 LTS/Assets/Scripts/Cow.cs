using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour
{
    public Transform path;
    public float speed;

    private Rigidbody rb;
    private int currentNode;

    private Vector3 startPos;
    bool raceStarted = false;

    private GameObject[] otherCows;
    int soundCount = 2;

    public LayerMask mask;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;

        float startPoint = Random.Range(0f, 1f);
        GetComponent<Animator>().Play("New State", -1, startPoint);

        otherCows = GameObject.FindGameObjectsWithTag("Cow");

        soundCount = Random.Range(2, 5);


        //int x = Random.Range(0, 3);
        //StartCoroutine(playSound(x));


        



    }

    // Update is called once per frame
    void Update()
    {
        GroundNormalRotation();
        moveOnPath();

        if(RACE_MANAGER.RACE_STARTED && !raceStarted)
        {
            raceStarted = true;
            transform.position = startPos;
        }
    }

    void GroundNormalRotation()
    {
       
            //ground normal rotation
       Ray ground = new Ray(transform.position, Vector3.down);
       RaycastHit hit;
        if (Physics.Raycast(ground, out hit, 10, mask) && hit.normal.y > 0.5f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 2, hit.normal) * transform.rotation, 3f * Time.deltaTime);
        }


    }

    void moveOnPath()
    {
        Vector3 vel = transform.forward * speed;
        vel.y = rb.velocity.y;
        rb.velocity = vel;


        //angle calc
        Vector3 myangle = path.GetChild(currentNode).position - transform.position;
        Vector3 angle = Vector3.Cross(transform.forward, myangle);
        float dir = Vector3.Dot(angle, transform.up);

        float none = 0;

        float y = Mathf.SmoothDamp(transform.eulerAngles.y, transform.eulerAngles.y + dir, ref none, 15f * Time.deltaTime);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);

        if(Vector3.Distance(path.GetChild(currentNode).position, transform.position) < 5)
        {
            currentNode++;
        }

        if(currentNode == path.childCount)
        {
            currentNode = 0;
        }


    }

    public bool checkIfPlayingSounds()
    {
        for(int i = 2; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<AudioSource>().isPlaying)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator playSound(int x)
    {
        yield return new WaitForSeconds(x);

        while (!RACE_MANAGER.RACE_COMPLETED)
        {
            int cowsPlayingSounds = 0;

            for (int i = 0; i < otherCows.Length; i++)
            {
                if (!otherCows[i].GetComponent<Cow>().checkIfPlayingSounds())
                {
                    cowsPlayingSounds++;
                }
            }

            if (cowsPlayingSounds < 2)
            {
                transform.GetChild(soundCount).GetComponent<AudioSource>().Play();
                soundCount++;
                if (soundCount >= transform.childCount)
                {
                    soundCount = 2;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
        
    }

    public IEnumerator hitByPowerup()
    {
        Vector3 position = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);

        for(int i = 0; i < 15; i++)
        {
            transform.position = Vector3.SlerpUnclamped(transform.position, endPos, 6 * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
        for (int i = 0; i < 80; i++)
        {
            transform.position = Vector3.SlerpUnclamped(transform.position, position, 6 * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }

    }
    



}
