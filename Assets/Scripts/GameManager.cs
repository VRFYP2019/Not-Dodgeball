﻿using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using Utils;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks {
    public static GameManager Instance;
    public PlayerPlatform playerPlatform;
    public bool isGameEnded = false;
    public float gameDuration = 60.0f;   // 1 minute
    public float timeLeft;
    public UnityEvent RestartEvent;
    public UnityEvent TimeOverEvent;

    private void Awake() {
        Instance = this;
        string modelName = XRDevice.model;

        if (modelName.StartsWith("Oculus")) {
            playerPlatform = PlayerPlatform.OCULUS;
        } else if (modelName.StartsWith("OpenVR") || modelName.StartsWith("HTC")) {    // TODO: remove whichever is wrong
            playerPlatform = PlayerPlatform.STEAMVR;
        } else if (Application.isEditor) {
            playerPlatform = PlayerPlatform.EDITOR;
        } else {
            Debug.Log("Unhandled model: " + modelName);
        }
    }

    private void Start() {
        if (PhotonNetwork.IsConnected) {
            PhotonHashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
            object temp;
            if (hash.TryGetValue("RoomRoundDuration", out temp)) {
                gameDuration = (int)temp * 60;
                Debug.Log("duration for this game: " + gameDuration + " seconds");
            } else {
                Debug.Log("RoomRoundDuration: custom property not found");
            }
        }
        timeLeft = gameDuration;
        PlaytestRecording.RecordRoundDuration((int)gameDuration / 60);
    }

    private void Update() {
        if (!isGameEnded) {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0) {
                isGameEnded = true;
                AudioManager.PlaySoundOnce("buzz");
                TimeOverEvent.Invoke();
            }
        }
    }

    [PunRPC]
    public void GameManager_Restart() {
        BallManager.LocalInstance.Restart();
        StartCoroutine(DelayAndRestart());
    }

    public void Restart() {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("GameManager_Restart", RpcTarget.All, null);
        } else {
            GameManager_Restart();
        }
    }

    private IEnumerator DelayAndRestart() {
        yield return new WaitForSeconds(0.25f);
        RestartEvent.Invoke();
        timeLeft = gameDuration;
        isGameEnded = false;
    }
}
