using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    public Transform camera;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float v = Input.GetAxis("Vertical");

        rb.AddForce(transform.forward * v * speed);
        rb.MoveRotation(camera.rotation);
    }
}
