using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To keep all follwer objects in one place to reduce clutter in editor runtime
public class FollowersParent : MonoBehaviour {
    public static FollowersParent Instance;
    public Transform tools;
    public Transform balls;

    private void Awake() {
        Instance = this;
    }
}
