﻿using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

// The actual tangible part of the tool, instantiated separately from player
public class ToolFollower : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback {
    private Tool tool;
    private Rigidbody rb;
    private Collider col;
    private PhotonView pv;
    private Vector3 velocity;
    private bool isHuman;
    private List<Material> materials = new List<Material>();
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
    [SerializeField]
    private GameObject sparkPrefab = null;

    [SerializeField]
    private float _sensitivity = 100f;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        pv = GetComponent<PhotonView>();
    }

    private void Start() {
        foreach (Renderer r in GetComponents<Renderer>()) {
            materials.AddRange(r.materials);
        }
    }
 
    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        if (FollowersParent.Instance != null) {
            transform.parent = FollowersParent.Instance.tools;
        }
    }

    private void Update() {
        if (!PhotonNetwork.IsConnected || pv.IsMine) {
            HandleFade();
        }
        if (fadeState == FadeState.FADED) {
            col.isTrigger = true;
        } else {
            col.isTrigger = false;
        }
    }

    private void FixedUpdate() {
        if (!PhotonNetwork.IsConnected || pv.IsMine) {
            Vector3 destination = tool.transform.position;
            rb.transform.rotation = transform.rotation;

            velocity = (destination - rb.transform.position) * _sensitivity;

            rb.velocity = velocity;
            transform.rotation = tool.transform.rotation;
        }
    }

    public void SetFollowTarget(Tool target) {
        tool = target;
    }

    public void SetHumanity(bool isHuman) {
        this.isHuman = isHuman;
    }

    // Fade/unfade based on the state of the tool and the magnitude of valocity
    public void HandleFade() {
        float magnitude = velocity.magnitude;
        if (magnitude > velocityLowerThreshold && fadeState < FadeState.NORMAL) {
            float a = Mathf.Clamp(materials[0].color.a + Time.deltaTime * unfadeSpeed, minOpacityValue, 1);
            if (PhotonNetwork.IsConnected) {
                pv.RPC("PhotonSetAlpha", RpcTarget.AllBuffered, a);
            } else {
                PhotonSetAlpha(a);
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
                    pv.RPC("PhotonSetAlpha", RpcTarget.AllBuffered, a);
                } else {
                    PhotonSetAlpha(a);
                }
                if (materials[0].color.a <= minOpacityValue) {
                    fadeState = FadeState.FADED;
                } else {
                    fadeState = FadeState.FADING;
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (isHuman) {
            if (!PhotonNetwork.IsConnected || pv.IsMine) {
                ((ToolHuman)tool).TriggerHapticFeedback(0.1f, 100, 30);
            }
        }
        if (velocity.magnitude > velocityLowerThreshold
            && collision.collider.gameObject.layer == LayerMask.NameToLayer("Ball")) {
            if (PhotonNetwork.IsConnected) {
                PhotonNetwork.Instantiate(sparkPrefab.name, collision.GetContact(0).point, Quaternion.identity);
            } else {
                Instantiate(sparkPrefab, collision.GetContact(0).point, Quaternion.identity);
            }
        }
    }

    [PunRPC]
    private void PhotonSetAlpha(float a) {
        foreach (Material m in materials) {
            m.color = new Color(m.color.r, m.color.g, m.color.b, a);
        }
    }

    [PunRPC]
    private void PhotonSetState(bool active) {
        gameObject.SetActive(active);
    }

    public void SetActive(bool active) {
        if (PhotonNetwork.IsConnected) {
            pv.RPC("PhotonSetState", RpcTarget.AllBuffered, active);
        }
        else {
            gameObject.SetActive(active);
        }
    }
}
