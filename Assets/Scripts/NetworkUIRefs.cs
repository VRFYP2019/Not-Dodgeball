using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUIRefs : MonoBehaviour {
    public static NetworkUIRefs Instance;

    public Text ConnectionStatusText;
    public GameObject LobbyInfoPanel;
    public GameObject RoomInfoPanel;

    [Header("Login Panel")]
    public InputField PlayerNameInput;

    [Header("Create Room Panel")]
    public InputField RoomNameInputField;

    [Header("Room List Panel")]
    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("Room Info Panel")]
    public GameObject RoomInfoContent;
    public Button StartGameButton;
    public GameObject PlayerListEntryPrefab;

    [Header("Host Interactables")]
    public Toggle
        RegGoalToggle,
        VWallGoalToggle,
        HWallGoalToggle;
    public Slider RoundDurationSlider;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    public void PlayerJoinOrCreateRoom() {
        NetworkController.Instance.PlayerJoinOrCreateRoom();
    }

    public void SetLocalPlayerName() {
        NetworkController.Instance.SetLocalPlayerName();
    }

    public void PlayerLeaveRoom() {
        NetworkController.Instance.PlayerLeaveRoom();
    }

    public void PlayerStartGame() {
        NetworkController.Instance.PlayerStartGame();
    }

    public void HostChangedRoomGoalType() {
        NetworkController.Instance.HostChangedRoomGoalType();
    }
}
