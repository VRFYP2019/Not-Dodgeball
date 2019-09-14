using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the goalpost position and keeps track of score for that player
// Goalpost MUST be a child of the VR camera
// Goalpost is to follow player's position and Y-axis rotation (Yaw)
// Goalpost cannot move outside the bounds of the room
public class Goal : MonoBehaviour {
    [SerializeField]
    private float offsetX, offsetY, offsetZ;
    [SerializeField]
    private float xMin, xMax, yMin, yMax, zMin, zMax;
    private Vector3 parentPos, newPos;
    private int playerScore;

    // Start is called before the first frame update
    void Start() {
        resetPlayerScore();
    }

    // Update is called once per frame
    void Update() {
        handleGoalPosition();
    }
    
    private void handleGoalPosition() {
        parentPos = transform.parent.position;
        // Prevent goal from exceeding room bounds
        newPos.x = Mathf.Clamp(parentPos.x + offsetX, xMin, xMax);
        newPos.y = Mathf.Clamp(parentPos.y + offsetY, yMin, yMax);
        newPos.z = Mathf.Clamp(parentPos.z + offsetZ, zMin, zMax);
        updateGoalPosition(newPos);
    }

    private void updateGoalPosition(Vector3 pos) {
        transform.position = pos;
        transform.rotation = GeneralizedLookRotation(Vector3.up, Vector3.up, Vector3.back, transform.parent.forward);
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
