using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hand for spawning balls. Set to active only when there is a ball in the queue for this player and player
// wants to spawn. Otherwise, a Tool should be active on the player's hand instead.
public class Spawner : MonoBehaviour {
    private Transform parentOfBallsToThrow;
    private ArmSpeed armSpeed;
    private readonly float throwForceMultiplier = 10;
    private readonly float maxThrowForce = 1000;
    private readonly float spawnDelay = 0.25f;
    public GameObject currentBall;
    private HandController handController;
    private PlayerManager.PlayerNumber playerNumber;

    // Start is called before the first frame update
    void Start() {
        handController = GetComponentInParent<HandController>();
        playerNumber = GetComponentInParent<Player>().playerNumber;
        parentOfBallsToThrow = BallManager.Instance.playerBallQueues[(int)playerNumber];
        armSpeed = GetComponentInParent<ArmSpeed>();
    }

    private void ThrowCurrentBall(Vector3 force) {
        if (force.magnitude > maxThrowForce) {
            force *= (maxThrowForce / force.magnitude);
        }
        currentBall.GetComponent<Ball>().transformToFollow = null;
        currentBall.GetComponent<Rigidbody>().isKinematic = false;
        currentBall.GetComponent<Rigidbody>().AddForce(force);
        currentBall.transform.parent = BallManager.Instance.activeBalls;
        currentBall.GetComponent<Collider>().enabled = true;
        currentBall = null;
    }

    // Depends only on instantaneous velocity.
    public void ThrowCurrentBallWithoutAcceleration() {
        ThrowCurrentBall(armSpeed.velocity * throwForceMultiplier);
    }

    public void ThrowCurrentBallWithAcceleration(Vector3 acc) {
        ThrowCurrentBall(acc * throwForceMultiplier);
    }

    // Makes currentBall follow this hand
    private void SetCurrentBallToFollow() {
        currentBall.GetComponent<Ball>().transformToFollow = transform;
    }

    // Takes the next ball out of the queue and into the hand
    private void PutNextBallInHand() {
        if (!parentOfBallsToThrow.GetChild(0).gameObject.activeInHierarchy) {
            currentBall = parentOfBallsToThrow.GetChild(0).gameObject;
        } else {    // if first in list is already held by other hand, get next in list
            currentBall = parentOfBallsToThrow.GetChild(1).gameObject;
        }
        currentBall.GetComponent<Collider>().enabled = false;
        currentBall.GetComponent<Rigidbody>().isKinematic = true;
        SetCurrentBallToFollow();
        currentBall.SetActive(true);
    }

    public void RestartState() {
        if (currentBall != null) {
            UnspawnBall();
        }
    }

    public void UnspawnBall() {
        BallManager.Instance.PutBallInQueue((int)playerNumber, currentBall);
        currentBall.GetComponent<Ball>().transformToFollow = null;
        currentBall = null;
    }

    public IEnumerator TrySpawn() {
        yield return new WaitForSeconds(spawnDelay);
        if (gameObject.activeInHierarchy == false) {
            yield break;
        }

        if (parentOfBallsToThrow.childCount > 1
            || (parentOfBallsToThrow.childCount == 1 && !parentOfBallsToThrow.GetChild(0).gameObject.activeInHierarchy)) {
            PutNextBallInHand();
        } else if (currentBall == null) {
            FinishThrowing();
        }
    }

    // To be called when all balls are tossed
    private void FinishThrowing() {
        handController.SwitchToTool();
    }
}
