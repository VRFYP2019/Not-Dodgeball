using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// Hand for spawning balls. Set to active only when there is a ball in the queue for this player. Otherwise, a Tool should
// be active on the player's hand instead.
public class SpawnerHand : MonoBehaviour {
    // A queue of balls that this player should throw. The balls are to be added as children to this GameObject but inactive.
    public Queue<GameObject> ballsToThrow;
    public SteamVR_Action_Boolean click = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    GameObject currentBall;
    HandController handController;
    SteamVR_Behaviour_Pose handPose;

    // Start is called before the first frame update
    void Start() {
        handPose = GetComponentInParent<SteamVR_Behaviour_Pose>();
        handController = handPose.GetComponent<HandController>();
        ballsToThrow = BallManager.Instance.PlayerBallQueues[0];
    }

    // Update is called once per frame
    void Update() {
        if (currentBall) {
            // TODO: set offset or just set ball as child of this
            currentBall.transform.position = this.transform.position;
        }

        if (click.GetStateDown(handPose.inputSource)) {
            ThrowCurrentBall();
            if (ballsToThrow.Count < 1) {
                FinishThrowing();
            } else {
                DequeueNextBall();
            }
        }
    }

    void ThrowCurrentBall() {
        currentBall.GetComponent<Rigidbody>().velocity = handPose.GetVelocity();
    }

    // Takes the next ball out of the queue and into the hand
    void DequeueNextBall() {
        currentBall = ballsToThrow.Dequeue();
        currentBall.SetActive(true);
    }

    public void TrySpawn() {
        if (ballsToThrow.Count < 1) {
            FinishThrowing();
        } else {
            DequeueNextBall();
        }
    }

    // To be called when all balls are tossed
    void FinishThrowing() {
        handController.SwitchToTool();
    }
}
