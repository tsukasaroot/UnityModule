using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHorn : MonoBehaviour
{

    private AudioSource m_hornEffect;

    private void Awake()
    {
        m_hornEffect = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
<<<<<<< HEAD
        /*Debug.Log(m_hornEffect.isPlaying);
        Debug.Log(collision.collider.tag);*/
=======
>>>>>>> e5d53ef03f1b78d839c0e5155c7ed7eeccf78705
        if (collision.collider.tag == "Player" && !m_hornEffect.isPlaying)
        {
            m_hornEffect.Play();
        }
    }
}
