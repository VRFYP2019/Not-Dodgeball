using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject responsible for keeping track of balls
public class BallManager : MonoBehaviour {
    public static BallManager Instance;
    public GameObject[] BallTypes;
    // PlayerBallQueues[0] will give the queue of balls for player 1 to spawn, and [1] for player 2
    public Queue<GameObject>[] PlayerBallQueues;
    // For debugging at the current stage of development. Just give the player 10 balls for a start
    static readonly int numberOfStartingBalls = 10;
    // To add a new ball to the queue at fixed intervals
    static readonly float ballSpawnInterval = 5f;

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

    private void Start() {
        StartCoroutine(AddBallsToQueuePeriodically(3f));
    }

    IEnumerator AddBallsToQueuePeriodically(float addInterval) {
        while (true) {
            yield return new WaitForSeconds(addInterval);
            for (int i = 0; i < PlayerBallQueues.Length; i++) {
                GameObject newBall = Instantiate(BallTypes[0], this.transform);
                newBall.SetActive(false);
                PlayerBallQueues[i].Enqueue(newBall);
            }
        }
    }
}
