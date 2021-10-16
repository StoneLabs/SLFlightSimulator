using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// Prevents slip of wheel due to lack of anisotropic friction
/// </summary>
class WheelSlipPreventer : MonoBehaviour
{
    public enum Axis { X, Y, Z }
    public PlaneManager manager;
    public TouchDownWheelDetector touchDownDetector;
    public AudioSource slipSound;

    public Axis slipAxis;
    public float CounterForceMagnitude = 1000;
    public float GizmosThrustDivider = 50;


    // Calculates local slip velocity
    public Vector3 LocalSlip
    {
        get
        {
            if (!touchDownDetector.IsTouchDown)
                return Vector3.zero;

            Vector3 worldVelocity = manager.physics.body.GetPointVelocity(transform.position) - manager.physics.body.velocity;
            Vector3 localVelocity = transform.worldToLocalMatrix * worldVelocity;
            switch (slipAxis)
            {
                case Axis.X:
                    return new Vector3(localVelocity.x, 0, 0);
                case Axis.Y:
                    return new Vector3(0, localVelocity.y, 0);
                case Axis.Z:
                    return new Vector3(0, 0, localVelocity.z);
                default:
                    throw new System.Exception("Illegal State in WheelSlipPreventer");
            }
        }
    }

    // Calculates force to counter slipping (functional but not physically accurate)
    public Vector3 CounterSlipForce
    {
        get
        {
            return transform.parent.rotation * -LocalSlip * CounterForceMagnitude;
        }
    }

    public void FixedUpdate()
    {
        // Apply force to wheel (force is zero if wheel is not on ground
        manager.physics.body.AddForceAtPosition(CounterSlipForce, transform.position);

        // Play sound on slip if slip speed exceedes threshold value 0.1m/s
        if (!touchDownDetector.IsTouchDown)
            slipSound.mute = true;  // No sound should play
        else
            slipSound.mute = !((manager.WheelBreaks && manager.physics.body.velocity.magnitude > 1f) || LocalSlip.magnitude >= 0.1f);
    }

    public void OnDrawGizmos()
    {
        // Visualize force
        GizmosUtils.SetT(transform);
        GizmosUtils.DrawArrow(Vector3.zero, CounterSlipForce, CounterSlipForce.magnitude / GizmosThrustDivider);
    }
}
