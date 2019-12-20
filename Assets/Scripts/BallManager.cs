using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameObject responsible for keeping track of balls
public class BallManager : MonoBehaviour {
    public static BallManager LocalInstance;
    public static BallManager RemoteInstance;
    public GameObject[] BallTypes;
    public Transform playerBallQueue;
    public Transform ballPool;
    // A transform to be parent of player's active balls in the scene
    public Transform activeBalls;
    private static readonly int numberOfStartingBalls = 2;

    private void Awake() {
        PhotonView pv = GetComponent<PhotonView>();
        if (!PhotonNetwork.IsConnected || pv.IsMine) {
            LocalInstance = this;
            // Uncomment the following 2 lines to allow periodic spawning
            //float ballSpawnInterval = 1f;
            //StartCoroutine(AddBallsToQueuePeriodically(ballSpawnInterval));
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
            PutBallInQueue(ballPool.GetChild(0).GetComponent<Ball>());
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
            PutBallInPool(activeBalls.GetChild(i).GetComponent<Ball>());
        }
        for (int i = playerBallQueue.childCount - 1; i > -1; i--) {
            PutBallInPool(playerBallQueue.GetChild(i).GetComponent<Ball>());
        }
    }

    public void PutBallInPool(Ball ball) {
        ball.SetCountTimeLivedToFalse();
        ball.SetState(false);
        ball.SetParent(ballPool);
    }

    // Put a ball into player ball queue
    public void PutBallInQueue(Ball ball) {
        ball.SetCountTimeLivedToFalse();
        ball.SetState(false);
        ball.SetParent(playerBallQueue);
    }

    public void AddNewBallsToPool(int numBallsToAdd = 1) {
        for (int i = 0; i < numBallsToAdd; i++) {
            GameObject newBall;
            if (PhotonNetwork.IsConnected) {
                newBall = PhotonNetwork.Instantiate(
                    BallTypes[0].name,
                    Vector3.zero,
                    Quaternion.identity,
                    0,
                    GetBallInitData());
            } else {
                newBall = Instantiate(BallTypes[0], ballPool);
            }
            newBall.GetComponent<Ball>().SetState(false);
        }
    }

    public void AddBallsToQueue(int numBalls) {
        for (int i = 0; i < numBalls; i++) {
            PutBallInQueue(ballPool.GetChild(0).GetComponent<Ball>());
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
            PutBallInQueue(ballPool.GetChild(0).GetComponent<Ball>());
        }
    }
}
