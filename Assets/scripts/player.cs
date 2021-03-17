using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    public Transform camera;
    private UDPClient client;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        client = SpeedTutorMainMenuSystem.MenuController.FindObjectOfType<UDPClient>().GetComponent<UDPClient>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(client.nickName);
        float v = Input.GetAxis("Vertical");

        rb.AddForce(transform.forward * v * speed);
        rb.MoveRotation(camera.rotation);
    }
}
