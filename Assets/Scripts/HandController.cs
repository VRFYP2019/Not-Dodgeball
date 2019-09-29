using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls what the hand  is doing, i.e. spawning or smacking
public class HandController : MonoBehaviour {
    private Tool tool;
    private Spawner spawnerHand;
    bool hasBeenInit = false;

    // Start is called before the first frame update
    void Start() {
        if (!hasBeenInit) {
            Init();
        }
    }

    private void Init() {
        tool = GetComponentInChildren<Tool>();
        spawnerHand = GetComponentInChildren<Spawner>(true);
        hasBeenInit = true;
    }

    public void Switch() {
        if (tool.gameObject.activeInHierarchy) {
            SwitchToSpawnerHand();
        } else {
            SwitchToTool();
        }
    }

    public void SwitchToTool() {
        if (!hasBeenInit) {
            Init();
        }
        if (spawnerHand.currentBall != null) {
            spawnerHand.UnspawnBall();
        }
        tool.SetState(true);
        tool.SetFollowerActive(true);
        spawnerHand.SetState(false);
    }

    // TODO: switch to default controller instead of tool
    public void ResetSpawnerStateAndSwitchToTool() {
        SwitchToTool();
        spawnerHand.RestartState();
    }

    public void SwitchToSpawnerHand() {
        tool.SetFollowerActive(false);
        tool.SetState(false);
        spawnerHand.SetState(true);
        StartCoroutine(spawnerHand.TrySpawn());
    }
}
