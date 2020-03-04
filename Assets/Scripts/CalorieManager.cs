﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalorieManager : MonoBehaviourPunCallbacks {
    public Canvas calorieCanvas;
    public GameObject
        useLastSavedPanel,
        genderPanel,
        agePanel,
        weightPanel,
        caloriePanel;
    public Text caloriesBurntText;
    public Canvas rematchCanvas;

    private bool isMovesenseConncted = false;
    private float matchTime = 0.0f;
    private float timeElapsed;

    private List<int?> heartRateBuffer = new List<int?>();
    private List<int?> aveHeartRates = new List<int?>();
    private bool isCalculating = false;

    void Start() {
        GameManager.Instance.TimeOverEvent.AddListener(ShowCalorieUI);
        // If player connected a movesense device
        if (MovesenseSubscriber.instance != null && MovesenseSubscriber.instance.heartRate != null) {
            Debug.Log("CalorieManager: Movesense is CONNECTED, recording heartrate");
            isMovesenseConncted = true;
            isCalculating = false;
            GameManager.Instance.TimeOverEvent.AddListener(StopRecordingHeartRate);
            GameManager.Instance.RestartEvent.AddListener(ResetRecordingHeartRate);
            InvokeRepeating("RecordAveHeartRate", 0, 1.0f);
        } else {
            Debug.Log("CalorieManager: Movesense is NOT CONNECTED, not recording heartrate");
            isMovesenseConncted = false;
        }
        calorieCanvas.gameObject.SetActive(false);
    }

    void Update() {
        if (isMovesenseConncted && !isCalculating) {
            heartRateBuffer.Add(MovesenseSubscriber.instance.heartRate);
        }
    }
 
    // Records the average of 1 seconds worth of heartrates
    public void RecordAveHeartRate() {
        isCalculating = true;
        int sum = 0;
        foreach (int heartrate in heartRateBuffer) {
            sum += heartrate;
        }
        int aveThatSecond = (int)Mathf.Round(sum / (float)heartRateBuffer.Count);
        aveHeartRates.Add(aveThatSecond);
        heartRateBuffer.Clear();
        isCalculating = false;
    }

    public void StopRecordingHeartRate() {
        if (isMovesenseConncted) {
            Debug.Log("CalorieManager: Heartrate Recording STOPPED");
            isCalculating = true;
            CancelInvoke("RecordAveHeartRate");
        }
    }

    public void ResetRecordingHeartRate() {
        if (isMovesenseConncted) {
            Debug.Log("CalorieManager: Heartrate Recording RESTARTED");
            heartRateBuffer.Clear();
            aveHeartRates.Clear();
            isCalculating = false;
            InvokeRepeating("RecordAveHeartRate", 0, 1.0f);
        }
    }

    public int CalculateCalories() {
        isCalculating = true;
        int age = GetComponent<AgeSelection>().GetPlayerAge();
        bool isPlayerMale = GetComponent<GenderSelection>().IsPlayerMale();
        int weight = GetComponent<WeightSelection>().GetPlayerWeight();

        int calories = 0;
        if (isMovesenseConncted) {
            if (isPlayerMale) {
                //Calories Burned = [(Age x 0.2017) — (Weight x 0.09036) + (Heart Rate x 0.6309) — 55.0969] x Time / 4.184
                foreach (int heartrate in aveHeartRates) { // since ave heart rate is for 1 second
                    calories += (int)Mathf.Round((((float)age * 0.2017f) - ((float)weight * 0.09036f) + ((float)heartrate * 0.6309f)) / 4.184f);
                }
            } else {
                //Calories Burned = [(Age x 0.074) — (Weight x 0.05741) + (HeartRate x 0.4472) — 20.4022] x Time / 4.184
                foreach (int heartrate in aveHeartRates) { // since ave heart rate is for 1 second
                    calories += (int)Mathf.Round((((float)age * 0.074f) - ((float)weight * 0.05741f) + ((float)heartrate * 0.4472f)) / 4.184f);
                }
            }
        }

        return calories;
    }

    public void ShowCalorieUI() {
        Debug.Log("CalorieManager: Match over showing calorie UI");

        // If prefs are present, show last saved panel
        if (PlayerPrefs.HasKey("isPlayerMale")) {
            useLastSavedPanel.SetActive(true);
            genderPanel.SetActive(false);
        } else {
            useLastSavedPanel.SetActive(false);
            genderPanel.SetActive(true);

        }
        agePanel.SetActive(false);
        weightPanel.SetActive(false);
        caloriePanel.SetActive(false);
        calorieCanvas.gameObject.SetActive(true);

        #if !UNITY_EDITOR
        OculusUIHandler.instance.laserLineRenderer.enabled = true;
        #endif
    }

    public void ToggleNextPanel() {
        bool useLastSaved = false;
        if (useLastSavedPanel.activeInHierarchy) {
            useLastSavedPanel.SetActive(false);
            useLastSaved = GetComponent<LastSavedProfileSelection>().IsUseLastSaved();
            if (useLastSaved) { // skip to calorie panel directly
                caloriePanel.SetActive(true);
                // Disable re-saving to prevent user confusion
                caloriePanel.GetComponentInChildren<Toggle>(true).gameObject.SetActive(false);
                int caloriesBurnt = CalculateCalories();
                caloriesBurntText.text = caloriesBurnt.ToString();
            } else {
                genderPanel.SetActive(true);
                caloriePanel.GetComponentInChildren<Toggle>(true).gameObject.SetActive(true);
            }
            return;
        }
        if (genderPanel.activeInHierarchy) {
            genderPanel.SetActive(false);
            agePanel.SetActive(true);
            return;
        }
        if (agePanel.activeInHierarchy) {
            agePanel.SetActive(false);
            weightPanel.SetActive(true);
            return;
        }
        if (weightPanel.activeInHierarchy) {
            weightPanel.SetActive(false);
            caloriePanel.SetActive(true);

            int caloriesBurnt = CalculateCalories();
            caloriesBurntText.text = caloriesBurnt.ToString();
            return;
        }
        if (caloriePanel.activeInHierarchy) {
            // If last saved profile was chosen, no re-saving
            if (!useLastSaved) {
                ProfileSavingSelection psp = GetComponent<ProfileSavingSelection>();
                if (psp.IsSaveProfile()) {
                    psp.SaveProfile();
                }
            }
            caloriePanel.SetActive(false);
            calorieCanvas.gameObject.SetActive(false);
            rematchCanvas.gameObject.SetActive(true);
            return;
        }
    }
}
