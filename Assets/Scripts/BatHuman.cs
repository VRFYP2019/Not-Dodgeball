using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatHuman : Bat {
    private HandControllerHuman handControllerHuman;

    protected override void Awake() {
        base.Awake();
        handControllerHuman = GetComponentInParent<HandControllerHuman>();
    }

    public void TriggerHapticFeedback(float duration, float frequency, float amplitude) {
        handControllerHuman.TriggerHapticFeedback(duration, frequency, amplitude);
    }

    [PunRPC]
    protected override void Bat_SetAlpha(float a) {
        foreach (Material m in materials) {
            m.color = new Color(m.color.r, m.color.g, m.color.b, a);
        }
    }
}
