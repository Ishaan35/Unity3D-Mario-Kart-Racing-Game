using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
    public Transform planet;
    float w = 30;
    float velocity = 5;
    float radius = 11;
    float jumpForce = 5;
    float g = 10;
    bool jump = false;
    Rigidbody rigidbody;
	// Use this for initialization
	void Start () {
        rigidbody = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        float h = Input.GetAxis("Horizontal");
        if (Mathf.Abs(h) > 0.01f)
        {
            transform.Rotate(Vector3.up * w * h * Time.deltaTime);
        }
        float v = Input.GetAxis("Vertical");
        if (Mathf.Abs(v) > 0.01f)
        {
            transform.Translate(Vector3.forward * velocity * v * Time.deltaTime,Space.Self);
        }
        if (Input.GetKeyDown(KeyCode.Space) && !jump)
        {
            jump = true;
            rigidbody.AddRelativeForce(Vector3.up * jumpForce,ForceMode.Impulse);
        }
        Vector3 up = Vector3.Normalize(transform.position - planet.position);
        Vector3 forward = Vector3.Cross(transform.right, up);
        Quaternion q = Quaternion.LookRotation(forward, up);
        transform.rotation = q;
        float distance2 = (transform.position - planet.position).sqrMagnitude;
        if (distance2 <= radius * radius)
        {
            jump = false;
            transform.position = radius * transform.up + planet.position;
        }
        else
        {
            jump = true;
            rigidbody.AddRelativeForce(Vector3.down * g / distance2, ForceMode.Impulse);
        }
	}
}
