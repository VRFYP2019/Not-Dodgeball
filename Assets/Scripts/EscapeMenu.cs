using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class EscapeMenu : MonoBehaviourPunCallbacks {
    private bool isEscaped = false;
    public Canvas pauseMenuCanvas;
    public Canvas calorieCanvas;

    private readonly byte LeaveGameEvent = 2;

    void Start() {
        pauseMenuCanvas.gameObject.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown ("escape") || OVRInput.GetDown(OVRInput.Button.Start)) {
            // Cannot open the escape menu if calorie stats is open
            if (!calorieCanvas.gameObject.activeInHierarchy) {
                isEscaped = TogglePause();
            }
        }
    }
    
    #region PUN CALLBACKS
    public override void OnLeftRoom() {
        PhotonNetwork.LoadLevel(0);
    }
    #endregion

    private bool TogglePause() {
        #if !UNITY_EDITOR
        if (!isEscaped) {
            OculusUIHandler.instance.laserLineRenderer.enabled = true;
        } else {
            OculusUIHandler.instance.laserLineRenderer.enabled = false;
        }
        #endif

        if (!isEscaped) {
            pauseMenuCanvas.gameObject.SetActive(true);
            return true;
        } else {
            pauseMenuCanvas.gameObject.SetActive(false);
            return false;
        }
    }

    public void ReturnToLobby() {
        AudioManager.PlaySoundOnce("uiclick");
        if (PhotonNetwork.IsConnected) {
            bool isHost = (PhotonNetwork.IsMasterClient) ? true : false;
            TryLeaveRoom(true, isHost);
        } else {
            NetworkController.Instance.PlayerLeaveRoom();
        }
    }

    public void ReturnToRoom() {
        AudioManager.PlaySoundOnce("uiclick");
        if (PhotonNetwork.IsConnected) {
            bool isHost = (PhotonNetwork.IsMasterClient) ? true : false;
            TryLeaveRoom(false, isHost);
        } else {
            NetworkController.Instance.PlayerReturnToRoom();
        }

    }

    // Raise LeaveGameEvent to all players
    private void TryLeaveRoom(bool isReturnToLobby, bool isHost) {
        object[] content = new object[] { isReturnToLobby, isHost };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(LeaveGameEvent, content, raiseEventOptions, sendOptions);
    }
}
