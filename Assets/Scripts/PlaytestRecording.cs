using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlaytestRecording : MonoBehaviour {
    string  fileName = "PlayTestLog.txt";
    private static int
        numThrows,
        numHits,
        numGoalsScored,
        caloriesBurnt;
    // Start is called before the first frame update
    void Start() {
        ResetLog();
        GameManager.Instance.TimeOverEvent.AddListener(WriteLog);
        GameManager.Instance.RestartEvent.AddListener(ResetLog);
    }

    private void ResetLog() {
        numThrows = 0;
        numHits = 0;
        numGoalsScored = 0;
        caloriesBurnt = 0;

    }

    public static void RecordHit() {
        numHits++;
    }

    public static void RecordThrow() {
        numThrows++;
    }

    public static void RecordGoalScored() {
        numGoalsScored++;
    }

    public static void RecordCaloriesBurnt(int cal) {
        caloriesBurnt = cal;
    }

    private void WriteLog() {
        string path = "Assets/Resources/" + fileName;

        //Write some text to the test.txt file
        StreamWriter sw = new StreamWriter(path, true);
        sw.Write("\n");
        sw.WriteLine("===== Playtest Log =====");
        sw.WriteLine("------------------------");
        sw.Write("\n");

        // Log stuff here
        sw.WriteLine("1. Number of Balls Thrown:" + numThrows);
        sw.WriteLine("2. Number of Balls Hit:" + numHits);
        sw.WriteLine("3. Number of Goals Scored:" + numGoalsScored);
        sw.WriteLine("4. Number of Calories Burnt:" + caloriesBurnt);

        sw.Write("\n");
        sw.WriteLine("Timestamp: " + System.DateTime.Now);
        sw.WriteLine("------------------------");
        sw.WriteLine("========================");
        sw.Close();
    }
}
