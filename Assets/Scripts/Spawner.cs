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
    private Animator anim;
    private bool hasBeenInit = false;

    private void Awake() {
        pv = GetComponent<PhotonView>();
        handController = GetComponentInParent<HandController>();
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start() {
        if (!hasBeenInit) {
            Init();
        }
    }

    private void Init() {
        parentOfBallsToThrow = BallManager.LocalInstance.playerBallQueue;
        hasBeenInit = true;
    }

    public void ThrowCurrentBall() {
        anim.SetTrigger("throw");
        currentBall.OnDetachFromHand();
        currentBall = null;
        PlaytestRecording.RecordThrow();
        // Have a slight delay so that the ball does not immediately hit the tool
        StartCoroutine(DelayAndSwitchToTool(spawnDelay));
    }
    
    // Takes the next ball out of the queue and into the hand
    public void PutNextBallInHand() {
        if (!hasBeenInit) {
            Init();
        }
        anim.SetTrigger("reset");
        if (!parentOfBallsToThrow.GetChild(0).gameObject.activeInHierarchy) {
            currentBall = parentOfBallsToThrow.GetChild(0).GetComponent<Ball>();
        } else {    // if first in list is already held by other hand, get next in list
            currentBall = parentOfBallsToThrow.GetChild(1).GetComponent<Ball>();
        }
        currentBall.SetPlayerNumber(GetComponentInParent<Player>().playerNumber);
        currentBall.OnAttachToHand(transform);
    }

    public void RestartState() {
        if (currentBall != null) {
            UnspawnBall();
        }
    }

    private void UnspawnBall() {
        BallManager.LocalInstance.PutBallInQueue(currentBall);
        currentBall.SetTransformToFollowToNull();
        currentBall = null;
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

    private IEnumerator DelayAndSwitchToTool(float delay) {
        yield return new WaitForSeconds(delay);
        handController.SwitchToTool();
    }
}
