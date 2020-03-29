using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalIndicator : MonoBehaviourPunCallbacks {
    private static readonly int LENGTH_LINERENDERER = 3;
    private static readonly float LENGTH_OFFSET = 1.5f;
    [SerializeField]
    private GoalType goalType;
    private LineRenderer goalIndicator;
    [SerializeField]
    private Transform playerHead = null;

    void Start() {
        if (!PhotonNetwork.IsConnected || photonView.IsMine) {
            InitGoalIndicator();
        }
    }

    private void InitGoalIndicator() {
        if (playerHead == null) {
            playerHead = Camera.main.transform;
        }

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
        if (!PhotonNetwork.IsConnected || photonView.IsMine) {
            if (goalType == GoalType.REGULAR) {
                DrawRegGoalIndicator();
            } else if (goalType == GoalType.HORIZONTAL_WALL || goalType == GoalType.VERITCAL_WALL) {
                DrawWallGoalIndicator();
            }
        }
    }

    private void DrawRegGoalIndicator() {
        goalIndicator.positionCount = 2;
        goalIndicator.SetPosition(0, this.transform.position);
        goalIndicator.SetPosition(1, -this.transform.forward * LENGTH_LINERENDERER + this.transform.position);
    }

    private void DrawWallGoalIndicator() {
        float lengthToRender = Vector3.Distance(playerHead.position, this.transform.position) + LENGTH_OFFSET;
        goalIndicator.positionCount = 2;
        goalIndicator.SetPosition(0, this.transform.position);
        goalIndicator.SetPosition(1, -this.transform.forward * lengthToRender + this.transform.position);
    }
}
