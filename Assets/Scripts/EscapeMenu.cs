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

    [Header("Desktop")]
    [SerializeField]
    private GameObject[] desktopObjects = null;

    [Header("OVR")]
    [SerializeField]
    private GameObject[] oculusObjects = null;
    private LineRenderer laserLineRenderer = null;

    private readonly byte LeaveGameEvent = 2;

    void Start() {
        if (OVRPlugin.productName != null && OVRPlugin.productName.StartsWith("Oculus")) {
            foreach (GameObject go in desktopObjects) {
                go.SetActive(false);
            }
            foreach (GameObject go in oculusObjects) {
                go.SetActive(true);

                // UIHelpers must be the first go in oculusObjects
                if (laserLineRenderer == null) {
                    laserLineRenderer = go.GetComponentInChildren<LineRenderer>();
                }
            }
        }
        pauseMenuCanvas.gameObject.SetActive(false);
    }

    void Update() {
        if (Input.GetKeyDown ("escape") || OVRInput.GetDown(OVRInput.Button.Start)) {
            isEscaped = TogglePause();
        }
    }
    
    #region PUN CALLBACKS
    public override void OnLeftRoom() {
        PhotonNetwork.LoadLevel(0);
    }
    #endregion

    private bool TogglePause() {
        if (!isEscaped) {
            pauseMenuCanvas.gameObject.SetActive(true);
            laserLineRenderer.enabled = true;
            return true;
        } else {
            pauseMenuCanvas.gameObject.SetActive(false);
            laserLineRenderer.enabled = false;
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
