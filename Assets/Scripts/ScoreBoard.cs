﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

// Displays the scores of players and time left for the game
// Display scores will no longer update once time is over
public class ScoreBoard : MonoBehaviour {

    public Text player1ScoreText, player2ScoreText, timeLeftText;
    public GameObject timeOverPanel;
    public bool isTimeOver;
    private static readonly string timeOver = "TIME OVER";

    // Start is called before the first frame update
    void Start() {
        Init();
        GameManager.Instance.RestartEvent.AddListener(Init);
        GameManager.Instance.TimeOverEvent.AddListener(TimeOverHandler);
    }

    public void Init() {
        player1ScoreText.text = "0";
        player2ScoreText.text = "0";
        isTimeOver = false;
        timeOverPanel.SetActive(false);
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
        timeOverPanel.GetComponentInChildren<Text>().text = timeOver;
        timeOverPanel.SetActive(true);
    }
}
