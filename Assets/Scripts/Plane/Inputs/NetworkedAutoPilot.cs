using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Networked Autopilot. Communicates with autopilot using UDP.
/// </summary>
public class NetworkedAutoPilot : AutoPilot
{
    // Bit masks
    public const byte MSK_BRAKE     = 0b00000001;
    public const byte MSK_FLAPS     = 0b00000010;
    

    public PlaneManager plane;

    // Connection parameters
    public string ipAddress = "127.0.0.1";
    public ushort localPort = 11337;
    public ushort remotePort = 11338;

    UdpClient client = new UdpClient();
    IPEndPoint endpoint;

    // Input values saved for request by planeManager
    private float throttle = 0.0f;
    private float pitch = 0.0f;
    private float roll = 0.0f;
    private float yaw = 0.0f;
    private bool brake = false;
    private bool flaps = false;

    void Start()
    {
        // Receiver
        Task.Run(async () =>
        {
            UdpClient server = new UdpClient(localPort);
            while (true)
            {
                // Get package
                var receivedResults = await server.ReceiveAsync();

                // Convert to floats and flags as per specification
                float[] floats = new float[4];
                byte[] flags = new byte[1];

                // Check length before conversion
                if (receivedResults.Buffer.Length != floats.Length * sizeof(float) + flags.Length * sizeof(byte))
                {
                    Debug.LogError("AutoPilot Input is not of length 4*float!");
                    continue;
                }

                // Convert using memcpy
                Buffer.BlockCopy(receivedResults.Buffer, 0, floats, 0, floats.Length * sizeof(float));
                Buffer.BlockCopy(receivedResults.Buffer, floats.Length * sizeof(float), flags, 0, flags.Length * sizeof(byte));

                // Set input values for later request
                this.throttle = floats[0];
                this.pitch = floats[1];
                this.roll = floats[2];
                this.yaw = floats[3];
                this.brake = (flags[0] & MSK_BRAKE) > 0; // Apply bitmask
                this.flaps = (flags[0] & MSK_FLAPS) > 0; // Apply bitmask
            }
        });
    }

    void Update()
    {
        // Sender
        endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), remotePort);
        client.Connect(endpoint);

        // Data object to be sent
        float[] data =
        {
            plane.transform.position.x,
            plane.transform.position.y,
            plane.transform.position.z,
            plane.transform.rotation.eulerAngles.x,
            plane.transform.rotation.eulerAngles.y,
            plane.transform.rotation.eulerAngles.z,
            plane.physics.AirSpeed.magnitude,
            plane.physics.GForce.magnitude,
            plane.physics.DryMass,
            plane.fuelCapacity,
            plane.fuelLevel,
            plane.fuelWeight,
            plane.physics.engines[0].RPM,
            plane.environment.CalculatePressure(plane.physics.body.position.y),
            plane.environment.CalculateDensity(plane.physics.body.position.y),
            plane.environment.CalculateTemperature(plane.physics.body.position.y),
        };

        // Send data in byte[] buffer
        byte[] bytes = new byte[data.Length * 4];
        Buffer.BlockCopy(data, 0, bytes, 0, data.Length * 4);

        client.Send(bytes, bytes.Length);
    }


    public override float GetThrottle()
    {
        return this.throttle;
    }

    public override float GetPitch()
    {
        return this.pitch;
    }

    public override float GetRoll()
    {
        return this.roll;
    }

    public override float GetYaw()
    {
        return this.yaw;
    }

    public override bool GetBrake()
    {
        return this.brake;
    }

    public override bool GetFlaps()
    {
        return this.flaps;
    }
}
