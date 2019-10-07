using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HapticFeedback : MonoBehaviour {
    public SteamVR_Action_Vibration vibration;
    private SteamVR_Input_Sources trackedObj;
    [SerializeField]
    OVRInput.Controller controllerMask;
    OVRHaptics.OVRHapticsChannel channel;
    OVRHapticsClip clip;
    
    private void Awake() {
        if (GameManager.Instance.isOculusQuest) {
            InitializeOVRHaptics();
        } else {
            trackedObj = GetComponent<SteamVR_Behaviour_Pose>().inputSource;
        }
    }

    private void InitializeOVRHaptics() {
        int cnt = 100;
        clip = new OVRHapticsClip(cnt);
        for (int i = 0; i < cnt; i++) {
            clip.Samples[i] = i % 2 == 0 ? (byte)0 : (byte)255;
        }

        clip = new OVRHapticsClip(clip.Samples, clip.Samples.Length);
        channel = controllerMask == OVRInput.Controller.LTouch ? OVRHaptics.LeftChannel : OVRHaptics.RightChannel;
    }

    public void Vibrate(float duration, float frequency, float amplitude) {
        if (!GameManager.Instance.isOculusQuest) {
            vibration.Execute(0, duration, frequency, amplitude, trackedObj);
        } else {
            channel.Preempt(clip);
        }
    }
}
