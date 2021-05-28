using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class UnitConverter
{
    public static float Meter2Feet(float meter)
    {
        return meter / 0.3048f;
    }
    public static float MeterPerSecond2Knots(float meter)
    {
        return meter * (900.0f/463.0f);
    }
}
