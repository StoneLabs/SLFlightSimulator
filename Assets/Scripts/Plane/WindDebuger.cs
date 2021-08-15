using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindDebuger : MonoBehaviour
{
    public PlaneManager manager;

    public bool hide = false;
    public float range = 750;
    public float stepping = 50;
    public float lengthMultiplier = 25;
    public float yOffset = -100;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (hide)
            return;

        Vector3 planePos = new Vector3(
            this.transform.position.x - this.transform.position.x % stepping,
            this.transform.position.y,
            this.transform.position.z - this.transform.position.z % stepping);

        for (float x = -range; x <= range; x += stepping)
            for (float z = -range; z <= range; z += stepping)
            {
                Vector3 pos = planePos + new Vector3(x, yOffset, z);

                Vector3 wind = this.manager.environment.CalculateWind(pos);
                Color color = new Color(Math.Abs(wind.x) / wind.magnitude, Math.Abs(wind.y) / wind.magnitude, Math.Abs(wind.z) / wind.magnitude);
                GizmosUtils.DrawArrow(pos, wind, wind.magnitude * lengthMultiplier, color);
            }
#endif
    }
}
