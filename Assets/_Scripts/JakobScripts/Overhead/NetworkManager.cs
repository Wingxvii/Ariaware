using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class NetworkManager
{
    bool init = false;

    private NetworkManager() { }
    static private NetworkManager nM;
    static public NetworkManager NM
    {
        get { if (nM == null) { nM = new NetworkManager(); } return nM; }
        private set { nM = value; }
    }

    [DllImport("CNET.dll")]
    static extern IntPtr CreateClient();                            //Creates a client
    [DllImport("CNET.dll")]
    static extern void DeleteClient(IntPtr client);                 //Destroys a client
    [DllImport("CNET.dll")]
    static extern void Connect(string str, IntPtr client);          //Connects to c++ Server
    [DllImport("CNET.dll")]
    static extern void StartUpdating(IntPtr client);                //Starts updating
    [DllImport("CNET.dll")]
    static extern void SetupPacketReception(Action<int, int, string> action); //recieve packets from server
    [DllImport("CNET.dll")]
    static extern int GetPlayerNumber(IntPtr client);

    public IntPtr Client { get; private set; }

    //static Queue<List<string>> _appendQueue = new Queue<List<string>>();
    static public Queue<List<string>> _transformQueue { get; private set; } = new Queue<List<string>>();

    public void InitNetworking()
    {

        if (ConnectionLoader.ip != null)
        {
            if (!init)
            {
                Client = CreateClient();
                Connect(ConnectionLoader.ip, Client);
                StartUpdating(Client);
                SetupPacketReception(PacketRecieved);

                init = true;
            }
        }
    }

    static void PacketRecieved(int type, int sender, string data)
    {                ///does this get multithreaded because it gets invoked in a seperate thread???s
        List<string> parsedData = tokenize(',', data);
        //send to parsed data
        parsedData.Insert(0, sender.ToString());
        Debug.Log(parsedData.Count.ToString());

        lock (_transformQueue)
        {
            if ((PacketType)type == PacketType.TRANSFORMATION)
            {
                _transformQueue.Enqueue(parsedData);
            }
            else
            {
                Debug.Log("PACKET SEND ERROR");
            }
        }


    }

    static List<string> tokenize(char token, string text)
    {
        List<string> temp = new List<string>();
        int lastTokenLocation = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == token)
            {
                temp.Add(text.Substring(lastTokenLocation, i - lastTokenLocation));
                lastTokenLocation = i + 1;
            }
        }
        return temp;
    }
}
