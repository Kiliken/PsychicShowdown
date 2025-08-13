// Server-Side Implementation Of UDP:
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Configuration;
using System.Threading;
using System.ComponentModel.Design;
using System.Diagnostics;

class BasicUdpServer
{
    static int port = 25565;
    static UdpClient udpc = new UdpClient(port);
    
    static volatile byte[] p1Data;
    static volatile byte[] p2Data;

    static volatile int seed = 0;
    //static volatile Stopwatch stopWatch = new Stopwatch();


    public static void Main()
    {
        Thread udpServerThread;
        udpServerThread = new Thread(new ThreadStart(ServerLoop));
        udpServerThread.Start();
        //stopWatch.Start();
        string command = "";
        Console.WriteLine("[Server] started on port " + port);
        do
        {
            Console.Write("[Server] ");
            command = Console.ReadLine();

            if (command == "data")
            {
                Console.WriteLine("[Server] Player 1: ");
                StringifyBytes(p1Data);
                Console.WriteLine("[Server] Player 2: ");
                StringifyBytes(p2Data);
                //Console.WriteLine("[Server] Timer: " + stopWatch.ElapsedMilliseconds);
            }

            if (command == "reset")
            {
                ResetData();
            }

            if (command == "kick")
            {
                KickAndReset();
            }

            if (command == "clear")
            {
                Console.Clear();
                Console.WriteLine("[Server] console cleared ");
            }

            if (command == "help") Help();


        } while (command != "close");

        udpServerThread.Abort();
        Console.WriteLine("[Server] closed \n PRESS CTRL + C TO CLOSE THE CONSOLE");
    }

    static void StringifyBytes(byte[] bytes)
    {
        foreach (byte b in bytes)
        {
            Console.Write(String.Format("0x{0:X2}-", b));
        }
        Console.WriteLine("");
    }

    static void Help()
    {
        Console.WriteLine("[Server] avibile commands:\n" +
        " data\t SHOWS CURRENT STORED DATA\n" +
        " reset\t RESETS STORED DATA\n" +
        " clear\t CLEAR THE CONSOLE\n" +
        " close\t CLOSE SERVER\n" +
        " help\t SHOWS LIST OF AVIBILE COMMANDS\n"
        );
    }

    static void ResetData()
    {
        p1Data = new byte[] { 0x4E };
        p2Data = new byte[] { 0x4E };
        //stopWatch.Reset();
        //stopWatch.Start();
        Console.WriteLine("[Server] data resetted ");
    }
    
    static void KickAndReset()
    {
        p1Data = new byte[] { 0x4B };
        p2Data = new byte[] { 0x4B };
		//stopWatch.Stop();

        Console.WriteLine("[Server] kicking player... ");
    }
    
    private static void ServerLoop()
    {
        byte[] sdata;
        byte[] receivedData;

        p1Data = new byte[] { 0x4E };
        p2Data = new byte[] { 0x4E };


        while (true)
        {
            //ServerLoopStart
            try
            {
                IPEndPoint ep = null;
                receivedData = udpc.Receive(ref ep);


                if (receivedData[0] == 0x41)
                {
                    p1Data = receivedData;
                    if (p2Data[0] == 0x4E)
                    {
                        sdata = new byte[] { 0x4E };
                        udpc.Send(sdata, sdata.Length, ep);
                        continue;
                    }
                    if (p2Data[0] == 0x4B || p2Data[0] == 0x4C)
                    {
                        sdata = new byte[] { 0x4B };
                        p1Data = new byte[] { 0x4C };
                        udpc.Send(sdata, sdata.Length, ep);
                        continue;
                    }
                    udpc.Send(p2Data, p2Data.Length, ep);
                }
                else if (receivedData[0] == 0x42)
                {
                    p2Data = receivedData;
                    if (p1Data[0] == 0x4E)
                    {
                        sdata = new byte[] { 0x4E };
                        udpc.Send(sdata, sdata.Length, ep);
                        continue;
                    }
                    if (p1Data[0] == 0x4B || p1Data[0] == 0x4C)
                    {
                        sdata = new byte[] { 0x4B };
                        p2Data = new byte[] { 0x4C };
                        udpc.Send(sdata, sdata.Length, ep);
                        continue;
                    }
                    udpc.Send(p1Data, p1Data.Length, ep);
                }

                if (p1Data[0] == 0x4C  && p2Data [0] == 0x4C )
                {
                    ResetData();
                }
				
				if (p1Data[0] == 0x52  || p2Data [0] == 0x52 )
                {
                    ResetData();
                }

                /*if (stopWatch.ElapsedMilliseconds > 180000)
                {
                    KickAndReset();
                }*/

            }
            catch (SocketException ex)
            {

                // Handle errors gracefully, e.g., log or attempt to reconnect
            }
        }
    }
}