using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

// Controls what the hand is doing, i.e. spawning or smacking
public class HandController : MonoBehaviour {
    private Tool tool;
    private Spawner spawnerHand;
    public HandObject currHandObject;
    public HandSide handSide;
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
        if (handSide == HandSide.RIGHT) {
            spawnerHand.transform.localScale = new Vector3(
                -spawnerHand.transform.localScale.x,
                spawnerHand.transform.localScale.y,
                spawnerHand.transform.localScale.z);
            HandScreen handScreen = spawnerHand.GetComponentInChildren<HandScreen>();
            // Flip the screen back if there is one (dummy bot does not have one)
            if (handScreen != null) {
                Vector3 screenScale = spawnerHand.GetComponentInChildren<HandScreen>().transform.localScale;
                spawnerHand.GetComponentInChildren<HandScreen>().transform.localScale = new Vector3(
                    -screenScale.x, screenScale.y, screenScale.z);
            }
        }
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
        spawnerHand.RestartState();
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
        spawnerHand.PutNextBallInHand();
    }
}
