using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mound : MonoBehaviour
{
    [SerializeField] private AudioSource clank_sound;
    [SerializeField] private Rigidbody2D rb;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (clank_sound != null)
            clank_sound.Play();
    }
}
