using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using UnityEditor;

public class Hands : MonoBehaviour
{
    Socket sender;
    Socket listener;
    byte[] buffer;
    // Start is called before the first frame update
    Vector3[] landmarks;
    string[] sp = { " ", "}" };
    void Start()
    {
        landmarks = new Vector3[21];
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
        String received;
        if (sender.Available > 0)
        {
            sender.Receive(buffer);
            received = Encoding.ASCII.GetString(buffer);
            string[] words = received.Split('{', StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains('}'))
                {
                    print(words[i]);
                    string[] vals = words[i].Split(sp, StringSplitOptions.RemoveEmptyEntries);
                    landmarks[int.Parse(vals[0])] = new Vector3(float.Parse(vals[1]), float.Parse(vals[2]), float.Parse(vals[3])) * 10; 
                }
                else
                {
                    continue;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        foreach (Vector3 landmark in landmarks)
        {
            Gizmos.DrawSphere(landmark, 0.1f);
        }
    }
}
