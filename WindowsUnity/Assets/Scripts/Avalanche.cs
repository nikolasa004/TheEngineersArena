using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avalanche : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);
        animator = GetComponent<Animator>();
        StartCoroutine(Fall());
    }
    public IEnumerator Fall()
    {
        while(true)
        {
            int random = Random.Range(0, 30);

            yield return new WaitForSeconds(random);

            int random2 = Random.Range(0, 2);

            GetComponent<AudioSource>().Play();

            if(random2 == 0)
            {
                animator.SetTrigger("AvalancheLeft");
            }
            else
            {
                animator.SetTrigger("AvalancheRight");
            }

            yield return new WaitForSeconds(2);
        }
    }
}
