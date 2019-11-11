﻿using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;
    public int[] playerScores;
    private PhotonView photonView;

    private readonly byte GoalWasScoredEvent = 1;
    public Utils.PlayerNumber playerLastScored = Utils.PlayerNumber.NULL;

    private void Awake() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start() {
        // TODO: make it include number of players incl bot
        playerScores = new int[2];
        GameManager.Instance.RestartEvent.AddListener(ResetScores);
        playerLastScored = Utils.PlayerNumber.NULL;
    }

    [PunRPC]
    public void PhotonAddScore(int playerNumber, int score) {
        playerScores[playerNumber] += score;
    }

    public void AddScore(Utils.PlayerNumber playerNumber, int score) {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("PhotonAddScore", RpcTarget.AllBuffered, (int)playerNumber, score);
        } else {
            playerScores[(int)playerNumber] += score;
        }
    }

    public void AddScoreToOpponent(Utils.PlayerNumber me, int score) {
        Utils.PlayerNumber scoringPlayerNumber = me == Utils.PlayerNumber.ONE ? Utils.PlayerNumber.TWO : Utils.PlayerNumber.ONE;
        Debug.Log("VIC_DEBUG SCORE!: " + scoringPlayerNumber);

        if (PhotonNetwork.IsConnected) {
            photonView.RPC("PhotonAddScore", RpcTarget.AllBuffered, (int)scoringPlayerNumber, score);

            // Raise GoalWasScoredEvent to all players
            object[] content = new object[] { scoringPlayerNumber };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            Debug.Log("VIC_DEBUG Raising event: " + GoalWasScoredEvent);
            PhotonNetwork.RaiseEvent(GoalWasScoredEvent, content, raiseEventOptions, sendOptions);

        } else {
            playerScores[(int)scoringPlayerNumber] += score;
        }
    }

    [PunRPC]
    public void PhotonResetScores() {
        for (int i = 0; i < playerScores.Length; i++) {
            playerScores[i] = 0;
        }
    }

    public void ResetScores() {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("PhotonResetScores", RpcTarget.AllBuffered, null);
        } else {
            for (int i = 0; i < playerScores.Length; i++) {
                playerScores[i] = 0;
            }
        }
    }

    public Utils.PlayerNumber GetPlayerLastScored() {
        return playerLastScored;
    }
}
