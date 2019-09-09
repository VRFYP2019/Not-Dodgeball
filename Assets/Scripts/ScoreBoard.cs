using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Displays the scores of players and time left for the game
// Display scores will no longer update once time is over
public class ScoreBoard : MonoBehaviour {

    public Text blueScoreText, orangeScoreText, timeLeftText;
    public bool isTimeOver;

    [SerializeField]
    private GameObject blueGoal; //, orangeGoal;
    private Goal blueGoalScript; //, orangeGoalScript;
    private float timeLeft = 60.0f; // 1min TODO: get a set time from another script

    // Start is called before the first frame update
    void Start() {
        init();
    }

    private void init() {
        blueScoreText.text = "0";
        //orangeScoreText.text = "0";
        timeLeftText.text = timeLeft.ToString();
        isTimeOver = false;

        blueGoalScript = blueGoal.GetComponentInChildren<Goal>();
        //orangeGoalScript = orangeGoal.GetComponentsInChildren<Goal>();
    }

    // Update is called once per frame
    void Update() {
        if(!isTimeOver) {
            countdownTime();
            updateTimeLeft();
            updateDisplayScores();
        }
    }

    private void countdownTime() {
        timeLeft -= Time.deltaTime;
    }

    // Displays time left in minutes and seconds
    private void updateTimeLeft() {
        float minutes = Mathf.Floor(timeLeft / 60);
        float seconds = timeLeft % 60;
        if(seconds > 59){
            seconds = 59;
        }
        if(minutes < 0) {
            isTimeOver = true;
            minutes = 0;
            seconds = 0;
        }

        timeLeftText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    // Displays scores of both players
    private void updateDisplayScores() {
        blueScoreText.text = blueGoalScript.getPlayerScore().ToString();
        //orangeScoreText.text = orangeGoalScript.getPlayerScore().ToString();
    }
}
