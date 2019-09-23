using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Ball : MonoBehaviour {
    public Transform transformToFollow = null;
    private float lastReadTime = 0f;
    private int listPointer = 0;    // use a pointer instead of shifting the array every update
    private List<Collider> boundsInContact;
    private readonly List<Collider>[] boundsLists = new List<Collider>[numFramesToConsider];
    private readonly static int numFramesToConsider = 3;
    private readonly static float boundListUpdateInterval = 0.33f;   // interval to update lists

    public AudioClip defaultCollisionSound;
    public AudioClip toolCollisionSound;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start() {
        InitLists();
        audioSource = GetComponent<AudioSource>();
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
