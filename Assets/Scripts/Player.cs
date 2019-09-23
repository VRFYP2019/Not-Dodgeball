using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public PlayerManager.PlayerNumber playerNumber;
    public PlayerManager.PlayerType playerType;
    protected HandController leftHandController;
    protected HandController rightHandController;

    // Start is called before the first frame update
    protected virtual void Start() {
        HandController[] handControllers = GetComponentsInChildren<HandController>();
        leftHandController = handControllers[0].GetComponent<HandController>();
        rightHandController = handControllers[1].GetComponent<HandController>();
        leftHandController.SwitchToTool();
        rightHandController.SwitchToTool();
    }

    // Update is called once per frame
    protected virtual void Update() {
        // if game ended and trigger pressed, restart the game
        if (GameManager.Instance.isGameEnded) {
            rightHandController.ResetSpawnerStateAndSwitchToTool();
            leftHandController.ResetSpawnerStateAndSwitchToTool();
        }
    }
}
