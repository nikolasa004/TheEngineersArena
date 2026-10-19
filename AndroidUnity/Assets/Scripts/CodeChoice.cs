using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeChoice : MonoBehaviour
{
    public GameObject programmingCanvas;

    private void Start()
    {
        Tutorial.tutorialIndex = 5;
    }

    public void DefaultChoice()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        programmingCanvas.SetActive(true);
        Programming.defaultInput = true;
    }

    public void CustomChoice()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        programmingCanvas.SetActive(true);
        Programming.defaultInput = false;
    }
}
