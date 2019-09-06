using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    public Transform goalPosition;
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
        transform.position = goalPosition.position;
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
