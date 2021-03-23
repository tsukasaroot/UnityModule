using UnityEngine;
using System.Collections;

public class RotateTrophy : MonoBehaviour
{

    public Vector3 RotateAmount;

    void Start()
    {
        StartCoroutine("RotationTrophy");
    }

    void Update()
    {

    }

    IEnumerator RotationTrophy()
    {
        while (true)
        {
            transform.Rotate(RotateAmount * Time.deltaTime);
            yield return null;
        }
    }
}