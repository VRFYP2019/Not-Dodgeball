using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// To be attached on Spawner objects for human players, to control the use of them
public class SpawnerHumanController : MonoBehaviour {
    private SteamVR_Action_Boolean click = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");
    private Spawner spawner;
    private SteamVR_Behaviour_Pose handPose;
    enum Hand {
        LEFT,
        RIGHT
    }
    [SerializeField]
    private Hand hand;

    // Start is called before the first frame update
    void Start() {
        if (!GameManager.Instance.isOculusQuest && !GameManager.Instance.isEditor) {
            handPose = GetComponentInParent<SteamVR_Behaviour_Pose>();
        }
        spawner = GetComponent<Spawner>();
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.Instance.isOculusQuest) {
            if (hand == Hand.LEFT && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) {
                spawner.ThrowCurrentBall();
                StartCoroutine(spawner.TrySpawn());
            }
            if (hand == Hand.RIGHT && OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)) {
                spawner.ThrowCurrentBall();
                StartCoroutine(spawner.TrySpawn());
            }
        } else if (GameManager.Instance.isEditor) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                spawner.ThrowCurrentBall();
                StartCoroutine(spawner.TrySpawn());
            }
        } else if (click.GetStateDown(handPose.inputSource)) {
            // currentBall would be null if the player tries to throw before the delay from the previous throw is over
            if (spawner.currentBall != null) {
                spawner.ThrowCurrentBall();
                StartCoroutine(spawner.TrySpawn());
            }
        }
    }
}
