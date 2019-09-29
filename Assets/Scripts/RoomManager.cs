using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Creates and loads the scene
public class RoomManager : MonoBehaviourPunCallbacks {
    public int numPlayers = 1;
    public GameObject gameManagerPrefab;
    public GameObject followersParentPrefab;
    public GameObject ballManagerPrefab;
    // Start is called before the first frame update
    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame() {
        // Only the master should load the scene, since we set
        // PhotonNetwork.AutomaticallySyncScene to true in NetworkController
        numPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        if (PhotonNetwork.IsMasterClient) {
            PhotonNetwork.LoadLevel(1);
        }
        StartCoroutine(SpawnPlayerCoroutine());
    }

    IEnumerator SpawnPlayerCoroutine() {
        while(SceneManager.GetActiveScene().buildIndex == 0) {
            yield return null;
        }
        GameManager.Instance.numPlayers = numPlayers;
    }
}
