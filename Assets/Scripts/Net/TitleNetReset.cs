using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class TitleNetReset : MonoBehaviour
{
    [Header("Client")]
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 25565;
    static UdpClient udpc;
    //bool dataFlag = false;
    long timer = 0;
    private bool udpRunnig = true;

    public char playerSide = 'A';

    byte[] udpSend = new byte[] { 0x00 };
    byte[] udpGet = new byte[] { 0x00 };

    byte failCount = 0;

    Stopwatch stopWatch = new Stopwatch();

    long timeSpan = 0;

    void Start()
    {
        DebugController dbctr = GameObject.FindGameObjectWithTag("DebugCtrl").GetComponent<DebugController>();

        if (dbctr.ip != string.Empty) ip = dbctr.ip;
        if (dbctr.port != 0) port = dbctr.port;
        if (dbctr.playerSide != '0') playerSide = dbctr.playerSide;

        udpc = new UdpClient(ip, port);
        udpc.Client.ReceiveTimeout = 1000;

        byte[] buffer = new byte[2];

        //ResetFlag
        buffer[0] = 0x54;
        buffer[1] = (byte)playerSide;

        udpSend = buffer;

    }

    private void Update()
    {
        if (timer < 33)
        {
            timer++;
        }
        else
        {
            if (udpGet[0] == 0x00)
                SendGetData();
        }
    }

    void SendGetData()
    {
        //Debug.Log("threding");
        try
        {
            IPEndPoint ep = null;
            stopWatch.Reset();
            stopWatch.Start();


            udpc.Send(udpSend, udpSend.Length);


            udpGet = udpc.Receive(ref ep);

            stopWatch.Stop();
            timeSpan = stopWatch.ElapsedMilliseconds;
            //if (timeSpan < 33)
            //    Thread.Sleep((int)(33 - timeSpan));
            timer = 33 - timeSpan;

        }
        catch (SocketException ex)
        {
            Debug.LogError("Socket Exception: " + ex.Message);
            failCount++;
            if(failCount > 5)
            {
                udpGet = new byte[] { 0x4E };
            }
        }
    }
}
