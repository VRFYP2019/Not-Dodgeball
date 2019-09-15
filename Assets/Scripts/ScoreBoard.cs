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
    private float timeLeft;
    private IEnumerator restartPromptCoroutine;
    private static readonly string timeOver = "TIME OVER";
    //private static readonly string pressTriggerToRestart = "PRESS TRIGGER TO RESTART";

    // Start is called before the first frame update
    void Start() {
        restartPromptCoroutine = TimeOverRestartPrompt();
        Init();
    }

    private void Init() {
        timeLeft = GameManager.Instance.gameDuration;
        blueScoreText.text = "0";
        //orangeScoreText.text = "0";
        timeLeftText.text = timeLeft.ToString();
        isTimeOver = false;

        blueGoalScript = blueGoal.GetComponentInChildren<Goal>();
        //orangeGoalScript = orangeGoal.GetComponentsInChildren<Goal>();
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
        blueScoreText.text = blueGoalScript.GetPlayerScore().ToString();
        //orangeScoreText.text = orangeGoalScript.GetPlayerScore().ToString();
    }

    public void Restart() {
        StopCoroutine(restartPromptCoroutine);
        // TODO: remove these lines after moving scoring tracking from Goal to something else
        blueGoalScript.ResetPlayerScore();
        //orangeGoalScript.ResetPlayerScore();
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
