﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hand for spawning balls. Set to active only when there is a ball in the queue for this player and player
// wants to spawn. Otherwise, a Tool should be active on the player's hand instead.
public class Spawner : MonoBehaviour {
    private Transform parentOfBallsToThrow;
    private readonly float spawnDelay = 0.25f;
    public GameObject currentBall;
    private HandController handController;
    private PhotonView photonView;

    private void Awake() {
        photonView = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start() {
        handController = GetComponentInParent<HandController>();
        parentOfBallsToThrow = BallManager.LocalInstance.playerBallQueue;
    }
   
    public void ThrowCurrentBall() {
        currentBall.GetComponent<Ball>().OnDetachFromHand();
        currentBall = null;
    }

    // Makes currentBall follow this hand
    private void SetCurrentBallToFollow() {
        currentBall.GetComponent<Ball>().OnAttachToHand(transform);
    }
    
    // Takes the next ball out of the queue and into the hand
    private void PutNextBallInHand() {
        if (!parentOfBallsToThrow.GetChild(0).gameObject.activeInHierarchy) {
            currentBall = parentOfBallsToThrow.GetChild(0).gameObject;
        } else {    // if first in list is already held by other hand, get next in list
            currentBall = parentOfBallsToThrow.GetChild(1).gameObject;
        }
        currentBall.GetComponent<Collider>().enabled = false;
        currentBall.GetComponent<Rigidbody>().isKinematic = true;
        SetCurrentBallToFollow();
        currentBall.GetComponent<Ball>().SetState(true);
    }

    public void RestartState() {
        if (currentBall != null) {
            UnspawnBall();
        }
    }

    public void UnspawnBall() {
        BallManager.LocalInstance.PutBallInQueue(currentBall);
        currentBall.GetComponent<Ball>().transformToFollow = null;
        currentBall = null;
    }

    public IEnumerator TrySpawn() {
        yield return new WaitForSeconds(spawnDelay);
        if (gameObject.activeInHierarchy == false) {
            yield break;
        }

        if (parentOfBallsToThrow.childCount > 1
            || (parentOfBallsToThrow.childCount == 1 && !parentOfBallsToThrow.GetChild(0).gameObject.activeInHierarchy)) {
            PutNextBallInHand();
        } else if (currentBall == null) {
            FinishThrowing();
        }
    }

    // To be called when all balls are tossed
    private void FinishThrowing() {
        handController.SwitchToTool();
    }

    [PunRPC]
    private void PhotonSetState(bool active) {
        gameObject.SetActive(active);
    }

    public void SetState(bool active) {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("PhotonSetState", RpcTarget.AllBuffered, active);
        } else {
            gameObject.SetActive(active);
        }
    }
}