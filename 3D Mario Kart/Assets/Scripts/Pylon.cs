using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pylon : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(Vector3.down * 5000 * Time.deltaTime, ForceMode.Acceleration);
    }

    private IEnumerator OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Opponent")
        {
            Vector3 direction = transform.position - collision.contacts[0].point; //angle of collision
            float force = 200;
            float upforce = 120;
            for(int i = 0; i < 60; i++)
            {
                rb.AddForce(direction * force * Time.deltaTime, ForceMode.Impulse);
                force-=2;
                rb.AddForce(Vector3.up * upforce * Time.deltaTime, ForceMode.Impulse);
                upforce-= 7f;
                yield return new WaitForSeconds(0.01f);
            }
        }
    }
}
