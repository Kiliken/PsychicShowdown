using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class NetController : MonoBehaviour
{
    [Header("Client")]
    [SerializeField] string ip = "127.0.0.1";
    [SerializeField] int port = 25565;
    static UdpClient udpc;
    //bool dataFlag = false;
    float timer = 0;
    private Thread udpDataThread;
    private bool udpRunnig = true;

    private NetData data;

    private NetData thisSideData;

    private MapGenerator map;

    [SerializeField]
    private Transform gameUI;

    static volatile byte[] udpSend = new byte[] { 0x4E };
    static volatile byte[] udpGet = new byte[] { 0x4E };



    [Header("Players")]
    public char playerSide = 'A';
    [SerializeField] Transform player;
    [SerializeField] Transform playerOther;

    private Player playerScript;

    private byte rgCheckLeft = 0x00;
    private byte rgCheckRight = 0x00;

    private ThrowableObject leftObj;
    private ThrowableObject rightObj;

    private static void SendGetData()
    {
        long timeSpan = 0;
        Stopwatch stopWatch = new Stopwatch();
        Debug.Log("ThreadStarted");

        while (true)
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
                if (timeSpan < 33)
                    Thread.Sleep((int)(33 - timeSpan));

            }
            catch (SocketException ex)
            {
                Debug.LogError("Socket Exception: " + ex.Message);
                // Handle errors gracefully, e.g., log or attempt to reconnect
            }
        }
    }

    private void Awake()
    {
        player = (playerSide == 'A' ? GameObject.Find("Player").transform : GameObject.Find("Player2").transform);
        playerOther = (playerSide == 'A' ? GameObject.Find("Player2").transform : GameObject.Find("Player").transform);
        Destroy(playerOther.GetComponent<Player>());

        gameUI.GetChild(playerSide == 'A' ? 2 : 3).gameObject.SetActive(true);

        //player.GetComponent<Player>().playerNo = 1;
        playerOther.gameObject.AddComponent<NETPlayer>();
        //Destroy(playerOther.GetComponent<Rigidbody>());
        playerOther.GetComponent<Rigidbody>().isKinematic = true;

        player.transform.GetComponent<Rigidbody>().useGravity = true;
        GameObject.Find((playerSide == 'A' ? "PlayerCam2" : "PlayerCam")).SetActive(false);
        EnablePlayer(player);
    }


    void Start()
    {
        playerScript = player.GetComponent<Player>();
        thisSideData = new NetData();
        map = FindAnyObjectByType<MapGenerator>();

        udpc = new UdpClient(ip, port);
        udpc.Client.ReceiveTimeout = 1000;
        udpDataThread = new Thread(new ThreadStart(SendGetData));
        udpDataThread.Start();
    }


    void Update()
    {

        PrepareNetData();
        udpSend = NetManager.ParseByte(playerSide, thisSideData);

        if (udpGet[0] != 0x4E)
        {
            data = NetManager.RetriveByte(udpGet);
            UpdatePosition();


            if ((byte)(data.leftHand - rgCheckLeft) != 0)
            {
                //otherPlayer.ThrowLeftObject();
                if (data.leftObjId != 0)
                {
                    Debug.Log($"Enemy Left Object N{data.leftObjId} Taken");
                    leftObj = FindObjectById(data.leftObjId);
                    //grab LeftObject here
                }
                else
                {
                    Debug.Log("Enemy Left Object Throwed");
                    //throw LeftObject here
                    
                    leftObj = null;
                }


                rgCheckLeft++;

                if (rgCheckLeft >= 0x10)
                    rgCheckLeft -= 0x10;
            }



            if ((byte)(data.rightHand - rgCheckRight) != 0)
            {
                //otherPlayer.ThrowRightObject();

                if (data.rightObjId != 0)
                {
                    Debug.Log($"Enemy Right Object N{data.rightObjId} Taken");
                    rightObj = FindObjectById(data.leftObjId);
                    //grab RightObject here
                }
                else
                {
                    Debug.Log("Enemy Right Object Throwed");
                    //throw RightObject here
                    
                    rightObj = null;
                }

                rgCheckRight++;

                if (rgCheckRight >= 0x10)
                    rgCheckRight -= 0x10;
            }

        }
    }

    void OnDestroy()
    {
        Debug.Log("Thread shutdown");
        udpDataThread.Abort();
    }

    void UpdatePosition()
    {
        playerOther.position = Vector3.Lerp(playerOther.position, new Vector3(data.posX, data.posY, data.posZ), Time.deltaTime * 10f);
        playerOther.GetChild(0).eulerAngles = new Vector3(0, data.rotBody, 0);
    }

    void EnablePlayer(Transform side)
    {
        foreach (Behaviour behaviour in side.GetComponentsInChildren<Behaviour>())
            behaviour.enabled = true;
    }

    ThrowableObject FindObjectById(ushort id)
    {
        for (int i = 0; i < map.objects.Count; i++)
        {
            if (map.objects[i].GetComponent<ThrowableObject>().objectID == id)
                return map.objects[i].GetComponent<ThrowableObject>();
        }
        return null;
    }

    void PrepareNetData()
    {
        thisSideData.posX = player.transform.position.x;
        thisSideData.posY = player.transform.position.y;
        thisSideData.posZ = player.transform.position.z;
        thisSideData.rotBody = player.transform.GetChild(0).eulerAngles.y;
        thisSideData.leftHand = playerScript.leftHandFlag;
        thisSideData.leftObjId = playerScript.leftObjRef;
        thisSideData.rightHand = playerScript.rightHandFlag;
        thisSideData.rightObjId = playerScript.rightObjRef;
        thisSideData.hp = (sbyte)playerScript.hp;
    }
}
