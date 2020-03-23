using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRPlugin;

// Script to handle Oculus players because they think they're so special
public class OculusPlayerHandler : MonoBehaviourPunCallbacks {
    [SerializeField]
    private GameObject[] goals = null;
    [SerializeField]
    private GameObject hatAnchor = null;
    [SerializeField]
    private GameObject
        oVRCameraRig = null,
        leftHand = null,
        rightHand = null;

    void Awake() {
        if (!PhotonNetwork.IsConnected || photonView.IsMine) {
            oVRCameraRig.SetActive(true);
            oVRCameraRig.GetComponent<OVRManager>().enabled = true;
            oVRCameraRig.GetComponent<OVRCameraRig>().enabled = true;
        }
    }

    private void Start() {
        // Force invoke since it would not be enabled for Oculus and Start() would not be called
        GetComponentInChildren<GoalSwitcher>(true).SwitchGoals();
        if (PhotonNetwork.IsConnected && !photonView.IsMine) {
            ReparentGoals();
            ReparentHatAnchor();
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

    void ReparentGoals() {
        foreach (GameObject goal in goals) {
            goal.transform.parent = transform;
        }
    }

    void ReparentHatAnchor() {
        hatAnchor.transform.parent = transform;
    }
}
