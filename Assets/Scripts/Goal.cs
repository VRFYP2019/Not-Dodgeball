using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the goalpost position and keeps track of score for that player
// Goalpost MUST be a child of the VR camera
// Goalpost is to follow player's position and Y-axis rotation (Yaw)
// Goalpost cannot move outside the bounds of the room
public class Goal : MonoBehaviour {
    private static readonly float X_OFFSET = 0f, Y_OFFSET = 0f, Z_OFFSET_PLAYER_ONE = -1.25f, Z_OFFSET_PLAYER_TWO = 1.25f;
    private static readonly float X_MIN = -2f, X_MAX = 2f, Y_MIN = 0.5f, Y_MAX = 3.5f, Z_MIN = -8f, Z_MAX =2f;
    private static readonly float PLAYER_1_ROTATION = 180f, PLAYER_2_ROTATION = 0;
    
    private Vector3 parentPos, newPos;
    private int playerScore;
    private float yRotation;
    private float zOffset;

    private Utils.PlayerNumber playerNumber;

    // Start is called before the first frame update
    void Start() {
        if (GetComponentInParent<Player>() != null) {
            playerNumber = GetComponentInParent<Player>().playerNumber;
        } else {    // probably a dummy goal
            playerNumber = Utils.PlayerNumber.TWO;
        }
        InitRotationAndOffset();
    }

    private void InitRotationAndOffset() {
        if (playerNumber == Utils.PlayerNumber.ONE) {
            yRotation = PLAYER_1_ROTATION;
            zOffset = Z_OFFSET_PLAYER_ONE;
        } else {
            zOffset = Z_OFFSET_PLAYER_TWO;
            yRotation = PLAYER_2_ROTATION;
        }
    }

    // Update is called once per frame
    void Update() {
        HandleGoalPosition();
    }
    
    private void HandleGoalPosition() {
        parentPos = transform.parent.position;
        // Prevent goal from exceeding room bounds
        newPos.x = Mathf.Clamp(parentPos.x + X_OFFSET, X_MIN, X_MAX);
        newPos.y = Mathf.Clamp(parentPos.y + Y_OFFSET, Y_MIN, Y_MAX);
        newPos.z = Mathf.Clamp(parentPos.z + zOffset, Z_MIN, Z_MAX);
        UpdateGoalPosition(newPos);
    }

    private void UpdateGoalPosition(Vector3 pos) {
        //transform.localPosition = pos;
        transform.position = pos;
        transform.eulerAngles = new Vector3 (0, yRotation, 0);
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ball")) {
            ScoreManager.Instance.AddScoreToOpponent(playerNumber, 1);
            BallManager.LocalInstance.PutBallInPool(col.gameObject);
        }
    }
}
