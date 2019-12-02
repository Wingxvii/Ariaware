using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;
using RTSManagers;
using SceneManagement;


namespace netcodeRTS
{
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

        //all updates to turrets
        TURRETDATA = 9,
    }

    public class FPSDataBuffer
    {
        //LOOK UP SEPHAMORES
        //USE INSTEAD OF DOUBLE BUFFER

        public Vector3 Player1Pos = new Vector3(0, 0, 0);
        public Vector3 Player1Rot = new Vector3(0, 0, 0);
        public int Player1State = -1;
        public bool updated1 = false;

        public Vector3 Player2Pos = new Vector3(0, 0, 0);
        public Vector3 Player2Rot = new Vector3(0, 0, 0);
        public int Player2State = -1;
        public bool updated2 = false;

        public Vector3 Player3Pos = new Vector3(0, 0, 0);
        public Vector3 Player3Rot = new Vector3(0, 0, 0);
        public int Player3State = -1;
        public bool updated3 = false;

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
        private static IntPtr Client;
        private static int playerNumber = -1;

        #endregion

        //Incomming Data Type Queues
        static Queue<Tuple<int, string[]>> _InstanceDamageDealt = new Queue<Tuple<int, string[]>>();

        //Incomming state updates
        static string[] updatedWeaponP1;
        static string[] updatedWeaponP2;
        static string[] updatedWeaponP3;

        //dirty flag for state updates
        static bool updatedWeaponP1Flag = false;
        static bool updatedWeaponP2Flag = false;
        static bool updatedWeaponP3Flag = false;

        //buffers
        public static FPSDataBuffer ReadBuffer = new FPSDataBuffer();
        private static FPSDataBuffer WriteBuffer = new FPSDataBuffer();
        private static FPSDataBuffer tempBuffer;

        //turret stack
        static List<StringBuilder> turretSendStack = new List<StringBuilder>();

        // Start is called before the first frame update
        void Awake()
        {
            if (ScenePresent.Instance != null && ScenePresent.Instance.IP != null)
            {
                ip = ScenePresent.Instance.IP;
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
            /*
            if (Input.GetKey(KeyCode.F)) {
                StringBuilder finalMessage = new StringBuilder();


                finalMessage.Append(SelectionManager.Instance.mousePosition.x);
                finalMessage.Append(",");
                finalMessage.Append("0");
                finalMessage.Append(",");
                finalMessage.Append(SelectionManager.Instance.mousePosition.z);
                finalMessage.Append(",");
                finalMessage.Append("0");
                finalMessage.Append(",");
                finalMessage.Append("0");
                finalMessage.Append(",");
                finalMessage.Append("0");
                finalMessage.Append(",");
                finalMessage.Append("0");
                finalMessage.Append(",");

                SendData((int)PacketType.PLAYERDATA, finalMessage.ToString(), Client);
            }
            */

            //process instanced information recieved
            if (updatedWeaponP1Flag)
            {
                SelectionManager.Instance.players[0].GetComponent<Player>().SendWeapon(int.Parse(updatedWeaponP1[0]));
                updatedWeaponP1Flag = false;
            }
            if (updatedWeaponP2Flag)
            {
                SelectionManager.Instance.players[1].GetComponent<Player>().SendWeapon(int.Parse(updatedWeaponP2[0]));
                updatedWeaponP2Flag = false;
            }
            if (updatedWeaponP3Flag)
            {
                SelectionManager.Instance.players[2].GetComponent<Player>().SendWeapon(int.Parse(updatedWeaponP3[0]));
                updatedWeaponP3Flag = false;
            }
            if (_InstanceDamageDealt.Count > 0)
            {
                lock (_InstanceDamageDealt)
                {
                    while (_InstanceDamageDealt.Count > 0) {
                        Tuple<int, string[]> data = _InstanceDamageDealt.Dequeue();
                        //process instance damage
                        if (int.Parse(data.Item2[2]) > SelectableObject.indexedList.Count)
                        {
                            Debug.Log("Error: Indexed Object is Undefined");
                            break;
                        }
                        SelectableObject.indexedList[int.Parse(data.Item2[2])].OnDamage(int.Parse(data.Item2[1]), SelectableObject.indexedList[data.Item1 - 1]);

                    }
                }
            }
        }

        private void FixedUpdate()
        {
            SwitchBuffers();
            //buffer processing
            if (ReadBuffer.updated1) {
                SelectionManager.Instance.players[0].GetComponent<Player>().SendUpdate(ReadBuffer.Player1Pos, ReadBuffer.Player1Rot, ReadBuffer.Player1State);
                ReadBuffer.updated1 = false;
            }
            if (ReadBuffer.updated2)
            {
                SelectionManager.Instance.players[1].GetComponent<Player>().SendUpdate(ReadBuffer.Player2Pos, ReadBuffer.Player2Rot, ReadBuffer.Player2State);
                ReadBuffer.updated2 = false;
            }
            if (ReadBuffer.updated3)
            {
                SelectionManager.Instance.players[2].GetComponent<Player>().SendUpdate(ReadBuffer.Player3Pos, ReadBuffer.Player3Rot, ReadBuffer.Player3State);
                ReadBuffer.updated3 = false;
            }

        }

        //switches the buffers
        private void SwitchBuffers()
        {
            tempBuffer = ReadBuffer;
            ReadBuffer = WriteBuffer;
            WriteBuffer = tempBuffer;
        }

        //send list of string message to all others
        public static void OnSendMessage(List<String> messageStack)
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
        public static void OnSendMessage(string message)
        {
            StringBuilder finalMessage = new StringBuilder();
            finalMessage.Append(message);
            finalMessage.Append(",");
            SendData((int)PacketType.MESSAGE, finalMessage.ToString(), Client);

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
            data.TrimEnd();

            //parse the data
            string[] parsedData = data.Split(',');

            switch ((PacketType)type)
            {
                case PacketType.INIT:
                    if (parsedData.Length == 1)
                    {
                        playerNumber = Convert.ToInt32(parsedData[0]);
                    }
                    else
                    {
                        Debug.Log("Error: Invalid INIT Parsed Array Size");
                        
                    }
                    break;
                case PacketType.PLAYERDATA:
                    if (parsedData.Length == 8)
                    {
                        //lock and update by sender
                        lock (WriteBuffer)
                        {
                            switch (sender)
                            {
                                /*
                                //REMOVE THIS
                                case 1:
                                    Debug.Log("Recieved info");
                                    WriteBuffer.Player1Pos.x = float.Parse(parsedData[0]);
                                    WriteBuffer.Player1Pos.y = float.Parse(parsedData[1]);
                                    WriteBuffer.Player1Pos.z = float.Parse(parsedData[2]);
                                    WriteBuffer.Player1Rot.x = float.Parse(parsedData[3]);
                                    WriteBuffer.Player1Rot.y = float.Parse(parsedData[4]);
                                    WriteBuffer.Player1Rot.z = float.Parse(parsedData[5]);
                                    WriteBuffer.Player1State = int.Parse(parsedData[6]);
                                    WriteBuffer.updated1 = true;
                                    break;
                                    */
                                case 2:

                                    WriteBuffer.Player1Pos.x = float.Parse(parsedData[0]);
                                    WriteBuffer.Player1Pos.y = float.Parse(parsedData[1]);
                                    WriteBuffer.Player1Pos.z = float.Parse(parsedData[2]);
                                    WriteBuffer.Player1Rot.x = float.Parse(parsedData[3]);
                                    WriteBuffer.Player1Rot.y = float.Parse(parsedData[4]);
                                    WriteBuffer.Player1Rot.z = float.Parse(parsedData[5]);
                                    WriteBuffer.Player1State = int.Parse(parsedData[6]);
                                    WriteBuffer.updated1 = true;

                                    //Debug.Log(WriteBuffer.Player1Pos);

                                    break;
                                case 3:
                                    WriteBuffer.Player2Pos.x = float.Parse(parsedData[0]);
                                    WriteBuffer.Player2Pos.y = float.Parse(parsedData[1]);
                                    WriteBuffer.Player2Pos.z = float.Parse(parsedData[2]);
                                    WriteBuffer.Player2Rot.x = float.Parse(parsedData[3]);
                                    WriteBuffer.Player2Rot.y = float.Parse(parsedData[4]);
                                    WriteBuffer.Player2Rot.z = float.Parse(parsedData[5]);
                                    WriteBuffer.Player2State = int.Parse(parsedData[6]);
                                    WriteBuffer.updated2 = true;
                                    break;
                                case 4:
                                    WriteBuffer.Player3Pos.x = float.Parse(parsedData[0]);
                                    WriteBuffer.Player3Pos.y = float.Parse(parsedData[1]);
                                    WriteBuffer.Player3Pos.z = float.Parse(parsedData[2]);
                                    WriteBuffer.Player3Rot.x = float.Parse(parsedData[3]);
                                    WriteBuffer.Player3Rot.y = float.Parse(parsedData[4]);
                                    WriteBuffer.Player3Rot.z = float.Parse(parsedData[5]);
                                    WriteBuffer.Player3State = int.Parse(parsedData[6]);
                                    WriteBuffer.updated3 = true;
                                    break;
                                default:
                                    Debug.Log("Error: PLAYERDATA Sender Invalid");
                                    
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Error: Invalid PLAYERDATA Parsed Array Size");
                    }
                    break;
                case PacketType.WEAPONSTATE:
                    //update state by sender type

                    Debug.Log("Data Recieved");
                    if (parsedData.Length != 2)
                    {
                        Debug.Log(parsedData.Length);
                        Debug.Log("Error: Invalid WEAPONSTATE Parsed Array Size:" + data);
                    }
                    switch (sender)
                    {
                        case 2:
                            //set dirty flag on
                            updatedWeaponP1Flag = true;
                            updatedWeaponP1 = parsedData;
                            break;
                        case 3:
                            //set dirty flag on
                            updatedWeaponP2Flag = true;
                            updatedWeaponP2 = parsedData;
                            break;
                        case 4:
                            //set dirty flag on
                            updatedWeaponP3Flag = true;
                            updatedWeaponP3 = parsedData;
                            break;
                        default:
                            Debug.Log("Error: WEAPONSTATE Sender Invalid");
                            
                            break;
                    }
                    break;
                case PacketType.DAMAGEDEALT:

                    if (parsedData.Length != 4)
                    {
                        Debug.Log("Error: Invalid DAMAGEDEALT Parsed Array Size");
                        break;
                    }
                    if (sender == 1) {
                        Debug.Log("Error: Self Damage Recieved");
                    }

                    //pair sender with data
                    Tuple<int, string[]> temp = Tuple.Create(sender, parsedData);
                    lock (_InstanceDamageDealt)
                    {
                        _InstanceDamageDealt.Enqueue(temp);
                    }

                    break;

                case PacketType.DROIDLOCATIONS:
                    Debug.Log("Recieved DROIDLOCATIONS");
                    break;
                case PacketType.BUILD:
                    Debug.Log("Recieved BUILDDATA");
                    break;
                case PacketType.KILL:
                    Debug.Log("Recieved KILLDATA");
                    break;
                default:
                    Debug.Log("Error: Invalid Datatype recieved:" + type.ToString());
                    
                    break;
            }
        }

        //this sends all droid positions
        public static void SendDroidPositions()
        {
            StringBuilder dataToSend = new StringBuilder();

            foreach (Droid droid in DroidManager.Instance.ActiveDroidPool)
            {

                //send object id
                dataToSend.Append(droid.id);
                dataToSend.Append(",");

                //send object positions
                dataToSend.Append(droid.transform.position.x);
                dataToSend.Append(",");
                dataToSend.Append(droid.transform.position.y);
                dataToSend.Append(",");
                dataToSend.Append(droid.transform.position.z);
                dataToSend.Append(",");
                dataToSend.Append(droid.transform.rotation.eulerAngles.y);
                dataToSend.Append(",");

            }

            SendData((int)PacketType.DROIDLOCATIONS, dataToSend.ToString(), Client);
        }

        public static void SendBuildEntity(SelectableObject obj)
        {
            StringBuilder dataToSend = new StringBuilder();
            //Debug.Log(obj.type);

            if (obj.id == 0) {
                Debug.Log("Error Invalid ID Sent");
                Debug.Break();
            }

            //add object id
            dataToSend.Append(obj.id);
            dataToSend.Append(",");

            //add object type
            dataToSend.Append(((int)obj.type));
            dataToSend.Append(",");

            //add object position x
            dataToSend.Append(obj.transform.position.x);
            dataToSend.Append(",");

            //add object position y
            dataToSend.Append(obj.transform.position.y);
            dataToSend.Append(",");

            //add object position z
            dataToSend.Append(obj.transform.position.z);
            dataToSend.Append(",");

            //Debug.Log(dataToSend.ToString());

            SendData((int)PacketType.BUILD, dataToSend.ToString(), Client);
        }

        public static void SendKilledEntity(SelectableObject obj)
        {

            //Debug.Log("Dead Droid Sent");
            StringBuilder dataToSend = new StringBuilder();

            //add object id
            dataToSend.Append(obj.id);
            dataToSend.Append(",");

            SendData((int)PacketType.KILL, dataToSend.ToString(), Client);
        }

        //send game state
        public static void SendGameState(int state)
        {
            StringBuilder dataToSend = new StringBuilder();
            dataToSend.Append(state);
            dataToSend.Append(",");
            SendData((int)PacketType.GAMESTATE, dataToSend.ToString(), Client);

        }

        //send damaged player
        public static void SendDamagePlayer(int damage, int player, int culprit)
        {
            StringBuilder dataToSend = new StringBuilder();

            //RESET THIS
            dataToSend.Append(player);
            //dataToSend.Append("1");

            dataToSend.Append(",");
            dataToSend.Append(damage);
            dataToSend.Append(",");
            dataToSend.Append(culprit);
            dataToSend.Append(",");

            SendData((int)PacketType.DAMAGEDEALT, dataToSend.ToString(), Client);

        }

        public static void SendTurretStack() {
            if (turretSendStack.Count != 0)
            {
                foreach (StringBuilder packetData in turretSendStack)
                {
                    SendData((int)PacketType.TURRETDATA, packetData.ToString(), Client);
                }
                turretSendStack.Clear();
            }
        }


        public static void AddDataToStack(int id, Vector3 turretRot, int state) {

            //Debug.Log("Added to Stack");

            StringBuilder dataToSend = new StringBuilder();
            dataToSend.Append(id);
            dataToSend.Append(",");
            dataToSend.Append(turretRot.x);
            dataToSend.Append(",");
            dataToSend.Append(turretRot.y);
            dataToSend.Append(",");
            dataToSend.Append(turretRot.z);
            dataToSend.Append(",");
            dataToSend.Append(state);
            dataToSend.Append(",");

            turretSendStack.Add(dataToSend);
        }
    }
}