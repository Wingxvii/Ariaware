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
    INIT = 0,

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

public class FPSDataBuffer {
    //LOOK UP SEPHAMORES
    //USE INSTEAD OF DOUBLE BUFFER

    public Vector3 Player1Pos = new Vector3(0,0,0);
    public Vector3 Player1Rot = new Vector3(0, 0, 0);
    public int Player1State = -1;

    public Vector3 Player2Pos = new Vector3(0, 0, 0);
    public Vector3 Player2Rot = new Vector3(0, 0, 0);
    public int Player2State = -1;

    public Vector3 Player3Pos = new Vector3(0, 0, 0);
    public Vector3 Player3Rot = new Vector3(0, 0, 0);
    public int Player3State = -1;

}

public class NetworkManager : MonoBehaviour
{
    //THIS CANNOT BE SINGLETON

    #region Netcode

    //net code
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

    public string ip;
    private IntPtr Client;
    private static int playerNumber = -1;

    #endregion

    //Incomming Data Type Queues
    static Queue<Tuple<int, string[]>> _InstanceDamageDealt = new Queue<Tuple<int, string[]>>();

    //Incomming state updates
    static string updatedWeaponP1;
    static string updatedWeaponP2;
    static string updatedWeaponP3;

    //dirty flag for state updates
    static bool updatedWeaponP1Flag = false;
    static bool updatedWeaponP2Flag = false;
    static bool updatedWeaponP3Flag = false;

    //buffers
    public static FPSDataBuffer ReadBuffer;
    public static FPSDataBuffer WriteBuffer;
    private static FPSDataBuffer tempBuffer;

    int fixedTimeStep = 50;

    // Start is called before the first frame update
    void Start()
    {
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
        //process instanced information recieved
        if (updatedWeaponP1Flag)
        {
            //process weapon update
        }
        if (updatedWeaponP2Flag)
        {
            //process weapon update
        }
        if (updatedWeaponP3Flag)
        {
            //process weapon update
        }
        if (_InstanceDamageDealt.Count > 0) {
            foreach (Tuple<int, string[]> data in _InstanceDamageDealt) {
                //process instance damage

                _InstanceDamageDealt.Dequeue();
            }
        }

    }

    private void FixedUpdate()
    {
        SwitchBuffers();

        

        #region Fixed Tick
        //count down
        --fixedTimeStep;

        //tick is called 10 times per 50 updates
        if (fixedTimeStep % 5 == 0) {
            TickUpdate();
        }

        //reset the clock
        if (fixedTimeStep <= 0) {
            //updates 50Hz
            fixedTimeStep = 50;
        }
        #endregion
    }

    //called 10 times per second
    public void TickUpdate() {

    }
    
    //switches the buffers
    private void SwitchBuffers() {
        tempBuffer = ReadBuffer;
        ReadBuffer = WriteBuffer;
        WriteBuffer = tempBuffer;
    }

    //send list of string message to all others
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

    //send single string message to all others
    public void OnSendMessage(string message)
    {
        StringBuilder finalMessage = new StringBuilder();
        finalMessage.Append(message);
        finalMessage.Append(",");
        SendData(1, finalMessage.ToString(), Client);

    }

    //call c++ cleanup
    private void OnDestroy()
    {
        //clean up client
        DeleteClient(Client);

    }

    //called on data recieve action, then process
    static void PacketRecieved(int type, int sender, string data)
    {
        //parse the data
        string[] parsedData = data.Split(',');

        switch ((PacketType)type) {
            case PacketType.INIT:
                if (parsedData.Length == 1)
                {
                    playerNumber = Convert.ToInt32(parsedData[0]);
                }
                else {
                    Debug.Log("Error: Invalid INIT Parsed Array Size");
                    Debug.Break();
                }
                break;
            case PacketType.PLAYERDATA:
                if (parsedData.Length == 7) {
                    //lock and update by sender
                    lock (ReadBuffer)
                    {
                        switch (sender)
                        {
                            case 2:
                                WriteBuffer.Player1Pos.x = float.Parse(parsedData[0]);
                                WriteBuffer.Player1Pos.y = float.Parse(parsedData[1]);
                                WriteBuffer.Player1Pos.z = float.Parse(parsedData[2]);
                                WriteBuffer.Player1Rot.x = float.Parse(parsedData[3]);
                                WriteBuffer.Player1Rot.y = float.Parse(parsedData[4]);
                                WriteBuffer.Player1Rot.z = float.Parse(parsedData[5]);
                                WriteBuffer.Player1State = int.Parse(parsedData[6]);
                                break;
                            case 3:
                                WriteBuffer.Player2Pos.x = float.Parse(parsedData[0]);
                                WriteBuffer.Player2Pos.y = float.Parse(parsedData[1]);
                                WriteBuffer.Player2Pos.z = float.Parse(parsedData[2]);
                                WriteBuffer.Player2Rot.x = float.Parse(parsedData[3]);
                                WriteBuffer.Player2Rot.y = float.Parse(parsedData[4]);
                                WriteBuffer.Player2Rot.z = float.Parse(parsedData[5]);
                                WriteBuffer.Player2State = int.Parse(parsedData[6]);
                                break;
                            case 4:
                                WriteBuffer.Player3Pos.x = float.Parse(parsedData[0]);
                                WriteBuffer.Player3Pos.y = float.Parse(parsedData[1]);
                                WriteBuffer.Player3Pos.z = float.Parse(parsedData[2]);
                                WriteBuffer.Player3Rot.x = float.Parse(parsedData[3]);
                                WriteBuffer.Player3Rot.y = float.Parse(parsedData[4]);
                                WriteBuffer.Player3Rot.z = float.Parse(parsedData[5]);
                                WriteBuffer.Player3State = int.Parse(parsedData[6]);
                                break;
                            default:
                                Debug.Log("Error: PLAYERDATA Sender Invalid");
                                Debug.Break();
                                break;
                        }
                    }
                }
                else
                {
                    Debug.Log("Error: Invalid PLAYERDATA Parsed Array Size");
                    Debug.Break();
                }
                break;
            case PacketType.WEAPONSTATE:
                //update state by sender type
                switch (sender) {
                    case 2:
                        //set dirty flag on
                        updatedWeaponP1Flag = true;
                        updatedWeaponP1 = data;
                        break;
                    case 3:
                        //set dirty flag on
                        updatedWeaponP2Flag = true;
                        updatedWeaponP2 = data;
                        break;
                    case 4:
                        //set dirty flag on
                        updatedWeaponP3Flag = true;
                        updatedWeaponP3 = data;
                        break;
                    default:
                        Debug.Log("Error: WEAPONSTATE Sender Invalid");
                        Debug.Break();
                        break;
                }
                break;
            case PacketType.DAMAGEDEALT:
                //pair sender with data
                Tuple<int, string[]> temp = Tuple.Create(sender, parsedData);
                _InstanceDamageDealt.Enqueue(temp);

                break;
            default:
                Debug.Log("Error: Invalid Datatype recieved");
                Debug.Break();

                break;
        }
    }

}
