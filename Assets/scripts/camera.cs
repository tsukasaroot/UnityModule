using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public float sensitivity = 10.0f;
    public float upSensitivity = 3.0f;
    public float distance = 5f;
    public Transform targetToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xRotation = transform.rotation.eulerAngles.x + (Input.GetAxis("Mouse Y") * -1 * upSensitivity);
        float yRotation = transform.rotation.eulerAngles.y + (Input.GetAxis("Mouse X") * sensitivity);
        float zRotation = transform.rotation.eulerAngles.z;

        if (xRotation > 30 && xRotation < 330)
        {
            if (transform.rotation.eulerAngles.x <= 40)
            {
                xRotation = 30;
            } else
            {
                xRotation = 330;
            }
        }
        transform.eulerAngles = new Vector3(xRotation, yRotation, zRotation);
        float angle = transform.rotation.eulerAngles.y;
        if (angle < 0)
            angle += 360;
        transform.position = new Vector3(
            targetToFollow.position.x - (distance * Mathf.Sin(Mathf.PI * 2 * angle / 360)),
            targetToFollow.position.y + (((xRotation < 180) ? xRotation : xRotation - 360) / 10),
            targetToFollow.position.z - (distance * Mathf.Cos(Mathf.PI * 2 * angle / 360))
        );
    }
}
