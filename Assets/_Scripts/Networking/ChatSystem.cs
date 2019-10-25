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
public struct Vec3 {
    public float x;
    public float y;
    public float z;
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
    static extern void SendTransformation(Vec3 pos, Vec3 rot, IntPtr client);          //Sends Position data to all other clients
    [DllImport("CNET.dll")]
    static extern void StartUpdating(IntPtr client);                //Starts updating
    [DllImport("CNET.dll")]
    static extern void SetupPacketReception(Action<int, int, string> action); //recieve packets from server
    [DllImport("CNET.dll")]
    static extern int GetPlayerNumber(IntPtr client);


    public Text userLog;
    public InputField userInput;
    public float timeSend = 5;
    public int xe = 0;
    public static int ab = 0;

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
        if (timeSend > 0) {
            List<String> messages = new List<string>();
            for (int x = 0; x < 900; x++) {
                messages.Add(x.ToString());
            }
            OnSendMultipleMessage(messages);
            timeSend -= Time.deltaTime;
        }

        /*
        //out data
        SendTransformation(this.transform.position.x, this.transform.position.y, this.transform.position.z,
            this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z,
            this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z, Client);
        */

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

    public void OnSendMultipleMessage(List<String> messageStack) {
        StringBuilder finalMessage = new StringBuilder();
        foreach (String message in messageStack) {
            finalMessage.Append(message);
            finalMessage.Append(",");
        }
        SendMsg(finalMessage.ToString(), Client);

    }

    public void OnSendMessage()
    {
        string message = "Player " + GetPlayerNumber(Client) + ": " + userInput.text;
        Debug.Log(message);
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

            sb.Clear();
            sb.Append(xe.ToString());
            ++xe;
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

        Debug.Log("Recieved" + ab.ToString());
        ++ab;
        lock (_appendQueue)
        {
            if ((PacketType)type != PacketType.ERROR)
            {
                _appendQueue.Enqueue(parsedData);
            }
            else
            {
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


    //Sending types
    char[] ParseByte(float data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
       return new char[4]{(char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3] };
    }
    char[] ParseByte(Vector3 data)
    {
        byte[] bytes = BitConverter.GetBytes(data.x);
        byte[] bytes2 = BitConverter.GetBytes(data.y);
        byte[] bytes3 = BitConverter.GetBytes(data.z);

        return new char[12] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3], (char)bytes[4], (char)bytes[5], (char)bytes[6], (char)bytes[7], (char)bytes[8], (char)bytes[9], (char)bytes[10], (char)bytes[11] };
    }
    char[] ParseByte(int data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        return new char[4] { (char)bytes[0], (char)bytes[1], (char)bytes[2], (char)bytes[3] };
    }
    //Sending types with string return
    string ParseBytetoString(float data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        return "" + (char)bytes[0] + (char)bytes[1] + (char)bytes[2] + (char)bytes[3];
    }
    string ParseBytetoString(Vector3 data)
    {
        byte[] bytes = BitConverter.GetBytes(data.x);
        byte[] bytes2 = BitConverter.GetBytes(data.y);
        byte[] bytes3 = BitConverter.GetBytes(data.z);
        return "" + (char)bytes[0] + (char)bytes[1] + (char)bytes[2] + (char)bytes[3] + (char)bytes2[0] + (char)bytes2[1] + (char)bytes2[2] + (char)bytes2[3] + (char)bytes3[0] + (char)bytes3[1] + (char)bytes3[2] + (char)bytes3[3];
    }
    string ParseBytetoString(int data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        return "" + (char)bytes[0] + (char)bytes[1] + (char)bytes[2] + (char)bytes[3];
    }

    //Recieving types
    float ToByteFloat(string s)
    {
        byte[] bytes = { (byte)s[0], (byte)s[1], (byte)s[2], (byte)s[3] };
        return BitConverter.ToSingle(bytes, 0);
    }
    Vector3 ToByteVector(string s)
    {
        byte[] bytes = { (byte)s[0], (byte)s[1], (byte)s[2], (byte)s[3] };
        byte[] bytes2 = { (byte)s[4], (byte)s[5], (byte)s[6], (byte)s[7] };
        byte[] bytes3 = { (byte)s[8], (byte)s[9], (byte)s[10], (byte)s[11] };

        return new Vector3(BitConverter.ToSingle(bytes, 0), BitConverter.ToSingle(bytes2, 0), BitConverter.ToSingle(bytes3, 0));
    }
    int ToByteInt(string s)
    {
        byte[] bytes = { (byte)s[0], (byte)s[1], (byte)s[2], (byte)s[3] };
        return BitConverter.ToInt32(bytes, 0);
    }

}
