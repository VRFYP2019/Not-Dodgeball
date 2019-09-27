using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;

public class Tool : MonoBehaviourPunCallbacks {
    [SerializeField]
    private ToolFollower _toolFollowerPrefab = null;
    protected ToolFollower follower;

    // Start is called before the first frame update
    protected virtual void Start() {
        SpawnToolFollower();
    }

    [PunRPC]
    protected virtual void SpawnToolFollower() {
        if (PhotonNetwork.IsConnected) {
            follower = PhotonNetwork.Instantiate(_toolFollowerPrefab.name, transform.position, transform.rotation).GetComponent<ToolFollower>();
        } else {
            follower = Instantiate(_toolFollowerPrefab);
            follower.transform.position = transform.position;
        }
        follower.SetFollowTarget(this);
        //StartCoroutine(WaitForFollowersParentThenSetParent());
    }
    
    [PunRPC]
    public void SetFollowerActive(bool on) {
        if (follower != null) {
            follower.SetActive(on);
        }
    }

    IEnumerator WaitForFollowersParentThenSetParent() {
        while (FollowersParent.LocalInstance == null) {
            yield return null;
        }
        follower.transform.parent = FollowersParent.LocalInstance.tools;

    }

    [PunRPC]
    public void SetState(bool active) {
        gameObject.SetActive(active);
    }
}
