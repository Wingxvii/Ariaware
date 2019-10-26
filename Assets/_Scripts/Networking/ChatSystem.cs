using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;

enum PacketType {
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

};

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
    static extern void SendData(int type, string str, IntPtr client);       //Sends Data to all other Clients
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

            OnSendPlayer(818881, new Vector3(82522, 1999, -0.001f), Vector3.one);
        }
    }

    // Update is called once per frame
    void Update()
    {

        //cycle through data processing
        if (_appendQueueMessage.Count > 0)
        {
            lock (_appendQueueMessage)
            {

                foreach (List<string> data in _appendQueueMessage)
                {
                    this.ProcessMessage(data);
                }
                _appendQueueMessage.Clear();

            }
        }
        if (_appendQueuePlayerData.Count > 0)
        {
            lock (_appendQueuePlayerData)
            {

                foreach (List<string> data in _appendQueuePlayerData)
                {
                    this.ProcessPlayerData(data);
                }
                _appendQueuePlayerData.Clear();

            }
        }
        if (_appendQueueWeaponState.Count > 0)
        {
            lock (_appendQueueWeaponState)
            {

                foreach (List<string> data in _appendQueueWeaponState)
                {
                    //this.ProcessWeaponState(data);
                }
                _appendQueueWeaponState.Clear();

            }
        }
        if (_appendQueueDamageDealt.Count > 0)
        {
            lock (_appendQueueDamageDealt)
            {

                foreach (List<string> data in _appendQueueDamageDealt)
                {
                    //this.ProcessDamageDealt(data);
                }
                _appendQueueDamageDealt.Clear();

            }
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

    public void OnSendPlayer(int playerState, Transform ptransform)
    {
        char[] playerData = new char[28];
        ByteTools.ParseByte(playerState).CopyTo(playerData, 0);
        ByteTools.ParseByte(ptransform.position).CopyTo(playerData, 4);
        ByteTools.ParseByte(ptransform.rotation.eulerAngles).CopyTo(playerData, 16);

        string send = new string(playerData);
        Debug.Log(playerData.Length);
        SendData((int)PacketType.PLAYERDATA, new string(playerData), Client);
    }

    public void OnSendPlayer(int playerState, Vector3 pposition, Vector3 protation)
    {
        char[] playerData = new char[28];
        ByteTools.ParseByte(playerState).CopyTo(playerData, 0);
        ByteTools.ParseByte(pposition).CopyTo(playerData, 4);
        ByteTools.ParseByte(protation).CopyTo(playerData, 16);

        string send = new string(playerData);
        Debug.Log(playerData.Length);
        for (int i = 0; i < playerData.Length; i++)
        {
            Debug.Log((int)playerData[i]);
        }

        SendData((int)PacketType.PLAYERDATA, new string(playerData), Client);
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

        data1.CopyTo(finalData, 0);
        data2.CopyTo(finalData, 4);
        data3.CopyTo(finalData, 8);
        //how do i fix this without using string concatenation???
        //finalData[0] = data1[0];
        //finalData[1] = data1[1];
        //finalData[2] = data1[2];
        //finalData[3] = data1[3];
        //finalData[4] = data2[0];
        //finalData[5] = data2[1];
        //finalData[6] = data2[2];
        //finalData[7] = data2[3];
        //finalData[8] = data3[0];
        //finalData[9] = data3[1];
        //finalData[10] = data3[2];
        //finalData[11] = data3[3];
        //finalData[12] = data3[4];
        //finalData[13] = data3[5];
        //finalData[14] = data3[6];
        //finalData[15] = data3[7];
        //finalData[16] = data3[8];
        //finalData[17] = data3[9];
        //finalData[18] = data3[10];
        //finalData[19] = data3[11];


        SendMsg(new string(finalData), Client);

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
        int sender = ByteTools.ToByteInt(parsedData[0]);

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

    private void ProcessPlayerData(List<string> parsedData)
    {
        int sender = ByteTools.ToByteInt(parsedData[0]);
        Debug.Log(parsedData[0].Length);
        Debug.Log(parsedData[1].Length);

        if (sender == 0)
        {
            Debug.Log(">:(");
        }
        else
        {
            int state = ByteTools.ToByteInt(parsedData[1].Substring(0, 4));
            Vector3 playerPos = ByteTools.ToByteVector(parsedData[1].Substring(4, 12));
            Vector3 playerRot = ByteTools.ToByteVector(parsedData[1].Substring(16, 12));

            //Debug.Log("PING!");
            //Debug.Log(state + ", " + playerPos + ", " + playerRot);
        }
    }

    #endregion

    //called on data recieve action, then process
    static void PacketRecieved(int type, int sender, string data) {                ///does this get multithreaded because it gets invoked in a seperate thread???s
        Debug.Log(data.Length);
        //string[] parsedDataSplit = data.Split(',');
        //List<string> parsedData = new List<string>();
        //
        ////send to parsed data
        //for (int counter = 0; counter < parsedDataSplit.Length; counter++) {
        //    parsedData.Add(parsedDataSplit[counter]);
        //}
        //parsedData.Insert(0, sender.ToString());
        //
        //lock (_appendQueueMessage)
        //{
        //    if ((PacketType)type != PacketType.ERROR)
        //    {
        //        _appendQueueMessage.Enqueue(parsedData);
        //    }
        //    else
        //    {
        //        Debug.Log("PACKET SEND ERROR");
        //    }
        //}
        //
        //lock (_appendQueuePlayerData)
        //{
        //    if ((PacketType)type != PacketType.ERROR)
        //    {
        //        _appendQueuePlayerData.Enqueue(parsedData);
        //    }
        //    else
        //    {
        //        Debug.Log("PACKET SEND ERROR");
        //    }
        //}
        //
        //lock (_appendQueueMessage)
        //{
        //    if ((PacketType)type != PacketType.ERROR)
        //    {
        //        _appendQueueMessage.Enqueue(parsedData);
        //    }
        //    else
        //    {
        //        Debug.Log("PACKET SEND ERROR");
        //    }
        //}
        //
        //lock (_appendQueueMessage)
        //{
        //    if ((PacketType)type != PacketType.ERROR)
        //    {
        //        _appendQueueMessage.Enqueue(parsedData);
        //    }
        //    else
        //    {
        //        Debug.Log("PACKET SEND ERROR");
        //    }
        //}
        List<string> parsedData = new List<string>();
        //parsedData.Insert(0, sender.ToString());
        parsedData.Add(new string (ByteTools.ParseByte(sender)));
        //ByteTools.ParseByte(sender).CopyTo(parsedData[0], 0);

        switch ((PacketType)type)
        {
            case PacketType.MESSAGE:
                string[] parsedDataSplit = data.Split(',');

                for (int counter = 0; counter < parsedDataSplit.Length; counter++)
                {
                    parsedData.Add(parsedDataSplit[counter]);
                }

                lock (_appendQueueMessage)
                {
                    _appendQueueMessage.Enqueue(parsedData);
                }
                break;

            case PacketType.PLAYERDATA:
                parsedData.Add(data);

                lock(_appendQueuePlayerData)
                {
                    _appendQueuePlayerData.Enqueue(parsedData);
                }

                break;

            case PacketType.WEAPONSTATE:
                break;
            case PacketType.DAMAGEDEALT:
                break;
            case PacketType.DROIDLOCATIONS:
                break;
            case PacketType.BUILD:
                break;
            case PacketType.KILL:
                break;
            case PacketType.GAMESTATE:
                break;

            default:
                Debug.Log("PACKET SEND ERROR: " + type);
                break;
        }

    }

    private void OnDestroy()
    {
        //clean up client
        DeleteClient(Client);

    }

}
