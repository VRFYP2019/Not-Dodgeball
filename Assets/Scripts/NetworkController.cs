using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using PhotonPlayer = Photon.Realtime.Player;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviourPunCallbacks, IOnEventCallback {
    public static NetworkController Instance;

    private Text ConnectionStatusText;
    private InputField PlayerNameInput;
    private InputField RoomNameInputField;

    private GameObject
        LobbyInfoPanel,
        RoomInfoPanel,
        RoomListContent,
        RoomListEntryPrefab,
        RoomInfoContent,
        PlayerListEntryPrefab;

    private Button StartGameButton;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    private bool isReturnToRoom = false;

    void Awake() {
        InitInstance();
        PhotonNetwork.AutomaticallySyncScene = true;
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();
    }

    void InitInstance() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad (gameObject);
        } else if (Instance != this) {
            Destroy (gameObject);
        }
    }
		
    void Start() {
        AssignRefs();
        ConnectionStatusText.text = "Connecting";

        PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
        PhotonNetwork.ConnectUsingSettings();
        LobbyInfoPanel.SetActive(false);
        RoomInfoPanel.SetActive(false);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        AssignRefs();
        UnreadyPlayers();
        if (scene.buildIndex == 0) {
            string LocalNickname = PhotonNetwork.LocalPlayer.NickName;
            PlayerNameInput.text = (LocalNickname.Equals(string.Empty)) ? "Player " + Random.Range(1000, 10000) : LocalNickname;
        }
        if (isReturnToRoom || scene.buildIndex == 1) {
            isReturnToRoom = false;
            OnLeftRoom(); // To clear player entries
            OnJoinedRoom();
        }
    }

    private void AssignRefs() {
        ConnectionStatusText = NetworkUIRefs.Instance.ConnectionStatusText;
        LobbyInfoPanel = NetworkUIRefs.Instance.LobbyInfoPanel;
        RoomInfoPanel = NetworkUIRefs.Instance.RoomInfoPanel;

        PlayerNameInput = NetworkUIRefs.Instance.PlayerNameInput;

        RoomNameInputField = NetworkUIRefs.Instance.RoomNameInputField;
        RoomListContent = NetworkUIRefs.Instance.RoomListContent;
        RoomListEntryPrefab = NetworkUIRefs.Instance.RoomListEntryPrefab;

        RoomInfoContent = NetworkUIRefs.Instance.RoomInfoContent;
        StartGameButton = NetworkUIRefs.Instance.StartGameButton;
        PlayerListEntryPrefab = NetworkUIRefs.Instance.PlayerListEntryPrefab;
    }

    public void OnEvent(EventData photonEvent) {
        byte LeaveGameEvent = 2;
        byte eventCode = photonEvent.Code;

        if (eventCode == LeaveGameEvent) {
            Debug.Log("Event recieved: Leave Game Event");
            object[] data = (object[])photonEvent.CustomData;
            bool isReturnToLobby = (bool)data[0];
            bool isHostDecision = (bool)data[1];

            if (!isReturnToLobby) {     // Either picks room, Both to room
                isReturnToRoom = true;
            } else if (isHostDecision) {// Host picks lobby, Both to lobby
                NetworkController.Instance.PlayerLeaveRoom();
            } else {                    // Client picks lobby, Host to room
                if (!PhotonNetwork.IsMasterClient) {
                    NetworkController.Instance.PlayerLeaveRoom();
                } else if (PhotonNetwork.IsMasterClient) {
                    isReturnToRoom = true;
                }
            }

            if (PhotonNetwork.IsMasterClient) {
                PhotonNetwork.DestroyAll();
                PhotonNetwork.LoadLevel(0); // ensure sync is true
            }
        }
    }

    #region PUN CALLBACKS

    public override void OnConnectedToMaster() {
        ConnectionStatusText.text = "Connected to: " + PhotonNetwork.CloudRegion + " Region";
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnJoinedLobby() {
        ConnectionStatusText.text = "In Lobby: " + PhotonNetwork.CloudRegion + " Region";
        LobbyInfoPanel.SetActive(true);
        RoomInfoPanel.SetActive(false);
    }

    public override void OnLeftLobby() {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnJoinedRoom() {
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            ConnectionStatusText.text = "In Room: " + PhotonNetwork.CurrentRoom.Name;
            LobbyInfoPanel.SetActive(false);
            RoomInfoPanel.SetActive(true);
        }

        if (playerListEntries == null) {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        // Create and add PlayerListEntryPrefabs for every player in the room to scrollview
        foreach (PhotonPlayer p in PhotonNetwork.PlayerList) {
            GameObject entry = CreateEntry(p);

            if (p.CustomProperties.TryGetValue("PLAYER_READY_KEY", out  object isPlayerReady)) {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }
        if (PhotonNetwork.IsMasterClient) {
            StartGameButton.gameObject.SetActive(true);
            StartGameButton.interactable = CheckPlayersReady();
        } else {
            StartGameButton.gameObject.SetActive(false);
        }

        PhotonHashtable props = new PhotonHashtable {{"PLAYER_LOADED_LEVEL_KEY", false}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer) {
        // Add new player to scrollview list
        GameObject entry = CreateEntry(newPlayer);
        playerListEntries.Add(newPlayer.ActorNumber, entry);

        if (PhotonNetwork.IsMasterClient) {
            StartGameButton.interactable = CheckPlayersReady();
        }
    }

    public override void OnLeftRoom() {
        foreach (GameObject entry in playerListEntries.Values) {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerLeftRoom(PhotonPlayer otherPlayer) {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.IsMasterClient) {
            StartGameButton.gameObject.SetActive(true);
            StartGameButton.interactable = CheckPlayersReady();
        }
    }

    public override void OnPlayerPropertiesUpdate(PhotonPlayer targetPlayer, PhotonHashtable changedProps) {
        if (playerListEntries == null) {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out GameObject entry)) {
            if (changedProps.TryGetValue("PLAYER_READY_KEY", out object isPlayerReady)) {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }
        }
        if (PhotonNetwork.IsMasterClient) {
            StartGameButton.interactable = CheckPlayersReady();
        }
    }

    void OnJoinedRoomFailed(short returnCode, string message) {
        Debug.Log("Joined Room Failed: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Create Room Failed: " + message);
    }	

    #endregion

    public void PlayerJoinOrCreateRoom() {
        SetLocalPlayerName();

        string roomName = RoomNameInputField.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

        RoomOptions roomOptions = new RoomOptions {};
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public void SetLocalPlayerName() {
        string playerName = PlayerNameInput.text;
        playerName = (playerName.Equals(string.Empty)) ? "Player " + Random.Range(1000, 10000) : playerName;
        PlayerNameInput.text = playerName;
        PhotonNetwork.LocalPlayer.NickName = playerName;
    }
		
    public void PlayerLeaveRoom() {
        PhotonHashtable props = new PhotonHashtable() {{"PLAYER_READY_KEY", false}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        PhotonNetwork.LeaveRoom();
    }

    private GameObject CreateEntry(PhotonPlayer p) {
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(RoomInfoContent.transform, false);
        entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

        return entry;
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList) {
        foreach (RoomInfo info in roomList) {
        	// Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList) {
                if (cachedRoomList.ContainsKey(info.Name)) {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name)) {
                cachedRoomList[info.Name] = info;
            } else { // Add new room info to cache
                cachedRoomList.Add(info.Name, info);
            }
        }
    }

    private void ClearRoomListView() {
        foreach (GameObject entry in roomListEntries.Values) {
            Destroy(entry.gameObject);
        }

        roomListEntries.Clear();
    }

    private void UpdateRoomListView() {
        foreach (RoomInfo info in cachedRoomList.Values) {
            GameObject entry = Instantiate(RoomListEntryPrefab);
            entry.transform.SetParent(RoomListContent.transform, false);
            entry.GetComponent<RoomListEntry>().Initialize(info.Name);

            roomListEntries.Add(info.Name, entry);
        }
    }

    public void PlayerStartGame() {
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void PlayerReturnToRoom() {
        isReturnToRoom = true;
    }

    public void LocalPlayerPropertiesUpdated() {
        if (PhotonNetwork.IsMasterClient) {
            StartGameButton.interactable = CheckPlayersReady();
        }
    }

    private bool CheckPlayersReady() {
        foreach (PhotonPlayer p in PhotonNetwork.PlayerList) {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue("PLAYER_READY_KEY", out isPlayerReady)) {
                if (!(bool) isPlayerReady) {
                    return false;
                }
            } else {
                return false;
            }
        }

        return true;
    }

    // Set all players to not ready
    public void UnreadyPlayers() {
        foreach (PhotonPlayer p in PhotonNetwork.PlayerList) {
            PhotonHashtable props = new PhotonHashtable() { { "PLAYER_READY_KEY", false } };
            p.SetCustomProperties(props);
        }
    }
}
