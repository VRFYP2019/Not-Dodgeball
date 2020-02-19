using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovesenseSubscriber : MonoBehaviour {
    public static MovesenseSubscriber instance;
    public string MacID;
    public string Serial;
    public int? heartRate = null;
    public bool isConnected = false;

    private void Awake() {
        instance = this;
    }
    // Start is called before the first frame update
    void Start() {
        MovesenseController.Event += OnMovesenseControllerCallbackEvent;
        DontDestroyOnLoad(this);
    }

    void OnMovesenseControllerCallbackEvent(object sender, MovesenseController.EventArgs e) {
        switch (e.Type) {
            case MovesenseController.EventType.CONNECTED:       // a sensor succesfully connected
                isConnected = true;
                break;
            case MovesenseController.EventType.DISCONNECTED:    // a sensor disconnected
                isConnected = false;
                heartRate = null;
                break;
            case MovesenseController.EventType.NOTIFICATION:    // got data from a sensor
                for (int i = 0; i < e.OriginalEventArgs.Count; i++) {
                    var ne = (NotificationCallback.EventArgs)e.OriginalEventArgs[i];
                    RefreshHeartrate(ne);
                }
                break;
            default:
                break;
        }
    }

    void RefreshHeartrate(NotificationCallback.EventArgs e) {
        if (e == null) {
            return;
        }

        if (e.Subscriptionpath == SubscriptionPath.HeartRate) {
            var notificationHeartRateArgs = (NotificationCallback.HeartRateArgs)e;
            heartRate = (int)notificationHeartRateArgs.Pulse;
        }
    }
}
