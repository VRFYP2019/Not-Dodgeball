using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public bool isGameEnded = false;
    public float gameDuration = 60.0f;   // 1 minute
    public int numPlayers = 1;
    public ScoreBoard[] scoreBoards;

    private void Awake() {
        Instance = this;
    }

    public void Restart() {
        BallManager.Instance.Restart();
        StartCoroutine(DelayAndRestart());
    }

    private IEnumerator DelayAndRestart() {
        yield return new WaitForSeconds(1f);
        isGameEnded = false;
        foreach (ScoreBoard scoreBoard in scoreBoards) {
            scoreBoard.Restart();
        }
    }
}
