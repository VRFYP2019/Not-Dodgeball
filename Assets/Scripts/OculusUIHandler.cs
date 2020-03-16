using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class OculusUIHandler : MonoBehaviour {
    public static OculusUIHandler instance;
    public Canvas[] canvases;

    #if UNITY_EDITOR
    private Camera playerCam;
    #endif

    [Header("Desktop")]
    [SerializeField]
    private GameObject[] desktopObjects = null;

    [Header("OVR")]
    [SerializeField]
    private GameObject[] oculusObjects = null;
    [HideInInspector]
    public LineRenderer laserLineRenderer = null;

    public bool IsAnyUIOpen {
        get {
            foreach (Canvas canvas in canvases) {
                if (canvas.isActiveAndEnabled) {
                    return true;
                }
            }
            return false;
        }
    }

    private void Awake() {
        instance = this;
    }

    // Start is called before the first frame update
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
            if (SceneManager.GetActiveScene().buildIndex == 0) {
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)) {
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                }
            }
        }

        #if UNITY_EDITOR
        Camera[] cams = FindObjectsOfType<Camera>();
        foreach (Camera c in cams) {
            PhotonView pv = c.GetComponentInParent<PhotonView>();
            if (!c.isActiveAndEnabled) {    // if camera is inactive or disabled, skip
                continue;
            }

            playerCam = c;
            if (pv != null) {   // if there is a photonview, that's the camera we want
                break;
            }
        }
        foreach (Canvas canvas in canvases) {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera = playerCam;
            VRFollowCanvas vRFollowCanvas = canvas.GetComponentInParent<VRFollowCanvas>();
            if (vRFollowCanvas != null) {
                vRFollowCanvas.enabled = false;
            }
        }
        #endif

        if (SceneManager.GetActiveScene().buildIndex == 1) {
            GameManager.Instance.RestartEvent.AddListener(CloseAllUI);
        }
    }

    private void CloseAllUI() {
        foreach (Canvas canvas in canvases) {
            canvas.gameObject.SetActive(false);
        }
        #if !UNITY_EDITOR
        laserLineRenderer.enabled = false;
        #endif
    }
}
