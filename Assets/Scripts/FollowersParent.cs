using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To keep all follwer objects in one place to reduce clutter in editor runtime
public class FollowersParent : MonoBehaviour {
    public static FollowersParent LocalInstance;
    public static FollowersParent RemoteInstance;
    public Transform tools;
    public Transform balls;
    private PhotonView photonView;

    private void Awake() {
        photonView = GetComponent<PhotonView>();
        if (!PhotonNetwork.IsConnected || photonView.IsMine) {
            LocalInstance = this;
        } else {
            RemoteInstance = this;
        }
    }
}
