using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject responsible for keeping track of balls
public class BallManager : MonoBehaviour {
    public static BallManager LocalInstance;
    public static BallManager RemoteInstance;
    public GameObject[] BallTypes;
    // playerBallQueues[0] will give the parent of balls for player 1 to spawn, and [1] for player 2
    public Transform playerBallQueue;
    public Transform ballPool;
    // A transform to be parent of all active balls in the scene
    public Transform activeBalls;
    // For debugging at the current stage of development. Just give the player 10 balls for a start
    private static readonly int numberOfStartingBalls = 2;
    // To add a new ball to the queue at fixed intervals (for debug only)
    private static readonly float ballSpawnInterval = 1f;
    private IEnumerator spawnCoroutine;
    private PhotonView photonView;

    private void Awake() {
        photonView = GetComponent<PhotonView>();
        if (!PhotonNetwork.IsConnected || photonView.IsMine) {
            LocalInstance = this;
            //spawnCoroutine = AddBallsToQueuePeriodically(ballSpawnInterval);
            //StartCoroutine(spawnCoroutine);
        } else {
            RemoteInstance = this;
        }
        InitPlayerQueues();
    }

    private void Start() {
        GameManager.Instance.RestartEvent.AddListener(Restart);
    }

    private void InitPlayerQueues() {
        // Insufficient balls in the pool, add new balls
        if (numberOfStartingBalls > ballPool.childCount) {
            AddNewBallsToPool(numberOfStartingBalls - ballPool.childCount);
        }

        for (int j = 0; j < numberOfStartingBalls; j++) {
            PutBallInQueue(ballPool.GetChild(0).gameObject);
        }
    }

    public void Restart() {
        PutAllBallsInPool();
        InitPlayerQueues();
    }

    private void Update() {
        // Ensure pool is never empty
        if (ballPool.childCount < 2) {
            AddNewBallsToPool(1);
        }
    }

    public void PutAllBallsInPool() {
        for (int i = activeBalls.childCount - 1; i > -1; i--) {
            PutBallInPool(activeBalls.GetChild(i).gameObject);
        }
        for (int i = playerBallQueue.childCount - 1; i > -1; i--) {
            PutBallInPool(playerBallQueue.GetChild(i).gameObject);
        }
    }

    public void PutBallInPool(GameObject ball) {
        ball.GetComponent<Ball>().SetCountTimeLivedToFalse();
        ball.GetComponent<Ball>().SetState(false);
        ball.GetComponent<Ball>().SetParent(ballPool);
    }

    // Put a ball into player ball queue
    public void PutBallInQueue(GameObject ball) {
        ball.GetComponent<Ball>().SetCountTimeLivedToFalse();
        ball.GetComponent<Ball>().SetState(false);
        ball.GetComponent<Ball>().SetParent(playerBallQueue);
    }

    // When a player scores, 2 balls are added
    public void HandleBallScored(GameObject ball) {
        PutBallInQueue(ball);
        PutBallInQueue(ballPool.GetChild(0).gameObject);
    }

    public void AddNewBallsToPool(int numBallsToAdd = 1) {
        for (int i = 0; i < numBallsToAdd; i++) {
            GameObject newBall;
            if (PhotonNetwork.IsConnected) {
                newBall = PhotonNetwork.Instantiate(BallTypes[0].name, Vector3.zero, Quaternion.identity, 0, GetBallInitData());
            } else {
                newBall = Instantiate(BallTypes[0], ballPool);
            }
            newBall.GetComponent<Ball>().SetState(false);
        }
    }

    public void AddBallsToQueue(int numBalls) {
        for (int i = 0; i < numBalls; i++) {
            PutBallInQueue(ballPool.GetChild(0).gameObject);
        }
    }

    private object[] GetBallInitData() {
        object[] data = new object[1];
        data[0] = false;
        return data;
    }

    private IEnumerator AddBallsToQueuePeriodically(float addInterval) {
        while (true) {
            yield return new WaitForSeconds(addInterval);
            PutBallInQueue(ballPool.GetChild(0).gameObject);
        }
    }
}
