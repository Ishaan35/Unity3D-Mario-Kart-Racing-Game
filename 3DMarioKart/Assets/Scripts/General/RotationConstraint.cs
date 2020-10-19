
using UnityEngine;
using System.Collections;
    
public class RotationConstraint : MonoBehaviour {

    public enum ConstraintAxis{X = 0, Y = 1, Z = 2};
    public ConstraintAxis axis; // Rotation around this axis is constrained
    public float min; // Relative value in degrees
    public float max; // Relative value in degrees

	private Transform thisTransform;
    private Vector3 rotateAround;
    private Quaternion minQuaternion;
    private Quaternion maxQuaternion;
    private float range;




    void Start()
    {

		thisTransform = transform;
	    // Set the axis that we will rotate around
	    switch ( axis )
	    {
	    case ConstraintAxis.X:
	    rotateAround = Vector3.right;
	    break;
	    case ConstraintAxis.Y:
	    rotateAround = Vector3.up;
	    break;
	    case ConstraintAxis.Z:
	    rotateAround = Vector3.forward;
	    break;
	    }
	    // Set the min and max rotations in quaternion space
	    var axisRotation = Quaternion.AngleAxis( thisTransform.localRotation.eulerAngles[ (int)axis ], rotateAround );
		minQuaternion = axisRotation * Quaternion.AngleAxis( min, rotateAround );
	    maxQuaternion = axisRotation * Quaternion.AngleAxis( max, rotateAround );
	    range = max - min;
    }
    // We use LateUpdate to grab the rotation from the Transform after all Updates from
    // other scripts have occured

    void LateUpdate()
    {
    // We use quaternions here, so we don't have to adjust for euler angle range [ 0, 360 ]
    var localRotation = thisTransform.localRotation;
    var axisRotation = Quaternion.AngleAxis( localRotation.eulerAngles[ (int)axis ], rotateAround );
    var angleFromMin = Quaternion.Angle( axisRotation, minQuaternion );
    var angleFromMax = Quaternion.Angle( axisRotation, maxQuaternion );
    if ( angleFromMin <= range && angleFromMax <= range )
    return; // within range
    else
    {
    // Let's keep the current rotations around other axes and only
    // correct the axis that has fallen out of range.
    var euler = localRotation.eulerAngles;
    if ( angleFromMin > angleFromMax )
    euler[ (int)axis ] = maxQuaternion.eulerAngles[ (int)axis ];
    else
    euler[ (int)axis ] = minQuaternion.eulerAngles[ (int)axis ];
    thisTransform.localEulerAngles = euler;
    }

  }
}
