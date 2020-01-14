using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Valve.VR;

// To be attached on Spawner objects for human players, to control the use of them
public class SpawnerHumanController : MonoBehaviour {
    private SteamVR_Action_Boolean click = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    private Spawner spawner;
    private SteamVR_Behaviour_Pose handPose;
    private HandSide handSide;

    // Start is called before the first frame update
    void Start() {
        if (GameManager.Instance.playerPlatform == PlayerPlatform.STEAMVR) {
            handPose = GetComponentInParent<SteamVR_Behaviour_Pose>();
        }
        spawner = GetComponent<Spawner>();
        handSide = GetComponentInParent<HandControllerHuman>().handSide;
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.playerPlatform == PlayerPlatform.OCULUS) {
            if (handSide == HandSide.LEFT && OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger)) {
                spawner.ThrowCurrentBall();
            }
            if (handSide == HandSide.RIGHT && OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger)) {
                spawner.ThrowCurrentBall();
            }
        } else if (GameManager.Instance.playerPlatform == PlayerPlatform.EDITOR) {
            if (Input.GetKeyUp(KeyCode.Space)) {
                spawner.ThrowCurrentBall();
            }
        } else if (GameManager.Instance.playerPlatform == PlayerPlatform.STEAMVR) {
            if (click.GetStateUp(handPose.inputSource)) {
                spawner.ThrowCurrentBall();
            }
        }
    }
}
