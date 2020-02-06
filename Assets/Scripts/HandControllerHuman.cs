using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class HandControllerHuman : HandController {
    private HapticFeedback hapticFeedback;
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        if (GameManager.Instance.playerPlatform == PlayerPlatform.OCULUS
            && handSide == HandSide.UNSPECIFIED) {
            Debug.LogWarning("Hand side not selected.");
        }
        if (!(GameManager.Instance.playerPlatform == PlayerPlatform.EDITOR)) {
            hapticFeedback = GetComponent<HapticFeedback>();
        }
    }

    public void TriggerHapticFeedback(float duration, float frequency, float amplitude) {
        if (!(GameManager.Instance.playerPlatform == PlayerPlatform.EDITOR)) {
            hapticFeedback.Vibrate(duration, frequency, amplitude);
        }
    }
}
