using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Tool : MonoBehaviour {
    HapticFeedback hapticFeedback;
    SteamVR_Behaviour_Pose hand;
    Collider thisCollider;  // calling it "collider" triggers a useless warning so it'll be called "thisCollider" instead
    public float forceMultiplier = 5;
    readonly float velocityUpperThreshold = 1;  // anything above this value will be considered "fast" and treated differently to avoid clipping

    // Start is called before the first frame update
    void Start() {
        hapticFeedback = GetComponentInParent<HapticFeedback>();
        hand = GetComponentInParent<SteamVR_Behaviour_Pose>();
        thisCollider = GetComponent<Collider>();
    }

    private void Update() {
        float handVelocityMagnitude = hand.GetVelocity().magnitude;
        if (handVelocityMagnitude < velocityUpperThreshold) {
            thisCollider.isTrigger = false;
        } else {
            thisCollider.isTrigger = true;
        }
    }

    // Used for slow movement
    private void OnCollisionEnter(Collision collision) {
        hapticFeedback.Vibrate(0.1f, 100, 15);
        ExertForce(collision.collider.GetComponent<Rigidbody>());
    }

    // Used for fast movement
    private void OnTriggerEnter(Collider other) {
        hapticFeedback.Vibrate(0.1f, 100, 30);
        // TODO: account for bounce using normal of supposed collision point
        ExertForce(other.GetComponent<Rigidbody>());
    }

    // Exert a force on the other rigidbody
    private void ExertForce(Rigidbody r) {
        // TODO: Improve accuracy of physics and possibly add spin
        r.AddForce(hand.GetVelocity() * forceMultiplier, ForceMode.Impulse);
    }
}
