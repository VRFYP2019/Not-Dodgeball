using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerLoader : MonoBehaviour {
    public GameObject OpenVRPrefab;
    public GameObject OculusPrefab;
    public GameObject EditorPlayerPrefab;
    public GameObject DevCamPrefab;
    public GameObject BotPrefab;
    public GameObject OffScreenUIPrefab;
    public GameObject BallManagerPrefab;
    public Transform player1SpawnPoint;
    public Transform player2SpawnPoint;
    private GameObject player1, player2;
    // Start is called before the first frame update
    void Start() {
        // TODO: add a new prefab for editor
        GameObject humanPrefab = GameManager.Instance.isOculusQuest ? OculusPrefab :
            GameManager.Instance.isEditor ? EditorPlayerPrefab : OpenVRPrefab;
        if (PhotonNetwork.IsConnected) {
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1) {
                player1 = PhotonNetwork.Instantiate(humanPrefab.name, player1SpawnPoint.position, player1SpawnPoint.rotation);
                GameObject offScreenUI1 = Instantiate(OffScreenUIPrefab, OffScreenUIPrefab.transform.position, OffScreenUIPrefab.transform.rotation);
                offScreenUI1.GetComponent<WallIndicator>().playerLocation = player1.GetComponentInChildren<Camera>(true).transform;
                offScreenUI1.GetComponent<Canvas>().worldCamera = player1.GetComponentInChildren<Camera>();
                player1.GetComponent<Player>().SetPlayerNumber(Utils.PlayerNumber.ONE);
                player1.name = PhotonNetwork.LocalPlayer.NickName;
                if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
                    // Spawn a bot if single player
                    player2 = PhotonNetwork.Instantiate(BotPrefab.name, player2SpawnPoint.position, player2SpawnPoint.rotation);
                    player2.GetComponent<Player>().SetPlayerNumber(Utils.PlayerNumber.TWO);
                }
            } else if (PhotonNetwork.LocalPlayer.ActorNumber == 2) {
                player2 = PhotonNetwork.Instantiate(humanPrefab.name, player2SpawnPoint.position, player2SpawnPoint.rotation);
                GameObject offScreenUI2 = Instantiate(OffScreenUIPrefab, OffScreenUIPrefab.transform.position, OffScreenUIPrefab.transform.rotation);
                offScreenUI2.GetComponent<WallIndicator>().playerLocation = player2.GetComponentInChildren<Camera>(true).transform;
                offScreenUI2.GetComponent<Canvas>().worldCamera = player2.GetComponentInChildren<Camera>();
                player2.GetComponent<Player>().SetPlayerNumber(Utils.PlayerNumber.TWO);
                player2.name = PhotonNetwork.LocalPlayer.NickName;
            }
            GameObject ballManager = PhotonNetwork.Instantiate(BallManagerPrefab.name, Vector3.zero, Quaternion.identity);
            ballManager.name = "BallManager " + PhotonNetwork.LocalPlayer.ActorNumber;
        } else {
            player1 = Instantiate(humanPrefab, player1SpawnPoint.position, player1SpawnPoint.rotation);
            GameObject offScreenUI1 = Instantiate(OffScreenUIPrefab, OffScreenUIPrefab.transform.position, OffScreenUIPrefab.transform.rotation);
            offScreenUI1.GetComponent<WallIndicator>().playerLocation = player1.GetComponentInChildren<Camera>(true).transform;
            offScreenUI1.GetComponent<Canvas>().worldCamera = player1.GetComponentInChildren<Camera>();
            player1.GetComponent<Player>().playerNumber = Utils.PlayerNumber.ONE;

            player2 = Instantiate(BotPrefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
            player2.GetComponent<Player>().playerNumber = Utils.PlayerNumber.TWO;

            GameObject ballManager = Instantiate(BallManagerPrefab);
        }
        if (GameManager.Instance.isEditor) {
            Instantiate(DevCamPrefab);
        }
    }
}
