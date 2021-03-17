using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{
    public float sensitivity = 0.1f;
    public float distance = 5f;
    public Transform targetToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y, targetToFollow.position.z - 5);
        transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y + (Input.GetAxis("Mouse X") * sensitivity), transform.rotation.z, transform.rotation.w);
        float angle = transform.rotation.eulerAngles.y - 180;
        if (angle < 0)
            angle += 360;
        transform.position = new Vector3(
            targetToFollow.position.x + (distance * Mathf.Sin(Mathf.PI * 2 * angle / 360)),
            transform.position.y,
            targetToFollow.position.z + (distance * Mathf.Cos(Mathf.PI * 2 * angle / 360))
        );
    }
}
