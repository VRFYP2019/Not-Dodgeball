using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Person : MonoBehaviour {
    public bool isRightHandSpawner;
    public SteamVR_Action_Boolean grab = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    HandController leftHandController;
    HandController rightHandController;
    SpawnerHand currentSpawner = null;
    SteamVR_Behaviour_Pose leftHand;
    SteamVR_Behaviour_Pose rightHand;
    [SerializeField]
    private bool _isSpawning;
    public bool IsSpawning { get { return _isSpawning; } set { _isSpawning = value; } }

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
        if (BallManager.Instance.PlayerBallQueues[0].Count > 0) {
            if (!_isSpawning) {
                _isSpawning = true;
                if (isRightHandSpawner) {
                    rightHandController.SwitchToSpawnerHand(null);
                    currentSpawner = rightHand.GetComponentInChildren<SpawnerHand>();
                } else {
                    leftHandController.SwitchToSpawnerHand(null);
                    currentSpawner = leftHand.GetComponentInChildren<SpawnerHand>();
                }
            }
        }
        if (grab.GetStateDown(SteamVR_Input_Sources.Any)) {
            isRightHandSpawner = !isRightHandSpawner;
            if (_isSpawning) {
                if (isRightHandSpawner) {
                    rightHandController.SwitchToSpawnerHand(currentSpawner.currentBall);
                    currentSpawner = rightHand.GetComponentInChildren<SpawnerHand>();
                    leftHandController.SwitchToTool();
                } else {
                    leftHandController.SwitchToSpawnerHand(currentSpawner.currentBall);
                    currentSpawner = leftHand.GetComponentInChildren<SpawnerHand>();
                    rightHandController.SwitchToTool();
                }
            }
        }
    }
}
