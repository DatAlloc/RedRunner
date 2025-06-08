using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using RedRunner;
using RedRunner.Characters;

namespace DatAlloc
{
    public class NodeTcpClient : MonoBehaviour
    {
        public static string sNodedIPAddress = "127.0.0.1";
        public static string sNodedTcpPort = "9123";

        private TcpClient m_Connection = null;
        private IPAddress m_NodedIPAddress = null;
        private int m_NodedTcpPort = 0;
        private float m_Score = 0.0f;

        [SerializeField]
        private Character m_Character;

        #region Getters
        private bool IsConnected
        {
            get
            {
                return m_Connection != null && m_Connection.Connected;
            }
        }
        #endregion

        private void LoadConfig()
        {
            string config = Application.persistentDataPath + "/noded.config";
            try
            {
                IniFileReader rd = new IniFileReader(config);
                m_NodedIPAddress = IPAddress.Parse(
                    rd.Get("default", "ipaddr", sNodedIPAddress));
                m_NodedTcpPort = int.Parse(
                    rd.Get("default", "port", sNodedTcpPort));
            }
            catch (Exception e)
            {
                Debug.LogError("[DA] LoadConfig(). " + e.Message);
                m_NodedIPAddress = IPAddress.Parse(sNodedIPAddress);
                m_NodedTcpPort = int.Parse(sNodedTcpPort);
            }
            Debug.Log("[DA] Noded endpoint " + m_NodedIPAddress.ToString()
                + ":" + m_NodedTcpPort.ToString());
        }

        private void Connect(bool reconnect = false)
        {
            try
            {
                if (reconnect && IsConnected)
                {
                    Disconnect();
                }

                if (m_Connection == null)
                {
                    m_Connection = new TcpClient();
                }

                if (!m_Connection.Connected)
                {
                    m_Connection.Connect(m_NodedIPAddress, m_NodedTcpPort);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[DA] Connect(). " + e.Message);
            }
            Debug.Log("[DA] " + (IsConnected ? "Connected to Noded :)" : "Not connected to Noded :("));
        }

        private void Disconnect()
        {
            if (IsConnected)
            {
                m_Connection.Close();
                Debug.Log("[DA] Disconnected from Noded");
            }
            m_Connection = null;
        }

        private void SendBytes(byte[] message)
        {
            try
            {
                if (IsConnected)
                {
                    m_Connection.GetStream().Write(message, 0, message.Length);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[DA] SendBytes(). " + e.Message);
            }
        }
 
        // Start is called before the first frame update
        void Start()
        {
            LoadConfig();
            Connect();
            GameManager.Singleton.m_Coin.AddEventAndFire(OnCoinCollected, this);
            GameManager.OnScoreChanged += OnScoreChanged;
            m_Character.IsDead.AddEventAndFire(OnDeathEvent, this);
        }
        
        private void OnScoreChanged(float newScore, float highScore, float lastScore)
        {
            m_Score = newScore; /* 32 bits*/
        }

        private void OnCoinCollected(int newCoinValue)
        {
            Debug.Log("[DA] Coin collected " + newCoinValue.ToString());

            byte[] m = new byte[32];
            m[0] = 0xa3; /* map (3) */
            m[1] = 0x62; /*  text (2) */
            m[2] = 0x74; /*   t */
            m[3] = 0x73; /*   s */
            m[4] = 0x1b; /*  unsigned */
            m[5] = 0x00; /*   timestamp*/
            m[6] = 0x00;
            m[7] = 0x00;
            m[8] = 0x00;
            m[9] = 0x00;
            m[10] = 0x00;
            m[11] = 0x00;
            m[12] = 0x00;
            m[13] = 0x63; /*  text (3) */
            m[14] = 0x6b; /*   k */
            m[15] = 0x65; /*   e */
            m[16] = 0x79; /*   y */
            m[17] = 0x65; /*  text (5) */
            m[18] = 0x63; /*   c */
            m[19] = 0x6f; /*   o */
            m[20] = 0x69; /*   i */
            m[21] = 0x6e; /*   n */
            m[22] = 0x73; /*   s */
            m[23] = 0x63; /*  text (3) */
            m[24] = 0x76; /*   v */
            m[25] = 0x61; /*   a */
            m[26] = 0x6c; /*   l */
            m[27] = 0x1a; /*  unsigned */
            m[28] = (byte)((newCoinValue & 0xff000000) >> 24);
            m[29] = (byte)((newCoinValue & 0xff0000) >> 16);
            m[30] = (byte)((newCoinValue & 0xff00) >> 8);
            m[31] = (byte)(newCoinValue & 0xff);
 
            SendBytes(m);
        }

        private void OnDeathEvent(bool isDead)
        {
            if (isDead)
            {
                Debug.Log("[DA] Reddy is dead :( Score " + m_Score.ToString());
                /* TODO: send score */
                Connect(true);
            }
            m_Score = 0.0f;
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }
    }
}