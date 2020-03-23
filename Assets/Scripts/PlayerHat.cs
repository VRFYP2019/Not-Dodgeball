using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerHat : MonoBehaviourPunCallbacks {
    private Vector3 parentPos;

    void Start() {
        if (PhotonNetwork.IsConnected && !photonView.IsMine) {
            enabled = false;
        }
    }

    void Update() {
        UpdateHatPosition();
    }

    private void UpdateHatPosition() {
        parentPos = transform.parent.position;
        transform.position = parentPos;
    }
}
