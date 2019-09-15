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
    private SpawnerHand currentSpawner = null;
    private SteamVR_Behaviour_Pose leftHand;
    private SteamVR_Behaviour_Pose rightHand;
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
        // if game ended and trigger pressed, restart the game
        if (GameManager.Instance.isGameEnded) {
            rightHandController.ResetSpawnerStateAndSwitchToTool();
            leftHandController.ResetSpawnerStateAndSwitchToTool();
            _isSpawning = false;
            if (trigger.GetStateDown(SteamVR_Input_Sources.Any)) {
                GameManager.Instance.Restart();
            }
        }

        // if balls are available for spawning, spawn
        if (!GameManager.Instance.isGameEnded && BallManager.Instance.PlayerBallQueues[0].Count > 0) {
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

        // if grab is pressed, change spawn hand
        // TODO: grab should cause the hand that pressed it to change to a spawner
        // instead of immediately changing a hand to spawner upon a ball in queue
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
