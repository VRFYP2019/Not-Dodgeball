using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls what the hand  is doing, i.e. spawning or smacking
public class HandController : MonoBehaviour {
    private Tool tool;
    private SpawnerHand spawnerHand;
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

    // TODO: switch to default controller instead of tool
    public void ResetSpawnerStateAndSwitchToTool() {
        spawnerHand.RestartState();
        SwitchToTool();
    }

    public void SwitchToSpawnerHand(GameObject existingBall) {
        isSpawning = true;
        tool.gameObject.SetActive(false);
        spawnerHand.gameObject.SetActive(true);
        if (existingBall) {
            spawnerHand.InheritBall(existingBall);
        } else {
            // if no existingBall was passed in, try to spawn a new one
            StartCoroutine(spawnerHand.TrySpawn());
        }
    }
}
