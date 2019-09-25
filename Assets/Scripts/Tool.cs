using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Tool : MonoBehaviour {
    [SerializeField]
    private ToolFollower _toolFollowerPrefab = null;
    protected ToolFollower follower;

    // Start is called before the first frame update
    protected virtual void Start() {
        SpawnToolFollower();
    }

    protected virtual void SpawnToolFollower() {
        follower = Instantiate(_toolFollowerPrefab);
        follower.transform.position = transform.position;
        follower.SetFollowTarget(this);
        follower.transform.parent = FollowersParent.Instance.tools;
    }

    public void SetState(bool on) {
        if (follower != null) {
            if (on) {
                follower.gameObject.SetActive(true);
            } else {
                follower.gameObject.SetActive(false);
            }
        }
    }
}
