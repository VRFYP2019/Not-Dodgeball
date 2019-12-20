using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRPlugin;

// Script to handle Oculus players because they think they're so special
public class OculusPlayerHandler : MonoBehaviourPunCallbacks {
    [SerializeField]
    private GameObject
        goal = null,
        oVRCameraRig = null,
        leftHand = null,
        rightHand = null;

    // Start is called before the first frame update
    void Start() {
        if (PhotonNetwork.IsConnected && !photonView.IsMine) {
            goal.transform.parent = transform;
            goal.GetComponent<Goal>().enabled = false;
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
