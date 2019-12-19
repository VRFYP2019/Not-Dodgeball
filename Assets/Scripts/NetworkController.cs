using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks {
    public static NetworkController Instance;

    public Text ConnectionStatusText;
    public GameObject LobbyInfoPanel;
    public GameObject RoomInfoPanel;

    [Header("Login Panel")]
    public InputField PlayerNameInput;

    [Header("Create Room Panel")]
    public InputField RoomNameInputField;
    public Button JoinOrCreateRoomButton;

    [Header("Room List Panel")]
    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Room Info Panel")]
    public GameObject RoomInfoContent;
    public Button StartGameButton;
    public Button LeaveRoomButton;
    public GameObject PlayerListEntryPrefab;

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, GameObject> playerListEntries;

    void Awake() {
        InitInstance();

        PhotonNetwork.AutomaticallySyncScene = true;

        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
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
        ConnectionStatusText.text = "In Lobby";
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
        ConnectionStatusText.text = "In Room";
        LobbyInfoPanel.SetActive(false);
        RoomInfoPanel.SetActive(true);

        if (playerListEntries == null) {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        // Create and add PlayerListEntryPrefabs for every player in the room to scrollview
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList) {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(RoomInfoContent.transform, false);
            entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);
            playerListEntries.Add(p.ActorNumber, entry);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        // Add new player to scrollview list
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(RoomInfoContent.transform, false);
        entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
        playerListEntries.Add(newPlayer.ActorNumber, entry);
    }

    public override void OnLeftRoom() {
        foreach (GameObject entry in playerListEntries.Values) {
            Destroy(entry.gameObject);
        }

        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        //StartGameButton.gameObject.SetActive(CheckPlayersReady());
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
        //RoomOptions options = new RoomOptions {MaxPlayers = 2};

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
            PhotonNetwork.LoadLevel(1);
        }
    }
}
