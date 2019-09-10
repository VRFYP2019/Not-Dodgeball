using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the goalpost position and keeps track of score for that player
// Goalpost MUST be a child of the VR camera
// Goalpost is to follow player's position and Y-axis rotation (Yaw)
// TODO: Goalpost cannot move outside the bounds of the room
public class Goal : MonoBehaviour {
    [SerializeField]
    private float offsetX, offsetY, offsetZ;
    private int playerScore;

    // Start is called before the first frame update
    void Start() {
        resetPlayerScore();
    }

    // Update is called once per frame
    void Update() {
        updateGoalPosition();
    }

    private void updateGoalPosition() {
        Vector3 parentPos = transform.parent.position;
        transform.position = new Vector3(parentPos.x + offsetX, parentPos.y + offsetY, parentPos.z + offsetZ);
        transform.rotation = GeneralizedLookRotation(Vector3.up, Vector3.up, Vector3.back, transform.parent.forward); // yaw with parent
    }

    // Takes a vector in local coordinates, localExactAxis, and rotate it to point exactly along globalExactAxis in world coordinates.
    private Quaternion GeneralizedLookRotation(Vector3 localExactAxis, Vector3 globalExactAxis, Vector3 localApproxAxis, Vector3 globalApproxAxis) {
        // Rotate chosen local axes into a standard orientation.
        Quaternion standardize = Quaternion.Inverse( Quaternion.LookRotation(localExactAxis, localApproxAxis));
        // Rotate standard orientation to point to the chosen global axes.
        Quaternion turn = Quaternion.LookRotation(globalExactAxis, globalApproxAxis);
        // Chain both operations to rotate the local axes to the global axes.
        return turn * standardize;
    }

    void OnTriggerEnter(Collider col) {
        addPlayerScore(1);
        Destroy(col.gameObject);
    }

    public void resetPlayerScore() {
        playerScore = 0;
    }

    public void addPlayerScore(int points) {
        playerScore += points;
    }

    public int getPlayerScore() {
        return playerScore;
    }
}
