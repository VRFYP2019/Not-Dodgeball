using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;
    public int[] playerScores;
    private PhotonView pv;

    private readonly byte GoalWasScoredEvent = 1;

    private void Awake() {
        Instance = this;
        pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start() {
        // TODO: make it include number of players incl bot
        playerScores = new int[2];
        GameManager.Instance.RestartEvent.AddListener(ResetScores);
    }

    [PunRPC]
    public void ScoreManager_AddScore(int playerNumber, int score) {
        playerScores[playerNumber] += score;
    }

    public void AddScoreToOpponent(PlayerNumber me, int score) {
        PlayerNumber scoringPlayerNumber = me == PlayerNumber.ONE ? PlayerNumber.TWO : PlayerNumber.ONE;

        if (PhotonNetwork.IsConnected) {
            pv.RPC("ScoreManager_AddScore", RpcTarget.AllBuffered, (int)scoringPlayerNumber, score);

            // Raise GoalWasScoredEvent to all players
            object[] content = new object[] { scoringPlayerNumber };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(GoalWasScoredEvent, content, raiseEventOptions, sendOptions);

        } else {
            ScoreManager_AddScore((int)scoringPlayerNumber, score);
        }
    }

    [PunRPC]
    public void ScoreManager_ResetScores() {
        for (int i = 0; i < playerScores.Length; i++) {
            playerScores[i] = 0;
        }
    }

    public void ResetScores() {
        if (PhotonNetwork.IsConnected) {
            pv.RPC("ScoreManager_ResetScores", RpcTarget.AllBuffered, null);
        } else {
            ScoreManager_ResetScores();
        }
    }
}
