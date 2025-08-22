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
using UnityEngine.Rendering.Universal;
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
        data.leftHand = bytes[17];
        data.rightHand = bytes[18];
        data.leftObjId = BitConverter.ToUInt16(bytes, 19);
        data.rightObjId = BitConverter.ToUInt16(bytes, 21);
        data.hp = (sbyte)bytes[23];
        data.camPosX = BitConverter.ToSingle(bytes, 24);
        data.camPosY = BitConverter.ToSingle(bytes, 28);
        data.camPosZ = BitConverter.ToSingle(bytes, 32);
        data.camRotX = BitConverter.ToSingle(bytes, 36);
        data.camRotY = BitConverter.ToSingle(bytes, 40);
        data.soundFlag = bytes[45];
        data.soundIndex = (sbyte)bytes[46];
        
        return data;
    }

    public static byte[] ParseByte(char side, NetData data)
    {
        byte[] buffer = new byte[47];

        buffer[0] = (byte)side;
        Array.Copy(BitConverter.GetBytes(data.posX), 0, buffer, 1, 4);
        Array.Copy(BitConverter.GetBytes(data.posY), 0, buffer, 5, 4);
        Array.Copy(BitConverter.GetBytes(data.posZ), 0, buffer, 9, 4);
        Array.Copy(BitConverter.GetBytes(data.rotBody), 0, buffer, 13, 4);
        buffer[17] = data.leftHand;
        buffer[18] = data.rightHand;
        Array.Copy(BitConverter.GetBytes(data.leftObjId), 0, buffer, 19, 2);
        Array.Copy(BitConverter.GetBytes(data.rightObjId), 0, buffer, 21, 2);
        buffer[23] = (byte)data.hp;
        Array.Copy(BitConverter.GetBytes(data.camPosX), 0, buffer, 24, 4);
        Array.Copy(BitConverter.GetBytes(data.camPosY), 0, buffer, 28, 4);
        Array.Copy(BitConverter.GetBytes(data.camPosZ), 0, buffer, 32, 4);
        Array.Copy(BitConverter.GetBytes(data.camRotX), 0, buffer, 36, 4);
        Array.Copy(BitConverter.GetBytes(data.camRotY), 0, buffer, 40, 4);
        buffer[45] = data.soundFlag;
        buffer[46] = (byte)data.soundIndex;

        return buffer;
    }

   

  
}


public class NetData
{
    //Movement
    public float posX;
    public float posY;
    public float posZ;
    public float rotBody;

    //ObjectInteraction
    public byte leftHand;
    public byte rightHand;
    public ushort leftObjId;
    public ushort rightObjId;
    public sbyte hp;

    //Camera Pos
    public float camPosX;
    public float camPosY;
    public float camPosZ;
    public float camRotX;
    public float camRotY;

    //Sounds
    public byte soundFlag;
    public sbyte soundIndex;
}

