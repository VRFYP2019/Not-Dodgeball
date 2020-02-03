using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Tool : MonoBehaviourPunCallbacks {
    [SerializeField]
    private ToolFollower toolFollowerPrefab = null;
    protected ToolFollower follower;

    // Start is called before the first frame update
    protected virtual void Start() {
        SpawnToolFollower();
    }

    protected virtual void SpawnToolFollower() {
        if (PhotonNetwork.IsConnected) {
            follower = PhotonNetwork.Instantiate(toolFollowerPrefab.name, transform.position, transform.rotation).GetComponent<ToolFollower>();
        } else {
            follower = Instantiate(toolFollowerPrefab);
            follower.transform.position = transform.position;
        }
        if (GetComponentInParent<HandController>().handSide == Utils.HandSide.RIGHT) {
            follower.transform.localScale = new Vector3(
                follower.transform.localScale.x,
                -follower.transform.localScale.y,
                follower.transform.localScale.z
            );
            follower.FlipCollider();
        }
        follower.SetFollowTarget(this);
    }

    public void SetFollowerActive(bool on) {
        if (follower != null) {
            follower.SetActive(on);
        }
    }

    public void SetState(bool active) {
        gameObject.SetActive(active);
    }
}
