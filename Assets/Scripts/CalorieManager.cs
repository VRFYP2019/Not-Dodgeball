using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalorieManager : MonoBehaviourPunCallbacks {
    public Canvas calorieCanvas;
    public GameObject 
        genderPanel,
        agePanel,
        weightPanel,
        caloriePanel;
    public Text caloriesBurntText;

    private bool isMovesenseConncted = false;
    private float matchTime = 0.0f;
    private float timeElapsed;

    private List<int?> heartRateBuffer = new List<int?>();
    private List<int?> aveHeartRates = new List<int?>();
    private bool isCalculating = false;

    #if UNITY_EDITOR
    private Camera playerCam;
    #endif

    [Header("Desktop")]
    [SerializeField]
    private GameObject[] desktopObjects = null;

    [Header("OVR")]
    [SerializeField]
    private GameObject[] oculusObjects = null;
    private LineRenderer laserLineRenderer = null;

    void Start() {
        if (OVRPlugin.productName != null && OVRPlugin.productName.StartsWith("Oculus")) {
            foreach (GameObject go in desktopObjects) {
                go.SetActive(false);
            }
            foreach (GameObject go in oculusObjects) {
                go.SetActive(true);

                // UIHelpers must be the first go in oculusObjects
                if (laserLineRenderer == null) {
                    laserLineRenderer = go.GetComponentInChildren<LineRenderer>();
                }
            }
        }

        #if UNITY_EDITOR
        Camera[] cams = FindObjectsOfType<Camera>();
        foreach (Camera c in cams) {
            PhotonView pv = c.GetComponentInParent<PhotonView>();
            if (pv == null || !c.isActiveAndEnabled) {
                continue;
            } else {
                playerCam = c;
                break;
            }
        }
        calorieCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        calorieCanvas.worldCamera = playerCam;
        GetComponent<VRFollowCanvas>().enabled = false;
        #endif

        calorieCanvas.gameObject.SetActive(false);
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

    public int calculateCalories() {
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
        genderPanel.SetActive(true);
        agePanel.SetActive(false);
        weightPanel.SetActive(false);
        caloriePanel.SetActive(false);
        calorieCanvas.gameObject.SetActive(true);

        #if !UNITY_EDITOR
        laserLineRenderer.enabled = true;
        #endif
    }

    public void ToggleNextPanel() {
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

            int caloriesBurnt = calculateCalories();
            caloriesBurntText.text = caloriesBurnt.ToString();
            return;
        }
        if (caloriePanel.activeInHierarchy) {
            caloriePanel.SetActive(false);
            calorieCanvas.gameObject.SetActive(false);

            #if !UNITY_EDITOR
            laserLineRenderer.enabled = false;
            #endif
            return;
        }
    }
}
