using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrolleySystem : MonoBehaviour
{
    public Transform trolley;
    public Material[] materials;
    private int materialNum = 0;

    private float timeSinceStartedRace = 0;
    public float interval = 30;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (RACE_MANAGER.RACE_STARTED)
        {
            timeSinceStartedRace += Time.deltaTime;
            if(timeSinceStartedRace > interval)
            {
                timeSinceStartedRace = 0;
                Transform clone = Instantiate(trolley, transform.position, transform.rotation);
                if(materials.Length > 0)
                {
                    clone.GetChild(1).GetComponent<Renderer>().material = materials[materialNum];
                }
                clone.GetComponent<Trolley>().enabled = true;
                materialNum++;
                if(materialNum >= materials.Length)
                {
                    materialNum = 0;
                }
            }
        }
    }

    public void InstantiateFirstTram()
    {
        Transform clone = Instantiate(trolley, transform.position, transform.rotation);
        clone.GetChild(1).GetComponent<Renderer>().material = materials[materialNum];
        clone.GetComponent<Trolley>().enabled = true;

        materialNum++;
        if (materialNum >= materials.Length)
        {
            materialNum = 0;
        }
    }
}
