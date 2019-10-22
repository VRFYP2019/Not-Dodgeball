using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

// The actual tangible part of the tool, instantiated separately from player
public class ToolFollower : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback {
    private Tool _tool;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Vector3 _velocity;
    private bool _isHuman;
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
    private PhotonView pView;
    [SerializeField]
    private GameObject sparkPrefab;

    [SerializeField]
    private float _sensitivity = 100f;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        pView = GetComponent<PhotonView>();
    }

    private void Start() {
        foreach (Renderer r in GetComponents<Renderer>()) {
            materials.AddRange(r.materials);
        }
    }
 
    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        if (FollowersParent.LocalInstance != null) {
            transform.parent = FollowersParent.LocalInstance.tools;
        }
    }

    private void Update() {
        HandleFade();
        if (fadeState == FadeState.FADED) {
            _collider.isTrigger = true;
        } else {
            _collider.isTrigger = false;
        }
    }

    private void FixedUpdate() {
        if (!PhotonNetwork.IsConnected || pView.IsMine) {
            Vector3 destination = _tool.transform.position;
            _rigidbody.transform.rotation = transform.rotation;

            _velocity = (destination - _rigidbody.transform.position) * _sensitivity;

            _rigidbody.velocity = _velocity;
            transform.rotation = _tool.transform.rotation;
        }
    }

    public void SetFollowTarget(Tool target) {
        _tool = target;
    }

    public void SetHumanity(bool isHuman) {
        _isHuman = isHuman;
    }

    // Fade/unfade based on the state of the tool and the magnitude of valocity
    public void HandleFade() {
        float magnitude = _velocity.magnitude;
        if (magnitude > velocityLowerThreshold && fadeState < FadeState.NORMAL) {
            foreach (Material m in materials) {
                float r = m.color.r;
                float g = m.color.g;
                float b = m.color.b;
                float a = Mathf.Clamp(m.color.a + Time.deltaTime * unfadeSpeed, minOpacityValue, 1);
                m.color = new Color(r, g, b, a);
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
                foreach (Material m in GetComponent<Renderer>().materials) {
                    float r = m.color.r;
                    float g = m.color.g;
                    float b = m.color.b;
                    float a = Mathf.Clamp(m.color.a - Time.deltaTime, minOpacityValue, 1);
                    m.color = new Color(r, g, b, a);
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
        if (_isHuman) {
            if (!PhotonNetwork.IsConnected || pView.IsMine) {
                ((ToolHuman)_tool).TriggerHapticFeedback(0.1f, 100, 30);
            }
        }
        if (_velocity.magnitude > velocityLowerThreshold && collision.collider.gameObject.layer == LayerMask.NameToLayer("Ball")) {
            if (PhotonNetwork.IsConnected) {
                PhotonNetwork.Instantiate(sparkPrefab.name, collision.GetContact(0).point, Quaternion.identity);
            } else {
                Instantiate(sparkPrefab, collision.GetContact(0).point, Quaternion.identity);
            }
        }
    }

    [PunRPC]
    private void PhotonSetState(bool active) {
        gameObject.SetActive(active);
    }

    public void SetActive(bool active) {
        if (PhotonNetwork.IsConnected) {
            pView.RPC("PhotonSetState", RpcTarget.AllBuffered, active);
        }
        else {
            gameObject.SetActive(active);
        }
    }
}
