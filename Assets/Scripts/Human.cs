using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Human : Player {
    public SteamVR_Action_Boolean grab = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public SteamVR_Action_Boolean trigger = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    private SteamVR_Behaviour_Pose leftHand;
    private SteamVR_Behaviour_Pose rightHand;
    public MonoBehaviour[] localScripts;
    public GameObject[] localObjects;
    private PhotonView photonView;

    protected override void Start() {
        playerType = Utils.PlayerType.HUMAN;
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsConnected && !photonView.IsMine) {
            foreach (MonoBehaviour m in localScripts) {
                m.enabled = false;
            }
            foreach (GameObject g in localObjects) {
                g.SetActive(false);
            }
            foreach (Camera cam in GetComponentsInChildren<Camera>()) {
                cam.enabled = false;
            }
            foreach (AudioListener al in GetComponentsInChildren<AudioListener>()) {
                GetComponentInChildren<AudioListener>().enabled = false;
            }
            this.enabled = false;
        } else {
            base.Start();
            if (!GameManager.Instance.isOculusQuest && !Application.isEditor) {
                SteamVR_Behaviour_Pose[] hands = GetComponentsInChildren<SteamVR_Behaviour_Pose>();
                leftHand = hands[0];
                rightHand = hands[1];
            }
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        // if game ended and trigger pressed, restart the game
        if (GameManager.Instance.isGameEnded) {
            if (GameManager.Instance.isOculusQuest) {
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
                    GameManager.Instance.Restart();
                }
            } else if (Application.isEditor) {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                    GameManager.Instance.Restart();
                }

            } else if (trigger.GetStateDown(SteamVR_Input_Sources.Any)) {
                GameManager.Instance.Restart();
            }
        }

        if (GameManager.Instance.isOculusQuest) {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger)) {
                leftHandController.Switch();
            }

            if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)) {
                rightHandController.Switch();
            }
        } else if (!Application.isEditor) {
            // if grab is pressed, switch between tool and spawn for that hand
            if (grab.GetStateDown(rightHand.inputSource)) {
                rightHandController.Switch();
            }
            if (grab.GetStateDown(leftHand.inputSource)) {
                leftHandController.Switch();
            }
        }
    }
}
