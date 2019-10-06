using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerLoader : MonoBehaviour {
    public GameObject OpenVRPrefab;
    public GameObject OculusPrefab;
    public GameObject BotPrefab;
    public GameObject OffScreenUIPrefab;
    public GameObject BallManagerPrefab;
    public Transform player1SpawnPoint;
    public Transform player2SpawnPoint;
    private GameObject player1, player2;
    // Start is called before the first frame update
    void Start() {
        // TODO: differentiate which is connected
        GameObject humanPrefab = OculusPrefab;
        Vector3 oculusOffset = new Vector3(0, 1.0f, 0); // because y=0 will put you underground
        if (PhotonNetwork.IsConnected) {
            if (PhotonNetwork.LocalPlayer.ActorNumber == 1) {
                player1 = PhotonNetwork.Instantiate(humanPrefab.name, player1SpawnPoint.position + oculusOffset, player1SpawnPoint.rotation);
                GameObject offScreenUI1 = Instantiate(OffScreenUIPrefab, OffScreenUIPrefab.transform.position, OffScreenUIPrefab.transform.rotation);
                offScreenUI1.GetComponent<WallIndicator>().playerLocation = player1.GetComponentInChildren<Camera>(true).transform;
                offScreenUI1.GetComponent<Canvas>().worldCamera = player1.GetComponentInChildren<Camera>();
                player1.GetComponent<Player>().playerNumber = Utils.PlayerNumber.ONE;
                player1.name = PhotonNetwork.LocalPlayer.NickName;
            } else if (PhotonNetwork.LocalPlayer.ActorNumber == 2) {
                player2 = PhotonNetwork.Instantiate(humanPrefab.name, player2SpawnPoint.position, player2SpawnPoint.rotation);
                GameObject offScreenUI2 = Instantiate(OffScreenUIPrefab, OffScreenUIPrefab.transform.position, OffScreenUIPrefab.transform.rotation);
                offScreenUI2.GetComponent<WallIndicator>().playerLocation = player2.GetComponentInChildren<Camera>(true).transform;
                offScreenUI2.GetComponent<Canvas>().worldCamera = player2.GetComponentInChildren<Camera>();
                player2.GetComponent<Player>().playerNumber = Utils.PlayerNumber.TWO;
                player2.name = PhotonNetwork.LocalPlayer.NickName;
            }
            GameObject ballManager = PhotonNetwork.Instantiate(BallManagerPrefab.name, Vector3.zero, Quaternion.identity);
            ballManager.name = "BallManager " + PhotonNetwork.LocalPlayer.ActorNumber;
        } else {
            player1 = Instantiate(humanPrefab, player1SpawnPoint.position + oculusOffset, player1SpawnPoint.rotation);
            GameObject offScreenUI1 = Instantiate(OffScreenUIPrefab, OffScreenUIPrefab.transform.position, OffScreenUIPrefab.transform.rotation);
            offScreenUI1.GetComponent<WallIndicator>().playerLocation = player1.GetComponentInChildren<Camera>(true).transform;
            offScreenUI1.GetComponent<Canvas>().worldCamera = player1.GetComponentInChildren<Camera>();
            player1.GetComponent<Player>().playerNumber = Utils.PlayerNumber.ONE;

            player2 = Instantiate(BotPrefab, player2SpawnPoint.position, player2SpawnPoint.rotation);
            player2.GetComponent<Player>().playerNumber = Utils.PlayerNumber.TWO;

            GameObject ballManager = Instantiate(BallManagerPrefab);
        }
    }
}
