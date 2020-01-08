using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using Valve.VR;

public class HapticFeedback : MonoBehaviour {
    public SteamVR_Action_Vibration vibration;
    private SteamVR_Input_Sources trackedObj;
    private OVRInput.Controller controllerMask;
    
    private void Start() {
        if (GameManager.Instance.playerPlatform == PlayerPlatform.STEAMVR) {
            trackedObj = GetComponentInParent<SteamVR_Behaviour_Pose>().inputSource;
        }
        HandSide handSide = GetComponent<HandControllerHuman>().handSide;
        if (handSide == HandSide.LEFT) {
            controllerMask = OVRInput.Controller.LTouch;
        } else if (handSide == HandSide.RIGHT) {
            controllerMask = OVRInput.Controller.RTouch;
        }
    }

    public void Vibrate(float duration, float frequency, float amplitude) {
        if (GameManager.Instance.playerPlatform == PlayerPlatform.OCULUS) {
            // Hardcode to frequency and amplitude both 1 because Oculus haptics are weaker
            OVRInput.SetControllerVibration(1, 1, controllerMask);
            StartCoroutine(StopVibrationAfter(duration));
        } else {
            vibration.Execute(0, duration, frequency, amplitude, trackedObj);
        }
    }

    private IEnumerator StopVibrationAfter(float duration) {
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, controllerMask);
    }
}
