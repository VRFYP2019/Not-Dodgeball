using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Person : MonoBehaviour {
    public bool isRightHandSpawner;
    public SteamVR_Action_Boolean grab = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public SteamVR_Action_Boolean trigger = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    private HandController leftHandController;
    private HandController rightHandController;
    private SteamVR_Behaviour_Pose leftHand;
    private SteamVR_Behaviour_Pose rightHand;

    // Start is called before the first frame update
    void Start() {
        SteamVR_Behaviour_Pose[] hands = GetComponentsInChildren<SteamVR_Behaviour_Pose>();
        leftHand = hands[0];
        rightHand = hands[1];
        leftHandController = hands[0].GetComponent<HandController>();
        rightHandController = hands[1].GetComponent<HandController>();
        leftHandController.SwitchToTool();
        rightHandController.SwitchToTool();
    }

    // Update is called once per frame
    void Update() {
        // if game ended and trigger pressed, restart the game
        if (GameManager.Instance.isGameEnded) {
            rightHandController.ResetSpawnerStateAndSwitchToTool();
            leftHandController.ResetSpawnerStateAndSwitchToTool();
            if (trigger.GetStateDown(SteamVR_Input_Sources.Any)) {
                GameManager.Instance.Restart();
            }
        }

        // if grab is pressed, switch between tool and spawn for that hand
        if (grab.GetStateDown(rightHand.inputSource)) {
            rightHandController.Switch();
        }
        if (grab.GetStateDown(leftHand.inputSource)) {
            leftHandController.Switch();
        }
    }
}
