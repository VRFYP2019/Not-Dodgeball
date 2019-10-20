using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ToolHuman : Tool {
    private HapticFeedback hapticFeedback;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        if (!Application.isEditor) {
            hapticFeedback = GetComponentInParent<HapticFeedback>();
        }
    }

    public void TriggerHapticFeedback(float duration, float frequency, float amplitude) {
        if (!Application.isEditor) {
            hapticFeedback.Vibrate(duration, frequency, amplitude);
        }
    }

    protected override void SpawnToolFollower() {
        base.SpawnToolFollower();
        follower.SetHumanity(true);
    }
}
