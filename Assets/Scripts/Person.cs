using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Person : MonoBehaviour {
    public bool isRightHandSpawner;
    HandController leftHandController;
    HandController rightHandController;
    SteamVR_Behaviour_Pose leftHand;
    SteamVR_Behaviour_Pose rightHand;

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
        if (BallManager.Instance.PlayerBallQueues[0].Count > 0
            && !rightHandController.isSpawning && !leftHandController.isSpawning ) {
            if (isRightHandSpawner) {
                rightHandController.SwitchToSpawnerHand();
            } else {
                leftHandController.SwitchToSpawnerHand();
            }
        }
    }
}
