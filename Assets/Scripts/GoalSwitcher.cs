using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class GoalSwitcher : MonoBehaviour {
    public GameObject regularGoal;
    public GameObject hWallGoal;
    public GameObject vWallGoal;
    [HideInInspector]
    public UnityEvent GoalInitEvent;

    // Start is called before the first frame update
    void Start() {
        SwitchGoals();
    }

    public void SwitchGoals() {
        GoalType goalType = GoalType.REGULAR;
        PhotonHashtable hash = PhotonNetwork.CurrentRoom.CustomProperties;
        object temp;
        if (hash.TryGetValue("RoomGoalType", out temp)) {
            if (temp is byte) {
                goalType = (GoalType)System.Enum.ToObject(typeof(GoalType), temp);
                Debug.Log("goalType for this game: " + goalType);
            } else {
                Debug.Log("RoomGoalType: unexpected custom property value type");
            }
        } else {
            Debug.Log("RoomGoalType: custom property not found");
        }
        switch (goalType) {
            case GoalType.REGULAR:
                regularGoal.SetActive(true);
                hWallGoal.SetActive(false);
                vWallGoal.SetActive(false);
                break;
            case GoalType.HORIZONTAL_WALL:
                regularGoal.SetActive(false);
                hWallGoal.SetActive(true);
                vWallGoal.SetActive(false);
                break;
            case GoalType.VERITCAL_WALL:
                regularGoal.SetActive(false);
                hWallGoal.SetActive(false);
                vWallGoal.SetActive(true);
                break;
        }
        GoalInitEvent.Invoke();
    }
}
