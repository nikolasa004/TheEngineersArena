using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer==6 || collision.gameObject.layer==8)
        {
            if(!gameObject.GetComponent<AudioSource>().isPlaying)
                gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
