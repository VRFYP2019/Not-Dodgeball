using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls what the hand  is doing, i.e. spawning or smacking
public class HandController : MonoBehaviour {
    Tool tool;
    SpawnerHand spawnerHand;
    // true if spawnerHand is supposed to be active, false otherwise
    public bool isSpawning;

    // Start is called before the first frame update
    void Start() {
        tool = GetComponentInChildren<Tool>();
        spawnerHand = GetComponentInChildren<SpawnerHand>();
    }

    public void SwitchToTool() {
        isSpawning = false;
        tool.gameObject.SetActive(true);
        spawnerHand.gameObject.SetActive(false);
    }

    public void SwitchToSpawnerHand() {
        isSpawning = true;
        tool.gameObject.SetActive(false);
        spawnerHand.gameObject.SetActive(true);
        StartCoroutine(spawnerHand.TrySpawn());
    }
}
