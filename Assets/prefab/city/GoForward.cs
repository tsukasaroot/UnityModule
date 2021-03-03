using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoForward : MonoBehaviour
{
    public float fMovementSpeed;
    public GameObject linkedSpawner;

    private Vector3 vSpawnerPosition;

    private void Awake()
    {
        vSpawnerPosition = linkedSpawner.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * fMovementSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name.Contains("TunnelEnding"))
        {
            Vector3 vCurrentPosition = transform.position;

            transform.position = new Vector3(vSpawnerPosition.x, vCurrentPosition.y, vSpawnerPosition.z);
        }
    }
}
