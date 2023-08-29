using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlueShellRaycast : MonoBehaviour
{
    public Ray upRay = new Ray();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        upRay = new Ray(transform.GetChild(0).position, transform.up);

    }

}

