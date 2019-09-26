using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkController : MonoBehaviourPunCallbacks {
	public Text ConnectionStatusText;

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

	//private Dictionary<string, RoomInfo> cachedRoomList;
	//private Dictionary<string, GameObject> roomListEntries;
	//private Dictionary<int, GameObject> playerListEntries;

	void Awake() {
		PhotonNetwork.AutomaticallySyncScene = true;

		//cachedRoomList = new Dictionary<string, RoomInfo>();
		//roomListEntries = new Dictionary<string, GameObject>();

		PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
		ConnectionStatusText.text = "Connecting";
	}
		
	void Start() {
		Debug.Log("Connecting to Photon . . .");
		PhotonNetwork.ConnectUsingSettings();
	}

	// --------------------------START OF PHOTON CALLBACKS---------------------------- //

	public override void OnConnectedToMaster() {
		Debug.Log("Connected! To server: " + PhotonNetwork.CloudRegion);
		ConnectionStatusText.text = "Connected to: " + PhotonNetwork.CloudRegion + " Region";

		PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.JoinLobby(TypedLobby.Default);
	}

	public override void OnCreateRoomFailed(short returnCode, string message) {
		Debug.Log("Create Room Failed: " + message);
	}

	public override void OnJoinedLobby() {
		Debug.Log("Joined Lobby");
		ConnectionStatusText.text = "In Lobby";
	}

	public override void OnJoinedRoom() {
		Debug.Log("Joined Room");
		ConnectionStatusText.text = "In Room";
		Debug.Log(PhotonNetwork.CountOfPlayersInRooms);

		foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList) {
			GameObject entry = Instantiate(PlayerListEntryPrefab);
			entry.transform.SetParent(RoomInfoContent.transform);
			entry.transform.localScale = Vector3.one;
			//entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

			//playerListEntries.Add(p.ActorNumber, entry);
		}
	}

	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
	    GameObject entry = Instantiate(PlayerListEntryPrefab);
	    entry.transform.SetParent(RoomInfoContent.transform);
	    entry.transform.localScale = Vector3.one;


	  	//entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);
		//playerListEntries.Add(newPlayer.ActorNumber, entry);
		//StartGameButton.gameObject.SetActive(CheckPlayersReady());
	}

	void OnJoinedRoomFailed(short returnCode, string message) {
		Debug.Log("Joined Room Failed: " + message);
	}

	// --------------------------END OF PHOTON CALLBACKS---------------------------- //

	public void OnCreateOrJoinRoomButtonClicked(){
	    string roomName = RoomNameInputField.text;
	    roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;
	
//	    byte maxPlayers;
//	    byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
//	    maxPlayers = (byte) Mathf.Clamp(maxPlayers, 2, 8);
//		RoomOptions options = new RoomOptions {MaxPlayers = maxPlayers};

		RoomOptions roomOptions = new RoomOptions {};
		PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
	}
		
	public void OnLeaveGameButtonClicked() {
    	PhotonNetwork.LeaveRoom();
	}

	//public override void OnRoomListUpdate(List<RoomInfo> roomList) {
	//	ClearRoomListView();
	//	UpdateCachedRoomList(roomList);
	//	UpdateRoomListView();
	//}

	//public override void OnLeftLobby() {
	//	cachedRoomList.Clear();
	//	ClearRoomListView();
	//}
}
