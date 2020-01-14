using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHuman : Tool {
    private HandControllerHuman handControllerHuman;

    private void Awake() {
        handControllerHuman = GetComponentInParent<HandControllerHuman>();
    }

    public void TriggerHapticFeedback(float duration, float frequency, float amplitude) {
        handControllerHuman.TriggerHapticFeedback(duration, frequency, amplitude);
    }

    protected override void SpawnToolFollower() {
        base.SpawnToolFollower();
        follower.SetHumanity(true);
    }
}
