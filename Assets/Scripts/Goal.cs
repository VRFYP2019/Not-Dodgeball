using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the goalpost position and keeps track of score for that player
// Goalpost MUST be a child of the VR camera
// Goalpost is to follow player's position and Y-axis rotation (Yaw)
// Goalpost cannot move outside the bounds of the room
public class Goal : MonoBehaviour {
    private static readonly float X_OFFSET = 0f, Y_OFFSET = 0f, Z_OFFSET = -1.25f;
    private static readonly float X_MIN = -2f, X_MAX = 2f, Y_MIN = 0.5f, Y_MAX = 3.5f, Z_MIN = -8f, Z_MAX =2f;
    private static readonly float BLUE_Y_ROTATION = 0f, ORANGE_Y_ROTATION = 180f;
    
    private Vector3 parentPos, newPos;
    private int playerScore;
    private float yRotation;

    // TODO: Change how player color is set on init for multiplayer support
    // Temp solution, we could check for the parent.name or other indicators of player color in future
    private enum PlayerColor {
        BLUE,
        ORANGE
    }

    [SerializeField]
    private PlayerColor playerColor = PlayerColor.ORANGE;

    // Start is called before the first frame update
    void Start() {
        ResetPlayerScore();
        InitRotation();
    }

    private void InitRotation() {
        if (playerColor == PlayerColor.BLUE) {
            yRotation = BLUE_Y_ROTATION;
        } else {
            yRotation = ORANGE_Y_ROTATION;
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
        newPos.z = Mathf.Clamp(parentPos.z + Z_OFFSET, Z_MIN, Z_MAX);
        UpdateGoalPosition(newPos);
    }

    private void UpdateGoalPosition(Vector3 pos) {
        transform.position = pos;
        transform.eulerAngles = new Vector3 (0, yRotation, 0);
    }

    void OnTriggerEnter(Collider col) {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ball")) {
            AddPlayerScore(1);
            BallManager.Instance.PutBallInPool(col.gameObject);
        }
    }

    public void ResetPlayerScore() {
        playerScore = 0;
    }

    public void AddPlayerScore(int points) {
        playerScore += points;
    }

    public int GetPlayerScore() {
        return playerScore;
    }
}
