// Client-Side Implementation Of UDP:
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class StudentUDPClient
{
    public static void Main(string[] args)
    {
        Console.WriteLine("IP ADDRESS: ");
        string ip = Console.ReadLine();
        UdpClient udpc = new UdpClient(ip, 25565);
        IPEndPoint ep = null;

        string message;

        // Prevent example from ending if CTL+C is pressed.

        Console.WriteLine("PLAYER SIDE: ");
        string side = Console.ReadLine() == "1" ? "A" : "B";
        ConsoleKeyInfo cki;

        //Console.TreatControlCAsInput = true;

        Console.WriteLine("Press any combination of CTL, ALT, and SHIFT, and a console key.");
        Console.WriteLine("Press the Escape (Esc) key to quit: \n");



        do
        {
            cki = Console.ReadKey();
            //Console.Write(" --- You pressed ");
            //Console.WriteLine(cki.Key.ToString());

            /*if (cki.KeyChar == 'k')
            {
                byte[] msg = Encoding.ASCII.GetBytes("S<EOF>");
                udpc.Send(msg, msg.Length);
                continue;
            }*/


            // Data to send
            byte[] msg = Encoding.ASCII.GetBytes(side + cki.Key.ToString());
            udpc.Send(msg, msg.Length);

            // received Data
            byte[] rdata = udpc.Receive(ref ep);
            message = Encoding.ASCII.GetString(rdata);
            Console.WriteLine(message);

            //if (message.Substring(0, 4) != side)
            {
                //Console.WriteLine(message);
            }

        }while (cki.KeyChar != 'k');

    }
}