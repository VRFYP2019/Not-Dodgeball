using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject responsible for keeping track of balls
public class BallManager : MonoBehaviour {
    public static BallManager Instance;
    public GameObject[] BallTypes;
    // playerBallQueues[0] will give the parent of balls for player 1 to spawn, and [1] for player 2
    public Transform[] playerBallQueues;
    public Transform ballPool;
    // A transform to be parent of all active balls in the scene
    public Transform activeBalls;
    // For debugging at the current stage of development. Just give the player 10 balls for a start
    private static readonly int numberOfStartingBalls = 20;
    // To add a new ball to the queue at fixed intervals
    private static readonly float ballSpawnInterval = 1f;
    private IEnumerator spawnCoroutine;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        spawnCoroutine = AddBallsToQueuePeriodically(ballSpawnInterval);
        StartCoroutine(spawnCoroutine);
        InitPlayerQueues();
    }

    private void InitPlayerQueues() {
        // Insufficient balls in the pool, add new balls
        if (playerBallQueues.Length * numberOfStartingBalls > ballPool.childCount) {
            AddNewBallsToPool(playerBallQueues.Length * numberOfStartingBalls - ballPool.childCount);
        }

        for (int i = 0; i < playerBallQueues.Length; i++) {
            for (int j = 0; j < numberOfStartingBalls; j++) {
                PutBallInQueue(i, ballPool.GetChild(0).gameObject);
            }
        }
    }

    public void Restart() {
        PutAllBallsInPool();
        InitPlayerQueues();
    }

    public void PutAllBallsInPool() {
        for (int i = activeBalls.childCount - 1; i > -1; i--) {
            PutBallInPool(activeBalls.GetChild(i).gameObject);
        }
        for (int i = 0; i < playerBallQueues.Length; i++) {
            for (int j = 0; j < playerBallQueues[i].childCount; j++) {
                PutBallInPool(playerBallQueues[i].GetChild(j).gameObject);
            }
        }
    }

    public void PutBallInPool(GameObject ball) {
        ball.SetActive(false);
        ball.transform.parent = ballPool;
    }

    // Put a ball into a specified queue
    public void PutBallInQueue(int playerNum, GameObject ball) {
        ball.SetActive(false);
        ball.transform.parent = playerBallQueues[playerNum];
    }

    public void AddNewBallsToPool(int numBallsToAdd = 1) {
        for (int i = 0; i < numBallsToAdd; i++) {
            GameObject newBall = Instantiate(BallTypes[0], ballPool);
            newBall.SetActive(false);
        }
    }

    private IEnumerator AddBallsToQueuePeriodically(float addInterval) {
        while (true) {
            yield return new WaitForSeconds(addInterval);
            // Insufficient balls in the pool, add new balls
            if (ballPool.childCount < playerBallQueues.Length) {
                AddNewBallsToPool(playerBallQueues.Length - ballPool.childCount);
            }
 
            for (int i = 0; i < playerBallQueues.Length; i++) {
                PutBallInQueue(i, ballPool.GetChild(0).gameObject);
            }
        }
    }
}
