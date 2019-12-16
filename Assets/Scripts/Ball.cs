using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Utils;

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
    private bool countTimeLived = false;    // set true if active
    private readonly static float timeToLive = 10;  // kill self in 10 seconds
    private float timeLived = 0;
    private Vector3 prevPos;
    private bool hasBeenInit = false;
    private PlayerNumber playerNumber;
    private Material mat;
    private ParticleSystem ps;

    public AudioClip defaultCollisionSound;
    public AudioClip toolCollisionSound;
    private AudioSource audioSource;

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
        if (countTimeLived) {
            timeLived += Time.deltaTime;
        }
        if (IsDead()) {
            timeLived = 0;
            countTimeLived = false;
            InitLists();
            BallManager.LocalInstance.PutBallInQueue(gameObject);
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
        rb.isKinematic = true;
        col.enabled = false;
    }

    public void OnDetachFromHand() {
        timeLived = 0;
        countTimeLived = true;
        Vector3 throwVecetor = (transformToFollow.position - prevPos) * forceMultiplier;
        transformToFollow = null;
        rb.isKinematic = false;
        col.enabled = true;
        rb.AddForce(throwVecetor, ForceMode.Impulse);
        SetParent(BallManager.LocalInstance.activeBalls);
    }

    // There should only be a need to set it to false from outside
    public void SetCountTimeLivedToFalse() {
        countTimeLived = false;
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

    [PunRPC]
    private void PhotonSetState(bool active) {
        gameObject.SetActive(active);
    }

    public void SetState(bool active) {
        if (!hasBeenInit) {
            Init();
        }
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("PhotonSetState", RpcTarget.All, active);
        } else {
            gameObject.SetActive(active);
        }
    }

    [PunRPC]
    private void PhotonSetParent(string parentName) {
        BallManager instance;
        if (GetComponent<PhotonView>().IsMine) {
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
            photonView.RPC("PhotonSetParent", RpcTarget.All, parent.name);
        } else {
            transform.parent = parent;
        }
    }

    [PunRPC]
    private void PhotonSetPlayerNumber(int playerNumber) {
        this.playerNumber = (PlayerNumber)playerNumber;
        UpdateColor();
    }

    public void SetPlayerNumber(PlayerNumber playerNumber) {
        if (!hasBeenInit) {
            Init();
        }
        if (PhotonNetwork.IsConnected) {
            photonView.RPC("PhotonSetPlayerNumber", RpcTarget.All, (int)playerNumber);
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
        mat.color = playerNumber == PlayerNumber.ONE ? Constants.blue : Constants.orange;
    }
}
