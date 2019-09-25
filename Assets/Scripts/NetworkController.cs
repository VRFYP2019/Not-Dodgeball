using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks {
	private string ROOM_NAME = "TestRoomName";

	// Start is called before the first frame update
	void Start() {
		Debug.Log("Connecting to Photon . . .");
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster() {
		Debug.Log("Connected! To server: " + PhotonNetwork.CloudRegion);
		PhotonNetwork.AutomaticallySyncScene = true;

		PhotonNetwork.JoinLobby(TypedLobby.Default);

		//RoomOptions roomOptions = new RoomOptions() { };
		//PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, roomOptions, TypedLobby.Default);
	}

	public override void OnCreateRoomFailed(short returnCode, string message) {
		Debug.Log("Create Failed: " + message);
	}

	public override void OnJoinedLobby() {
		Debug.Log("Joined Lobby");

		//RoomOptions roomOptions = new RoomOptions() { };
		//PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, roomOptions, TypedLobby.Default);
	}

	public override void OnJoinedRoom() {
		Debug.Log("Joined Room :)");
		//PhotonNetwork.Instantiate("", Vector3.zero, Quaternion.identity, 0);
	}

	void OnJoinedRoomFailed(short returnCode, string message) {
		Debug.Log("Joined Failed: " + message);
	}
}
