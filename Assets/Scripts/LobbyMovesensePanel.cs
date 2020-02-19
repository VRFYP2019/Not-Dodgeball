using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMovesensePanel : MonoBehaviour {
    [SerializeField]
    private GameObject connectMovesenseButton = null;
    [SerializeField]
    private GameObject movesenseConnectedTextPanel = null;
    [SerializeField]
    private Text heartRateText = null;

    // Start is called before the first frame update
    void Start() {
        // No Oculus no Movesense
        if (OVRPlugin.productName == null || !OVRPlugin.productName.StartsWith("Oculus")) {
            return;
        }

        if (FindObjectOfType<MovesenseController>() == null
            || !MovesenseDevice.IsAnyConnectedOrConnecting()) {
            OnMovesenseDisconnected();
        } else {
            OnMovesenseConnected();
        }
    }

    private void Update() {
        if (MovesenseSubscriber.instance == null) { // do nothing in update if not connected
            return;
        }
        if (!MovesenseSubscriber.instance.isConnected) {
            OnMovesenseDisconnected();
        }
        if (MovesenseSubscriber.instance.heartRate != null) {
            heartRateText.text = MovesenseSubscriber.instance.heartRate.ToString();
        } else {    // if heartrate is null
            heartRateText.text = ("--");
        }
    }

    private void OnMovesenseConnected() {
        connectMovesenseButton.SetActive(false);
        movesenseConnectedTextPanel.SetActive(true);
    }

    private void OnMovesenseDisconnected() {
        heartRateText.text = ("--");
        connectMovesenseButton.SetActive(true);
        movesenseConnectedTextPanel.SetActive(false);
    }

    public void GoToScanScene() {
        SceneManager.LoadScene("ScanScene");
    }
}
