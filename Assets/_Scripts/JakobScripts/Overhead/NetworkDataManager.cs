using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace NET_PACKET
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
    }

    public class RTSsingle
    {
        public uint index;
        public Vector3 position = Vector3.zero;
        public bool flag = false;
    }

    public class RTSDataBuffer
    {
        public RTSDataBuffer(int arrSize)
        {
            droidData = new RTSsingle[arrSize];
        }

        public RTSsingle[] droidData;
    }

    public class FPSsingle
    {
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public int state = -1;
        public bool flag = false;
    }

    public class FPSDataBuffer
    {
        public FPSDataBuffer(int playerMax)
        {
            playerData = new FPSsingle[playerMax];
        }

        public FPSsingle[] playerData;
    }

    public class WeaponDataPackage
    {
        public uint WeaponState;
        public bool flag = false;
    }

    public class DamagePlayerPackage
    {
        public DamagePlayerPackage(uint _receiver, int _damage, uint _culprit)
        {
            receiver = _receiver;
            damage = _damage;
            culprit = _culprit;
        }

        public uint receiver;
        public int damage;
        public uint culprit;
    }

    public class DamageNPCPackage
    {
        public DamageNPCPackage(int _damage, uint _target)
        {
            damage = _damage;
            target = _target;
        }

        public int damage;
        public uint target;
    }

    public class BuildPackage
    {
        public BuildPackage(uint _id, uint _type, Vector3 _position)
        {
            ID = _id;
            type = _type;
            position = _position;
        }

        public uint ID;
        public uint type;
        public Vector3 position;
    }

    public class NetworkDataManager : UpdateableObject
    {
        const int droidMax = 100;
        const int FPSmax = 3;

        public static List<ReadBuild> builds { get; protected set; }
        public static List<ReadDamage> damages { get; protected set; }
        public static List<ReadDroid> droids { get; protected set; }
        public static List<ReadGameState> gameStates { get; protected set; }
        public static List<ReadKilled> killed { get; protected set; }

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

        public static RTSDataBuffer ReadRTS = new RTSDataBuffer(droidMax);
        private static RTSDataBuffer WriteRTS = new RTSDataBuffer(droidMax);
        private static RTSDataBuffer tempRTS;

        public static FPSDataBuffer ReadFPS = new FPSDataBuffer(FPSmax);
        private static FPSDataBuffer WriteFPS = new FPSDataBuffer(FPSmax);
        private static FPSDataBuffer tempFPS;

        public static WeaponDataPackage[] weaponStates = new WeaponDataPackage[FPSmax];
        public static Queue<DamagePlayerPackage> damagePlayer = new Queue<DamagePlayerPackage>();
        public static Queue<DamageNPCPackage> damageNPC = new Queue<DamageNPCPackage>();

        public static Queue<BuildPackage> build = new Queue<BuildPackage>();
        public static Queue<int> kill = new Queue<int>();
        public static Queue<int> gameState = new Queue<int>();

        private void Update()
        {
            SendData((int)PacketType.PLAYERDATA,
                (0.3f).ToString() + "," + (0.3f).ToString() + "," + (0.3f).ToString() + "," + (0.3f).ToString() + "," + (0.3f).ToString() + "," + (0.3f).ToString() + "," + (1).ToString() + ",", 
                Client);
        }

        private void SwitchRTSBuffers()
        {
            tempRTS = ReadRTS;
            ReadRTS = WriteRTS;
            WriteRTS = tempRTS;
        }

        private void SwitchFPSBuffers()
        {
            tempFPS = ReadFPS;
            ReadFPS = WriteFPS;
            WriteFPS = tempFPS;
        }

        protected override bool CreateVars()
        {
            if (base.CreateVars())
            {
                if (ip != null)
                {
                    //client Init  
                    Client = CreateClient();
                    Connect(ip, Client);
                    StartUpdating(Client);
                    SetupPacketReception(PacketRecieved);

                }

                return true;
            }

            return false;
        }

        protected override void DestroyVars()
        {
            //clean up client
            DeleteClient(Client);

            base.DestroyVars();
        }

        static void PacketRecieved(int type, int sender, string data)
        {
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
                        Debug.Break();
                    }
                    break;
                case PacketType.PLAYERDATA:
                    if (parsedData.Length == 7)
                    {
                        //lock and update by sender
                        lock (WriteFPS)
                        {
                            if (sender > 1 && sender <= FPSmax + 1)
                            {
                                ParseVector3(ref WriteFPS.playerData[sender - 2].position, parsedData, 0);
                                ParseVector3(ref WriteFPS.playerData[sender - 2].rotation, parsedData, 3);
                                WriteFPS.playerData[sender - 2].state = int.Parse(parsedData[6]);
                                WriteFPS.playerData[sender - 2].flag = true;
                                Debug.Log(data);
                            }
                            else
                            {
                                Debug.Log("Error: PLAYERDATA Sender Invalid");
                                Debug.Break();
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
                    if (parsedData.Length != 1)
                    {
                        Debug.Log("Error: Invalid WEAPONSTATE Parsed Array Size");
                        Debug.Break();
                    }
                    if (sender > 1 && sender <= FPSmax + 1)
                    {
                        weaponStates[sender - 2].WeaponState = uint.Parse(parsedData[0]);
                        weaponStates[sender - 2].flag = true;
                    }
                    else
                    {
                        Debug.Log("Error: WEAPONSTATE Sender Invalid");
                        Debug.Break();
                    }
                    break;
                case PacketType.DAMAGEDEALT:
                    if (sender == 1 && parsedData.Length != 3)
                    {
                        Debug.Log("Error: Invalid DAMAGEDEALT Parsed Array Size");
                        Debug.Break();
                    }
                    else if (parsedData.Length != 2)
                    {
                        Debug.Log("Error: Invalid DAMAGEDEALT Parsed Array Size");
                        Debug.Break();
                    }
                    if (sender == 1)
                    {
                        DamagePlayerPackage dpp = new DamagePlayerPackage(uint.Parse(parsedData[0]), int.Parse(parsedData[1]), uint.Parse(parsedData[2]));
                        damagePlayer.Enqueue(dpp);
                    }
                    else if (sender > 1 && sender <= FPSmax + 1)
                    {
                        DamageNPCPackage dnp = new DamageNPCPackage(int.Parse(parsedData[0]), uint.Parse(parsedData[1]));
                        damageNPC.Enqueue(dnp);
                    }
                    else
                    {
                        Debug.Log("Error: DAMAGEDEALT Sender Invalid!");
                        Debug.Break();
                    }
                    break;
                case PacketType.DROIDLOCATIONS:
                    if (parsedData.Length % 4 == 0)
                    {
                        lock (WriteRTS)
                        {
                            if (sender == 1)
                            {
                                for (int i = 0; i < parsedData.Length; i += 4)
                                {
                                    WriteRTS.droidData[i / 4].index = uint.Parse(parsedData[i]);
                                    ParseVector3(ref WriteRTS.droidData[i / 4].position, parsedData, i + 1);
                                    WriteRTS.droidData[i / 4].flag = true;
                                }
                            }
                            else
                            {
                                Debug.Log("Error: DROIDLOCATIONS Sender Invalid!");
                                Debug.Break();
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Error: Invalid DROIDLOCATIONS Parsed Array Size");
                        Debug.Break();
                    }
                    break;
                case PacketType.BUILD:
                    if (parsedData.Length != 5)
                    {
                        Debug.Log("Error: Invalid BUILD Parsed Array Size");
                        Debug.Break();
                    }
                    if (sender == 1)
                    {
                        BuildPackage bp = new BuildPackage(uint.Parse(parsedData[0]), uint.Parse(parsedData[1]), ParseIntoVector3(parsedData, 2));
                        build.Enqueue(bp);
                    }
                    else
                    {
                        Debug.Log("Error: BUILD Sender Invalid!");
                        Debug.Break();
                    }
                    break;
                case PacketType.KILL:
                    if (parsedData.Length != 1)
                    {
                        Debug.Log("Error: Invalid KILL Parsed Array Size");
                        Debug.Break();
                    }
                    if (sender == 1)
                    {
                        kill.Enqueue(int.Parse(parsedData[0]));
                    }
                    else
                    {
                        Debug.Log("Error: KILL Sender Invalid!");
                        Debug.Break();
                    }
                    break;
                case PacketType.GAMESTATE:
                    if (parsedData.Length != 1)
                    {
                        Debug.Log("Error: Invalid GAMESTATE Parsed Array Size");
                        Debug.Break();
                    }
                    if (sender == 1)
                    {
                        gameState.Enqueue(int.Parse(parsedData[0]));
                    }
                    else
                    {
                        Debug.Log("Error: GAMESTATE Sender Invalid!");
                        Debug.Break();
                    }
                    break;

                default:
                    Debug.Log("Error: Invalid Datatype recieved");
                    Debug.Break();

                    break;
            }
        }

        static void ParseVector3(ref Vector3 destination, string[] data, int startIndex)
        {
            destination.x = float.Parse(data[startIndex]);
            destination.y = float.Parse(data[startIndex + 1]);
            destination.z = float.Parse(data[startIndex + 2]);
        }

        static Vector3 ParseIntoVector3(string[] data, int startIndex)
        {
            return new Vector3(float.Parse(data[startIndex]), float.Parse(data[startIndex + 1]), float.Parse(data[startIndex + 2]));
        }
    }
}
