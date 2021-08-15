using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkedAutoPilot : AutoPilot
{
    public const byte MSK_BRAKE     = 0b00000001;

    public PlaneManager plane;

    public string ipAddress = "127.0.0.1";
    public ushort localPort = 11337;
    public ushort remotePort = 11338;

    UdpClient client = new UdpClient();
    IPEndPoint endpoint;

    private float throttle = 0.0f;
    private float pitch = 0.0f;
    private float roll = 0.0f;
    private float yaw = 0.0f;
    private bool brake = false;

    void Start()
    {
        // Receiver
        Task.Run(async () =>
        {
            UdpClient server = new UdpClient(localPort);
            while (true)
            {
                var receivedResults = await server.ReceiveAsync();
                String data = Encoding.ASCII.GetString(receivedResults.Buffer);

                float[] floats = new float[4];
                byte[] flags = new byte[1];

                if (receivedResults.Buffer.Length != floats.Length * sizeof(float) + flags.Length * sizeof(byte))
                {
                    Debug.LogError("AutoPilot Input is not of length 4*float!");
                    continue;
                }

                Buffer.BlockCopy(receivedResults.Buffer, 0, floats, 0, floats.Length * sizeof(float));
                Buffer.BlockCopy(receivedResults.Buffer, floats.Length * sizeof(float), flags, 0, flags.Length * sizeof(byte));

                this.throttle = floats[0];
                this.pitch = floats[1];
                this.roll = floats[2];
                this.yaw = floats[3];
                this.brake = (flags[0] & MSK_BRAKE) > 0;
            }
        });
    }

    void Update()
    {
        // Sender
        endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), remotePort);
        client.Connect(endpoint);


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
}
