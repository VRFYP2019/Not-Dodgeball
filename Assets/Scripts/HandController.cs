using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls what the hand  is doing, i.e. spawning or smacking
public class HandController : MonoBehaviour {
    private Tool tool;
    private Spawner spawnerHand;

    // Start is called before the first frame update
    void Start() {
        tool = GetComponentInChildren<Tool>();
        spawnerHand = GetComponentInChildren<Spawner>();
    }

    public void Switch() {
        if (tool.gameObject.activeInHierarchy) {
            SwitchToSpawnerHand();
        } else {
            SwitchToTool();
        }
    }

    public void SwitchToTool() {
        if (spawnerHand.currentBall != null) {
            spawnerHand.UnspawnBall();
        }
        tool.gameObject.SetActive(true);
        spawnerHand.gameObject.SetActive(false);
    }

    // TODO: switch to default controller instead of tool
    public void ResetSpawnerStateAndSwitchToTool() {
        spawnerHand.RestartState();
        SwitchToTool();
    }

    public void SwitchToSpawnerHand() {
        tool.gameObject.SetActive(false);
        spawnerHand.gameObject.SetActive(true);
        StartCoroutine(spawnerHand.TrySpawn());
    }
}
