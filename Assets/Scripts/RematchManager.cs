using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RematchManager : MonoBehaviour {
    private Canvas rematchCanvas;

    // Start is called before the first frame update
    void Start() {
        rematchCanvas = GetComponentInChildren<Canvas>();
        rematchCanvas.gameObject.SetActive(false);
    }

    // Leave room without forcing the other player out
    public void LeaveRoomAlone() {
        #if !UNITY_EDITOR
        OculusUIHandler.instance.laserLineRenderer.enabled = false;
        #endif
        if (NetworkController.Instance != null) {
            NetworkController.Instance.PlayerLeaveRoom();
        } else {    // Probably lobby not loaded/built
            Application.Quit();
        }
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
