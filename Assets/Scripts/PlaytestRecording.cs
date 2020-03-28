using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class PlaytestRecording : MonoBehaviour {
    private static readonly string fileName = "PlayTestLog.txt";
    private static int
        numThrows,
        numHits,
        numGoalsScored,
        caloriesBurnt,
        roundDuration;
    private static float distHeadMoved;
    private static GoalType goalType;

    // Start is called before the first frame update
    void Start() {
        ResetLog();
        GameManager.Instance.RestartEvent.AddListener(ResetLog);
    }

    private void ResetLog() {
        numThrows = 0;
        numHits = 0;
        numGoalsScored = 0;
        caloriesBurnt = 0;
        distHeadMoved = 0.0f;
        // do not reset goalType
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

    public static void RecordGoalType(GoalType type) {
        goalType = type;
    }

    public static void RecordRoundDuration(int duration) {
        roundDuration = duration;
    }

    public static void RecordDistHeadMoved(float dist) {
        distHeadMoved = dist;
    }

    public static void WriteLog() {
        string path;
        #if UNITY_EDITOR
        path = "Assets/Resources/" + fileName;
        #elif UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)) {
            Debug.Log("No write permission. Not recording log for this game");
            return;
        } else {
            path = Application.persistentDataPath + "/" + fileName;
        }
        #else
        path = Application.persistentDataPath + "/" + fileName;
        #endif

        //Write some text to the test.txt file
        StreamWriter sw = new StreamWriter(path, true);
        sw.Write("\n");
        sw.WriteLine("===== Playtest Log =====");
        sw.WriteLine("------------------------");
        sw.WriteLine("Timestamp: " + System.DateTime.Now.ToString("F"));
        sw.Write("\n");

        // Log stuff here
        sw.WriteLine("1. Number of Balls Thrown:" + numThrows);
        sw.WriteLine("2. Number of Balls Hit:" + numHits);
        sw.WriteLine("3. Number of Goals Scored:" + numGoalsScored);
        sw.WriteLine("4. Number of Calories Burnt:" + caloriesBurnt);
        sw.WriteLine("5. Type of Goal:" + goalType.ToString());
        sw.WriteLine("6. Round Duration:" + roundDuration + (roundDuration == 1 ? " minute" : " minutes"));
        sw.WriteLine("7. Total Head Distance Moved:" + distHeadMoved);

        sw.Write("\n");
        sw.WriteLine("------------------------");
        sw.WriteLine("========================");
        sw.Close();
    }
}
