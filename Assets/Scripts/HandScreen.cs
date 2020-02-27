using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandScreen : MonoBehaviour {
    private Transform trans;
    [SerializeField]
    private Text
        ballText = null,
        heartText = null;
    private Camera cam = null;

    // Start is called before the first frame update
    void Start() {
        Camera[] cams = FindObjectsOfType<Camera>();
        foreach (Camera c in cams) {
            PhotonView pv = c.GetComponentInParent<PhotonView>();
            if (pv == null || !c.isActiveAndEnabled) {
                continue;
            } else {
                cam = c;
                break;
            }
        }
        if (cam == null) {
            Debug.LogWarning("Camera failed to assign in arm screen");
            enabled = false;
        }
        trans = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update() {
        trans.LookAt(cam.transform);
        trans.localEulerAngles = new Vector3(0, 0, trans.localEulerAngles.z);
        ballText.text = BallManager.LocalInstance.playerBallQueue.childCount.ToString();
        if (MovesenseSubscriber.instance != null && MovesenseSubscriber.instance.heartRate != null) {
            heartText.text = MovesenseSubscriber.instance.heartRate.ToString();
        } else {
            heartText.text = "--";
        }
    }
}
