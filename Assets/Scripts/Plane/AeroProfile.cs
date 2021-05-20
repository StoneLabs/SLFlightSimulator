using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AeroProfile : SubscribeableSettings
{
    public AnimationCurve CA_Curve;
    public AnimationCurve CD_Curve;

    [Range(0, 1.0f)]
    //[Description("More realistic, but can lead to oscillation")]
    public float RotationWindImpact = 0.1f;
}
