using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class Client : TcpEngine
{
    // Connection
    public string host = "192.168.1.158";
    public int port = 8001;

    private Transform player;
    internal Transform playerCamera;

    internal GameObject statsCanvas;
    private Vector3 playerPos;
    private Vector3 playerRot;
    private Vector3 playerCamRot;

    public GameObject localPlayerClone;
    public NetworkPlayer playerClone;
    public Transform fakeLocalPlayer;

    const int SERIAL_X = 12345;
    const int SERIAL_Y = 67890;
    private static bool isHandshaking = true;

    public int id;
    public string localName = "No name";
    public int localWeaponId;
    public bool isAlive = false;

    // stats
    public float kills = 0;
    public float deaths = 0;
    public bool hasTheLead = false;

    public readonly static Queue<Action> RunOnMainThread = new Queue<Action>();
    private Thread thread;
    public Hashtable playerDatas = new Hashtable();
    GlobalResources globalResources;

    internal LobbyLog log;
    Transform[] spawnPoints;
	
    // init
    void Start()
    {
        // set name
        if (PlayerPrefs.HasKey("username")) localName = PlayerPrefs.GetString("username");

        statsCanvas = GameObject.Find("Lobby Stats UI");
        globalResources = GameObject.Find("/Global Resources").GetComponent<GlobalResources>();
        log = GameObject.Find("/MenuSystem/Game Log UI/1_log_container").GetComponent<LobbyLog>();
        spawnPoints = GameObject.Find("/Environment/spawns").GetComponentsInChildren<Transform>();
        AssignLocalPlayer(null);

        ConnectNotBlocking(host, port);
    }

    // play sound when player gets the lead or loses it
    private void CheckTheLead()
    {
        float maxValue = 0;

        foreach (object o in playerDatas.Values)
        {
            PlayerData p = (PlayerData)o;
            if (maxValue < p.kills) maxValue = p.kills;
        }

        bool lead = kills>maxValue;

        if (hasTheLead != lead && GameObject.Find("/Player"))
        {
            GameObject.Find("/Player").SendMessage("HasTheLead", lead);
            hasTheLead = lead;
        }
            
    }

    // Hide / Show stats
    void ToggleStats(bool show)
    {
         statsCanvas.transform.GetChild(0).gameObject.SetActive(show);     
    }

    // Hide / show chat
    void ToggleChat()
    {
        if (!log.chatActive)
        {
            log.ToggleChat(true);

            if (GameObject.Find("/Player"))
            {
                GameObject p = GameObject.Find("/Player");
                p.GetComponent<PlayerInput>().enabled = false; // Stop inputs
                p.GetComponent<PlayerMotor>().Move(0f, 0f); // Stop player
                p.GetComponent<PlayerMotor>().MouseLook(0f, 0f);
            }
        }
        else
        {
            log.ToggleChat(false);
            if (GameObject.Find("/Player")) {
                if(!GameObject.Find("/Player").GetComponent<PlayerManager>().gamePaused)
                GameObject.Find("/Player").GetComponent<PlayerInput>().enabled = true;
                //Cursor.visible = false;
                //Cursor.lockState = CursorLockMode.Locked;
            }
            
        }
    }

    // Handle player transform when dead/alive
    public void AssignLocalPlayer(Transform newLocalPlayer)
    {
        if (newLocalPlayer != null) player = newLocalPlayer;
        else player = fakeLocalPlayer;
        
        playerCamera = player.GetChild(0);
    }

    // Send chat message to everyone
    void SendChatMessage(string message)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.Put((byte)2);
        buffer.Put(4); // put int
        buffer.Put(message);
        Send(buffer.Trim().Get());
    }

    // Send information when local player dies
    internal void LocalPlayerDied(int killerID, int criticalCode)
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.Put((byte)2);
        buffer.Put(3); // put int
        buffer.Put(killerID);
        buffer.Put(criticalCode);
        Send(buffer.Trim().Get());
    }

    // Send damage caused to other players
    internal void DamageDealedTo(int receiverID, float amount, int criticalCode, Vector3 sourcePos)
    {
        //send to network
        ByteBuffer buffer = new ByteBuffer();
        buffer.Put((byte)2);
        buffer.Put(2); // put int
        buffer.Put(receiverID);
        buffer.Put(amount);
        buffer.Put(criticalCode);
        buffer.Put(sourcePos.x);
        buffer.Put(sourcePos.y);
        buffer.Put(sourcePos.z);
        Send(buffer.Trim().Get());
    }

    // send weapon swap to network
    void LocalPlayerChangedToWeapon(int weaponId)
    {   
        ByteBuffer buffer = new ByteBuffer();
        buffer.Put((byte)2);
        buffer.Put(0); // put int
        buffer.Put(weaponId);
        Send(buffer.Trim().Get());

        localWeaponId = weaponId;
    }

    void LocalPlayerCrouch(bool value)
    {
        //TODO
    }

    void DetonateFinalWord()
    {
        //detonate id is weapons lenght + 1
        int wid = globalResources.weapons.Count+1;
        LocalPlayerChangedToWeapon(wid);
    }

    // Send primary fire to network
    void LocalPlayerFiredWeapon()
    {      
        ByteBuffer buffer = new ByteBuffer();
        buffer.Put((byte)2);
        buffer.Put(1); // put int
        Send(buffer.Trim().Get());
    }

    // Spawn client to the game
    public void LocalPlayerSpawn()
    {
        ByteBuffer buffer = new ByteBuffer();
        buffer.Put((byte)2);
        buffer.Put(2); // put int
        Send(buffer.Trim().Get());

        // player obj
        if (GameObject.Find("/Dead Player")) Destroy(GameObject.Find("/Dead Player"));
        int randomSpawn = UnityEngine.Random.Range(1,spawnPoints.Length);//dont include [0]
        GameObject pClone = Instantiate(localPlayerClone,spawnPoints[randomSpawn].position,spawnPoints[randomSpawn].rotation);
        pClone.name = "Player";
        isAlive = true;

        SendAppereances();
    }

    // Send equipped appereances to network
    void SendAppereances()
    {
        List<GameObject> a = globalResources.appereances;

        int holo = a.IndexOf(Resources.Load(PlayerPrefs.GetString("equipped_holo"), typeof(GameObject)) as GameObject);
        int head = a.IndexOf(Resources.Load(PlayerPrefs.GetString("equipped_head"), typeof(GameObject)) as GameObject);
        int face = a.IndexOf(Resources.Load(PlayerPrefs.GetString("equipped_face"), typeof(GameObject)) as GameObject);
        int gloves = a.IndexOf(Resources.Load(PlayerPrefs.GetString("equipped_gloves"), typeof(GameObject)) as GameObject);
        int upperBody = a.IndexOf(Resources.Load(PlayerPrefs.GetString("equipped_upperbody"), typeof(GameObject)) as GameObject);
        int lowerBody = a.IndexOf(Resources.Load(PlayerPrefs.GetString("equipped_lowerbody"), typeof(GameObject)) as GameObject);
        int boots = a.IndexOf(Resources.Load(PlayerPrefs.GetString("equipped_boots"), typeof(GameObject)) as GameObject);

        ByteBuffer buffer = new ByteBuffer();
        buffer.Put((byte)2);
        buffer.Put(5); // put int
        buffer.Put(holo);
        buffer.Put(head);
        buffer.Put(face);
        buffer.Put(gloves);
        buffer.Put(upperBody);
        buffer.Put(lowerBody);
        buffer.Put(boots);
        Send(buffer.Trim().Get());
    }

    public override void ConnectionResolve(bool success)
    {
        if (success == false)
        {
            Debug.LogWarning("Connection failed: " + host);
        }
        else
        {
            print("Connected: " + host);
            // Start server thread
            thread = new Thread(new ThreadStart(Refresh));
            thread.IsBackground = true;
            thread.Start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHandshaking)
        {
            playerPos = player.position;
            playerRot = player.rotation.eulerAngles;
            playerCamRot = playerCamera.localRotation.eulerAngles;

            ByteBuffer buffer = new ByteBuffer();
            buffer.Put((byte)1);

            buffer.Put(playerPos.x);
            buffer.Put(playerPos.y);
            buffer.Put(playerPos.z);

            buffer.Put(playerRot.x);
            buffer.Put(playerRot.y);
            buffer.Put(playerRot.z);

            buffer.Put(playerCamRot.x);
            buffer.Put(playerCamRot.y);
            buffer.Put(playerCamRot.z);

            Send(buffer.Trim().Get());

        }

        while (RunOnMainThread.Count > 0)
        {
            RunOnMainThread.Dequeue().Invoke();
        }

        // Get TAB key
        // This Input is hardcoded. We should make input axis for this later
        ToggleStats(Input.GetKey(KeyCode.Tab));

        // Toggle chat
        if (Input.GetKeyDown("return")) { if (log.chatActive) log.ChatEditEnd(); ToggleChat(); }
        if (Input.GetKeyDown(KeyCode.Escape)) { if (log.chatActive) ToggleChat();}
    }

    public override void Packet(byte[] data)
    {
        ByteBuffer buffer = new ByteBuffer(data);

        if (isHandshaking)
        {
            // Check if handshake is valid
            if (data.Length >= 16)
            {
                int x = buffer.GetInt();
                int y = buffer.GetInt();

                if (x == SERIAL_X && y == SERIAL_Y)
                {
                    id = buffer.GetInt();

                    // Send response back
                    ByteBuffer sendBuffer = new ByteBuffer();
                    sendBuffer.Put((byte)0);

                    sendBuffer.Put(SERIAL_Y);
                    sendBuffer.Put(SERIAL_X);

                    sendBuffer.Put(localName); //NAME STRING

                    RunOnMainThread.Enqueue(() => {
                        SendAppereances(); //APPEREANCES
                    });

                    ByteBuffer z = sendBuffer.Trim();

                    Send(z.Get());

                    int playerCount = buffer.GetInt();

                    for (int i = 0; i < playerCount; i++)
                    {
                        int playerId = buffer.GetInt();
                        PlayerJoined(playerId, buffer.GetString());
                        ReceiveAppereances(playerId, buffer.GetInt(), buffer.GetInt(), buffer.GetInt(), buffer.GetInt(), buffer.GetInt(), buffer.GetInt(), buffer.GetInt());
                        InitPlayerStats(playerId, buffer.GetInt(), buffer.GetInt());
                    }

                    isHandshaking = false;
                }
            }

        }
        else
        {
            while (buffer.GetPointer() != data.Length)
            {
                byte protocol = buffer.GetByte();

                if (protocol == 1)
                {
                    UpdatePlayerPositions(buffer);
                }

                if (protocol == 2)
                {
                    byte argument = buffer.GetByte();                

                    if (argument == 0)
                    {
                        PlayerJoined(buffer.GetInt(), buffer.GetString());
                    }
                    if (argument == 1)
                    {
                        PlayerLeft(buffer.GetInt());
                    }
                    if (argument == 2)
                    {
                        PlayerChangeWeapon(buffer.GetInt(), buffer.GetInt());
                    }
                    if (argument == 3)
                    {
                        PlayerFireWeapon(buffer.GetInt());
                    }
                    if (argument == 4)
                    {
                        DamageReceived(buffer.GetInt(),buffer.GetFloat(),buffer.GetInt(),
                            new Vector3(buffer.GetFloat(), buffer.GetFloat(), buffer.GetFloat()));
                    }
                    if (argument == 5)
                    {
                        PlayerDied(buffer.GetInt(), buffer.GetInt(), buffer.GetInt());
                    }
                    if (argument == 6)
                    {
                        ReceiveChatMessage(buffer.GetInt(),buffer.GetString());
                    }
                    if (argument == 7)
                    {
                        ReceiveAppereances(buffer.GetInt(), buffer.GetInt(), buffer.GetInt(), buffer.GetInt(), buffer.GetInt(), buffer.GetInt(), buffer.GetInt(), buffer.GetInt());
                    }

                }
            }

        }
    }

    internal void UpdatePlayerPositions(ByteBuffer buffer)
    {
        int playerCount = buffer.GetInt();

        for (int i = 0; i < playerCount; i++)
        {
            int pid = buffer.GetInt();

            if (pid != id)
            {
                PlayerData pData = (PlayerData)playerDatas[pid];
                if (pData != null) {
                    pData.position = new Vector3(buffer.GetFloat(), buffer.GetFloat(), buffer.GetFloat());
                    pData.rotation = new Vector3(buffer.GetFloat(), buffer.GetFloat(), buffer.GetFloat());
                    pData.subRotation = new Vector3(buffer.GetFloat(), buffer.GetFloat(), buffer.GetFloat());
                }
                else
                {
                    buffer.GetFloat(); // x
                    buffer.GetFloat(); // y
                    buffer.GetFloat(); // z

                    buffer.GetFloat(); // rx
                    buffer.GetFloat(); // ry
                    buffer.GetFloat(); // rz

                    buffer.GetFloat(); // rcx
                    buffer.GetFloat(); // rcy
                    buffer.GetFloat(); // rcz
                }
            }
            else
            {
                // self
                buffer.GetFloat(); // x
                buffer.GetFloat(); // y
                buffer.GetFloat(); // z

                buffer.GetFloat(); // rx
                buffer.GetFloat(); // ry
                buffer.GetFloat(); // rz

                buffer.GetFloat(); // rcx
                buffer.GetFloat(); // rcy
                buffer.GetFloat(); // rcz
            }
        }
    }

    // Network player joined
    internal void PlayerJoined(int playerID, string playerName)
    {
        playerDatas.Add(playerID, new PlayerData(playerName));

        RunOnMainThread.Enqueue(() =>{
            NetworkPlayer clone = Instantiate(playerClone);
            clone.networkId = playerID;
            clone.networkName = playerName;
        });
    }

    // Network player left
    internal void PlayerLeft(int playerID)
    {
        //Debug.Log("leaved player: " + playerID);
        ((PlayerData)playerDatas[playerID]).destroy = true;
        playerDatas.Remove(playerID);
    }

    // Network player changes weapon
    internal void PlayerChangeWeapon(int playerID, int weaponID)
    {
        //final w detonate
        if (weaponID == globalResources.weapons.Count + 1 && playerID != id) PlayerDetonatedFinalWord(playerID);
        else
        {
            ((PlayerData)playerDatas[playerID]).weapon = weaponID;
            ((PlayerData)playerDatas[playerID]).weaponChanged = true;
        }
        // lead
        RunOnMainThread.Enqueue(() => {          
            CheckTheLead();
        });
    }

    // Network player fires weapon
    internal void PlayerFireWeapon(int playerID)
    {
        ((PlayerData)playerDatas[playerID]).pendingPrimaryFire = true;
    }

    // Receive chat messages
    void ReceiveChatMessage(int senderID, string message)
    {
        string senderName = host;
        string color = "silver";

        if (senderID <= 0) color = "red";
        else senderName = (senderID == id) ? localName : GetLatestDatas(senderID).name;

        RunOnMainThread.Enqueue(() => {
            log.AddChatMessage("<color=" + color + ">" + senderName + ": " + "</color>" + message, (senderID == id));
        });
        
    }

    void PlayerCrouch(int playerId, byte value)
    {
        if (playerId == id) return; // self
        ((PlayerData)playerDatas[playerId]).crouch = Convert.ToBoolean(value);
    }

    void PlayerDetonatedFinalWord(int playerId)
    {
        if (playerId == id) return; // self
        ((PlayerData)playerDatas[playerId]).pendingFinalWord = true;
    }

    // Network player dies
    internal void PlayerDied(int killedID, int killerID, int criticalCode)
    {
        // kill network players
        if (killedID != id) ((PlayerData)playerDatas[killedID]).die = true;

        RunOnMainThread.Enqueue(() => {
            // Get critical type
            string criticalType = "killed";
            if (criticalCode == 1) criticalType = "headshot";
            if (criticalCode == 2) criticalType = "nutshot";
            if (criticalCode == 3) criticalType = "smackdown";

            // Log Message
            if (killerID < 0) // suicides
            {
                // log
                log.LogMessage((killedID == id) ? localName : GetLatestDatas(killedID).name, " suicided","", false, false);

                // stats
                if (killedID == id) { deaths++; kills--; }
                else { ((PlayerData)playerDatas[killedID]).deaths++; ((PlayerData)playerDatas[killedID]).kills--; }
            }
            else // kills
            {
                //data
                if (killedID != id) ((PlayerData)playerDatas[killedID]).lastKiller=killerID;

                // log
                log.LogMessage((killerID == id) ? localName : GetLatestDatas(killerID).name, " " + criticalType + " ",(killedID == id) ? localName : GetLatestDatas(killedID).name, false, false);

                // stats kills
                if (killerID == id) kills++;
                else ((PlayerData)playerDatas[killerID]).kills++;

                //stats deaths
                if (killedID == id) deaths++;
                else ((PlayerData)playerDatas[killedID]).deaths++;
            }
            
            // Add my kills
            if (killerID == id && isAlive) GameObject.Find("/Player").GetComponent<PlayerManager>().GotKill(killedID, criticalCode);

        });  
    }

    // Local player receives damage
    internal void DamageReceived(int dealerID, float amount, int criticalCode, Vector3 sourcePos)
    {
        if (isAlive)
        {
            //Debug.Log("client "+dealerID+" dealed you "+amount+" damage");
            RunOnMainThread.Enqueue(() => {
                GameObject.Find("/Player").GetComponent<PlayerManager>().TakeDamage(amount, sourcePos, criticalCode, dealerID);
            });
        }       
    }

    // Receive network player appereances and put them to playerDatas
    internal void ReceiveAppereances(int playerID, int holo, int head, int face, int gloves, int upperbody, int lowerbody, int  boots)
    {
        if (playerID == id) return; // skip self

        //print(playerID+", "+ holo + ", " + head + ", " + face + ", " + gloves + ", " + upperbody + ", " + lowerbody + ", " + boots);

        // save to playerdata
        PlayerData pd  = ((PlayerData) playerDatas[playerID]);
        pd.holo = holo;
        pd.head = head;
        pd.face = face;
        pd.gloves = gloves;
        pd.upperbody = upperbody;
        pd.lowerbody = lowerbody;
        pd.boots = boots;
        pd.appereancesChanged = true;

    }

    // init player stats if join during the game
    void InitPlayerStats(int pID, int kills, int deaths)
    {
        ((PlayerData)playerDatas[pID]).kills = kills;
        ((PlayerData)playerDatas[pID]).deaths = deaths;
    }

    // Close connection and disable this client gameObject
    internal void Quit()
    {
        Disconnect();
        gameObject.SetActive(false);
    }

    // This is called every frame by every NetworkPlayer
    public PlayerData GetLatestDatas(int networkId)
    {
        foreach (object i in playerDatas.Keys)
        {
            if (((int)i) == networkId)
            {
                return (PlayerData)playerDatas[i];
            }
        }

        return null;
    }
}