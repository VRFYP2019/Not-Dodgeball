using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// To be attached on Spawner objects for human players, to control the use of them
public class SpawnerHumanController : MonoBehaviour {
    private SteamVR_Action_Boolean click = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    private Spawner spawner;
    private SteamVR_Behaviour_Pose handPose;
    private Vector3 acceleration = Vector3.zero;
    private Vector3 prevVelocity = Vector3.zero;
    private float prevTime = 0;

    // Start is called before the first frame update
    void Start() {
        handPose = GetComponentInParent<SteamVR_Behaviour_Pose>();
        spawner = GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update() {
        if (click.GetStateDown(handPose.inputSource)) {
            // currentBall would be null if the player tries to throw before the delay from the previous throw is over
            if (spawner.currentBall != null) {
                spawner.ThrowCurrentBallWithAcceleration(acceleration);
                StartCoroutine(spawner.TrySpawn());
            }
        }
    }

    private void FixedUpdate() {
        float currTime = Time.time;
        acceleration = ((handPose.GetVelocity() - prevVelocity) / (currTime - prevTime)).magnitude * Vector3.Normalize(handPose.GetVelocity() + prevVelocity - handPose.transform.up);
        prevVelocity = handPose.GetVelocity();
        prevTime = currTime;
    }
}
