﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Ball : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback {
    public Transform transformToFollow = null;
    private Collider col;
    private Rigidbody rb;
    private float lastReadTime = 0f;
    private int listPointer = 0;    // use a pointer instead of shifting the array every update
    private List<Collider> boundsInContact;
    private readonly List<Collider>[] boundsLists = new List<Collider>[numFramesToConsider];
    private readonly static int numFramesToConsider = 3;
    private readonly static float boundListUpdateInterval = 0.33f;   // interval to update lists
    private readonly static float forceMultiplier = 100;
    private Vector3 prevPos;
    private PhotonView pView;
    private bool hasBeenInit = false;

    public AudioClip defaultCollisionSound;
    public AudioClip toolCollisionSound;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
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
        pView = GetComponent<PhotonView>();
        hasBeenInit = true;
    }

    private void InitLists() {
        for (int i = 0; i < numFramesToConsider; i++) {
            boundsLists[i] = new List<Collider>();
        }
        boundsInContact = new List<Collider>();
    }

    // Update is called once per frame
    void Update() {
        if (transformToFollow != null) {
            transform.position = transformToFollow.position;
            transform.rotation = transformToFollow.rotation;
            prevPos = transform.position;
        } else {
            if (IsDead()) {
                InitLists();
                BallManager.LocalInstance.PutBallInPool(gameObject);
            }
        }
        if (Time.time - lastReadTime > boundListUpdateInterval) {
            boundsLists[listPointer] = new List<Collider>(boundsInContact.ToArray());
            listPointer++;
            listPointer %= numFramesToConsider;
            lastReadTime = Time.time;
        }
        if (IsDead()) {
            InitLists();
            BallManager.LocalInstance.PutBallInPool(gameObject);
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        object[] instantiationData = info.photonView.InstantiationData;
        SetState((bool)instantiationData[0]);
        transform.parent = BallManager.LocalInstance.ballPool;
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
        if (FollowersParent.LocalInstance != null) {
            transform.parent = FollowersParent.LocalInstance.balls;
        }
        prevPos = hand.position;
        transformToFollow = hand;
        rb.isKinematic = true;
        col.enabled = false;
    }

    public void OnDetachFromHand() {
        Vector3 throwVecetor = (transformToFollow.position - prevPos) * forceMultiplier;
        transformToFollow = null;
        rb.isKinematic = false;
        col.enabled = true;
        rb.AddForce(throwVecetor, ForceMode.Impulse);
        SetParent(BallManager.LocalInstance.activeBalls);
    }

    private bool IsDead() {
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

    [PunRPC]
    private void PhotonSetState(bool active) {
        gameObject.SetActive(active);
    }

    public void SetState(bool active) {
        if (!hasBeenInit) {
            // null if this is called upon enabling gameobject
            // i.e. before Awake() runs
            Init();
        }
        if (PhotonNetwork.IsConnected) {
            pView.RPC("PhotonSetState", RpcTarget.All, active);
        } else {
            gameObject.SetActive(active);
        }
    }

    [PunRPC]
    private void PhotonSetParent(Transform parent) {
        transform.parent = parent;
    }

    public void SetParent(Transform parent) {
        if (PhotonNetwork.IsConnected) {
            PhotonSetParent(parent);
        } else {
            transform.parent = parent;
        }
    }
}