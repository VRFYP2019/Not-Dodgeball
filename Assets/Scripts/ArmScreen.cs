using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArmScreen : MonoBehaviour {
    private readonly Color[] colors = new Color[2];
    private float scaleX, scaleY, scaleZ;
    private Transform trans;
    private Text text;
    private RawImage image;
    [SerializeField]
    private Camera cam = null;
    // Start is called before the first frame update
    void Start() {
        text = GetComponentInChildren<Text>();
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
        text.text = "Balls: " + BallManager.Instance.playerBallQueues[0].childCount.ToString();
        float a = Vector3.Dot(transform.forward, cam.transform.forward) > 0.5f
            && !(Mathf.Abs(Vector3.Dot(transform.right, cam.transform.up)) < 0.8f) ? Vector3.Dot(transform.forward, cam.transform.forward) : 0;
        ChangeTransparency(a);
    }

    private void ChangeTransparency(float alpha) {
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, alpha);
        c = image.color;
        image.color = new Color(c.r, c.g, c.b, alpha);
    }
}
