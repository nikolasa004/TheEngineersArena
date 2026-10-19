using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour
{
    public Sprite break1;
    public Sprite break2;

    private int timePassed = 0;
    private bool isBreaking = false;
    private bool timerWorking = false;

    private AudioSource[] audioSources;

    private void Start()
    {
        audioSources = GetComponents<AudioSource>();
        audioSources[0].volume = PlayerPrefs.GetFloat("Maps", 1);
        audioSources[1].volume = PlayerPrefs.GetFloat("Maps", 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isBreaking = true;
        if (!timerWorking)
        {
            StartCoroutine(Break());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isBreaking = false;
    }

    public IEnumerator Break()
    {
        timerWorking = true;
        while(isBreaking)
        {
            timePassed++;

            if (timePassed==3)
            {
                audioSources[0].Play();
                GetComponent<SpriteRenderer>().sprite = break1;
            }
            else if (timePassed == 6)
            {
                audioSources[0].Play();
                GetComponent<SpriteRenderer>().sprite = break2;
            }
            else if (timePassed == 10)
            {
                audioSources[1].Play();
                StartCoroutine(Demolish());
            }
            yield return new WaitForSeconds(1);
        }
        timerWorking = false;
    }

    public IEnumerator Demolish()
    {
        
        GetComponentInChildren<ParticleSystem>().Play();
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
