using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class Hands : MonoBehaviour
{
    Socket sender;
    Socket listener;
    byte[] buffer;
    // Start is called before the first frame update
    void Start()

    
    {
        buffer = new byte[2048];
        try
        {
            print("starting");
            IPAddress iP = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(iP, 12345);
            //listener = new Socket(iP.AddressFamily, SocketType.Stream, ProtocolType.IPv4);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            print("Listener created");
            listener.Bind(localEndPoint);
            listener.Listen(1);
            print("Set up listener");
            sender = listener.Accept();
        }
        catch (SocketException e)
        {
            print(e);
            print("failed");
        }


        
    }

    // Update is called once per frame
    void Update()
    {
        if (sender.Available > 0)
        {
            sender.Receive(buffer);
            print(Encoding.ASCII.GetString(buffer));
        }
        
        
    }
}
