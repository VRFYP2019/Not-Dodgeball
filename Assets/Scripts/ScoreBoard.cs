using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Displays the scores of players and time left for the game
// Display scores will no longer update once time is over
public class ScoreBoard : MonoBehaviour {

    public Text player1ScoreText, player2ScoreText, timeLeftText;
    public bool isTimeOver;
    // Set to public to allow setting via PlayerManager script
    public GameObject player1Goal = null, player2Goal = null;
    private Goal player1GoalScript, player2GoalScript;
    private float timeLeft;
    private IEnumerator restartPromptCoroutine;
    private static readonly string timeOver = "TIME OVER";
    //private static readonly string pressTriggerToRestart = "PRESS TRIGGER TO RESTART";

    // Start is called before the first frame update
    void Start() {
        restartPromptCoroutine = TimeOverRestartPrompt();
        Init();
    }

    public void Init() {
        timeLeft = GameManager.Instance.gameDuration;
        player1ScoreText.text = "0";
        player2ScoreText.text = "0";
        timeLeftText.text = timeLeft.ToString();
        isTimeOver = false;

        player1GoalScript = player1Goal.GetComponentInChildren<Goal>();
        player2GoalScript = player2Goal.GetComponentInChildren<Goal>();
    }

    // Update is called once per frame
    void Update() {
        if (!isTimeOver) {
            CountdownTime();
            UpdateTimeLeft();
            UpdateDisplayScores();
        }
    }

    private void CountdownTime() {
        timeLeft -= Time.deltaTime;
    }

    // Displays time left in minutes and seconds
    private void UpdateTimeLeft() {
        float minutes = Mathf.Floor(timeLeft / 60);
        float seconds = timeLeft % 60;
        if (seconds > 59){
            seconds = 59;
        }
        if (minutes < 0) {
            isTimeOver = true;
            GameManager.Instance.isGameEnded = true;
            StartCoroutine(restartPromptCoroutine);
            minutes = 0;
            seconds = 0;
        }

        timeLeftText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    // Displays scores of both players
    private void UpdateDisplayScores() {
        // Note: player 1 goal refers to goal attached to player 1.
        // Hence, player 1's score is dependent on player 2 goal and vice versa.
        player1ScoreText.text = player2GoalScript.GetPlayerScore().ToString();
        player2ScoreText.text = player1GoalScript.GetPlayerScore().ToString();
    }

    public void Restart() {
        StopCoroutine(restartPromptCoroutine);
        // TODO: remove these lines after moving scoring tracking from Goal to something else
        player1GoalScript.ResetPlayerScore();
        player2GoalScript.ResetPlayerScore();
        Init();
    }

    private IEnumerator TimeOverRestartPrompt() {
        while (true) {
            yield return new WaitForSeconds(3f);
            timeLeftText.text = timeOver;
            yield return new WaitForSeconds(3f);
            // uncomment following line after figuring out resizing the text
            //timeLeftText.text = pressTriggerToRestart;
        }
    }
}
