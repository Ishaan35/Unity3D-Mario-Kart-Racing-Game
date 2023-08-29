using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float torque = 0;
    private Rigidbody rb;

    public float timer = 1f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < 0.1)
        {
            spin();
            timer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" || other.gameObject.tag == "Opponent")
        {
            timer = 0;
        }
    }

    void spin()
    {
        rb.AddTorque(transform.forward * torque);
    }
}
