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

                var segments = data.Split('|');
                try
                {
                    this.throttle = float.Parse(segments[0]);
                    this.pitch = float.Parse(segments[1]);
                    this.roll = float.Parse(segments[2]);
                    this.yaw = float.Parse(segments[3]);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning("Invalid autopilot command received!");
                    return;
                }


            }
        });
    }

    void Update()
    {
        // Sender
        endpoint = new IPEndPoint(IPAddress.Parse(ipAddress), remotePort);
        client.Connect(endpoint);


        String data = "";
        void addData(object _data)
        {
            data += _data.ToString() + "|";
        }

        addData(plane.transform.position);
        addData(plane.transform.rotation.eulerAngles);
        addData(plane.physics.AirSpeed.magnitude);
        addData(plane.physics.GForce.magnitude);
        addData(plane.physics.DryMass);
        addData(plane.fuelCapacity);
        addData(plane.fuelLevel);
        addData(plane.fuelWeight);
        addData(plane.environment.CalculatePressure(plane.transform.position.y));
        addData(plane.environment.CalculateDensity(plane.transform.position.y));
        addData(plane.environment.CalculateTemperature(plane.transform.position.y));

        byte[] bytes = Encoding.ASCII.GetBytes(data); 
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
}
