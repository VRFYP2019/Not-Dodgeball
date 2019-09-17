using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// Hand for spawning balls. Set to active only when there is a ball in the queue for this player and player
// wants to spawn. Otherwise, a Tool should be active on the player's hand instead.
public class SpawnerHand : MonoBehaviour {
    private Transform parentOfBallsToThrow;
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
        parentOfBallsToThrow = BallManager.Instance.playerBallQueues[0];
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
        currentBall.GetComponent<Ball>().transformToFollow = null;
        currentBall.GetComponent<Rigidbody>().isKinematic = false;
        currentBall.GetComponent<Rigidbody>().AddForce(handPose.GetVelocity() * throwingForce);
        currentBall.transform.parent = BallManager.Instance.activeBalls;
        currentBall.GetComponent<Collider>().enabled = true;
        currentBall = null;
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
        BallManager.Instance.PutBallInQueue(0, currentBall);
        currentBall.GetComponent<Ball>().transformToFollow = null;
        currentBall = null;
    }

    public IEnumerator TrySpawn() {
        yield return new WaitForSeconds(spawnDelay);
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
