using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using Debug = UnityEngine.Debug;
using UnityEngine;
using System;
using System.Buffers;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

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

    LoadedDataStorage loadedData;

    [SerializeField]
    private Transform spinner;

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
        
        byte[] buffer = new byte[2];

        buffer[0] = 0x6C;
        buffer[1] = (byte)playerSide;

        udpSend = buffer;

        loadedData = GameObject.FindFirstObjectByType<LoadedDataStorage>();
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

        spinner.eulerAngles += Vector3.forward * 100 *Time.deltaTime;
        if (spinner.eulerAngles.z > 360f)
            spinner.eulerAngles -= Vector3.forward * 360f;

        if (udpGet[0] != 0x4E)
        {
            loadedData.seed = BitConverter.ToInt32(udpGet, 2);
            SceneManager.LoadScene("NetBeta");
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
