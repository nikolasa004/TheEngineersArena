using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoEffect : MonoBehaviour
{
    public ParticleSystem ufoLeft;
    public ParticleSystem ufoRight;

    private void Start()
    {
        StartCoroutine(PlayUfoEffect(ufoLeft));
        StartCoroutine(PlayUfoEffect(ufoRight));
    }

    public IEnumerator PlayUfoEffect(ParticleSystem ufo)
    {
        while (true)
        {
            int random = Random.Range(0, 60);
            yield return new WaitForSeconds(random);
            ufo.Play();
        }
    }
}
