using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballspawner : MonoBehaviour
{
    public GameObject obj;
    float timeLastSpawned = 0;
    public float spawnInterval = 3f;

    // Start is called before the first frame update
    void Start()
    {
        timeLastSpawned = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - timeLastSpawned > spawnInterval) {
            Spawn();
            timeLastSpawned = Time.time;
        }
    }

    public void Spawn() {
        Instantiate(obj);
    }
}
