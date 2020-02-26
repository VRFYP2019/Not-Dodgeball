using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmScreen : MonoBehaviour {
    private float scaleX, scaleY, scaleZ;
    private Transform trans;
    [SerializeField]
    private Image
        ballImage = null,
        heartImage = null;
    [SerializeField]
    private Text
        ballText = null,
        heartText = null;
    private RawImage image;
    private Camera cam = null;
    // Start is called before the first frame update
    void Start() {
        cam = GetComponentInParent<Human>().gameObject.GetComponentInChildren<Camera>();
        if (cam == null) {
            Debug.LogWarning("Camera failed to assign in arm screen");
            enabled = false;
        }
        image = GetComponent<RawImage>();
        trans = GetComponent<Transform>();
        scaleX = trans.localScale.x;
        scaleY = trans.localScale.y;
        scaleZ = trans.localScale.z;
    }

    // Update is called once per frame
    void Update() {
        if (Vector3.Dot(transform.right, cam.transform.up) > 0) {
            trans.localScale = new Vector3(-scaleX, -scaleY, scaleZ);
        } else {
            trans.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }
        ballText.text = BallManager.LocalInstance.playerBallQueue.childCount.ToString();
        if (MovesenseSubscriber.instance != null && MovesenseSubscriber.instance.heartRate != null) {
            heartText.text = MovesenseSubscriber.instance.heartRate.ToString();
        } else {
            heartText.text = "--";
        }
        float a = (
                Vector3.Dot(transform.forward, cam.transform.forward) > 0.5f)
                && (!(Mathf.Abs(Vector3.Dot(transform.right, cam.transform.up)) < 0.8f)
            )
            ? Vector3.Dot(transform.forward, cam.transform.forward)
            : 0;
        ChangeTransparency(a);
    }

    private void ChangeTransparency(float alpha) {
        Color c = ballText.color;
        ballText.color = new Color(c.r, c.g, c.b, alpha);
        heartText.color = new Color(c.r, c.g, c.b, alpha);
        c = image.color;
        ballImage.color = new Color(c.r, c.g, c.b, alpha);
        heartImage.color = new Color(c.r, c.g, c.b, alpha);
        image.color = new Color(c.r, c.g, c.b, alpha);
    }
}
