using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;
    public int[] playerScores;
    private PhotonView photonView;

    private void Awake() {
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start() {
        // TODO: make it include number of players incl bot
        playerScores = new int[2];
        GameManager.Instance.RestartEvent.AddListener(ResetScores);
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
        Utils.PlayerNumber playerNumber = me == Utils.PlayerNumber.ONE ? Utils.PlayerNumber.TWO : Utils.PlayerNumber.ONE;
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("PhotonAddScore", RpcTarget.AllBuffered, (int)playerNumber, score);
        } else {
            playerScores[(int)playerNumber] += score;
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
}
