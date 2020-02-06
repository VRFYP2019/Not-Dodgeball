using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to hold objects that would otherwise lag on enable/instantiate
// Objects added here will increase initial load time
// Currently does not handle the presence of colliders in such objects
public class Preloader : MonoBehaviour {
    private GameObject[] allChildren;

    private void Start() {
        allChildren = new GameObject[transform.childCount];
        int i = 0;

        //Find all child obj and store to that array
        foreach (Transform child in transform) {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        StartCoroutine(DestroyChildren());
    }

    // Destroy to free up memory, but destroying is resource intensive
    // Hence, stagger the destruction
    private IEnumerator DestroyChildren() {
        foreach (GameObject go in allChildren) {
            yield return new WaitForEndOfFrame();
            Destroy(go);
        }
    }
}
