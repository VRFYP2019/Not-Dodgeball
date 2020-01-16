using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class EscapeMenu : MonoBehaviourPunCallbacks {
    private Camera playerCam;
    private bool isEscaped = false;
    public Canvas pauseMenuCanvas;

    [Header("Desktop")]
    [SerializeField]
    private GameObject[] desktopObjects = null;

    [Header("OVR")]
    [SerializeField]
    private GameObject[] oculusObjects = null;

    void Start() {
        if (OVRPlugin.productName != null && OVRPlugin.productName.StartsWith("Oculus")) {
            foreach (GameObject go in desktopObjects) {
                go.SetActive(false);
            }
            foreach (GameObject go in oculusObjects) {
                go.SetActive(true);
            }
            playerCam = GameObject.Find("CenterEyeAnchor").GetComponent<Camera>();
            pauseMenuCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            pauseMenuCanvas.worldCamera = playerCam;
            pauseMenuCanvas.planeDistance = 3f;
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
            return true;
        } else {
            pauseMenuCanvas.gameObject.SetActive(false);
            return false;
        }
    }

    public void ReturnToLobby() {
        AudioManager.PlaySoundOnce("goalding"); //TODO: UI sounds
        NetworkController.Instance.PlayerLeaveRoom();
    }

    public void ReturnToRoom() {
        AudioManager.PlaySoundOnce("goalding");
        PhotonNetwork.AutomaticallySyncScene = false;
        NetworkController.Instance.ToggleLobbyUI();
        NetworkController.Instance.PlayerReturnToRoom();
        PhotonNetwork.LoadLevel(0);
    }
}
