using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace NET_PACKET
{
    public enum TurretStateData
    {
        Idle,
        IdleShooting,
        PositionalShooting,
        TargetedShooting,
        Recoil,
        Reloading,
    }

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
        // list
        TURRET_ROT = 9,
    }

    public class TurretSingle
    {
        public int ID;
        public Vector3 euler;
        public uint state;
    }

    public class TurretBuffer
    {
        public TurretBuffer()
        {
            turretData = new List<TurretSingle>();
        }

        public bool DataReady = false;

        public List<TurretSingle> turretData;
    }

    public class RTSsingle
    {
        //public uint index;
        public Vector3 position = Vector3.zero;
        public float Yrot = 0f;
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

    public class NetworkDataManager : InitializableObject
    {


        const int droidMax = 100;
        public const int FPSmax = 3;

        public static List<ReadBuild> builds { get; protected set; }
        public static List<ReadDamagePlayer> damagesPlayer { get; protected set; }
        public static List<ReadDamageNPC> damagesNPC { get; protected set; }
        public static List<ReadDroid> droids { get; protected set; }
        public static List<ReadGameState> gameStates { get; protected set; }
        public static List<ReadKilled> kills { get; protected set; }
        public static List<ReadPlayerData> playersData { get; protected set; }
        public static List<ReadWeaponSwitch> weaponsSwitch { get; protected set; }
        public static List<TurretSingle> orderedTurretData { get; protected set; }

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

        public int GetPlayerNum()
        {
            if (!noServer)
                yourID = GetPlayerNumber(Client) - 2;
            else
                yourID = 0;
            return yourID;
        }

        public static void SendNetData(int type, string str)
        {
            SendData(type, str, Client);
        }

        public string ip = null;
        private static IntPtr Client;
        private static int playerNumber = -1;
        bool noServer = false;

        #endregion

        public static RTSDataBuffer ReadRTS = new RTSDataBuffer(droidMax);
        private static RTSDataBuffer WriteRTS = new RTSDataBuffer(droidMax);
        private static RTSDataBuffer tempRTS;

        public static FPSDataBuffer ReadFPS = new FPSDataBuffer(FPSmax);
        private static FPSDataBuffer WriteFPS = new FPSDataBuffer(FPSmax);
        private static FPSDataBuffer tempFPS;

        public static TurretBuffer ReadTurret = new TurretBuffer();
        private static TurretBuffer WriteTurret = new TurretBuffer();
        private static TurretBuffer tempTurret;

        public static WeaponDataPackage[] weaponStates = new WeaponDataPackage[FPSmax];
        public static Queue<DamagePlayerPackage> damagePlayer = new Queue<DamagePlayerPackage>();
        public static Queue<DamageNPCPackage> damageNPC = new Queue<DamageNPCPackage>();

        public static Queue<BuildPackage> build = new Queue<BuildPackage>();
        public static Queue<int> kill = new Queue<int>();
        public static uint gameState;

        int yourID = 0;

        private static void SwitchRTSBuffers()
        {
            tempRTS = ReadRTS;
            ReadRTS = WriteRTS;
            WriteRTS = tempRTS;
        }

        private static void SwitchFPSBuffers()
        {
            tempFPS = ReadFPS;
            ReadFPS = WriteFPS;
            WriteFPS = tempFPS;
        }

        private static void SwitchTurretBuffers()
        {
            tempTurret = ReadTurret;
            ReadTurret = WriteTurret;
            WriteTurret = tempTurret;
        }

        protected override bool CreateVars()
        {
            FPSManager.FM.netParsed = false;

            if (base.CreateVars())
            {
                builds = new List<ReadBuild>();
                damagesPlayer = new List<ReadDamagePlayer>();
                damagesNPC = new List<ReadDamageNPC>();
                droids = new List<ReadDroid>();
                gameStates = new List<ReadGameState>();
                kills = new List<ReadKilled>();
                playersData = new List<ReadPlayerData>();
                weaponsSwitch = new List<ReadWeaponSwitch>();
                orderedTurretData = new List<TurretSingle>();

                for (int i = 0; i < FPSmax; ++i)
                {
                    weaponStates[i] = new WeaponDataPackage();
                    ReadFPS.playerData[i] = new FPSsingle();
                    WriteFPS.playerData[i] = new FPSsingle();
                }

                for (int i = 0; i < droidMax; ++i)
                {
                    ReadRTS.droidData[i] = new RTSsingle();
                    WriteRTS.droidData[i] = new RTSsingle();
                }

                if (SceneManagement.ScenePresent.Instance != null)
                {
                    //Debug.Log(ip);
                    ip = SceneManagement.ScenePresent.Instance.IP;
                }
                else
                    noServer = true;

                if (ip != null)
                {
                    //Debug.Log(ip);
                    //client Init  
                    Client = CreateClient();
                    Connect(ip, Client);
                    StartUpdating(Client);
                    SetupPacketReception(PacketRecieved);
                    //blah
                    //yourID = GetPlayerNumber(Client);
                    //Debug.Log(GetPlayerNumber(Client));
                    //if (yourID < 0)
                    //    yourID = 0;
                    //Debug.Log(yourID);
                }

                //AddFirst();
                //AddSecond();
                //AddThird();
                //AddFourth();
                //AddFifth();

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

        //protected override void First()
        //{
        //    
        //}
        //
        //protected override void Second()
        //{
        //    
        //}
        //
        //protected override void Third()
        //{
        //    
        //}
        //
        //protected override void Fourth()
        //{
        //    
        //}
        //
        //protected override void Fifth()
        //{
        //    
        //}

        public void ChugDamagePlayerQueue()
        {
            while (damagePlayer.Count > 0)
            {
                for (int j = 0; j < damagesPlayer.Count; ++j)
                {
                    if (damagesPlayer[j].b.ID == damagePlayer.Peek().receiver)
                    {
                        damagesPlayer[j].Damages.Enqueue(new DamagePlayerUnit(damagePlayer.Peek().damage, damagePlayer.Peek().culprit));
                        //break;
                    }
                }
                damagePlayer.Dequeue();
            }
        }

        public void ChugDamageNPCQueue()
        {
            while (damageNPC.Count > 0)
            {
                for (int j = 0; j < damagesNPC.Count; ++j)
                {
                    damagesNPC[j].Damages.Enqueue(damageNPC.Peek().damage);
                }
                damagePlayer.Dequeue();
            }
        }

        public void ChugBuildQueue()
        {
            while (build.Count > 0)
            {
                for (int j = 0; j < builds.Count; ++j)
                {
                    builds[j].Build.Enqueue(new BuildUnit(build.Peek().ID, build.Peek().type, build.Peek().position));
                }
                build.Dequeue();
            }
        }

        public void ChugKillQueue()
        {
            while (kill.Count > 0)
            {
                for (int j = 0; j < kills.Count; ++j)
                {
                    kills[j].Kills.Enqueue(kill.Peek());
                }

                kill.Dequeue();
            }
        }

        public void ChugTurrets()
        {
            if (ReadTurret.DataReady)
            {
                for (int i = 0; i < orderedTurretData.Count; ++i)
                {
                    orderedTurretData[i] = null;
                }

                ReadTurret.DataReady = false;

                for (int i = 0; i < ReadTurret.turretData.Count; ++i)
                {
                    while (orderedTurretData.Count <= ReadTurret.turretData[i].ID)
                    {
                        orderedTurretData.Add(null);
                    }

                    orderedTurretData[ReadTurret.turretData[i].ID] = ReadTurret.turretData[i];
                }

                //Debug.Log("turrets chugged");
            }
        }

        static void PacketRecieved(int type, int sender, string data)
        {
            //Debug.Log(type);
            //Debug.Log(sender);
            //Debug.Log(data);
            //parse the data
            string[] parsedData = data.Split(',');
            //for (int i = 0; i < 5; i++)
            //{
            //    Debug.Log(parsedData[i]);
            //}

            switch ((PacketType)type)
            {
                case PacketType.INIT:
                    if (parsedData.Length == 2)
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
                    if (parsedData.Length == 8)
                    {
                        //lock and update by sender
                        lock (WriteFPS)
                        {
                            if (sender > 1 && sender <= FPSmax + 1)
                            {
                                //Debug.Log(sender + " RECEIVED");
                                ParseVector3(ref WriteFPS.playerData[sender - 2].position, parsedData, 0);
                                ParseVector3(ref WriteFPS.playerData[sender - 2].rotation, parsedData, 3);
                                WriteFPS.playerData[sender - 2].state = int.Parse(parsedData[6]);
                                WriteFPS.playerData[sender - 2].flag = true;
                                //Debug.Log(data);
                            }
                            else
                            {
                                Debug.Log("Error: PLAYERDATA Sender Invalid");
                                Debug.Break();
                            }
                        }

                        SwitchFPSBuffers();
                    }
                    else
                    {
                        Debug.Log("Error: Invalid PLAYERDATA Parsed Array Size");
                        Debug.Break();
                    }
                    break;
                case PacketType.WEAPONSTATE:
                    //update state by sender type
                    if (parsedData.Length != 2)
                    {
                        Debug.Log("Error: Invalid WEAPONSTATE Parsed Array Size");
                        Debug.Break();
                    }
                    if (sender > 1 && sender <= FPSmax + 1)
                    {
                        weaponStates[sender - 2].WeaponState = uint.Parse(parsedData[0]);
                        weaponStates[sender - 2].flag = true;
                        //Debug.Log((sender - 2) + ", " + weaponStates[sender - 2].WeaponState);
                    }
                    else
                    {
                        Debug.Log("Error: WEAPONSTATE Sender Invalid");
                        Debug.Break();
                    }
                    break;
                case PacketType.DAMAGEDEALT:
                    if (sender == 1 && (parsedData.Length != 4 || parsedData.Length != 3))
                    {
                        Debug.Log(parsedData.Length);
                        Debug.Log("Error: Invalid DAMAGEDEALT Parsed Array Size");
                        Debug.Break();
                    }
                    //else if (parsedData.Length != 3)
                    //{
                    //    Debug.Log("Error: Invalid DAMAGEDEALT Parsed Array Size");
                    //    Debug.Break();
                    //}
                    if (sender == 1)
                    {
                        DamagePlayerPackage dpp = new DamagePlayerPackage(uint.Parse(parsedData[0]) - 1, int.Parse(parsedData[1]), uint.Parse(parsedData[2]));
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
                    if (parsedData.Length % 5 - 1 == 0)
                    {
                        lock (WriteRTS)
                        {
                            if (sender == 1)
                            {
                                for (int i = 0; i < parsedData.Length - 1; i += 5)
                                {
                                    int index = int.Parse(parsedData[i]) - FPSmax - 1;
                                    //WriteRTS.droidData[i / 4].index = uint.Parse(parsedData[i]);
                                    ParseVector3(ref WriteRTS.droidData[index].position, parsedData, i + 1);
                                    WriteRTS.droidData[index].Yrot = float.Parse(parsedData[i + 4]);
                                    //Debug.Log(index + ", " + WriteRTS.droidData[index].position);
                                    WriteRTS.droidData[index].flag = true;
                                }
                            }
                            else
                            {
                                Debug.Log("Error: DROIDLOCATIONS Sender Invalid!");
                                Debug.Break();
                            }
                        }

                        SwitchRTSBuffers();
                    }
                    else
                    {
                        Debug.Log("Error: Invalid DROIDLOCATIONS Parsed Array Size");
                        Debug.Break();
                    }
                    break;
                case PacketType.BUILD:
                    if (parsedData.Length != 6)
                    {
                        Debug.Log("Error: Invalid BUILD Parsed Array Size");
                        Debug.Break();
                    }
                    if (sender == 1)
                    {
                        //Debug.Log(parsedData[1]);
                        //Debug.Log(uint.Parse(parsedData[0]));
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
                    if (parsedData.Length != 2)
                    {
                        Debug.Log("Error: Invalid KILL Parsed Array Size");
                        Debug.Break();
                    }
                    if (sender == 1)
                    {
                        //Debug.Log("KILLED! " + parsedData[0] + ", " + parsedData[1]);
                        kill.Enqueue(int.Parse(parsedData[0]));
                    }
                    else
                    {
                        Debug.Log("Error: KILL Sender Invalid!");
                        Debug.Break();
                    }
                    break;
                case PacketType.GAMESTATE:
                    if (parsedData.Length != 2)
                    {
                        Debug.Log("Error: Invalid GAMESTATE Parsed Array Size");
                        Debug.Break();
                    }
                    if (sender == 1)
                    {
                        gameState = uint.Parse(parsedData[0]);
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
                case PacketType.TURRET_ROT:

                    Debug.Log(parsedData.Length);
                    if (parsedData.Length % 5 - 1 == 0)
                    {
                        if (sender == 1)
                        {
                            lock(WriteTurret)
                            {
                                WriteTurret.turretData.Clear();

                                for (int i = 0; i < parsedData.Length - 1; i += 5)
                                {
                                    TurretSingle TS = new TurretSingle();
                                    TS.ID = int.Parse(parsedData[i]);
                                    ParseVector3(ref TS.euler, parsedData, i + 1);
                                    TS.state = uint.Parse(parsedData[i + 4]);
                                    WriteTurret.turretData.Add(TS);
                                }

                                WriteTurret.DataReady = true;
                            }

                            SwitchTurretBuffers();
                        }
                        else
                        {
                            Debug.Log("Error: TURRET_ROT Sender Invalid!");
                            Debug.Break();
                        }
                    }
                    else
                    {
                        Debug.Log("Error: Invalid TURRET_ROT Parsed Array Size");
                        Debug.Break();
                    }

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
