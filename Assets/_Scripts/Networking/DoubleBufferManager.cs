using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;

[StructLayout(LayoutKind.Sequential)]

public class DoubleBufferManager : MonoBehaviour
{
    [DllImport("CNET.dll")]
    static extern IntPtr CreateClient();                            //Creates a client
    [DllImport("CNET.dll")]
    static extern void DeleteClient(IntPtr client);                 //Destroys a client
    [DllImport("CNET.dll")]
    static extern void Connect(string str, IntPtr client);          //Connects to c++ Server
    [DllImport("CNET.dll")]
    static extern void SendData(int type, string str, IntPtr client);          //Sends Message to all other clients    
    [DllImport("CNET.dll")]
    static extern void StartUpdating(IntPtr client);                //Starts updating
    [DllImport("CNET.dll")]
    static extern void SetupPacketReception(Action<int, int, string> action); //recieve packets from server
    [DllImport("CNET.dll")]
    static extern int GetPlayerNumber(IntPtr client);

    public Text DisplayField;
    public Text Counter;
    public Text bufferOn;
    
    public const bool USEDOUBLEBUFFER = true;

    [SerializeField]
    int packetsPerFrame = 1;
    [SerializeField]
    float testDuration = 1.0f;
    [SerializeField]
    string ip;

    bool testRunning = false;
    float timeElapsed = 0.0f;
    static bool buff1Read = true;
    IntPtr Client;
    int packetsRecieved = 0;

    static Queue<List<string>> _buffer1 = new Queue<List<string>>();
    static Queue<List<string>> _buffer2 = new Queue<List<string>>();


    // Start is called before the first frame update
    void Start()
    {
        if (USEDOUBLEBUFFER)
        {
            bufferOn.text = "On";
        }
        else {
            bufferOn.text = "Off";
        }

        if (ip != null)
        {
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
        Counter.text = packetsRecieved.ToString();

        if (testRunning && (timeElapsed < testDuration))
        {
            timeElapsed += Time.deltaTime;
            List<String> messages = new List<String>();

            System.Random r = new System.Random();

            for (int i = 0; i < packetsPerFrame; i++)
            {
                for (int y = 0; y < 100; y++)
                {
                    int num1 = r.Next(0, int.MaxValue);
                    messages.Add(num1.ToString());
                }
                OnSendMessage(messages);
                messages.Clear();
            }

            if (timeElapsed >= testDuration)
            {
                testRunning = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.T)) {
            timeElapsed = 0;
            testRunning = true;
            packetsRecieved = 0;
        }



        if (USEDOUBLEBUFFER)
        {
            if (buff1Read)
            {
                //in data
                lock (_buffer1)
                {
                    packetsRecieved += _buffer1.Count;
                    if (_buffer1.Count != 0)
                    {
                        this.ProcessMessage(_buffer1.Peek());
                        _buffer1.Clear();
                    }
                    buff1Read = false;
                }
            }
            else
            {
                //in data
                lock (_buffer2)
                {
                    packetsRecieved += _buffer2.Count;
                    if (_buffer2.Count != 0)
                    {
                        this.ProcessMessage(_buffer2.Peek());
                        _buffer2.Clear();
                    }
                    buff1Read = true;
                }
            }
        }
        else
        {
            //in data
            lock (_buffer1)
            {
                if (_buffer1.Count != 0)
                {
                    packetsRecieved += _buffer1.Count;
                    foreach (List<string> data in _buffer1)
                    {
                        this.ProcessMessage(data);
                    }
                    _buffer1.Clear();
                }
            }
        }

    }

    public void StartTest() {
        timeElapsed = 0;
        testRunning = true;
        packetsRecieved = 0;
    }

    public void OnSendMessage(List<String> messageStack)
    {
        StringBuilder finalMessage = new StringBuilder();
        foreach (String message in messageStack)
        {
            finalMessage.Append(message);
            finalMessage.Append(",");
        }
        SendData(1, finalMessage.ToString(), Client);

    }

    public void OnSendMessage(string message)
    {
        StringBuilder finalMessage = new StringBuilder();
        finalMessage.Append(message);
        finalMessage.Append(",");
        SendData(1, finalMessage.ToString(), Client);

    }

    private void OnDestroy()
    {
        //clean up client
        DeleteClient(Client);

    }

    private void ProcessMessage(List<string> parsedData)
    {
        int sender = int.Parse(parsedData[0]);

        if (USEDOUBLEBUFFER) {
            StringBuilder sb = new StringBuilder();

            for (int counter = 1; counter < parsedData.Count; counter++)
            {
                sb.Append(parsedData[counter]);
                sb.Append("\n");
                //Debug.Log("added message");
            }
            DisplayField.text = sb.ToString();
        }
        else
        {
            StringBuilder sb = new StringBuilder();

            for (int counter = 1; counter < parsedData.Count; counter++)
            {
                sb.Append(parsedData[counter]);
                sb.Append("\n");
                //Debug.Log("added message");
            }
            DisplayField.text = sb.ToString();
        }


        if (parsedData[1] == "Stop") {
            testRunning = false;
        }
    }



    //called on data recieve action, then process
    static void PacketRecieved(int type, int sender, string data)
    {       

        string[] parsedDataSplit = data.Split(',');
        List<string> parsedData = new List<string>();

        //send to parsed data
        for (int counter = 0; counter < parsedDataSplit.Length; counter++)
        {
            parsedData.Add(parsedDataSplit[counter]);
        }
        parsedData.Insert(0, sender.ToString());

        if (USEDOUBLEBUFFER && buff1Read)
        {
            lock (_buffer2)
            {
                _buffer2.Enqueue(parsedData);
            }
        }
        else
        {
            lock (_buffer1)
            {
                _buffer1.Enqueue(parsedData);
            }

        }
    }
}
