using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerrisWHeelGondola : MonoBehaviour
{
    private HingeJoint hj;

    // Start is called before the first frame update
    void Start()
    {
        hj = GetComponent<HingeJoint>();

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Opponent")
        {
            Vector3 myangle = collision.transform.position - transform.position;
            Vector3 angle = Vector3.Cross(transform.forward, myangle);


            float dir = Vector3.Dot(angle, transform.up);

            if(dir > 0)
            {
                JointMotor j = new JointMotor();
                j.targetVelocity = 50;
                hj.motor = j;
            }
            else
            {
                JointMotor j = new JointMotor();
                j.targetVelocity = -50;
                hj.motor = j;
            }
            JointMotor m = new JointMotor();
            m.targetVelocity = 0;
            hj.motor = m;

        }
    }
}
