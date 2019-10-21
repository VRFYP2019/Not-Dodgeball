using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviourPunCallbacks {
    public static GameManager Instance;
    public bool isGameEnded = false;
    public float gameDuration = 60.0f;   // 1 minute
    public int numPlayers = 1;
    public UnityEvent RestartEvent;
    public bool isOculusQuest = false;
    public bool isEditor = false;

    private void Awake() {
        Instance = this;
        isOculusQuest = OVRPlugin.productName == "Oculus Quest";
        isEditor = Application.isEditor;
    }

    [PunRPC]
    public void PhotonRestart() {
        BallManager.LocalInstance.Restart();
        StartCoroutine(DelayAndRestart());
    }

    public void Restart() {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("PhotonRestart", RpcTarget.All, null);
        } else {
            BallManager.LocalInstance.Restart();
            StartCoroutine(DelayAndRestart());
        }
    }

    private IEnumerator DelayAndRestart() {
        yield return new WaitForSeconds(0.25f);
        RestartEvent.Invoke();
        isGameEnded = false;
    }
}
