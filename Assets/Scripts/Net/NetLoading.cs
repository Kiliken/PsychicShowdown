using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using Debug = UnityEngine.Debug;
using UnityEngine;

public class NetLoading : MonoBehaviour
{
    [Header("Client")]
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 25565;
    static UdpClient udpc;
    //bool dataFlag = false;
    long timer = 0;
    private bool udpRunnig = true;

    public char playerSide = 'A';

    private NetData data;

    private NetData thisSideData;

    private MapGenerator map;

    [SerializeField]
    private Transform gameUI;

    byte[] udpSend = new byte[] { 0x4E };
    byte[] udpGet = new byte[] { 0x4E };

    Stopwatch stopWatch = new Stopwatch();

    long timeSpan = 0;

    void Awake()
    {
        udpc = new UdpClient(ip, port);
        udpc.Client.ReceiveTimeout = 1000;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 33)
        {
            timer++;
        }
        else
        {
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
            // Handle errors gracefully, e.g., log or attempt to reconnect
        }
    }
}
