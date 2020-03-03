using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

public class VRFollowCanvas : MonoBehaviour {
    public RectTransform canvasRectTransform; // rectangle transform of canvas
    private Camera playerCam;

    enum CanvasState { 
        InScreen,
        NotInScreen,
        Moving };
    private CanvasState currentState;
    
    void Start () {
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
        currentState = CanvasState.InScreen;
    }
    
    void Update () {
        switch (currentState) {
            case CanvasState.InScreen:
                if (!IsFullyVisibleFrom(canvasRectTransform, playerCam)) {
                    // If the menu isn't fully visible anymore switch to NotInScreen state.
                    currentState = CanvasState.NotInScreen;
                }
                break;
            case CanvasState.NotInScreen:
                // If the menu isn't in the screen anymore, start moving it towards the player.
                currentState = CanvasState.Moving;
                StartCoroutine(MoveToFrontOfPlayer());
                break;
        }
    }

    // Get the foward Vector of camera with offset 
    private Vector3 GetFrontOfPlayer() {
        float distance = 3.0f;
        Vector3 forward = playerCam.transform.position + playerCam.transform.forward * distance;
        return forward;
    }

    // Move canvas to the front of player every frame
    private IEnumerator MoveToFrontOfPlayer() {
        while (Vector3.Distance(transform.position, GetFrontOfPlayer()) >= 0.05f) {
            transform.eulerAngles = playerCam.transform.eulerAngles;
            float speed = 4f * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, GetFrontOfPlayer(), speed);
            yield return null;
        }
        currentState = CanvasState.InScreen;
    }

    // Returns amount of bounding box corners that are visible from the Camera or -1 if a corner isn't in the screen
    private int CountCornersVisibleFrom(RectTransform rectTransform, Camera camera) {
        // Screen space bounds (assumes camera renders across the entire screen)
        Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height);
        Vector3[] objectCorners = new Vector3[4];
        rectTransform.GetWorldCorners(objectCorners);

        int visibleCorners = 0;
        // For each corner in rectTransform
        for (var i = 0; i < objectCorners.Length; i++) {
            // Transform world space position of corner to screen space
            Vector3 tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]);
            // If the corner is inside the screen
            if (screenBounds.Contains(tempScreenSpaceCorner)) {
                visibleCorners++;
            } else {
                return -1;
            }
        }

        return visibleCorners;
    }

    // Determines if this RectTransform is fully visible from the specified camera.
    private bool IsFullyVisibleFrom(RectTransform rectTransform, Camera camera) {
        return CountCornersVisibleFrom(rectTransform, camera) == 4 // True if all 4 corners are visible
            && Vector3.Dot(rectTransform.forward, camera.transform.forward) > 0.5f; // True if at least half aligned
    }
}
