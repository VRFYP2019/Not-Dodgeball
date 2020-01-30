using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Player : MonoBehaviourPunCallbacks {
    public PlayerNumber playerNumber;
    public PlayerType playerType;
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

    [PunRPC]
    public void Player_SetPlayerNumber(int num) {
        playerNumber = (PlayerNumber)num;
        GetComponentInChildren<Goal>(true).SetPlayerNumber((PlayerNumber)num);
    }

    public void SetPlayerNumber(PlayerNumber playerNumber) {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("Player_SetPlayerNumber", RpcTarget.All, (int)playerNumber);
        } else {
            Player_SetPlayerNumber((int)playerNumber);
        }
    }

    protected virtual void TrySwitchHandToSpawner(HandSide handSide) {
        if (handSide == HandSide.LEFT) {
            if (BallManager.LocalInstance.playerBallQueue.childCount == 1
                && rightHandController.currHandObject == HandObject.SPAWNER
                ) {
                // If, when trying to switch this hand to spawner, there is
                // only 1 ball left and it is being held by another hand,
                // take it out of that hand
                rightHandController.ResetSpawnerStateAndSwitchToTool();
            }
            // Only switch to spawner if there is at least 1 ball in queue
            if (BallManager.LocalInstance.playerBallQueue.childCount > 0) {
                leftHandController.SwitchToSpawnerHand();
            }
        } else if (handSide == HandSide.RIGHT) {
            if (BallManager.LocalInstance.playerBallQueue.childCount == 1
                && leftHandController.currHandObject == HandObject.SPAWNER) {
                leftHandController.ResetSpawnerStateAndSwitchToTool();
            }
            // Only switch to spawner if there is at least 1 ball in queue
            if (BallManager.LocalInstance.playerBallQueue.childCount > 0) {
                rightHandController.SwitchToSpawnerHand();
            }
        }
    }
}
