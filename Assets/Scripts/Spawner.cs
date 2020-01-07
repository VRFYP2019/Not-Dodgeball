using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hand for spawning balls. Set to active only when there is a ball in the queue for this player and player
// wants to spawn. Otherwise, a Tool should be active on the player's hand instead.
public class Spawner : MonoBehaviour {
    private Transform parentOfBallsToThrow;
    private readonly float spawnDelay = 0.25f;
    public Ball currentBall;
    private HandController handController;
    private PhotonView pv;

    private void Awake() {
        pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start() {
        handController = GetComponentInParent<HandController>();
        parentOfBallsToThrow = BallManager.LocalInstance.playerBallQueue;
    }
   
    public void ThrowCurrentBall() {
        currentBall.OnDetachFromHand();
        currentBall = null;
    }

    // Makes currentBall follow this hand
    private void SetCurrentBallToFollow() {
        currentBall.OnAttachToHand(transform);
    }
    
    // Takes the next ball out of the queue and into the hand
    private void PutNextBallInHand() {
        if (!parentOfBallsToThrow.GetChild(0).gameObject.activeInHierarchy) {
            currentBall = parentOfBallsToThrow.GetChild(0).GetComponent<Ball>();
        } else {    // if first in list is already held by other hand, get next in list
            currentBall = parentOfBallsToThrow.GetChild(1).GetComponent<Ball>();
        }
        currentBall.GetComponent<Collider>().enabled = false;
        currentBall.GetComponent<Rigidbody>().isKinematic = true;
        SetCurrentBallToFollow();
        currentBall.SetPlayerNumber(GetComponentInParent<Player>().playerNumber);
    }

    public void RestartState() {
        if (currentBall != null) {
            UnspawnBall();
        }
    }

    public void UnspawnBall() {
        BallManager.LocalInstance.PutBallInQueue(currentBall);
        currentBall.SetTransformToFollowToNull();
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
            yield return new WaitForEndOfFrame();
            currentBall.SetState(true);
        } else if (currentBall == null) {
            FinishThrowing();
        }
    }

    // To be called when all balls are tossed
    private void FinishThrowing() {
        handController.SwitchToTool();
    }

    [PunRPC]
    private void Spawner_SetState(bool active) {
        gameObject.SetActive(active);
    }

    public void SetState(bool active) {
        if (PhotonNetwork.IsConnected) {
            pv.RPC("Spawner_SetState", RpcTarget.AllBuffered, active);
        } else {
            Spawner_SetState(active);
        }
    }
}
