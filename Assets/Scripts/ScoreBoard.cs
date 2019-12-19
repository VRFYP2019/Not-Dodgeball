using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Displays the scores of players and time left for the game
// Display scores will no longer update once time is over
public class ScoreBoard : MonoBehaviour {

    public Text player1ScoreText, player2ScoreText, timeLeftText;
    public bool isTimeOver;
    private IEnumerator restartPromptCoroutine;
    private int fontSizeNormal = 25;
    private int fontSizeSmall = 12;
    private static readonly string timeOver = "TIME OVER";
    private static readonly string pressTriggerToRestart = "PRESS TRIGGER TO RESTART";

    // Start is called before the first frame update
    void Start() {
        restartPromptCoroutine = TimeOverRestartPrompt();
        Init();
        GameManager.Instance.RestartEvent.AddListener(Restart);
        GameManager.Instance.TimeOverEvent.AddListener(TimeOverHandler);
    }

    public void Init() {
        player1ScoreText.text = "0";
        player2ScoreText.text = "0";
        timeLeftText.fontSize = fontSizeNormal;
        isTimeOver = false;
    }

    // Update is called once per frame
    void Update() {
        if (!isTimeOver) {
            UpdateTimeLeft();
            UpdateDisplayScores();
        }
    }

    // Displays time left in minutes and seconds
    private void UpdateTimeLeft() {
        float timeLeft = GameManager.Instance.timeLeft;
        float minutes = Mathf.Floor(timeLeft / 60);
        float seconds = timeLeft % 60;
        if (seconds > 59){
            seconds = 59;
        }
        if (minutes < 0) {
            minutes = 0;
            seconds = 0;
        }

        timeLeftText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    // Displays scores of both players
    private void UpdateDisplayScores() {
        player1ScoreText.text = ScoreManager.Instance.playerScores[0].ToString();
        player2ScoreText.text = ScoreManager.Instance.playerScores[1].ToString();
    }

    public void TimeOverHandler() {
        isTimeOver = true;
        StartCoroutine(restartPromptCoroutine);
    }

    public void Restart() {
        StopCoroutine(restartPromptCoroutine);
        Init();
    }

    private IEnumerator TimeOverRestartPrompt() {
        while (true) {
            timeLeftText.fontSize = fontSizeNormal;
            timeLeftText.text = timeOver;
            yield return new WaitForSeconds(3f);
            // TODO? use scrolling text to keep normal font size
            timeLeftText.fontSize = fontSizeSmall;
            timeLeftText.text = pressTriggerToRestart;
            yield return new WaitForSeconds(3f);
        }
    }
}
