using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
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
    
    // Start is called before the first frame update
    void Start() {
        InitLists();
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
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
                BallManager.Instance.PutBallInPool(gameObject);
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
            BallManager.Instance.PutBallInPool(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Bounds")) {
            boundsInContact.Add(collision.collider);
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Bounds")) {
            boundsInContact.Remove(collision.collider);
        }
    }

    public void OnAttachToHand(Transform hand) {
        transform.parent = FollowersParent.Instance.balls;
        prevPos = hand.position;
        transformToFollow = hand;
        // Due to how these balls start as inactive and this is called the moment
        // it is set active the first time, Start() may not have been called yet
        if (rb == null) {
            rb = GetComponent<Rigidbody>();
        }
        if (col == null) {
            col = GetComponent<Collider>();
        }
        rb.isKinematic = true;
        col.enabled = false;
    }

    public void OnDetachFromHand() {
        Vector3 throwVecetor = (transformToFollow.position - prevPos) * forceMultiplier;
        transformToFollow = null;
        rb.isKinematic = false;
        col.enabled = true;
        rb.AddForce(throwVecetor, ForceMode.Impulse);
        transform.parent = BallManager.Instance.activeBalls;
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
}
