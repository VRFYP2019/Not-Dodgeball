using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// Used for retrieving velocity of arms.
// Can be called by human and bot, will be handled separately
public class ArmSpeed : MonoBehaviour {
    public Vector3 velocity = Vector3.zero;
    private SteamVR_Behaviour_Pose hand;
    private Vector3 lastPos;

    private void Start() {
        if (GetComponentInParent<Player>().playerType == Utils.PlayerType.HUMAN) {
            hand = GetComponent<SteamVR_Behaviour_Pose>();
            StartCoroutine(UpdateVelocityHuman());
        } else {
            lastPos = transform.position;
            StartCoroutine(UpdateVelocityBot());
        }
    }

    IEnumerator UpdateVelocityHuman() {
        while (true) {
            yield return new WaitForFixedUpdate();
            velocity = hand.GetVelocity();
        }
    }

    IEnumerator UpdateVelocityBot() {
        while (true) {
            yield return new WaitForFixedUpdate();
            velocity = (transform.position - lastPos) / Time.deltaTime;
            lastPos = transform.position;
        }
    }
}
