using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LogoAnimation());
    }

    public IEnumerator LogoAnimation()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(1);
    }
}
