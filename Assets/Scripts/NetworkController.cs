namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// This class is for holding and initialising connections between a client as server and (an)other client(s).
    /// </summary>
    public class NetworkController : MonoBehaviour
    {
        #region "global properties"
        #region "Fiels"
        /// <summary>
        /// The server will not allow more than that number of players to be connected (inlcuding itself).
        /// </summary>
        public const int REALMAXIMUMOFPLAYER = 8;

        /// <summary>
        /// used port
        /// </summary>
        private int _port = 25002; // 50005;

        /// <summary>
        /// Holds the number of users in team 1 (serverside variable)
        /// </summary>
        private int _team1Counter;

        /// <summary>
        /// Holds the number of users in team 1 (serverside variable)
        /// </summary>
        private int _team2Counter;

        /// <summary>
        /// Equals 0 when no player said that he is ready to play in Spezialmode.
        /// </summary>
        private int _ready;
        #endregion

        #region "properties"
        /// <summary>
        /// Gets or sets number received by the gamehost.
        /// </summary>
        public int ClientNumber { get; set; }

        /// <summary>
        /// Gets or sets the team this player will be in / is in.
        /// </summary>
        public int TeamNumber { get; set; }

        /// <summary>
        /// Gets or sets the chosen character of this player.
        /// </summary>
        public string Character { get; set; }

        /// <summary>
        /// Gets or sets how many clients clicked the ready button after choosing a character.
        /// </summary>
        public int Ready
        {
            get
            {
                return _ready;
            }

            set
            {
                _ready = Mathf.Clamp(value, 0, REALMAXIMUMOFPLAYER);
            }
        } // SUBJECT TO CHANGE

        /// <summary>
        /// Gets or sets the maximum number of players which can be connected over network.
        /// </summary>
        public int MaxPlayer
        {
            get
            {
                return Network.maxConnections;
            }

            set
            {
                Network.maxConnections = value - 1;
            }
        }

        /// <summary>
        /// Gets or sets the name the user gave himself ingame.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets whether the opponent has quit by pressing the escape button.
        /// </summary>
        /// <remarks>
        /// His teamnumber is stored here so that this could indicate whether an whole team has quit.
        /// </remarks>
        public int OpponentEscaped { get; set; }

        /// <summary>
        /// Gets or sets the status of the connection.
        /// Used since unitys function state "connecting" doesn't work / get set while connecting.
        /// </summary>
        public NetworkPeerType Status { get; set; }

        /// <summary>
        /// Gets or sets the Ip of the game host (port incl.)
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Gets the number of player which are connected to the server. (Server excluded)
        /// </summary>
        public int NumOfConnections
        {
            get
            {
                return Network.connections.Length;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this player is the gamehost.
        /// </summary>
        public bool IsServer
        {
            get
            {
                return Network.isServer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this player is a gameclient.
        /// </summary>
        public bool IsClient
        {
            get
            {
                return Network.isClient;
            }
        }
        #endregion
        #endregion

        #region "Methods"
        #region "unity triggered"
        /// <summary>
        /// Sets default value of OpponentEscaped and Username.
        /// </summary>
        public void Start()
        {
            Username = string.Empty;
            OpponentEscaped = -1;
        }

        /// <summary>
        /// prevent destroying this object on scene changes
        /// </summary>
        public void Awake()
        {
            DontDestroyOnLoad(this);
        }

        /// <summary>
        /// Triggered when a client successfully connected to the server.
        /// </summary>
        public void OnConnectedToServer()
        {
            OpponentEscaped = -1;
            Status = NetworkPeerType.Client;
            Application.LoadLevel("GameSelection");
        }

        /// <summary>
        /// Prevent that a Client connects to the server when slots are not full and the players already playing.
        /// </summary>
        /// <param name="other">the player who connected</param>
        public void OnPlayerConnected(NetworkPlayer other)
        {
            if (Application.loadedLevel.Equals("SM_Game"))
            {
                Network.CloseConnection(other, true);
                return;
            }

            networkView.RPC("SetClientNumber", other, ClientNumber);
            ClientNumber++;

            if (_team2Counter < _team1Counter)
            {
                networkView.RPC("SetTeamNumber", other, 2);
                ++_team2Counter;
            }
            else
            {
                networkView.RPC("SetTeamNumber", other, 1);
                ++_team1Counter;
            }
        }

        /// <summary>
        /// Changes the scene when the host lose the connection to a client.
        /// </summary>
        /// <param name="client">player who got disconnected</param>
        public void OnPlayerDisconnected(NetworkPlayer client)
        {
            if (IsServer)
            {
                --Ready;
                string name = Application.loadedLevelName;
                if (!name.Equals("MainMenu"))
                {
                    if (name.Equals("SM_Game"))
                    {
                        Application.LoadLevel("StatisticScreen");
                    }
                    else if (IsServer && !name.Equals("SMHostConfig") && !name.Equals("StatisticScreen"))
                    {
                        Application.LoadLevel("SMHostConfig");
                    }
                }
            }
        }

        /// <summary>
        /// Changes the scene when the client gets disconnected from the server.
        /// </summary>
        public void OnDisconnectedFromServer()
        {
            Status = NetworkPeerType.Disconnected;
            string name = Application.loadedLevelName;
            if (!name.Equals("MainMenu"))
            {
                if (name.Equals("SM_Game"))
                {
                    Application.LoadLevel("StatisticScreen");
                }
                else if (IsClient && !name.Equals("StatisticScreen"))
                {
                    Application.LoadLevel("SMJoinConfig");
                }
            }
        }

        /// <summary>
        /// Sets the status of the connection to disconnected.
        /// </summary>
        /// <param name="error">The error which is send when the client stops trying to connect (mostly a timeout)</param>
        public void OnFailedToConnect(NetworkConnectionError error)
        {
            Status = NetworkPeerType.Disconnected;
        }
        #endregion

        /// <summary>
        /// Start trying to connect to a gamehost specified by the given ip.
        /// Methode could end without error before a connection is etablished!
        /// </summary>
        /// <param name="ip">ip to connect to</param>
        /// <returns>Error on initiate connecting routine</returns>
        public NetworkConnectionError Connect(string ip)
        {
            Status = NetworkPeerType.Connecting;
            NetworkConnectionError error = Network.Connect(ip, _port);
            return error;
        }

        /// <summary>
        /// Setting up a server (/Gamehost). Doesn't run correctly when a server already running on this port.
        /// </summary>
        /// <returns>Error on settingup (excluding port already used)</returns>
        public NetworkConnectionError CreateServer()
        {
            if (IsServer)
            {
                Disconnect();
            }

            OpponentEscaped = -1;
            bool useNat = !Network.HavePublicAddress();
            NetworkConnectionError error = Network.InitializeServer(REALMAXIMUMOFPLAYER - 1, _port, useNat);
            Ip = Network.player.ipAddress;
            Ip += ":" + Network.player.externalIP;
            Network.maxConnections = 2;
            TeamNumber = 1;
            ++_team1Counter;
            ClientNumber = 0;
            Status = NetworkPeerType.Server;

            /* // on adhoc netzwork errors: try it with this (outdated) function uncommented 
            // if it failed to initialise the server try it without nat punchthrough
            if (error != NetworkConnectionError.NoError)
            {
                error = Network.InitializeServer(_maxPlayer - 1, _port);//, useNat);
            }
             */

            Ready = 0;
            return error;
        }

        /// <summary>
        /// Close connection to server and/or clients.
        /// </summary>
        public void Disconnect()
        {
            if (IsServer || IsClient)
            {
                Status = NetworkPeerType.Disconnected;
                --Ready;
                Network.Disconnect(250);
            }
        }

        /// <summary>
        /// Sends if a player disconnected pressing escape.
        /// </summary>
        /// <param name="teamNo">the team number of the player who disconnected by pressig the escape button</param>
        public void SendEscapeDisconnect(int teamNo)
        {
            networkView.RPC("SetOpponentEscaped", RPCMode.Others, teamNo); // have to be called as seperate function because the properties set function isn't found even if it's marked as RPC (tested it)
        }

        /// <summary>
        /// Calls the loadLevel function on all connected players including itself.
        /// Only works on serverside.
        /// </summary>
        /// <param name="level">the level to be loaded</param>
        public void NetworkLoadLevel(string level)
        {
            networkView.RPC("LoadLevel", RPCMode.All, level);
        }

        /// <summary>
        /// Sets the Team of this player for SpecialMode.
        /// </summary>
        /// <param name="teamNo">team of this player</param>
        /// <remarks>
        /// Visible via network.
        /// </remarks>
        [RPC]
        public void SetTeamNumber(int teamNo)
        {
            TeamNumber = teamNo;
        }

        /// <summary>
        /// Load the Spezialgame Level.
        /// Methode is seen over network.
        /// </summary>
        /// <param name="level">the level to be loaded</param>
        [RPC]
        public void LoadLevel(string level)
        {
            // Application.LoadLevel("GameSelection");
            Application.LoadLevel(level);
        }

        /// <summary>
        /// This method class the IncreaseReady methode on the connected server.
        /// </summary>
        public void IncReady()
        { // SUBJECT TO CHANGE
            networkView.RPC("IncreaseReady", RPCMode.Server);
        }

        /// <summary>
        /// Increases the Ready counter.
        /// </summary>
        [RPC]
        private void IncreaseReady()
        { // SUBJECT TO CHANGE
            ++Ready;
        }

        /// <summary>
        /// Sets the OpponentEscaped to true.
        /// </summary>
        /// <param name="teamNo">team of the player who has been leaved</param>
        /// <remarks>
        /// Since this is visible via network it need to have this setter because an RPC call couldn't find RPC tagged propertie setters.
        /// (Visible via network.)
        /// </remarks>
        [RPC]
        private void SetOpponentEscaped(int teamNo)
        {
            OpponentEscaped = teamNo;
        }

        /// <summary>
        /// Sets the clientnumber.
        /// </summary>
        /// <remarks>[RPC] Visible via network.</remarks>
        /// <param name="number">number to be set</param>
        [RPC]
        private void SetClientNumber(int number)
        {
            ClientNumber = number;
        }
        #endregion
    }
}