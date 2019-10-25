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

    //THIS IS ONLY FOR RTS
    static Queue<List<string>> _appendQueueMessage = new Queue<List<string>>();
    static Queue<List<string>> _appendQueuePlayerData = new Queue<List<string>>();
    static Queue<List<string>> _appendQueueWeaponState = new Queue<List<string>>();
    static Queue<List<string>> _appendQueueDamageDealt = new Queue<List<string>>();

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

        //cycle through data processing
        if (_appendQueueMessage.Count == 0) return;
        lock (_appendQueueMessage)
        {

            foreach (List<string> data in _appendQueueMessage)
            {
                this.ProcessMessage(data);
            }
            _appendQueueMessage.Clear();

        }
        if (_appendQueuePlayerData.Count == 0) return;
        lock (_appendQueuePlayerData)
        {

            foreach (List<string> data in _appendQueuePlayerData)
            {
                //this.ProcessPlayerData(data);
            }
            _appendQueuePlayerData.Clear();

        }
        if (_appendQueueWeaponState.Count == 0) return;
        lock (_appendQueueWeaponState)
        {

            foreach (List<string> data in _appendQueueWeaponState)
            {
                //this.ProcessWeaponState(data);
            }
            _appendQueueWeaponState.Clear();

        }
        if (_appendQueueDamageDealt.Count == 0) return;
        lock (_appendQueueDamageDealt)
        {

            foreach (List<string> data in _appendQueueDamageDealt)
            {
                //this.ProcessDamageDealt(data);
            }
            _appendQueueDamageDealt.Clear();

        }
    }

    #region Broadcasting

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

    public void OnSendDroidLocations() {
        //do this later
    }

    public void OnSendBuildLocations(int id, EntityType type, Vector3 location )
    {
        char[] finalData = new char[20];

        char[] data1 = ByteTools.ParseByte(id);
        char[] data2 = ByteTools.ParseByte((int)type);
        char[] data3 = ByteTools.ParseByte(location);


        //how do i fix this without using string concatenation???
        finalData[0] = data1[0];
        finalData[1] = data1[1];
        finalData[2] = data1[2];
        finalData[3] = data1[3];
        finalData[4] = data2[0];
        finalData[5] = data2[1];
        finalData[6] = data2[2];
        finalData[7] = data2[3];
        finalData[8] = data3[0];
        finalData[9] = data3[1];
        finalData[10] = data3[2];
        finalData[11] = data3[3];
        finalData[12] = data3[4];
        finalData[13] = data3[5];
        finalData[14] = data3[6];
        finalData[15] = data3[7];
        finalData[16] = data3[8];
        finalData[17] = data3[9];
        finalData[18] = data3[10];
        finalData[19] = data3[11];


        SendMsg(finalData, Client);

    }

    public void OnSendKillLocations()
    {
        //do this later
    }

    public void OnSendDamageLocations()
    {
        //do this later
    }
    #endregion

    #region Processing

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

    #endregion

    //called on data recieve action, then process
    static void PacketRecieved(int type, int sender, string data) {                ///does this get multithreaded because it gets invoked in a seperate thread???s

        string[] parsedDataSplit = data.Split(',');
        List<string> parsedData = new List<string>();

        //send to parsed data
        for (int counter = 0; counter < parsedDataSplit.Length; counter++) {
            parsedData.Add(parsedDataSplit[counter]);
        }
        parsedData.Insert(0, sender.ToString());

        lock (_appendQueueMessage)
        {
            if ((PacketType)type != PacketType.ERROR)
            {
                _appendQueueMessage.Enqueue(parsedData);
            }
            else
            {
                Debug.Log("PACKET SEND ERROR");
            }
        }

        lock (_appendQueuePlayerData)
        {
            if ((PacketType)type != PacketType.ERROR)
            {
                _appendQueuePlayerData.Enqueue(parsedData);
            }
            else
            {
                Debug.Log("PACKET SEND ERROR");
            }
        }

        lock (_appendQueueMessage)
        {
            if ((PacketType)type != PacketType.ERROR)
            {
                _appendQueueMessage.Enqueue(parsedData);
            }
            else
            {
                Debug.Log("PACKET SEND ERROR");
            }
        }

        lock (_appendQueueMessage)
        {
            if ((PacketType)type != PacketType.ERROR)
            {
                _appendQueueMessage.Enqueue(parsedData);
            }
            else
            {
                Debug.Log("PACKET SEND ERROR");
            }
        }


    }

    private void OnDestroy()
    {
        //clean up client
        DeleteClient(Client);

    }

}
