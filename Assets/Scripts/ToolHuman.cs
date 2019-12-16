using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Valve.VR;

public class ToolHuman : Tool {
    private HapticFeedback hapticFeedback;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        if (!(GameManager.Instance.playerPlatform == PlayerPlatform.EDITOR)) {
            hapticFeedback = GetComponentInParent<HapticFeedback>();
        }
    }

    public void TriggerHapticFeedback(float duration, float frequency, float amplitude) {
        if (!(GameManager.Instance.playerPlatform == PlayerPlatform.EDITOR)) {
            hapticFeedback.Vibrate(duration, frequency, amplitude);
        }
    }

    protected override void SpawnToolFollower() {
        base.SpawnToolFollower();
        follower.SetHumanity(true);
    }
}
