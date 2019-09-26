using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public GameObject HumanPrefab;
    public GameObject BotPrefab;
    public GameObject OffScreenUIPrefab;
    // Controls whether we need to spawn player 1
    public bool isPlayerInScene = true;
    public Transform player1SpawnPoint;
    public Transform player2SpawnPoint;
    public ScoreBoard[] scoreBoards;
    private GameObject player1, player2;

    public enum PlayerNumber {
        ONE,
        TWO
    }
    public enum PlayerType {
        HUMAN,
        BOT
    }

    // Start is called before the first frame update
    void Start() {
        StartGame(1);
    }

    // To be called from somewhere else after implementing online multiplayer
    // Unsure at this point whether the number of players should be updated in GameManager before calling this,
    // or to let this update that. For now use the latter.
    public void StartGame(int numPlayers) {
        GameManager.Instance.numPlayers = numPlayers;
        // TODO: Uncomment player 2 lines after implementing bot
        if (!isPlayerInScene) {
            player1 = Instantiate(HumanPrefab, player1SpawnPoint.position, player1SpawnPoint.rotation);
            GameObject offScreenUI1 = Instantiate(OffScreenUIPrefab);
            offScreenUI1.GetComponent<WallIndicator>().playerLocation = player1.GetComponentInChildren<Camera>().transform;
            offScreenUI1.GetComponent<Canvas>().worldCamera = player1.GetComponentInChildren<Camera>();
        }
        if (numPlayers == 1) {
            // player2 = Instantiate(BotPrefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
        } else if (numPlayers == 2) {
            player2 = Instantiate(HumanPrefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
            GameObject offScreenUI2 = Instantiate(OffScreenUIPrefab);
            offScreenUI2.GetComponent<WallIndicator>().playerLocation = player2.GetComponentInChildren<Camera>().transform;
            offScreenUI2.GetComponent<Canvas>().worldCamera = player2.GetComponentInChildren<Camera>();
        } else {
            Debug.LogError("invalid number of players");
        }
        foreach (ScoreBoard scoreBoard in scoreBoards) {
            scoreBoard.player1Goal = player1.GetComponentInChildren<Goal>().gameObject;
            //scoreBoard.player2Goal = player2.GetComponentInChildren<Goal>().gameObject;
            scoreBoard.Init();
        }
    }
}
