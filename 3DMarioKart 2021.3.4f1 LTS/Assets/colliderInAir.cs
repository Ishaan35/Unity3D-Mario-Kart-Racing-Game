using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderInAir : MonoBehaviour
{
    public float force = 15000;
    public float groundForce = 0;
    public bool isForAir = true;
    public bool isForRaceEnd = true;
    public bool isONLYforRaceEnd = false;
    public bool relativeForce = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        
    }
}
