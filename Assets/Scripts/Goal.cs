using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

// Manages the goalpost position
// Goalpost MUST be a child of the VR camera
// Goalpost cannot move outside the bounds of the room
public enum GoalType {
    REGULAR = 0,
    HORIZONTAL_WALL = 1,
    VERITCAL_WALL = 2
}

public class Goal : MonoBehaviourPunCallbacks, IOnEventCallback {
    [SerializeField]
    private GoalType goalType;
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
        newPos;
    private float yRotation;
    private float zOffset;

    private PlayerNumber playerNumber;
    [SerializeField]
    private GameObject sparkPrefab = null;
    [SerializeField]
    private MeshRenderer[] goalSidesMR;
    [SerializeField]
    private Material
        followingMat = null,
        hitMat = null;

    void Start() {
        GameManager.Instance.RestartEvent.AddListener(ResetGoal);
        DisableIfNotMine();
    }

    public void DisableIfNotMine() {
        if (PhotonNetwork.IsConnected && !photonView.IsMine) {
            enabled = false;
        }
    }

    public void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
        DisableIfNotMine();
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
                HandleRegularGoalPosition();
                break;
            case GoalType.VERITCAL_WALL:
                InitWallGoal();
                HandleVerticalWallGoalPosition();
                break;
            case GoalType.HORIZONTAL_WALL:
                InitWallGoal();
                HandleHorizontalWallGoalPosition();
                break;
        }
    }

    public void OnEvent(EventData photonEvent) {
        byte GoalWasScoredEvent = 1;
        byte eventCode = photonEvent.Code;
        
        if (eventCode == GoalWasScoredEvent) {
            object[] data = (object[])photonEvent.CustomData;

            PlayerNumber playerLastScored = (PlayerNumber)data[0];
            if (playerNumber == playerLastScored) {
                AudioManager.PlaySoundOnce("goalding");
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
        // Prevent goal from exceeding room bounds
        newPos.x = Mathf.Clamp(parentPos.x + X_OFFSET, X_MIN, X_MAX);
        newPos.y = Mathf.Clamp(parentPos.y + Y_OFFSET, Y_MIN, Y_MAX);
        newPos.z = Mathf.Clamp(parentPos.z + zOffset, Z_MIN, Z_MAX);
        UpdateGoalPosition(newPos);
    }

    private void HandleVerticalWallGoalPosition() {
        parentPos = transform.parent.position;
        // Prevent goal from exceeding room bounds
        newPos = wallPos;
        newPos.x = Mathf.Clamp(parentPos.x, X_MIN, X_MAX);
        UpdateGoalPosition(newPos);
    }

    private void HandleHorizontalWallGoalPosition() {
        parentPos = transform.parent.position;
        // Prevent goal from exceeding room bounds
        newPos = wallPos;
        newPos.y = Mathf.Clamp(parentPos.y, Y_MIN, Y_MAX);
        UpdateGoalPosition(newPos);
    }

    private void UpdateGoalPosition(Vector3 pos) {
        transform.position = pos;
        transform.eulerAngles = new Vector3 (0, yRotation, goalType == GoalType.HORIZONTAL_WALL ? 90f : 0);
    }

    void OnTriggerEnter(Collider col) {
        if (!isActiveAndEnabled                             // case 1: 2 players, handle on thrower's side (receiver's goal is disabled)
            || !PhotonNetwork.IsConnected                   // case 2: not connected, both goals are on
            || PhotonNetwork.CurrentRoom.PlayerCount < 2) { // case 3: only one player, both goals are on
            if (col.gameObject.layer == LayerMask.NameToLayer("Ball")) {
                // Prevent own goal
                if (col.gameObject.GetComponent<Ball>().GetPlayerNumber() != playerNumber) {
                    ScoreManager.Instance.AddScoreToOpponent(playerNumber, 1);
                    BallManager.LocalInstance.PutBallInPool(col.GetComponent<Ball>());

                    if (PhotonNetwork.IsConnected) {
                        photonView.RPC("Goal_SetHitMaterial", RpcTarget.AllBuffered);
                        PhotonNetwork.Instantiate(sparkPrefab.name, col.transform.position, Quaternion.identity);
                    } else {
                        StartCoroutine(ShowGoalHitFor(0.5f));
                        Instantiate(sparkPrefab, col.transform.position, Quaternion.identity);
                    }
                    PlaytestRecording.RecordGoalScored();
                }
            }
        }
    }

    [PunRPC]
    private void Goal_SetHitMaterial() {
        StartCoroutine(ShowGoalHitFor(0.5f));
    }

    private void SwitchGoalMaterial(Material m) {
        foreach(MeshRenderer mr in goalSidesMR) {
            mr.material = m;
        }
    }

    private IEnumerator ShowGoalHitFor(float duration) {
        SwitchGoalMaterial(hitMat);
        yield return new WaitForSeconds(duration);
        SwitchGoalMaterial(followingMat);
    }

    [PunRPC]
    private void Goal_SetPlayerNumberAndResetGoal(int playerNumber) {
        this.playerNumber = (PlayerNumber)playerNumber;
        ResetGoal();
    }

    public void SetPlayerNumberAndResetGoal(PlayerNumber playerNumber) {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("Goal_SetPlayerNumberAndResetGoal", RpcTarget.All, (int)playerNumber);
        } else {
            this.playerNumber = playerNumber;
            ResetGoal();
        }
    }
}
