using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListEntry : MonoBehaviour {

    public Text PlayerNameText;
    private int ownerId;

    public void Initialize(int playerId, string playerName) {
        ownerId = playerId;
        PlayerNameText.text = playerName;
    }
}
