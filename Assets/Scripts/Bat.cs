using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviourPunCallbacks {
    List<BatChildFollower> followers = new List<BatChildFollower>();
    protected List<Material> materials = new List<Material>();
    private readonly Renderer[] renderers;
    private float fadeStartTime;
    private readonly float velocityLowerThreshold = 0.15f;   // anything below this value will be considered "stationary" and trigger a fade
    private readonly float fadeDelay = 0.15f;   // delay after tool first turns stationary to start the fade
    private readonly float unfadeSpeed = 5f; // multiplier to speed up the unfading
    private readonly float minOpacityValue = 0.05f;  // minimum alpha for the materials
    protected enum FadeState {
        FADED,
        FADING,
        UNFADING,
        NORMAL
    }
    protected FadeState fadeState = FadeState.NORMAL;
    private Vector3 velocity;
    private Vector3 lastPos;

    protected virtual void Awake() {
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) {
            foreach (Material m in r.materials) {
                if (m.GetFloat("_Mode") > 0) {  // if m's rendering mode is not opaque
                    materials.Add(m);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        foreach (BatChildFollowerSpawner spawner in GetComponentsInChildren<BatChildFollowerSpawner>()) {
            followers.Add(spawner.follower);
        }
    }

    private void Update() {
        HandleFade();
        if (fadeState == FadeState.FADED) {
            SetFollowersActive(false);
        } else if (gameObject.activeInHierarchy) {
            SetFollowersActive(true);
        }
    }

    public void SetState(bool active) {
        gameObject.SetActive(active);
        SetFollowersActive(active);
    }

    public void SetFollowersActive(bool active) {
        foreach (BatChildFollower f in followers) {
            f.gameObject.SetActive(active);
        }
    }

    private void FixedUpdate() {
        if (lastPos == null) {
            velocity = Vector3.zero;
        } else {
            velocity = (transform.position - lastPos) / Time.deltaTime;
        }
        lastPos = transform.position;
    }


    // Fade/unfade based on the state of the tool and the magnitude of valocity
    public void HandleFade() {
        float magnitude = velocity.magnitude;
        if (magnitude > velocityLowerThreshold && fadeState < FadeState.NORMAL) {
            float a = Mathf.Clamp(materials[0].color.a + Time.deltaTime * unfadeSpeed, minOpacityValue, 1);
            if (PhotonNetwork.IsConnected) {
                photonView.RPC("Bat_SetAlpha", RpcTarget.AllBuffered, a);
            } else {
                Bat_SetAlpha(a);
            }
            if (materials[0].color.a > 0.95f) {
                fadeState = FadeState.NORMAL;
            } else {
                fadeState = FadeState.UNFADING;
            }
        } else if (magnitude < velocityLowerThreshold && fadeState > FadeState.FADED) {
            if (fadeState > FadeState.FADING) {
                fadeStartTime = Time.time;
                fadeState = FadeState.FADING;
            } else if (Time.time - fadeStartTime > fadeDelay) {
                float a = Mathf.Clamp(materials[0].color.a - Time.deltaTime, minOpacityValue, 1);
                if (PhotonNetwork.IsConnected) {
                    photonView.RPC("Bat_SetAlpha", RpcTarget.AllBuffered, a);
                } else {
                    Bat_SetAlpha(a);
                }
                if (materials[0].color.a <= minOpacityValue) {
                    fadeState = FadeState.FADED;
                } else {
                    fadeState = FadeState.FADING;
                }
            }
        }
    }
    
    [PunRPC]
    protected virtual void Bat_SetAlpha(float a) {
        foreach (Material m in materials) {
            m.color = new Color(m.color.r, m.color.g, m.color.b, a);
        }
    }
}
