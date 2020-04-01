using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class BackWall : MonoBehaviour {
    [Tooltip("The player number of the person standing here")]
    public PlayerNumber playerNumber;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnCollisionEnter(Collision collision) {
        Collider col = collision.collider;
        if (col.gameObject.layer == LayerMask.NameToLayer("Ball")) {
            Debug.Log(col.gameObject.GetComponent<Ball>().GetPlayerNumber());
            // Prevent own goal
            if (col.gameObject.GetComponent<Ball>().GetPlayerNumber() != playerNumber) {
                BallManager.LocalInstance.PutBallInQueue(col.GetComponent<Ball>());
                PlaytestRecording.RecordMiss();
            }
        }
    }
}
