using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AutoPilot base class. To be overloaded by all autopilot input providers.
/// </summary>
public abstract class AutoPilot : MonoBehaviour
{
    public abstract float GetThrottle();
    public abstract float GetPitch();
    public abstract float GetYaw();
    public abstract float GetRoll();
    public abstract bool GetFlaps();
    public abstract bool GetBrake();
}
