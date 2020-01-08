using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

// Controls what the hand is doing, i.e. spawning or smacking
public class HandController : MonoBehaviour {
    private Tool tool;
    private Spawner spawnerHand;
    public HandObject currHandObject;
    bool hasBeenInit = false;

    // Start is called before the first frame update
    protected virtual void Start() {
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
        currHandObject = HandObject.TOOL;
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
        currHandObject = HandObject.SPAWNER;
        tool.SetFollowerActive(false);
        tool.SetState(false);
        spawnerHand.SetState(true);
        StartCoroutine(spawnerHand.TrySpawn());
    }
}
