using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    private float origStartPosX;
    private Vector3 startPos;

    float value = 0;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float none2 = 0;
        value = Mathf.SmoothDamp(value, Input.GetAxis("Horizontal"), ref none2, 7 * Time.deltaTime);
        Vector3 none = Vector3.zero;

        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(startPos.x + value * 10, startPos.y, startPos.z), ref none, 12 * Time.deltaTime);
    }
}
