using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public float sensitivity = 10.0f;
    public float upSensitivity = 5.0f;
    public float distance = 5f;
    public Transform targetToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float yRotation = (Input.GetAxis("Mouse X") * sensitivity);
        float xRotation = (Input.GetAxis("Mouse Y") * upSensitivity);
        //float yRotation = transform.rotation.y + (Input.GetAxis("Mouse X") * sensitivity);
        //if (yRotation > 1) yRotation -= 2;
        //if (yRotation < -1) yRotation += 2;
        //transform.rotation = new Quaternion(transform.rotation.x, yRotation, transform.rotation.z, transform.rotation.w);

        transform.Rotate(0.0f, yRotation, 0.0f, Space.Self);
        //transform.Rotate(xRotation, 0.0f, 0.0f, Space.World);
        //transform.Rotate(((yRotation > 0) ? Vector3.up : Vector3.down) * yRotation * Time.deltaTime);
        float angle = transform.rotation.eulerAngles.y;
        if (angle < 0)
            angle += 360;
        transform.position = new Vector3(
            targetToFollow.position.x - (distance * Mathf.Sin(Mathf.PI * 2 * angle / 360)),
            transform.position.y,
            targetToFollow.position.z - (distance * Mathf.Cos(Mathf.PI * 2 * angle / 360))
        );
    }
}
