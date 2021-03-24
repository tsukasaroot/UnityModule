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
        if (collision.collider.tag == "Player" && !m_hornEffect.isPlaying)
        {
            m_hornEffect.Play();
        }
    }
}
