﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRPlugin;

// Script to handle Oculus players because they think they're so special
public class OculusPlayerHandler : MonoBehaviourPunCallbacks {
    public GameObject goal;
    public GameObject oVRCameraRig;
    public GameObject leftHand;
    public GameObject rightHand;

    // Start is called before the first frame update
    void Start() {
        if (PhotonNetwork.IsConnected && !photonView.IsMine) {
            goal.transform.parent = transform;
            Destroy(oVRCameraRig);
        }
    }

    // Update is called once per frame
    void Update() {
        if (!PhotonNetwork.IsConnected || photonView.IsMine) {
            leftHand.transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            leftHand.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
            rightHand.transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            rightHand.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        }
    }
}