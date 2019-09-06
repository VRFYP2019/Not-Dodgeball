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
    public float throwingForce = 100;
    public float spawnDelay = 0.25f;
    public GameObject currentBall;
    HandController handController;
    Person person;
    SteamVR_Behaviour_Pose handPose;

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

    void ThrowCurrentBall() {
        currentBall.GetComponent<Rigidbody>().isKinematic = false;
        currentBall.GetComponent<Rigidbody>().AddForce(handPose.GetVelocity() * throwingForce);
        currentBall.transform.parent = BallManager.Instance.transform;
        currentBall.GetComponent<Collider>().enabled = true;
        currentBall = null;
    }

    // Makes currentBall follow this hand
    void SetCurrentBallToFollow() {
        currentBall.transform.parent = this.transform;
        currentBall.transform.position = this.transform.position;
    }

    // Takes the next ball out of the queue and into the hand
    void DequeueNextBall() {
        currentBall = ballsToThrow.Dequeue();
        currentBall.GetComponent<Collider>().enabled = false;
        currentBall.GetComponent<Rigidbody>().isKinematic = true;
        SetCurrentBallToFollow();
        currentBall.SetActive(true);
    }

    // Inherits a ball. Used when switching spawning hand while in spawning mode
    public void InheritBall(GameObject ball) {
        currentBall = ball;
        SetCurrentBallToFollow();
    }

    public IEnumerator TrySpawn() {
        yield return new WaitForSeconds(spawnDelay);
        if (ballsToThrow.Count > 0) {
            DequeueNextBall();
        } else if (currentBall == null) {
            FinishThrowing();
        }
    }

    // To be called when all balls are tossed
    void FinishThrowing() {
        handController.SwitchToTool();
        person.IsSpawning = false;
    }
}
