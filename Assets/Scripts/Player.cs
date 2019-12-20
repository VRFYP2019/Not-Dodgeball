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
}
