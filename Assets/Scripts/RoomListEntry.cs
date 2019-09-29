using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class RoomListEntry : MonoBehaviour{
    public Text RoomNameText;
    public Button JoinRoomButton;

    private string roomName;

    public void Start() {
        JoinRoomButton.onClick.AddListener(() => {
            if (PhotonNetwork.InLobby) {
                PhotonNetwork.LeaveLobby();
            }

            NetworkController.Instance.SetLocalPlayerName();
            PhotonNetwork.JoinRoom(roomName);
        });
    }

    public void Initialize(string name) {
        roomName = name;
        RoomNameText.text = name;
    }
}