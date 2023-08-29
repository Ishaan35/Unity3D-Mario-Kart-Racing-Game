using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trolley : MonoBehaviour
{
    public Transform path;
    public int currentWayPoint = 0;
    float y = 0;
    public float speed;
    private Rigidbody rb;

    public bool antiGravity = false;
    public bool isSubmarine = false;
    public LayerMask mask;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move();
        steer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform == path.GetChild(currentWayPoint))
        {
            currentWayPoint++;
            if(currentWayPoint == path.childCount -1)
            {
                Destroy(gameObject);
            }
        }
    }

    private void move()
    {
        Vector3 vel = transform.forward * speed;

        if (!antiGravity)
        {

            vel.y = rb.velocity.y;


            rb.AddForce(Vector3.down * 20000 * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            rb.AddRelativeForce(Vector3.down * 10000 * Time.deltaTime, ForceMode.Acceleration);
        }
        rb.velocity = vel;


        if (isSubmarine)
        {
            //ground normal rotation
            Ray ground = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            if (Physics.Raycast(ground, out hit, 5, mask) && (hit.normal.y > 0.5f || antiGravity))
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up * 5, hit.normal) * transform.rotation, 3 * Time.deltaTime);
            }
        }
    }

    void steer()
    {
        Vector3 lookat = path.GetChild(currentWayPoint).position;


        //angle calc
        Vector3 myangle = lookat - transform.position;
        Vector3 angle = Vector3.Cross(transform.forward, myangle);
        float dir = Vector3.Dot(angle, transform.up);


        float none = 0;



        y = Mathf.SmoothDamp(y, dir, ref none, 2.5f * Time.deltaTime);
        transform.Rotate(0, y / 20, 0, Space.Self);
    }

    public IEnumerator hitByPowerup()
    {
        Vector3 position = transform.position;
        Vector3 endPos = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);

        for (int i = 0; i < 15; i++)
        {
            transform.position = Vector3.SlerpUnclamped(transform.position, endPos, 6 * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }
        for (int i = 0; i < 80; i++)
        {
            transform.position = Vector3.SlerpUnclamped(transform.position, position, 6 * Time.deltaTime);
            yield return new WaitForSeconds(0.02f);
        }

    }
}
