using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Tool : MonoBehaviour {
    private Collider thisCollider;  // calling it "collider" triggers a useless warning so it'll be called "thisCollider" instead
    protected readonly float forceMultiplier = 0.25f;
    protected readonly float angularVelocityMultiplier = 0.01f;  // Used to make behaviour more consistent between flicks and full-body swings
    private List<Material> materials = new List<Material>();
    private readonly Renderer[] renderers;
    private ArmSpeed armSpeed;
    private float fadeStartTime;
    private readonly float velocityUpperThreshold = 2;  // anything above this value will be considered "fast" and treated differently to avoid clipping
    private readonly float velocityLowerThreshold = 0.1f;   // anything below this value will be considered "stationary" and trigger a fade
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

    // Start is called before the first frame update
    protected virtual void Start() {
        thisCollider = GetComponent<Collider>();
        foreach (Renderer r in GetComponents<Renderer>()) {
            materials.AddRange(r.materials);
        }
        armSpeed = GetComponentInParent<ArmSpeed>();
    }

    private void Update() {
        float handVelocityMagnitude = armSpeed.velocity.magnitude;
        HandleFade(handVelocityMagnitude);
        if (armSpeed.velocity.magnitude > velocityLowerThreshold) {
            if (armSpeed.velocity.magnitude < velocityUpperThreshold) {
                thisCollider.isTrigger = false;
            } else {
                thisCollider.isTrigger = true;
            }
        } else if (armSpeed.velocity.magnitude < velocityLowerThreshold) {
            if (fadeState == FadeState.FADED) {
                thisCollider.isTrigger = true;
            }
        }
    }

    // Fade/unfade based on the state of the tool and the magnitude of valocity
    private void HandleFade(float magnitude) {
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

    // Used for slow movement
    protected virtual void OnCollisionEnter(Collision collision) {
        ExertForce(collision.collider.GetComponent<Rigidbody>());
    }

    // Used for fast movement and no movement
    protected virtual void OnTriggerEnter(Collider other) {
        if (fadeState > FadeState.FADED) {
            // TODO: account for bounce using normal of supposed collision point
            ExertForce(other.GetComponent<Rigidbody>());
        }
    }

    // Exert a force on the other rigidbody
    protected virtual void ExertForce(Rigidbody r) {
        // TODO: update this while implementing bot
        // Human force is calculated in ToolHuman.cs and overrides this
        r.AddForce(armSpeed.velocity * forceMultiplier);
    }
}
