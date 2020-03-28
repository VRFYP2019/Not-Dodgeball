using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class HeadMoveDistance : MonoBehaviourPunCallbacks {

    private bool isRecording;
    private Vector3 prevHeadPos;
    private float totalDistMoved;

    // Start is called before the first frame update
    void Start() {
        GameManager.Instance.TimeOverEvent.AddListener(StopRecordingDistMoved);
        GameManager.Instance.RestartEvent.AddListener(ResetRecordingDistMoved);
        ResetRecordingDistMoved();
    }

    // Update is called once per frame
    void Update() {
        if (isRecording) {
            totalDistMoved += Vector3.Distance(prevHeadPos, transform.position);
            prevHeadPos = transform.position;
        }   
    }

    private void ResetRecordingDistMoved() {
        totalDistMoved = 0.0f;
        prevHeadPos = transform.position;
        isRecording = true;
    }

    private void StopRecordingDistMoved() {
        isRecording = false;
        Debug.Log("dist moved so far: " + totalDistMoved);
        PlaytestRecording.RecordDistHeadMoved(totalDistMoved);
    }
}
