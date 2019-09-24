using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Human : Player {
    public SteamVR_Action_Boolean grab = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public SteamVR_Action_Boolean trigger = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    private SteamVR_Behaviour_Pose leftHand;
    private SteamVR_Behaviour_Pose rightHand;

    protected override void Start() {
        base.Start();
        SteamVR_Behaviour_Pose[] hands = GetComponentsInChildren<SteamVR_Behaviour_Pose>();
        leftHand = hands[0];
        rightHand = hands[1];
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        // if game ended and trigger pressed, restart the game
        if (GameManager.Instance.isGameEnded) {
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
