using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalIndicator : MonoBehaviour {
    private static readonly int LENGTH_LINERENDERER = 3;
    private LineRenderer goalIndicator;
    PhotonView pv;

    void Start() {
        pv = GetComponent<PhotonView>();
        if (!PhotonNetwork.IsConnected || pv.IsMine) {
            InitGoalIndicator();
        }
    }

    private void InitGoalIndicator() {
        goalIndicator = gameObject.AddComponent<LineRenderer>();
        goalIndicator.material = new Material(Shader.Find("Sprites/Default"));
        goalIndicator.widthMultiplier = 0.025f;
        goalIndicator.positionCount = LENGTH_LINERENDERER;
        goalIndicator.useWorldSpace = true;

        Color c1 = Color.green;
        Color c2 = Color.white;
        float alpha1 = 1.0f;
        float alpha2 = 0.4f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha1, 0.0f), new GradientAlphaKey(alpha2, 1.0f) });
        goalIndicator.colorGradient = gradient;
    }

    // Update is called once per frame
    void Update() {
        if (!PhotonNetwork.IsConnected || pv.IsMine) {
            DrawGoalIndicator();
        }
    }

    private void DrawGoalIndicator() {
        goalIndicator.SetVertexCount(2);
        goalIndicator.SetPosition(0, this.transform.position);
        goalIndicator.SetPosition(1, -this.transform.forward * LENGTH_LINERENDERER + this.transform.position);
    }
}
