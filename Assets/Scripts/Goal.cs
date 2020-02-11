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
    private static readonly float X_OFFSET = 0f, Y_OFFSET = 0f, Z_OFFSET_PLAYER_ONE = -1.25f, Z_OFFSET_PLAYER_TWO = 1.25f;
    private static readonly float SNAP_THRESHOLD = 1.5f;
    private static readonly float X_MIN = -2f, X_MAX = 2f, Y_MIN = 0.5f, Y_MAX = 3.5f, Z_MIN = -12f, Z_MAX =3f;
    private static readonly float PLAYER_1_ROTATION = 180f, PLAYER_2_ROTATION = 0;

    private Vector3 parentPos, newPos, lastSafePos;
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

    private void InitRotationAndOffset() {
        if (playerNumber == PlayerNumber.ONE) {
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
        HandleGoalPosition();
    }

    private void HandleGoalPosition() {
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

            if (CheckForSnap()) {
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
    private bool CheckForSnap() {
        float distToPlayer = Vector3.Distance(parentPos, transform.position);
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
