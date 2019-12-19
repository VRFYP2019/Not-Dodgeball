using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Utils;

public class PlayerLoader : MonoBehaviour {
    [SerializeField]
    private GameObject
        OpenVRPrefab = null,
        OculusPrefab = null,
        EditorPlayerPrefab = null,
        DevCamPrefab = null,
        BotPrefab = null,
        OffScreenUIPrefab = null,
        BallManagerPrefab = null;
    [SerializeField]
    private Transform
        player1SpawnPoint = null,
        player2SpawnPoint = null;

    // Start is called before the first frame update
    void Start() {
        GameObject humanPrefab;
        switch (GameManager.Instance.playerPlatform) {
            case PlayerPlatform.OCULUS:
                humanPrefab = OculusPrefab;
                break;
            case PlayerPlatform.STEAMVR:
                humanPrefab = OpenVRPrefab;
                break;
            case PlayerPlatform.EDITOR:
            default:
                humanPrefab = EditorPlayerPrefab;
                break;
        }
        if (PhotonNetwork.IsConnected) {
            switch (PhotonNetwork.CurrentRoom.PlayerCount) {
                case 1:
                    SpawnHuman(PlayerNumber.ONE);
                    SpawnBot();
                    break;
                case 2:
                    if (PhotonNetwork.LocalPlayer.ActorNumber == 1) {
                        SpawnHuman(PlayerNumber.ONE);
                    } else {
                        SpawnHuman(PlayerNumber.TWO);
                    }
                    break;
                default:
                    Debug.LogError("Invalid number of players. Returning to lobby");
                    PhotonNetwork.LoadLevel(0);
                    return;
            }
            GameObject ballManager = PhotonNetwork.Instantiate(BallManagerPrefab.name, Vector3.zero, Quaternion.identity);
        } else {
            SpawnHuman(PlayerNumber.ONE);
            SpawnBot();
            GameObject ballManager = Instantiate(BallManagerPrefab);
        }
        if (GameManager.Instance.playerPlatform == PlayerPlatform.EDITOR) {
            Instantiate(DevCamPrefab);
        }
    }

    private void SpawnHuman(PlayerNumber playerNumber) {
        GameObject player, humanPrefab;
        Transform spawnPoint;
        switch (GameManager.Instance.playerPlatform) {
            case PlayerPlatform.OCULUS:
                humanPrefab = OculusPrefab;
                break;
            case PlayerPlatform.STEAMVR:
                humanPrefab = OpenVRPrefab;
                break;
            case PlayerPlatform.EDITOR:
            default:
                humanPrefab = EditorPlayerPrefab;
                break;
        }
        switch (playerNumber) {
            case PlayerNumber.ONE:
                spawnPoint = player1SpawnPoint;
                break;
            case PlayerNumber.TWO:
                spawnPoint = player2SpawnPoint;
                break;
            default:
                Debug.LogWarning("Player number is null. Player will not be spawned");
                return;
        }

        if (PhotonNetwork.IsConnected) {
            player = PhotonNetwork.Instantiate(humanPrefab.name, spawnPoint.position, spawnPoint.rotation);
            player.name = PhotonNetwork.LocalPlayer.NickName;
        } else {
            player = Instantiate(humanPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        GameObject offScreenUI = Instantiate(OffScreenUIPrefab, OffScreenUIPrefab.transform.position, OffScreenUIPrefab.transform.rotation);
        offScreenUI.GetComponent<WallIndicator>().playerLocation = player.GetComponentInChildren<Camera>(true).transform;
        offScreenUI.GetComponent<Canvas>().worldCamera = player.GetComponentInChildren<Camera>();
        player.GetComponent<Player>().SetPlayerNumber(playerNumber);
    }

    // Assume bot is always only player 2
    private void SpawnBot() {
        GameObject bot;
        if (PhotonNetwork.IsConnected) {
            bot = PhotonNetwork.Instantiate(BotPrefab.name, player2SpawnPoint.position, player2SpawnPoint.rotation);
        } else {
            bot = Instantiate(BotPrefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
        }
        bot.GetComponent<Player>().playerNumber = PlayerNumber.TWO;
    }
}
