using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatChildFollowerSpawner : MonoBehaviour {
    [SerializeField]
    private GameObject batChildFollowerPrefab;
    [HideInInspector]
    public BatChildFollower follower;
    private Bat bat;
    private bool isHuman;

    private void Awake() {
        bat = GetComponentInParent<Bat>();
        if (!PhotonNetwork.IsConnected || GetComponentInParent<PhotonView>().IsMine) {
            follower = Instantiate(batChildFollowerPrefab, transform.position, transform.rotation).GetComponent<BatChildFollower>();
            follower.transform.localScale = transform.localScale;
            follower.SetFollowTarget(this);
        }
    }

    // Start is called before the first frame update
    void Start() {
        if (GetComponentInParent<BatHuman>() != null) {
            isHuman = true;
        }
    }

    public void OnCollide(Collision collision) {
        if (isHuman) {
            ((BatHuman)bat).TriggerHapticFeedback(0.1f, 100, 30);
        }
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ball")) {
            PlaytestRecording.RecordHit();
        }
    }
}
