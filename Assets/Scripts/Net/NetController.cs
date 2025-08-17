using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.SceneManagement;


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
    private NETPlayer netPlayerScript;

    private byte rgCheckLeft = 0x00;
    private byte rgCheckRight = 0x00;

    private ThrowableObject leftObj;
    private ThrowableObject rightObj;
    private Transform playerShootPos;

    private Rigidbody otherplayerRb;

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
        netPlayerScript = playerOther.gameObject.AddComponent<NETPlayer>();

        //Destroy(playerOther.GetComponent<Rigidbody>());
        otherplayerRb = playerOther.GetComponent<Rigidbody>();
        otherplayerRb.isKinematic = true;

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

        if (playerShootPos == null)
            playerShootPos = playerScript.shootPos;

        PrepareNetData();
        udpSend = NetManager.ParseByte(playerSide, thisSideData);

        if (udpGet[0] == 0x4B)
        {
            SceneManager.LoadScene("TitleScreen");
        }

        if (udpGet[0] == 0x41 || udpGet[0] == 0x42)
        {
            data = NetManager.RetriveByte(udpGet);
            UpdatePosition();
            UpdateShootPos();


            if ((byte)(data.leftHand - rgCheckLeft) != 0)
            {
                //otherPlayer.ThrowLeftObject();
                if (data.leftObjId != 0 && leftObj == null)
                {
                    Debug.Log($"Enemy Left Object N{data.leftObjId} Taken");
                    leftObj = FindObjectById(data.leftObjId);
                    //grab LeftObject here
                    netPlayerScript.GrabLeftObject(leftObj);
                }
                else
                {
                    Debug.Log("Enemy Left Object Throwed");
                    //throw LeftObject here
                    netPlayerScript.ThrowLeftObject();

                    leftObj = null;
                }


                rgCheckLeft++;

                if (rgCheckLeft >= 0x10)
                    rgCheckLeft -= 0x10;
            }



            if ((byte)(data.rightHand - rgCheckRight) != 0)
            {
                //otherPlayer.ThrowRightObject();

                if (data.rightObjId != 0 && rightObj == null)
                {
                    Debug.Log($"Enemy Right Object N{data.rightObjId} Taken");
                    rightObj = FindObjectById(data.rightObjId);
                    //grab RightObject here
                    netPlayerScript.GrabRightObject(rightObj);
                }
                else
                {
                    Debug.Log("Enemy Right Object Throwed");
                    //throw RightObject here
                    netPlayerScript.ThrowRightObject();

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
        otherplayerRb.position = Vector3.Lerp(playerOther.position, new Vector3(data.posX, data.posY, data.posZ), Time.deltaTime * 10f);
        playerOther.GetChild(0).eulerAngles = new Vector3(0, data.rotBody, 0);
    }

    void UpdateShootPos()
    {
        netPlayerScript.tempShootPos.position = new Vector3(data.camPosX, data.camPosY, data.camPosZ);
        netPlayerScript.tempShootPos.eulerAngles = new Vector3(data.camRotX, data.camRotY, 0);
        playerOther.GetChild(1).eulerAngles = new Vector3(data.camRotX, data.camRotY, 0);
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
        thisSideData.camPosX = playerShootPos.position.x;
        thisSideData.camPosY = playerShootPos.position.y;
        thisSideData.camPosZ = playerShootPos.position.z;
        thisSideData.camRotX = playerShootPos.eulerAngles.x;
        thisSideData.camRotY = playerShootPos.eulerAngles.y;
    }
}
