using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

// Manages the goalpost position and keeps track of score for that player
// Goalpost MUST be a child of the VR camera
// Goalpost cannot move outside the bounds of the room
public class Goal : MonoBehaviourPunCallbacks, IOnEventCallback {
    [SerializeField]
    private GoalType goalType;
    enum GoalType {
        REGULAR,
        HORIZONTAL_WALL,
        VERITCAL_WALL
    }
    private static readonly float
        X_OFFSET = 0f,
        Y_OFFSET = 0f,
        Z_OFFSET_PLAYER_ONE = -1.25f,
        Z_OFFSET_PLAYER_TWO = 1.25f,
        Z_P1 = -12.65f, // for wall
        Z_P2 = 3.65f; // for wall
    private static readonly float
        X_MIN = -2f,
        X_MAX = 2f,
        X_FIXED = 0.0f, // for H wall
        Y_MIN = 0.5f,
        Y_MAX = 3.5f,
        Y_FIXED = 2f, // for V wall
        Z_MIN = -12f,
        Z_MAX = 3f;
    private static readonly float
        PLAYER_1_ROTATION = 180f,
        PLAYER_2_ROTATION = 0;
    private static readonly float SNAP_THRESHOLD = 1.5f;
    private Vector3
        wallPos, // keep wallgoals on the wall
        parentPos,
        newPos,
        lastSafePos;
    private float yRotation;
    private float zOffset;
    private GoalState goalState;
    enum GoalState {
        FOLLOWING,
        TRANSITION,
        STATIONARY
    }

    private PlayerNumber playerNumber;  // the playernumber of the player this goal is following
    [SerializeField]
    private GameObject sparkPrefab = null;
    [SerializeField]
    private MeshRenderer[] goalSidesMR;
    [SerializeField]
    private Material
        followingMat = null,
        stationaryMat = null,
        hitMat = null;

    void Start() {
        if (GetComponentInParent<Player>() != null) {
            playerNumber = GetComponentInParent<Player>().playerNumber;
        } else {
            playerNumber = PlayerNumber.TWO;
        }
        ResetGoal();
        GameManager.Instance.RestartEvent.AddListener(ResetGoal);
    }

    public void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void InitRegularGoal() {
        if (playerNumber == PlayerNumber.ONE) {
            yRotation = PLAYER_1_ROTATION;
            zOffset = Z_OFFSET_PLAYER_ONE;
        } else {
            yRotation = PLAYER_2_ROTATION;
            zOffset = Z_OFFSET_PLAYER_TWO;
        }
    }

    private void InitWallGoal() {
        if (playerNumber == PlayerNumber.ONE) {
            yRotation = PLAYER_1_ROTATION;
            wallPos = new Vector3(0f, 2f, Z_P1);
        } else {
            yRotation = PLAYER_2_ROTATION;
            wallPos = new Vector3(0f, 2f, Z_P2);
        }
    }

    public void ResetGoal() {
        switch (goalType) {
            case GoalType.REGULAR:
                InitRegularGoal();
                break;
            case GoalType.VERITCAL_WALL:
            case GoalType.HORIZONTAL_WALL:
                InitWallGoal();
                break;
        }
        goalState = GoalState.FOLLOWING;
    }

    public void OnEvent(EventData photonEvent) {
        byte GoalWasScoredEvent = 1;
        byte eventCode = photonEvent.Code;
        
        if (eventCode == GoalWasScoredEvent) {
            object[] data = (object[])photonEvent.CustomData;

            PlayerNumber playerLastScored = (PlayerNumber)data[0];
            if (playerNumber == playerLastScored) {
                AudioManager.PlaySoundOnce("goalding");
                SwitchGoalState(GoalState.TRANSITION);
                if (PhotonNetwork.IsConnected) {
                    photonView.RPC("Goal_SetMaterial", RpcTarget.AllBuffered, (byte)GoalState.FOLLOWING, false);
                } else {
                    SwitchGoalMaterial(followingMat);
                }
                BallManager.LocalInstance.AddBallsToQueue(2);
            } else {
                AudioManager.PlaySoundOnce("goalbuzz");
            }
        }
    }

    // Update is called once per frame
    void Update() {
        switch (goalType) {
            case GoalType.REGULAR:
                HandleRegularGoalPosition();
                break;
            case GoalType.VERITCAL_WALL:
                HandleVerticalWallGoalPosition();
                break;
            case GoalType.HORIZONTAL_WALL:
                HandleHorizontalWallGoalPosition();
                break;
        }
    }

    private void HandleRegularGoalPosition() {
        parentPos = transform.parent.position;
        if (goalState == GoalState.FOLLOWING) {
            // Prevent goal from exceeding room bounds
            newPos.x = Mathf.Clamp(parentPos.x + X_OFFSET, X_MIN, X_MAX);
            newPos.y = Mathf.Clamp(parentPos.y + Y_OFFSET, Y_MIN, Y_MAX);
            newPos.z = Mathf.Clamp(parentPos.z + zOffset, Z_MIN, Z_MAX);
            lastSafePos = newPos;
            UpdateGoalPosition(newPos);

        } else if (goalState == GoalState.TRANSITION) {
            newPos.x = Mathf.Clamp(Mathf.Lerp(lastSafePos.x, parentPos.x + X_OFFSET, Time.deltaTime), X_MIN, X_MAX);
            newPos.y = Mathf.Clamp(Mathf.Lerp(lastSafePos.y, parentPos.y + Y_OFFSET, Time.deltaTime), Y_MIN, Y_MAX);
            newPos.z = Mathf.Clamp(Mathf.Lerp(lastSafePos.z, parentPos.z + zOffset, Time.deltaTime), Z_MIN, Z_MAX);
            lastSafePos = newPos;
            UpdateGoalPosition(newPos);

            if (CheckForSnap(GoalType.REGULAR)) {
                SwitchGoalState(GoalState.FOLLOWING);
            }

        } else if (goalState == GoalState.STATIONARY) {
            UpdateGoalPosition(lastSafePos);
        }
    }

    private void HandleVerticalWallGoalPosition() {
        parentPos = transform.parent.position;
        if (goalState == GoalState.FOLLOWING) {
            // Prevent goal from exceeding room bounds
            newPos = wallPos;
            newPos.x = Mathf.Clamp(parentPos.x, X_MIN, X_MAX);
            lastSafePos = newPos;
            UpdateGoalPosition(newPos);

        } else if (goalState == GoalState.TRANSITION) {
            newPos.x = Mathf.Clamp(Mathf.Lerp(lastSafePos.x, parentPos.x, Time.deltaTime), X_MIN, X_MAX);
            lastSafePos = newPos;
            UpdateGoalPosition(newPos);

            if (CheckForSnap(GoalType.VERITCAL_WALL)) {
                SwitchGoalState(GoalState.FOLLOWING);
            }

        } else if (goalState == GoalState.STATIONARY) {
            UpdateGoalPosition(lastSafePos);
        }
    }

    private void HandleHorizontalWallGoalPosition() {
        parentPos = transform.parent.position;
        if (goalState == GoalState.FOLLOWING) {
            // Prevent goal from exceeding room bounds
            newPos = wallPos;
            newPos.y = Mathf.Clamp(parentPos.y, Y_MIN, Y_MAX);
            lastSafePos = newPos;
            UpdateGoalPosition(newPos);

        } else if (goalState == GoalState.TRANSITION) {
            newPos.x = Mathf.Clamp(Mathf.Lerp(lastSafePos.y, parentPos.y, Time.deltaTime), Y_MIN, Y_MAX);
            lastSafePos = newPos;
            UpdateGoalPosition(newPos);

            if (CheckForSnap(GoalType.HORIZONTAL_WALL)) {
                SwitchGoalState(GoalState.FOLLOWING);
            }

        } else if (goalState == GoalState.STATIONARY) {
            UpdateGoalPosition(lastSafePos);
        }
    }
    private void UpdateGoalPosition(Vector3 pos) {
        //transform.localPosition = pos;
        transform.position = pos;
        transform.eulerAngles = new Vector3 (0, yRotation, 0);
    }

    // Returns true the goals currPos is within the threshold
    private bool CheckForSnap(GoalType type) {
        float distToPlayer = 0f;
        switch (type) {
            case GoalType.REGULAR:
                distToPlayer = Vector3.Distance(parentPos, transform.position);
                break;
            case GoalType.VERITCAL_WALL:
                distToPlayer = Mathf.Abs(parentPos.x - transform.position.x);
                break;
            case GoalType.HORIZONTAL_WALL:
                distToPlayer = Mathf.Abs(parentPos.y - transform.position.y);
                break;
        }

        if (distToPlayer <= SNAP_THRESHOLD) {
            return true;
        }
        return false;
    }

    void OnTriggerEnter(Collider col) {
        if (isActiveAndEnabled) {
            if (col.gameObject.layer == LayerMask.NameToLayer("Ball")) {
                // Prevent own goal
                if (col.gameObject.GetComponent<Ball>().GetPlayerNumber() != playerNumber) {
                    ScoreManager.Instance.AddScoreToOpponent(playerNumber, 1);
                    BallManager.LocalInstance.PutBallInPool(col.GetComponent<Ball>());
                    SwitchGoalState(GoalState.STATIONARY);

                    if (PhotonNetwork.IsConnected) {
                        photonView.RPC("Goal_SetMaterial", RpcTarget.AllBuffered, (byte)GoalState.STATIONARY, true);
                        PhotonNetwork.Instantiate(sparkPrefab.name, col.transform.position, Quaternion.identity);
                    } else {
                        StartCoroutine(ShowGoalHitFor(0.5f));
                        Instantiate(sparkPrefab, col.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    // TODO: KIV on how to deal with collision for own goal
    // 1) Ball bounce out 2) Ball ignores collision 3) Put ball in pool
    /*void OnCollisionEnter(Collision col) {
      if (col.gameObject.GetComponent<Ball>().GetPlayerNumber() == playerNumber) {
          Physics.IgnoreCollision(col.gameObject.GetComponent<Collider>(), this.GetComponent<Collider>());
      }
    } */

    private void SwitchGoalState(GoalState stateToSwitch) {
        goalState = stateToSwitch;
    }

    [PunRPC]
    private void Goal_SetMaterial(byte stateToSwitch, bool isHit) {
        if (isHit) {
            StartCoroutine(ShowGoalHitFor(0.5f));
        } else {
            GoalState state = (GoalState)stateToSwitch;
            if (state == GoalState.STATIONARY) {
                SwitchGoalMaterial(stationaryMat);
            }
        }
    }

    private void SwitchGoalMaterial(Material m) {
        foreach(MeshRenderer mr in goalSidesMR) {
            mr.material = m;
        }
    }

    private IEnumerator ShowGoalHitFor(float duration) {
        SwitchGoalMaterial(hitMat);
        yield return new WaitForSeconds(duration);
        SwitchGoalMaterial(stationaryMat);
    }

    // Used for cases where this script will be disabled but the number is still needed
    // i.e. multiplayer
    public void SetPlayerNumber(PlayerNumber playerNumber) {
        this.playerNumber = playerNumber;
    }
}
