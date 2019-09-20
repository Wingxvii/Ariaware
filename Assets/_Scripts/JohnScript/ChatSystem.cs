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
}

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

    public Button sendButton;
    public Text userLog;
    public InputField userInput;

    private IntPtr Client;

    static volatile Queue<List<string>> _appendQueue = new Queue<List<string>>();

    // Start is called before the first frame update
    void Start()
    {

        //client Init  
        Client = CreateClient();
        Connect("127.0.0.1", Client);
        StartUpdating(Client);
        SetupPacketReception(PacketRecieved);
    }

    // Update is called once per frame
    void Update()
    {
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

    public void OnSendMessage()
    {
        string message = userInput.text;
        SendMsg(message, Client);
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
            string message = "";
            for (int counter = 1; counter < parsedData.Count; counter++)
            {
                message = message + parsedData[counter];
            }
            userLog.text += "CONSOLE: " + message + "\n";
        }
        else
        {
            string message = "";
            for (int counter = 1; counter < parsedData.Count; counter++)
            {
                message = message + parsedData[counter];
                Debug.Log("added message");
            }
            message = "Player " + parsedData[0] + ": " + message;
            Debug.Log(message);
            userLog.text = userLog.text + message + "\n";
        }
    }

    //called on data recieve action, then process
    static void PacketRecieved(int type, int sender, string data) {                ///does this get multithreaded because it gets invoked in a seperate thread???s
        List<string> parsedData = tokenize(',', data);
        //send to parsed data
        parsedData.Insert(0, sender.ToString());
        Debug.Log(parsedData.Count.ToString());

        lock (_appendQueue){
            if ((PacketType)type != PacketType.ERROR)
            {
                _appendQueue.Enqueue(parsedData);
            }
            else {
                Debug.Log("PACKET SEND ERROR");
            }
        }

    }

    //tokenizer migrated from c++
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
