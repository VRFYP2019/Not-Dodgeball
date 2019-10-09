﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the goalpost position and keeps track of score for that player
// Goalpost MUST be a child of the VR camera
// Goalpost cannot move outside the bounds of the room
public class Goal : MonoBehaviour {
    private static readonly float X_OFFSET = 0f, Y_OFFSET = 0f, Z_OFFSET_PLAYER_ONE = -1.25f, Z_OFFSET_PLAYER_TWO = 1.25f;
    private static readonly float X_MIN = -2f, X_MAX = 2f, Y_MIN = 0.5f, Y_MAX = 3.5f, Z_MIN = -8f, Z_MAX =2f;
    private static readonly float PLAYER_1_ROTATION = 180f, PLAYER_2_ROTATION = 0;
    
    private Vector3 parentPos, newPos, lastSafePos;
    private int playerScore;
    private float yRotation;
    private float zOffset;
    private GoalState goalState;
    enum GoalState {
        FOLLOWING,
        STATIONARY
    }

    private Utils.PlayerNumber playerNumber;

    // Start is called before the first frame update
    void Start() {
        if (GetComponentInParent<Player>() != null) {
            playerNumber = GetComponentInParent<Player>().playerNumber;
        } else {
            playerNumber = Utils.PlayerNumber.TWO;
        }
        ResetGoal();
        GameManager.Instance.RestartEvent.AddListener(ResetGoal);
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

    public void ResetGoal() {
        InitRotationAndOffset();
        goalState = GoalState.FOLLOWING;
    }

    // Update is called once per frame
    void Update() {
        HandleGoalPosition();
    }
    
    private void HandleGoalPosition() {
        if (goalState == GoalState.FOLLOWING) {
            parentPos = transform.parent.position;
            // Prevent goal from exceeding room bounds
            newPos.x = Mathf.Clamp(Mathf.Lerp(lastSafePos.x, parentPos.x + X_OFFSET, Time.deltaTime), X_MIN, X_MAX);
            newPos.y = Mathf.Clamp(Mathf.Lerp(lastSafePos.y, parentPos.y + Y_OFFSET, Time.deltaTime), Y_MIN, Y_MAX);
            newPos.z = Mathf.Clamp(Mathf.Lerp(lastSafePos.z, parentPos.z + zOffset, Time.deltaTime), Z_MIN, Z_MAX);
            lastSafePos = newPos;
            UpdateGoalPosition(newPos);
        } else if (goalState == GoalState.STATIONARY) {
            UpdateGoalPosition(lastSafePos);
        }
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
            SwitchGoalState();
        }
    }

    private void SwitchGoalState() {
        goalState = (goalState == GoalState.FOLLOWING) ? GoalState.STATIONARY : GoalState.FOLLOWING;
    }
}
