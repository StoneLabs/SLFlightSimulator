using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PlaneInput : MonoBehaviour
{
    public abstract float GetThrottle();
    public abstract float GetPitch();
    public abstract float GetYaw();
    public abstract float GetRoll();
    public abstract bool GetBreak();
    public abstract bool IsAutoPilot();
}
