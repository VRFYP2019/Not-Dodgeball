using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour {
    [SerializeField]
    private GameObject networkUi = null;

    [Header("Desktop")]
    [SerializeField]
    private GameObject[] desktopObjects = null;

    [Header("OVR")]
    [SerializeField]
    private GameObject[] oculusObjects = null;
    [SerializeField]
    private Camera oculusCam = null;
    
    // Start is called before the first frame update
    void Start() {
        if (OVRPlugin.productName != null && OVRPlugin.productName.StartsWith("Oculus")) {
            foreach (GameObject go in desktopObjects) {
                go.SetActive(false);
            }
            foreach (GameObject go in oculusObjects) {
                go.SetActive(true);
            }
            Canvas c = networkUi.GetComponent<Canvas>();
            c.renderMode = RenderMode.WorldSpace;
            c.worldCamera = oculusCam;
            RectTransform rt = networkUi.GetComponent<RectTransform>();
            rt.position = new Vector3(0, 1, 400);
            rt.sizeDelta = new Vector2(800, 400);
        }
    }
}
