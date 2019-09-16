using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject responsible for keeping track of balls
public class BallManager : MonoBehaviour {
    public static BallManager Instance;
    public GameObject[] BallTypes;
    // PlayerBallQueues[0] will give the queue of balls for player 1 to spawn, and [1] for player 2
    public Queue<GameObject>[] PlayerBallQueues;
    public Transform ballPool;
    // pointer to keep track of where in the pool the end of the queues are
    private int poolPointer = 0;
    // A transform to be parent of all active balls in the scene
    public Transform activeBalls;
    // For debugging at the current stage of development. Just give the player 10 balls for a start
    private static readonly int numberOfStartingBalls = 10;
    // To add a new ball to the queue at fixed intervals
    private static readonly float ballSpawnInterval = 20f;
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
        PlayerBallQueues = new Queue<GameObject>[GameManager.Instance.numPlayers];
        // Insufficient balls in the pool, add new balls
        if (PlayerBallQueues.Length * numberOfStartingBalls > ballPool.childCount) {
            AddNewBallsToPool(PlayerBallQueues.Length * numberOfStartingBalls - ballPool.childCount);
        }
 
        for (int i = 0; i < PlayerBallQueues.Length; i++) {
            PlayerBallQueues[i] = new Queue<GameObject>();
            for (int j = 0; j < numberOfStartingBalls; j++) {
                PlayerBallQueues[i].Enqueue(ballPool.GetChild(i * j + j).gameObject);
                poolPointer += 1;
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
    }

    public void PutBallInPool(GameObject ball) {
        ball.SetActive(false);
        ball.transform.parent = ballPool;
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
            if (poolPointer >= ballPool.childCount) {
                AddNewBallsToPool(poolPointer - ballPool.childCount + 1);
            }
 
            for (int i = 0; i < PlayerBallQueues.Length; i++) {
                PlayerBallQueues[i].Enqueue(ballPool.GetChild(poolPointer++).gameObject);
            }
        }
    }

    // To be called by spawner
    public void DecrementPoolPointer(int numToDecrement = 1) {
        poolPointer -= numToDecrement;
    }

    // To be called by spawner
    public void IncrementPoolPointer(int numToIncrement = 1) {
        poolPointer += numToIncrement;
    }
}
