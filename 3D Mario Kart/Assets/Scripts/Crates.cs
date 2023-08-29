using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crates : MonoBehaviour
{
    private Rigidbody rb;
    private Collider bc;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator destroy()
    {
        if(gameObject.name.IndexOf("Crash") >= 0)
        {
            rb.isKinematic = true;
            bc.enabled = false;
            transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
            transform.GetChild(2).GetComponent<ParticleSystem>().Play();
            transform.GetChild(3).GetComponent<ParticleSystem>().Play();
            GetComponent<AudioSource>().Play();
            transform.GetChild(4).GetComponent<AudioSource>().Play();
        }
        else if(gameObject.name.IndexOf("DK") >= 0)
        {
            rb.isKinematic = true;
            bc.enabled = false;
            transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().enabled = false;
            transform.GetChild(2).GetComponent<SkinnedMeshRenderer>().enabled = false;
            transform.GetChild(3).GetComponent<ParticleSystem>().Play();
            transform.GetChild(4).GetComponent<ParticleSystem>().Play();
            GetComponent<AudioSource>().Play();
            transform.GetChild(5).GetComponent<AudioSource>().Play();

        }




        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
