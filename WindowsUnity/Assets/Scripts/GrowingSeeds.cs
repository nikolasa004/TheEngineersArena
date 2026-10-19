using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingSeeds : MonoBehaviour
{
    public Animator tree1;
    public Animator tree2;
    public Animator tree3;
    public Animator tree4;
    public Animator tree5;

    private bool[] growing= {false, false, false, false, false};

    private void Start()
    {
        tree1.gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);
        tree2.gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);
        tree3.gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);
        tree4.gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);
        tree5.gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);

        StartCoroutine(GrowTree(tree1,0));
        StartCoroutine(GrowTree(tree2,1));
        StartCoroutine(GrowTree(tree3,2));
        StartCoroutine(GrowTree(tree4,3));
        StartCoroutine(GrowTree(tree5,4));
    }

    IEnumerator GrowTree(Animator tree, int index)
    {
        while (true)
        {
            int randomSeconds = Random.Range(1, 10);
            yield return new WaitForSeconds(randomSeconds);
            int randomGrow = Random.Range(0, 2);
            if (randomGrow == 0)
            {
                yield return null;
            }
            else
            {
                tree.gameObject.GetComponent<AudioSource>().Play();

                if (growing[index]== false)
                {
                    tree.SetBool("Grow",true);
                    growing[index] = true;
                }
                else
                {
                    tree.SetBool("Grow",false);
                    growing[index] = false;
                }
            }
        }
    }
}
