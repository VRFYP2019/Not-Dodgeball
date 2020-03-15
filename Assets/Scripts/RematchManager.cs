using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RematchManager : MonoBehaviour {
    private Canvas rematchCanvas;
    [SerializeField]
    private Button
        leaveGameButton,
        restartGameButton;
    private bool otherPlayerReadyToLeave = false;

    private readonly byte LeaveGameEvent = 2;
    private readonly byte ReadyToLeaveEvent = 3;

    // Start is called before the first frame update
    void Start() {
        rematchCanvas = GetComponentInChildren<Canvas>();
        rematchCanvas.gameObject.SetActive(false);
        if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.PlayerCount > 1) {
            NetworkController.Instance.readyToLeaveEvent.AddListener(SetOtherPlayerReadyToLeave);
        } else {
            // bot is always ready to leave
            SetOtherPlayerReadyToLeave();
        }
    }

    public void OnLeavePressed() {
        if (NetworkController.Instance == null) {
            #if UNITY_EDITOR
            EditorApplication.Exit(0);
            #else
            Application.Quit();
            #endif
            return;
        }
        if (otherPlayerReadyToLeave) {  // send a leave game event
            object[] content = new object[] { false, false }; // isReturnToLobby (false), isHostDecision (doesn't matter)
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(LeaveGameEvent, content, raiseEventOptions, sendOptions);
        } else {    // send a ready to leave event
            leaveGameButton.GetComponentInChildren<Text>().text = "Leave request sent!";
            leaveGameButton.interactable = false;
            restartGameButton.gameObject.SetActive(false);
            object[] content = new object[] { };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(ReadyToLeaveEvent, content, raiseEventOptions, sendOptions);
        }
    }

    public void SetOtherPlayerReadyToLeave() {
        otherPlayerReadyToLeave = true;
    }

    public void Restart() {
        #if !UNITY_EDITOR
        OculusUIHandler.instance.laserLineRenderer.enabled = false;
        #endif
        rematchCanvas.gameObject.SetActive(false);
        GameManager.Instance.Restart();
        if (NetworkController.Instance != null) {
            NetworkController.Instance.UnreadyPlayers();
        }
    }
}
