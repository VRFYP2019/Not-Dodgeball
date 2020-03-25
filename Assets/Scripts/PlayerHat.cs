using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerHat : MonoBehaviourPunCallbacks {
    private Vector3 parentPos;

    void Update() {
        UpdateHatPosition();
    }

    private void UpdateHatPosition() {
        parentPos = transform.parent.position;
        transform.position = parentPos;
    }
}
