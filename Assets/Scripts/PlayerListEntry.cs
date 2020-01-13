using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using PhotonPlayer = Photon.Realtime.Player;

public class PlayerListEntry : MonoBehaviour {

    public Text PlayerNameText;
    public Button PlayerReadyButton;
    public Color PlayerReadyColor;

    private int ownerId;
    private bool isPlayerReady;

    public void Initialize(int playerId, string playerName) {
        ownerId = playerId;
        PlayerNameText.text = playerName;
        PlayerNameText.color = Color.black;
        isPlayerReady = false;
    }

    public void Start() {
        // Only show player's own ready button
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId) {
            PlayerReadyButton.gameObject.SetActive(false);
        } else {
            PlayerReadyButton.onClick.AddListener(() => {
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);

                PhotonHashtable props = new PhotonHashtable() {{"PLAYER_READY_KEY", isPlayerReady}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                if (PhotonNetwork.IsMasterClient) {
                    FindObjectOfType<NetworkController>().LocalPlayerPropertiesUpdated();
                }
            });
        }
    }

    public void SetPlayerReady(bool playerReady) {
        PlayerReadyButton.GetComponentInChildren<Text>().text = playerReady ? "Not Ready" : "Ready!";
        PlayerNameText.gameObject.GetComponent<Text>().color = playerReady ? PlayerReadyColor : Color.black;
    }
}
