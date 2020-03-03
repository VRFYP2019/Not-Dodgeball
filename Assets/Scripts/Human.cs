using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Valve.VR;

public class Human : Player {
    private SteamVR_Behaviour_Pose leftHand;
    private SteamVR_Behaviour_Pose rightHand;
    public SteamVR_Action_Boolean grab = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public SteamVR_Action_Boolean trigger = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    public MonoBehaviour[] localScripts;
    public GameObject[] localObjects;

    protected override void Start() {
        playerType = PlayerType.HUMAN;
        if (PhotonNetwork.IsConnected && !photonView.IsMine) {
            foreach (MonoBehaviour m in localScripts) {
                if (m != null) {
                    m.enabled = false;
                }
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
            if (GameManager.Instance.playerPlatform == PlayerPlatform.STEAMVR) {
                SteamVR_Behaviour_Pose[] hands = GetComponentsInChildren<SteamVR_Behaviour_Pose>();
                leftHand = hands[0];
                rightHand = hands[1];
            }
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        // if any UI open, short circuit
        if (OculusUIHandler.instance.IsAnyUIOpen) {
            return;
        }

        // if game ended and trigger pressed, restart the game
        if (GameManager.Instance.isGameEnded) {
            if (GameManager.Instance.playerPlatform == PlayerPlatform.OCULUS) {
                if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)
                    || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
                    GameManager.Instance.Restart();
                }
            } else if (GameManager.Instance.playerPlatform == PlayerPlatform.EDITOR) {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) {
                    GameManager.Instance.Restart();
                }

            } else if (GameManager.Instance.playerPlatform == PlayerPlatform.STEAMVR) {
                if (trigger.GetStateDown(SteamVR_Input_Sources.Any)) {
                    GameManager.Instance.Restart();
                }
            }
        }

        if (GameManager.Instance.playerPlatform == PlayerPlatform.OCULUS) {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) {
                TrySwitchHandToSpawner(HandSide.LEFT);
            }
            if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
                TrySwitchHandToSpawner(HandSide.RIGHT);
            }
        } else if (GameManager.Instance.playerPlatform == PlayerPlatform.STEAMVR) {
            if (trigger.GetStateDown(SteamVR_Input_Sources.LeftHand)) {
                TrySwitchHandToSpawner(HandSide.LEFT);
            }
            if (trigger.GetStateDown(SteamVR_Input_Sources.RightHand)) {
                TrySwitchHandToSpawner(HandSide.RIGHT);
            }
        }
    }

    protected override void TrySwitchHandToSpawner(HandSide handSide) {
        base.TrySwitchHandToSpawner(handSide);
        // If trying to switch when no balls, vibrate
        if (BallManager.LocalInstance.playerBallQueue.childCount == 0) {
            if (handSide == HandSide.LEFT) {
                ((HandControllerHuman)leftHandController).TriggerHapticFeedback(0.1f, 100, 30);
            } else if (handSide == HandSide.RIGHT) {
                ((HandControllerHuman)rightHandController).TriggerHapticFeedback(0.1f, 100, 30);
            }
        }
    }
}
