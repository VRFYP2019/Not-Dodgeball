using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using PhotonPlayer = Photon.Realtime.Player;

public class NetworkController : MonoBehaviourPunCallbacks {
    public static NetworkController Instance;

    public Text ConnectionStatusText;
    public GameObject LobbyInfoPanel;
    public GameObject RoomInfoPanel;

    [Header("Login Panel")]
    public InputField PlayerNameInput;

    [Header("Create Room Panel")]
    public InputField RoomNameInputField;

    [Header("Room List Panel")]
    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Room Info Panel")]
    public GameObject RoomInfoContent;
    public Button StartGameButton;
    public GameObject PlayerListEntryPrefab;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    void Awake() {
        InitInstance();

        PhotonNetwork.AutomaticallySyncScene = true;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        if (PhotonNetwork.LocalPlayer.NickName.Equals("")) {
            PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
        } else {
            PlayerNameInput.text = PhotonNetwork.LocalPlayer.NickName;
        }
        ConnectionStatusText.text = "Connecting";
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
        PhotonNetwork.ConnectUsingSettings();
        LobbyInfoPanel.SetActive(false);
        RoomInfoPanel.SetActive(false);
    }

    #region PUN CALLBACKS

    public override void OnConnectedToMaster() {
        ConnectionStatusText.text = "Connected to: " + PhotonNetwork.CloudRegion + " Region";
        PhotonNetwork.AutomaticallySyncScene = true;
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
        ConnectionStatusText.text = "In Room: " + PhotonNetwork.CurrentRoom.Name;
        LobbyInfoPanel.SetActive(false);
        RoomInfoPanel.SetActive(true);

        if (playerListEntries == null) {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        // Create and add PlayerListEntryPrefabs for every player in the room to scrollview
        foreach (PhotonPlayer p in PhotonNetwork.PlayerList) {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(RoomInfoContent.transform, false);
            entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

            object isPlayerReady;
            if (p.CustomProperties.TryGetValue("PLAYER_READY_KEY", out isPlayerReady)) {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
            }

            playerListEntries.Add(p.ActorNumber, entry);
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());

        PhotonHashtable props = new PhotonHashtable {{"PLAYER_LOADED_LEVEL_KEY", false}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnPlayerEnteredRoom(PhotonPlayer newPlayer) {
        // Add new player to scrollview list
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(RoomInfoContent.transform, false);
        entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        playerListEntries.Add(newPlayer.ActorNumber, entry);

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
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

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    public override void OnPlayerPropertiesUpdate(PhotonPlayer targetPlayer, PhotonHashtable changedProps) {
        if (playerListEntries == null) {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        GameObject entry;
        // Update targetPlayer's ready status for local player
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry)) {
            object isPlayerReady;
            if (changedProps.TryGetValue("PLAYER_READY_KEY", out isPlayerReady)) {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
            }
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    void OnJoinedRoomFailed(short returnCode, string message) {
        Debug.Log("Joined Room Failed: " + message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("Create Room Failed: " + message);
    }	

    #endregion

    public void OnCreateOrJoinRoomButtonClicked() {
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
		
    public void OnLeaveGameButtonClicked() {
        PhotonHashtable props = new PhotonHashtable() {{"PLAYER_READY_KEY", false}};
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        PhotonNetwork.LeaveRoom();
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

    public void OnStartGameButtonClicked() {
        if (PhotonNetwork.IsMasterClient) {
            ToggleLobbyUI();
            PhotonNetwork.LoadLevel(1);
        }
    }

    public void ToggleLobbyUI() {
        if (LobbyInfoPanel.transform.parent.parent.gameObject.activeInHierarchy) {
            LobbyInfoPanel.transform.parent.parent.gameObject.SetActive(false);
        } else {
            LobbyInfoPanel.transform.parent.parent.gameObject.SetActive(true);
        }
    }

    public void LocalPlayerPropertiesUpdated() {
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }

    private bool CheckPlayersReady() {
        if (!PhotonNetwork.IsMasterClient) {
            return false;
        }

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
}
