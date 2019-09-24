using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ToolHuman : Tool {
    private HapticFeedback hapticFeedback;
    private SteamVR_Behaviour_Pose hand;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        hapticFeedback = GetComponentInParent<HapticFeedback>();
        hand = GetComponentInParent<SteamVR_Behaviour_Pose>();
    }

    protected override void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision);
        hapticFeedback.Vibrate(0.1f, 100, 15);
    }

    protected override void OnTriggerEnter(Collider other) {
        base.OnTriggerEnter(other);
        if (fadeState > FadeState.FADED) {
            hapticFeedback.Vibrate(0.1f, 100, 30);
        }
    }

    // Override the ExertForce in Tool.cs
    protected override void ExertForce(Rigidbody r) {
        // TODO: Improve accuracy of physics and possibly add spin
        hand.GetVelocitiesAtTimeOffset(-0.1f, out Vector3 oldVelocity, out Vector3 a);
        r.AddForce((hand.GetVelocity() - oldVelocity) * (1 + hand.GetAngularVelocity().magnitude * angularVelocityMultiplier) / 0.1f * forceMultiplier, ForceMode.Impulse);
    }
}
