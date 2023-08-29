using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapCounter : MonoBehaviour
{
    public int LAPCOUNT = 1;
    public Transform checkpoints; //parent
    [HideInInspector]
    public bool[] checkpointsVisited; //visited or not array
    [HideInInspector]
    public int currentCheckpointVal = 0;
    [HideInInspector]
    public int totalCheckpointVal = 0;
    [HideInInspector]
    public float distanceToNextCheckpoint;
    public int Position = 0;

    public int endPosition = 0;

    public Text[] UIDisplayLap;

    private RACE_MANAGER rm;
    // Start is called before the first frame update
    void Start()
    {
        checkpointsVisited = new bool[checkpoints.childCount];
        for(int i = 0; i < checkpointsVisited.Length; i++)
        {
            checkpointsVisited[i] = false;
        }
        rm = GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>();
    }

    // Update is called once per frame
    void Update()
    {

        if (currentCheckpointVal >= checkpoints.childCount)
        {
            currentCheckpointVal = 0;
        }

        calculateDist();


        if (gameObject.tag == "Player" && LAPCOUNT <= rm.MAXLAPS)
        {
            UIDisplayLap[0].text = LAPCOUNT + "/" + rm.MAXLAPS;
            UIDisplayLap[1].text = LAPCOUNT + "/" + rm.MAXLAPS;

        }
        if (gameObject.tag == "Player" && LAPCOUNT > rm.MAXLAPS && endPosition == 0)
        {
            RACE_MANAGER.RACE_COMPLETED = true;
            endPosition = Position;
            GetComponent<Player>().stopDrift();
            StartCoroutine(stopDriftRot());
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "NextLapCollider")
        {
            if (checkAllPoints())
            {
                LAPCOUNT++;
                for (int i = 0; i < checkpointsVisited.Length; i++)
                {
                    checkpointsVisited[i] = false;
                }
                if(gameObject.tag != "Player")
                {
                    int max = RACE_MANAGER.allPaths.childCount;

                    int rand = Random.Range(0, max);

                    GetComponent<ComputerDriver>().path = RACE_MANAGER.allPaths.GetChild(rand); //assigning a new path
                }

            }
        }
        else if (currentCheckpointVal < checkpoints.childCount && other.transform == checkpoints.GetChild(currentCheckpointVal))
        {
            checkpointsVisited[currentCheckpointVal] = true;
            currentCheckpointVal++;
            totalCheckpointVal++;

            
        }





    }

    bool checkAllPoints()
    {
        for(int i = 0; i < checkpointsVisited.Length; i++)
        {
            if(checkpointsVisited[i] == false)
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator stopDriftRot()
    {
 
            for (int i = 0; i < 120; i++)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localRotation = Quaternion.Lerp(transform.GetChild(0).localRotation, Quaternion.Euler(0, 0, 0), 8f * Time.deltaTime);
            }
    }

    void calculateDist()
    {
        Vector3 trackDirection;

        if (currentCheckpointVal-1 >= 0)
        {
            trackDirection = checkpoints.GetChild(currentCheckpointVal).position - checkpoints.GetChild(currentCheckpointVal - 1).position;
        }
        else
        {
            trackDirection = checkpoints.GetChild(currentCheckpointVal).position - checkpoints.GetChild(checkpoints.childCount - 1).position;
        }

        Vector3 playerToGoal = transform.position - checkpoints.GetChild(currentCheckpointVal).transform.position;
        Vector3 projectedPlayerToGoal = Vector3.Project(playerToGoal, trackDirection);
        distanceToNextCheckpoint = projectedPlayerToGoal.magnitude;


        //distanceToNextCheckpoint = Vector3.Distance(transform.position, checkpoints.GetChild(currentCheckpointVal).position);

    }
}
