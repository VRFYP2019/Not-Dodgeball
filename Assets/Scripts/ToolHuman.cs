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

    public void TriggerHapticFeedback(float duration, float frequency, float amplitude) {
        hapticFeedback.Vibrate(duration, frequency, amplitude);
    }

    protected override void SpawnToolFollower() {
        base.SpawnToolFollower();
        follower.SetHumanity(true);
    }
}
