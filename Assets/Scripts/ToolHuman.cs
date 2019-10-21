using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ToolHuman : Tool {
    private HapticFeedback hapticFeedback;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        if (!GameManager.Instance.isEditor) {
            hapticFeedback = GetComponentInParent<HapticFeedback>();
        }
    }

    public void TriggerHapticFeedback(float duration, float frequency, float amplitude) {
        if (!GameManager.Instance.isEditor) {
            hapticFeedback.Vibrate(duration, frequency, amplitude);
        }
    }

    protected override void SpawnToolFollower() {
        base.SpawnToolFollower();
        follower.SetHumanity(true);
    }
}
