using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallIndicator : MonoBehaviour {
    public Transform playerLocation;
    public RawImage leftIndicator, rightIndicator, backIndicator;
    private Color indicatorColor = new Color32(255,50,50,1);
    private static readonly float X_MIN = -2f, X_MAX = 2f, Z_MIN = -7f, Z_MAX =2f;
    private static readonly float WALL_THRESHOLD_DIST = 1f;
    private static readonly float ALPHA_MULTIPLIER = 1.3f;

    private float distFromLWall, distFromRWall, distFromBWall;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        distFromLWall = playerLocation.position.x - X_MIN;
        distFromRWall = X_MAX - playerLocation.position.x;
        distFromBWall = playerLocation.position.z - Z_MIN;

        if (distFromLWall <= WALL_THRESHOLD_DIST) {
            leftIndicator.color = SetIndicatorColor(HandleColorAlpha(distFromLWall)); 
        }

        if (distFromRWall <= WALL_THRESHOLD_DIST) {
            rightIndicator.color = SetIndicatorColor(HandleColorAlpha(distFromRWall)); 
        }

        if (distFromBWall <= WALL_THRESHOLD_DIST) {
            backIndicator.color = SetIndicatorColor(HandleColorAlpha(distFromBWall)); 
        }
    }

    private float HandleColorAlpha(float distFromWall) {
        float alpha = (WALL_THRESHOLD_DIST - distFromWall)/WALL_THRESHOLD_DIST * ALPHA_MULTIPLIER;
        return Mathf.Clamp(alpha, 0, 1);
    }

    private Color SetIndicatorColor(float a) {
        float r = indicatorColor.r;
        float g = indicatorColor.g;
        float b = indicatorColor.b;
        return new Color(r, g, b, a);
    }
}
