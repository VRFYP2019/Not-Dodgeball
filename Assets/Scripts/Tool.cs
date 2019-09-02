using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Tool : MonoBehaviour {
    HapticFeedback hapticFeedback;
    SteamVR_Behaviour_Pose hand;
    public float forceMultiplier = 5;

    // Start is called before the first frame update
    void Start() {
        hapticFeedback = GetComponentInParent<HapticFeedback>();
        hand = GetComponentInParent<SteamVR_Behaviour_Pose>();
    }

    private void OnCollisionEnter(Collision collision) {
        hapticFeedback.Vibrate(0.1f, 100, 15);
        ExertForce(collision.collider.GetComponent<Rigidbody>());
    }

    // Exert a force on the other rigidbody
    private void ExertForce(Rigidbody r) {
        // TODO: Improve accuracy of physics and possibly add spin
        r.AddForce(hand.GetVelocity() * forceMultiplier, ForceMode.Impulse);
    }
}
