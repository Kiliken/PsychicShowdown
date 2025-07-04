using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
/*

Data:

player,vector3x,vector3y,vector3z,eurlerbody,eulerhead,shootingflag

DataBytes:
1(byte),4(float),4(float),4(float),4(float),4(float),1(bool)

*/


public static class NetManager
{
    //public static volatile string udpSend = "N";
    //public static volatile string udpGet = "N";

    //public static UdpClient udpc;

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
                
                //udpc.Send(udpSend, udpSend.Length);

               
                //udpGet = udpc.Receive(ref ep);

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


        // Allow the thread to start
    }

    
    public static NetData RetriveByte(byte[] bytes)
    {
        NetData data = new NetData();
        //char[] converter = str.ToCharArray(0, str.Length);

        if (bytes[0] == 0x4E)
            return null;
        
        data.posX = BitConverter.ToSingle(bytes, 1);
        data.posY = BitConverter.ToSingle(bytes, 5);
        data.posZ = BitConverter.ToSingle(bytes, 9);
        data.rotBody = BitConverter.ToSingle(bytes, 13);
        //data.rotHead = BitConverter.ToSingle(bytes, 17);
        //data.shootingFlag = bytes[21];
        
        return data;
    }

    public static byte[] ParseByte(char side, Transform p/*, float r, byte shotFlag*/)
    {
        byte[] test = new byte[0];

        test = test.Concat(new byte[] { (byte)side }).ToArray();
        test = test.Concat(BitConverter.GetBytes(p.position.x)).ToArray();
        test = test.Concat(BitConverter.GetBytes(p.position.y)).ToArray();
        test = test.Concat(BitConverter.GetBytes(p.position.z)).ToArray();
        test = test.Concat(BitConverter.GetBytes(p.eulerAngles.y)).ToArray();
        //test = test.Concat(BitConverter.GetBytes(r)).ToArray();
        //test = test.Concat(BitConverter.GetBytes(shotFlag)).ToArray();

        return test;
    }

   

  
}


public class NetData
{
    public float posX;
    public float posY;
    public float posZ;
    public float rotBody;
    //public float rotHead;
    //public float rotZ;
    //public byte shootingFlag;
}

