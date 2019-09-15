using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HapticFeedback : MonoBehaviour {
    public SteamVR_Action_Vibration vibration;
    private SteamVR_Input_Sources trackedObj;

    // Start is called before the first frame update
    private void Start() {
        trackedObj = GetComponent<SteamVR_Behaviour_Pose>().inputSource;
    }

    public void Vibrate(float duration, float frequency, float amplitude) {
        vibration.Execute(0, duration, frequency, amplitude, trackedObj);
    }
}
