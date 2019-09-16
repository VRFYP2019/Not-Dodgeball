using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// Hand for spawning balls. Set to active only when there is a ball in the queue for this player. Otherwise, a Tool should
// be active on the player's hand instead.
public class SpawnerHand : MonoBehaviour {
    // A queue of balls that this player should throw. The balls are to be added as children to this GameObject but inactive.
    private Queue<GameObject> ballsToThrow;
    private SteamVR_Action_Boolean click = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    private readonly float throwingForce = 250;
    private readonly float spawnDelay = 0.25f;
    public GameObject currentBall;
    private HandController handController;
    private Person person;
    private SteamVR_Behaviour_Pose handPose;

    // Start is called before the first frame update
    void Start() {
        handPose = GetComponentInParent<SteamVR_Behaviour_Pose>();
        handController = handPose.GetComponent<HandController>();
        person = GetComponentInParent<Person>();
        ballsToThrow = BallManager.Instance.PlayerBallQueues[0];
    }

    // Update is called once per frame
    void Update() {
        if (click.GetStateDown(handPose.inputSource)) {
            // currentBall would be null if the player tries to throw before the delay from the previous throw is over
            if (currentBall != null) {
                ThrowCurrentBall();
                StartCoroutine(TrySpawn());
            }
        }
    }

    private void ThrowCurrentBall() {
        currentBall.GetComponent<Rigidbody>().isKinematic = false;
        currentBall.GetComponent<Rigidbody>().AddForce(handPose.GetVelocity() * throwingForce);
        currentBall.transform.parent = BallManager.Instance.activeBalls;
        currentBall.GetComponent<Collider>().enabled = true;
        currentBall = null;
    }

    // Makes currentBall follow this hand
    private void SetCurrentBallToFollow() {
        currentBall.transform.parent = this.transform;
        currentBall.transform.position = this.transform.position;
    }

    // Takes the next ball out of the queue and into the hand
    private void DequeueNextBall() {
        currentBall = ballsToThrow.Dequeue();
        BallManager.Instance.DecrementPoolPointer();
        currentBall.GetComponent<Collider>().enabled = false;
        currentBall.GetComponent<Rigidbody>().isKinematic = true;
        SetCurrentBallToFollow();
        currentBall.SetActive(true);
    }

    public void RestartState() {
        if (currentBall != null) {
            UnspawnBall();
        }
        ballsToThrow = null;
    }

    public void UnspawnBall() {
        BallManager.Instance.PutBallInPool(currentBall);
        currentBall = null;
    }

    public void PutBallBackInQueue() {
        ballsToThrow.Enqueue(currentBall);
        BallManager.Instance.PutBallInPool(currentBall);
        BallManager.Instance.IncrementPoolPointer();
        currentBall = null;
    }

    public IEnumerator TrySpawn() {
        yield return new WaitForSeconds(spawnDelay);
        if (ballsToThrow == null) { // dereferenced due to restart
            ballsToThrow = BallManager.Instance.PlayerBallQueues[0];
        }
 
        if (ballsToThrow.Count > 0) {
            DequeueNextBall();
        } else if (currentBall == null) {
            FinishThrowing();
        }
    }

    // To be called when all balls are tossed
    private void FinishThrowing() {
        handController.SwitchToTool();
    }
}
