using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatChildFollower : MonoBehaviour {
    [SerializeField]
    private GameObject sparkPrefab = null;
    [SerializeField]
    private float _sensitivity = 100f;
    private Rigidbody rb;
    private BatChildFollowerSpawner spawner;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate() {
        Vector3 destination = spawner.transform.position;
        rb.transform.rotation = transform.rotation;

        velocity = (destination - rb.transform.position) * _sensitivity;

        rb.velocity = velocity;
        transform.rotation = spawner.transform.rotation;
    }

    public void SetFollowTarget(BatChildFollowerSpawner target) {
        spawner = target;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ball")) {
            if (PhotonNetwork.IsConnected) {
                PhotonNetwork.Instantiate(sparkPrefab.name, collision.GetContact(0).point, Quaternion.identity);
            } else {
                Instantiate(sparkPrefab, collision.GetContact(0).point, Quaternion.identity);
            }
        }
        spawner.OnCollide(collision);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
