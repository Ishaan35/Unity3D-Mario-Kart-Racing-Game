using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadAnim : MonoBehaviour
{
    public float maxDelay = 1.1f;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().enabled = false;
        StartCoroutine(StartAnim());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartAnim()
    {
        float rand = Random.Range(0, maxDelay);
        yield return new WaitForSeconds(rand);
        GetComponent<Animator>().enabled = true;
    }
}
