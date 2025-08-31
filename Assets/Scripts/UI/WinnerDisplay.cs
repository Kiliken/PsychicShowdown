using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System;
using Debug = UnityEngine.Debug;

public class WinnerDisplay : MonoBehaviour
{
    [Header("Client")]
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 25565;
    static UdpClient udpc;
    //bool dataFlag = false;
    long timer = 0;
    private bool udpRunnig = true;

    public char playerSide = 'A';

    byte[] udpSend = new byte[] { 0x4E };
    byte[] udpGet = new byte[] { 0x4E };

    [Header("Title Stuff")]
    TextMeshProUGUI winnerText;
    [SerializeField] Transform winner;
    [SerializeField] Transform loser;
    [SerializeField] Transform torii;

    Stopwatch stopWatch = new Stopwatch();

    long timeSpan = 0;


    void Awake()
    {
        DebugController dbctr = GameObject.FindGameObjectWithTag("DebugCtrl").GetComponent<DebugController>();

        if (dbctr.ip != string.Empty) ip = dbctr.ip;
        if (dbctr.port != 0) port = dbctr.port;
        if (dbctr.playerSide != '0') playerSide = dbctr.playerSide;
    }

    void Start()
    {
        //GameData.winner = 1;
        winnerText = GetComponent<TextMeshProUGUI>();

        if (GameData.winner == 1)
        {
            winnerText.text = "プレイヤー１勝";
            winner.GetChild(0).gameObject.SetActive(true);
            loser.GetChild(1).gameObject.SetActive(true);
            torii.GetChild(0).gameObject.SetActive(true);
        }
        else if (GameData.winner == 2)
        {
            winnerText.text = "プレイヤー２勝";
            winner.GetChild(1).gameObject.SetActive(true);
            loser.GetChild(0).gameObject.SetActive(true);
            torii.GetChild(1).gameObject.SetActive(true);

        }
        else if (GameData.winner == 0)
        {
            winnerText.text = "引き分け";
            loser.GetChild(0).gameObject.SetActive(true);
            loser.GetChild(1).gameObject.SetActive(true);
            int leftPlayer = UnityEngine.Random.Range(0, 2);
            loser.GetChild(leftPlayer).transform.position = new Vector3(-82f, 21.1f, 58f);
            loser.GetChild(leftPlayer).transform.eulerAngles = new Vector3(100f, -1f, 65f);
        }

        udpc = new UdpClient(ip, port);
        udpc.Client.ReceiveTimeout = 1000;

        byte[] buffer = new byte[2];

        //ResetFlag
        buffer[0] = 0x57;
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
            SendGetData();
        }
    }

    public void ToTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen");
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
