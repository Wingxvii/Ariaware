using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;

public enum PacketType
{
    //initialization connection
    ERROR = 0,
    //single string
    MESSAGE = 1,
    //transformation data
    TRANSFORMATION = 2,
}

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


        List<String> messages = new List<String>();
        messages.Add(new String(ByteTools.ParseByte(41222)));
        messages.Add(new String(ByteTools.ParseByte(41512)));
        messages.Add(new String(ByteTools.ParseByte(34555)));

        OnSendMessage(messages);
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
            //Debug.Log(finalMessage.Length);
        }
        char[] temp = new char[finalMessage.Length];

        SendMsg(new String(temp), Client);

    }

    public void OnSendMessage(string message)
    {
        StringBuilder finalMessage = new StringBuilder();
        finalMessage.Append(message);
        finalMessage.Append(",");
        char[] temp = new char[finalMessage.Length];

        SendMsg(new String(temp), Client);

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

            char[] temp = new char[sb.Length];

            userLog.text += "CONSOLE: " + new String(temp) + "\n";
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Player ");
            sb.Append(parsedData[0]);
            sb.Append(": ");

            for (int counter = 1; counter < parsedData.Count; counter++)
            {
                //Debug.Log(parsedData[counter]);

                sb.Append(ByteTools.ToByteInt(parsedData[counter]).ToString());
                //Debug.Log("added message");
            }

            char[] temp = new char[sb.Length];

            Debug.Log("Processed" + new String(temp));
            
            userLog.text = userLog.text + new String(temp) + "\n";
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
            if ((PacketType)type != PacketType.ERROR)
            {
                _appendQueue.Enqueue(parsedData);
                //Debug.Log("Recieved:");
                //Debug.Log(data);
            }
            else
            {
                Debug.Log("PACKET SEND ERROR");
            }
        }
    }


}
