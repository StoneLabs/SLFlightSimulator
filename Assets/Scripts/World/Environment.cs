using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    private const float GRAVITATIONAL_ACCELERATION = 9.80665f;
    private const float GAS_CONSTANT = 8.3144598f;
    private const float MOLAR_MASS_AIR = 0.0289644f;

    private class Subscript
    {
        public readonly float ReferenceLevel;
        public readonly float ReferencePressure;
        public readonly float ReferenceTemperature;
        public readonly float TemperatureLapseRate;
        public readonly float MassDensity;

        public Subscript(float referenceLevel, float referencePressure, float referenceTemperature, float temperatureLapseRate, float massDensity)
        {
            this.ReferenceLevel = referenceLevel;
            this.ReferencePressure = referencePressure;
            this.ReferenceTemperature = referenceTemperature;
            this.TemperatureLapseRate = temperatureLapseRate;
            this.MassDensity = massDensity;
        }
    }

    private static readonly Subscript[] subscripts = new Subscript[] {
        new Subscript(0,        101325.00f,     288.15f,    -0.0065f,   1.2250f),
        new Subscript(11000,    22632.10f,      216.65f,    +0,         0.36391f),
        new Subscript(20000,    5474.89f,       216.65f,    +0.001f,    0.08803f),
        new Subscript(32000,    868.02f,        228.65f,    +0.0028f,   0.01322f),
        new Subscript(47000,    110.91f,        270.65f,    +0,         0.00143f),
        new Subscript(51000,    66.94f,         270.65f,    -0.0028f,   0.00086f),
        new Subscript(71000,    3.96f,          214.65f,    -0.002f,    0.000064f),
    };

    private static Subscript GetSubscript(float altitude)
    {
        if (altitude < 0)
            throw new ArgumentException("No subscript defined for altitude < 0");

        Subscript retVal = null;
        foreach (Subscript subscript in subscripts)
        {
            if (subscript.ReferenceLevel <= altitude)
                retVal = subscript;
            else
                return retVal ?? throw new Exception("Illegal state in subscript getter");
        }

        return retVal ?? throw new Exception("Illegal state in subscript getter");
    }

    public float CalculateTemperature(float altitude)
    {
        Subscript subscript = GetSubscript(altitude);

        return subscript.ReferenceTemperature + subscript.TemperatureLapseRate * (altitude - subscript.ReferenceLevel);
    }

    public float CalculatePressure(float altitude)
    {
        Subscript subscript = GetSubscript(altitude);

        if (subscript.TemperatureLapseRate == 0)
            return subscript.ReferencePressure * Mathf.Exp(
                (-GRAVITATIONAL_ACCELERATION * MOLAR_MASS_AIR * (altitude - subscript.ReferenceLevel)) / (GAS_CONSTANT * subscript.ReferenceTemperature));
        else
            return subscript.ReferencePressure * Mathf.Pow(
                (subscript.ReferenceTemperature + subscript.TemperatureLapseRate * (altitude - subscript.ReferenceLevel)) / subscript.ReferenceTemperature,
                (-GRAVITATIONAL_ACCELERATION * MOLAR_MASS_AIR) / (GAS_CONSTANT * subscript.TemperatureLapseRate));
    }
    public float CalculateDensity(float altitude)
    {
        Subscript subscript = GetSubscript(altitude);

        if (subscript.TemperatureLapseRate == 0)
            return subscript.MassDensity * Mathf.Exp(
                (-GRAVITATIONAL_ACCELERATION * MOLAR_MASS_AIR * (altitude - subscript.ReferenceLevel)) / (GAS_CONSTANT * subscript.ReferenceTemperature));
        else
            return subscript.MassDensity * Mathf.Pow(
                subscript.ReferenceTemperature / (subscript.ReferenceTemperature + subscript.TemperatureLapseRate * (altitude - subscript.ReferenceLevel)),
                1 + (GRAVITATIONAL_ACCELERATION * MOLAR_MASS_AIR) / (GAS_CONSTANT * subscript.TemperatureLapseRate));
    }
}
