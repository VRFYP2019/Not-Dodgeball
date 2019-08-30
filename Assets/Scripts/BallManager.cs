using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject responsible for keeping track of balls
public class BallManager : MonoBehaviour {
    public static BallManager Instance;
    public GameObject[] BallTypes;
    // PlayerBallQueues[0] will give the queue of balls for player 1 to spawn, and [1] for player 2
    public Queue<GameObject>[] PlayerBallQueues;
    // For debugging at the current stage of development. Just give the player 50 balls for now
    static readonly int numberOfStartingBalls = 50;

    private void Awake() {
        Instance = this;
        // TODO: Set to number of players based on a GameManager or something
        PlayerBallQueues = new Queue<GameObject>[1];
        for (int i = 0; i < PlayerBallQueues.Length; i++) {
            PlayerBallQueues[i] = new Queue<GameObject>();
            for (int j = 0; j < numberOfStartingBalls; j++) {
                GameObject newBall = Instantiate(BallTypes[0], this.transform);
                newBall.SetActive(false);
                PlayerBallQueues[i].Enqueue(newBall);
            }
        }
    }
}
