using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapCounter : MonoBehaviour
{
    public int LAPCOUNT = 1;
    public Transform checkpoints; //parent
    private bool[] checkpointsVisited; //visited or not array
    [HideInInspector]
    public int currentCheckpointVal = 0;
    [HideInInspector]
    public int totalCheckpointVal = 0;
    public int Position = 0;

    public int endPosition = 0;

    public Text[] UIDisplayLap;
    // Start is called before the first frame update
    void Start()
    {
        checkpointsVisited = new bool[checkpoints.childCount];
        for(int i = 0; i < checkpointsVisited.Length; i++)
        {
            checkpointsVisited[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "Player" && LAPCOUNT < 4)
        {
            UIDisplayLap[0].text = LAPCOUNT + "/3";
            UIDisplayLap[1].text = LAPCOUNT + "/3";
        }
        if (gameObject.tag == "Player" && LAPCOUNT > 3 && endPosition == 0)
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
                currentCheckpointVal = 0;
                for (int i = 0; i < checkpointsVisited.Length; i++)
                {
                    checkpointsVisited[i] = false;
                }     
            }
        }
        else if (other.transform == checkpoints.GetChild(currentCheckpointVal))
        {
            checkpointsVisited[currentCheckpointVal] = true;
            currentCheckpointVal++;
            totalCheckpointVal++;
        }
        else if (other.transform == checkpoints.GetChild(currentCheckpointVal + 1))
        {
            checkpointsVisited[currentCheckpointVal] = true;
            checkpointsVisited[currentCheckpointVal+1] = true;
            currentCheckpointVal += 2;
            totalCheckpointVal += 2;
        }
        else if (other.transform == checkpoints.GetChild(currentCheckpointVal + 2))
        {
            checkpointsVisited[currentCheckpointVal] = true;
            checkpointsVisited[currentCheckpointVal + 1] = true;
            checkpointsVisited[currentCheckpointVal + 2] = true;
            currentCheckpointVal += 3;
            totalCheckpointVal += 3;
        }
        else if (other.transform == checkpoints.GetChild(currentCheckpointVal + 3))
        {
            checkpointsVisited[currentCheckpointVal] = true;
            checkpointsVisited[currentCheckpointVal + 1] = true;
            checkpointsVisited[currentCheckpointVal + 2] = true;
            checkpointsVisited[currentCheckpointVal + 3] = true;
            currentCheckpointVal += 4;
            totalCheckpointVal += 4;
        }
        else if (other.transform == checkpoints.GetChild(currentCheckpointVal + 4))
        {
            checkpointsVisited[currentCheckpointVal] = true;
            checkpointsVisited[currentCheckpointVal + 1] = true;
            checkpointsVisited[currentCheckpointVal + 2] = true;
            checkpointsVisited[currentCheckpointVal + 3] = true;
            checkpointsVisited[currentCheckpointVal + 4] = true;
            currentCheckpointVal += 5;
            totalCheckpointVal += 5;
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
}
