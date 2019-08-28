using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour {
    HapticFeedback hapticFeedback;

    // Start is called before the first frame update
    void Start() {
        hapticFeedback = GetComponentInParent<HapticFeedback>();
    }

    private void OnCollisionEnter(Collision collision) {
        hapticFeedback.Vibrate(0.1f, 100, 15);
    }
}
