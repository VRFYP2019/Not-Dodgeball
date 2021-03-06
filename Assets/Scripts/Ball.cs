﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

public class Ball : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback {
    private Collider col;
    private Rigidbody rb;
    private Material mat;
    private ParticleSystem ps;
    private AudioSource audioSource;
    private bool hasBeenInit = false;
    private PlayerNumber playerNumber;
    private Transform transformToFollow = null;
    private readonly static float forceMultiplier = 69;
    // weights to multiply prev velocities with to get the throwing velocity
    private static float[] prevVelocityWeights = { 0.1f, 0.2f, 0.3f, 0.3f, 0.2f, 0.1f };
    private static int numFramesOfVelocities = prevVelocityWeights.Length;
    private static float sumPrevVelocityWeights = prevVelocityWeights.Sum();
    private Vector3[] prevVelocities;
    private int prevVelocitiesPointer = 0;
    private Vector3 prevPos;
    private float lastReadTime = 0f;
    private int boundsListPointer = 0;    // use a pointer instead of shifting the array every update
    private List<Collider> boundsInContact;
    private readonly List<Collider>[] boundsLists = new List<Collider>[numFramesToConsider];
    private readonly static int numFramesToConsider = 3;
    private readonly static float boundListUpdateInterval = 0.33f;   // interval to update lists
    private bool countTimeLived = false;    // set true if active
    private readonly static float timeToLive = 5;  // kill self in 5 seconds
    private float timeLived = 0;

    public AudioClip defaultCollisionSound;
    public AudioClip toolCollisionSound;

    void Awake() {
        if (!hasBeenInit) {
            Init();
        }
    }

    private void Init() {
        InitLists();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        mat = GetComponent<Renderer>().material;
        ps = GetComponent<ParticleSystem>();
        hasBeenInit = true;
    }

    public void InitLists() {
        for (int i = 0; i < numFramesToConsider; i++) {
            boundsLists[i] = new List<Collider>();
        }
        boundsInContact = new List<Collider>();
        boundsListPointer = 0;
        prevVelocities = new Vector3[numFramesOfVelocities];
        prevVelocitiesPointer = 0;
    }

    // Update is called once per frame
    void Update() {
        if (transformToFollow != null) {
            transform.position = transformToFollow.position;
            transform.rotation = transformToFollow.rotation;
            if (prevPos != null) {
                prevVelocities[prevVelocitiesPointer] = transform.position - prevPos;
                prevVelocitiesPointer++;
                prevVelocitiesPointer %= numFramesOfVelocities;
            }
            prevPos = transform.position;
        }
        if (Time.time - lastReadTime > boundListUpdateInterval) {
            boundsLists[boundsListPointer] = new List<Collider>(boundsInContact.ToArray());
            boundsListPointer++;
            boundsListPointer %= numFramesToConsider;
            lastReadTime = Time.time;
        }
        if (countTimeLived) {
            timeLived += Time.deltaTime;
        }
        if (IsDead()) {
            OnDeath();
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        if (!hasBeenInit) {
            Init();
        }
        object[] instantiationData = info.photonView.InstantiationData;
        SetState((bool)instantiationData[0]);
        SetParent(BallManager.LocalInstance.ballPool);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Bounds")) {
            boundsInContact.Add(collision.collider);
            audioSource.PlayOneShot(defaultCollisionSound);
        } else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Goal")) {
            audioSource.PlayOneShot(defaultCollisionSound);
        }
        
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Tool")) {
            audioSource.PlayOneShot(toolCollisionSound);
            ps.Play();
            // reset time lived
            timeLived = 0;
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Bounds")) {
            boundsInContact.Remove(collision.collider);
        }
    }

    public void OnAttachToHand(Transform hand) {
        if (!hasBeenInit) {
            Init();
        }
        prevPos = hand.position;
        transformToFollow = hand;
        SetPosition(hand.transform.position);
        rb.isKinematic = true;
        SetUseGravity(false);
        col.enabled = false;
        SetState(true);
    }

    public void OnDetachFromHand() {
        timeLived = 0;
        countTimeLived = true;
        Vector3 throwVecetor = Vector3.zero;
        for (int i = 0; i < numFramesOfVelocities; i++) {
            throwVecetor += prevVelocityWeights[i] * prevVelocities[Math.Modulo((prevVelocitiesPointer - i), numFramesOfVelocities)];
        }
        foreach (float w in prevVelocityWeights) {
            throwVecetor += w * prevVelocities[prevVelocitiesPointer];
        }
        throwVecetor /= sumPrevVelocityWeights;
        throwVecetor *= forceMultiplier;
        transformToFollow = null;
        rb.isKinematic = false;
        SetUseGravity(true);
        col.enabled = true;
        rb.AddForce(throwVecetor, ForceMode.Impulse);
        SetParent(BallManager.LocalInstance.activeBalls);
    }

    public void SetTransformToFollowToNull() {
        transformToFollow = null;
    }

    // There should only be a need to set it to false from outside
    public void SetCountTimeLivedToFalse() {
        countTimeLived = false;
        timeLived = 0;
    }

    private bool IsDead() {
        if (timeLived > timeToLive) {
            return true;
        }
        foreach (Collider c in boundsLists[0]) {
            bool isColliderInEveryList = true;
            for (int i = 1; i < numFramesToConsider; i++) {
                if (!boundsLists[i].Contains(c)) {
                    isColliderInEveryList = false;
                    break;
                }
            }
            if (isColliderInEveryList) {
                return true;
            }
        }
        return false;
    }

    private void OnDeath() {
        timeLived = 0;
        countTimeLived = false;
        InitLists();
        BallManager.LocalInstance.PutBallInQueue(this);
    }

    [PunRPC]
    private void Ball_SetPosition(float x, float y, float z) {
        transform.position = new Vector3(x, y, z);
    }

    public void SetPosition(Vector3 newPos) {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("Ball_SetPosition", RpcTarget.All, newPos.x, newPos.y, newPos.z);
        } else {
            transform.position = newPos;
        }
    }
    
    [PunRPC]
    private void Ball_SetUseGravity(bool shouldUseGravity) {
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
        }
        if (shouldUseGravity) {
            rb.useGravity = true;
        } else {
            rb.useGravity = false;
        }
    }

    private void SetUseGravity(bool shouldUseGravity) {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("Ball_SetUseGravity", RpcTarget.All, shouldUseGravity);
        } else {
            Ball_SetUseGravity(shouldUseGravity);
        }
    }

    [PunRPC]
    private void Ball_SetState(bool active) {
        gameObject.SetActive(active);
    }

    public void SetState(bool active) {
        if (!hasBeenInit) {
            Init();
        }
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("Ball_SetState", RpcTarget.All, active);
        } else {
            Ball_SetState(active);
        }
    }

    [PunRPC]
    private void Ball_SetParent(string parentName) {
        BallManager instance;
        if (photonView.IsMine) {
            instance = BallManager.LocalInstance;
        } else {
            instance = BallManager.RemoteInstance;
        }
        switch (parentName) {
            case "Pool":
                transform.parent = instance.ballPool;
                break;
            case "ActiveBalls":
                transform.parent = instance.activeBalls;
                break;
            case "PlayerBallQueue":
                transform.parent = instance.playerBallQueue;
                break;
            default:
                Debug.Log(parentName);
                return;
        }
    }

    public void SetParent(Transform parent) {
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("Ball_SetParent", RpcTarget.All, parent.name);
        } else {
            transform.parent = parent;
        }
    }

    [PunRPC]
    private void Ball_SetPlayerNumber(int playerNumber) {
        this.playerNumber = (PlayerNumber)playerNumber;
        UpdateColor();
    }

    public void SetPlayerNumber(PlayerNumber playerNumber) {
        if (!hasBeenInit) {
            Init();
        }
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("Ball_SetPlayerNumber", RpcTarget.All, (int)playerNumber);
        } else {
            this.playerNumber = playerNumber;
            UpdateColor();
        }
    }

    public PlayerNumber GetPlayerNumber() {
        return playerNumber;
    }

    private void UpdateColor() {
        if (mat == null) {
            mat = GetComponent<Renderer>().material;
        }
        mat.color = playerNumber == PlayerNumber.ONE ? Constants.Colors.blue : Constants.Colors.orange;
    }
}
