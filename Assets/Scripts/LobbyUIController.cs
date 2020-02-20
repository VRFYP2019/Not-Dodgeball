using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles transitions between menus in lobby
public class LobbyUIController : MonoBehaviour {
    [SerializeField]
    private RectTransform
        networkMenu = null,
        movesenseMenu = null;

    private RectTransform currMenu;

    private void Start() {
        currMenu = networkMenu;
    }

    public void GoToNetworkMenu() {
        StartCoroutine(GoToTargetMenu(networkMenu));
    }

    public void GoToMovesenseMenu() {
        StartCoroutine(GoToTargetMenu(movesenseMenu));
    }

    IEnumerator GoToTargetMenu(RectTransform targetMenu) {
        while (currMenu.localScale.x > 0) {
            currMenu.localScale = new Vector3(
                currMenu.localScale.x - 0.05f,
                1,
                1
            );
            yield return new WaitForEndOfFrame();
        }

        currMenu = targetMenu;

        while (targetMenu.localScale.x < 1) {
            targetMenu.localScale = new Vector3(
                targetMenu.localScale.x + 0.05f,
                1,
                1
            );
            yield return new WaitForEndOfFrame();
        }
    }
}
