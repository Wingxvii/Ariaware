using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;
using netcodeRTS;
/*
public enum PacketType
{
    //initialization connection
    ERROR = 0,

    //single string
    MESSAGE = 1,

    //FPS DATA TYPES

    //2vec3 + int
    PLAYERDATA = 2,
    //int
    WEAPONSTATE = 3,
    //int
    DAMAGEDEALT = 4,

    //RTS DATA TYPES

    //vec4[0-100]
    DROIDLOCATIONS = 5,
    //2int + vec3
    BUILD = 6,
    //int
    KILL = 7,
    //int
    GAMESTATE = 8,
}
*/
[StructLayout(LayoutKind.Sequential)]

public class ChatSystem : MonoBehaviour
{
    [DllImport("CNET.dll")]
    static extern IntPtr CreateClient();                            //Creates a client
    [DllImport("CNET.dll")]
    static extern void DeleteClient(IntPtr client);                 //Destroys a client
    [DllImport("CNET.dll")]
    static extern void Connect(string str, IntPtr client);          //Connects to c++ Server
    [DllImport("CNET.dll")]
    static extern void SendMsg(string str, IntPtr client);          //Sends Message to all other clients    
    [DllImport("CNET.dll")]
    static extern void StartUpdating(IntPtr client);                //Starts updating
    [DllImport("CNET.dll")]
    static extern void SetupPacketReception(Action<int, int, string> action); //recieve packets from server
    [DllImport("CNET.dll")]
    static extern int GetPlayerNumber(IntPtr client);


    public Text userLog;
    public InputField userInput;

    public string ip;

    private IntPtr Client;

    static Queue<List<string>> _appendQueue = new Queue<List<string>>();
    static Queue<List<string>> _appendQueueTransform = new Queue<List<string>>();

    // Start is called before the first frame update
    void Start()
    {
        if (ip != null) {
            //client Init  
            Client = CreateClient();
            Connect(ip, Client);
            StartUpdating(Client);
            SetupPacketReception(PacketRecieved);
        }
    }

    // Update is called once per frame
    void Update()
    {

        //in data
        if (_appendQueue.Count == 0) return;
        lock (_appendQueue)
        {

            foreach (List<string> data in _appendQueue)
            {
                this.ProcessMessage(data);
            }
            _appendQueue.Clear();

        }
    }

    public void OnSendMessage(List<String> messageStack)
    {
        StringBuilder finalMessage = new StringBuilder();
        foreach (String message in messageStack)
        {
            finalMessage.Append(message);
            finalMessage.Append(",");
        }
        SendMsg(finalMessage.ToString(), Client);

    }

    public void OnSendMessage(string message)
    {
        StringBuilder finalMessage = new StringBuilder();
        finalMessage.Append(message);
        finalMessage.Append(",");
        SendMsg(finalMessage.ToString(), Client);

    }

    public void OnSendMessage()
    {
        string message = "Player " + GetPlayerNumber(Client) + ": " + userInput.text;
        userLog.text = userLog.text + message + "\n";


        string message2 = userInput.text;
        SendMsg(message2, Client);
    }

    private void OnDestroy()
    {
        //clean up client
        DeleteClient(Client);
        
    }

    private void ProcessMessage(List<string> parsedData) {
        int sender = int.Parse(parsedData[0]);

        //filter by sender
        if (sender == 0)
        {
            StringBuilder sb = new StringBuilder();
            for (int counter = 1; counter < parsedData.Count; counter++)
            {
                sb.Append(parsedData[counter]);
            }
            userLog.text += "CONSOLE: " + sb.ToString() + "\n";
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Player ");
            sb.Append(parsedData[0]);
            sb.Append(": ");

            for (int counter = 1; counter < parsedData.Count; counter++)
            {
                sb.Append(parsedData[counter]);
                //Debug.Log("added message");
            }
            Debug.Log("Processed" + sb.ToString());
            userLog.text = userLog.text + sb.ToString() + "\n";
        }
    }

    //called on data recieve action, then process
    static void PacketRecieved(int type, int sender, string data) {                ///does this get multithreaded because it gets invoked in a seperate thread???s

        string[] parsedDataSplit = data.Split(',');
        List<string> parsedData = new List<string>();

        //send to parsed data
        for (int counter = 0; counter < parsedDataSplit.Length; counter++) {
            parsedData.Add(parsedDataSplit[counter]);
        }
        parsedData.Insert(0, sender.ToString());

        lock (_appendQueue)
        {
            if ((PacketType)type != PacketType.INIT)
            {
                _appendQueue.Enqueue(parsedData);
            }
            else
            {
                Debug.Log("PACKET SEND ERROR");
            }
        }
    }

}
