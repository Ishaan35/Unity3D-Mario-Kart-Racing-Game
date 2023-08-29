using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChainChompBall : MonoBehaviour
{
    private Rigidbody rb;
    private ConstantForce cf;
    public float force;
    Vector3 none = Vector3.zero;

    AudioSource audiosrc;
    public bool SceneEntry = false;

    float time;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        audiosrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        cf.force = Vector3.SmoothDamp(cf.force, new Vector3(0, force, 0), ref none, 80 * Time.deltaTime);

        transform.GetChild(0).Rotate(0, 5 * Time.deltaTime, 45 * Time.deltaTime, Space.Self);

        time += Time.deltaTime;

        if (SceneEntry)
        {
            if (time < 5)
            {
                audiosrc.maxDistance = 4000;
            }
            else
            {
                audiosrc.maxDistance = 500;
            }
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            if(cf.force.y < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * -1, rb.velocity.z);
            }
            cf.force = new Vector3(0, -force, 0);
            audiosrc.Play();

            if (Vector3.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < 150)
            {
                try
                {
                    Camera.main.GetComponent<Animator>().SetTrigger("Shake2");
                }
                catch (Exception e)
                {

                }
            }

        }
    }
}
