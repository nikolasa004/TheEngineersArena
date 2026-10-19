using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public List<GameObject> panels;
    public List<GameObject> buttons;
    public GameObject buttonsHolder;

    public GameObject button;

    public static Tutorial Instance { get; private set; }

    public static int tutorialIndex = 1;

    private GameObject currentPanel;
    private GameObject currentButton;

    private bool showButt = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if(PlayerPrefs.GetInt("Tutorial", 0) == 0)
        {
            ActivateTutorial(0);
            PlayerPrefs.SetInt("Tutorial", 1);
        }

        tutorialIndex = 1;

        StartCoroutine(showButton());
    }

    public IEnumerator showButton()
    {
        while (true)
        {
            if (button.activeInHierarchy && tutorialIndex == -1)
            {
                button.SetActive(false);
            }

            if (!button.activeInHierarchy && tutorialIndex != -1 && showButt)
            {
                button.SetActive(true);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void OnQuestionMarkClick()
    {
        SoundEffectsManager.Instance.PlayClickSound();
        
        ActivateTutorial(tutorialIndex);
    }

    public void ActivateTutorial(int index)
    {
        showButt = false;
        button.SetActive(false);
        buttonsHolder.SetActive(true);
        if(currentPanel != null && currentPanel != panels[index])
        {
            currentPanel.SetActive(false);
        }
        if(currentButton != null && currentButton != buttons[index])
        {
            currentButton.GetComponent<Image>().color=Color.white;
        }
        currentPanel = panels[index];
        currentButton = buttons[index];
        currentPanel.SetActive(true);
        currentButton.GetComponent<Image>().color=Color.green;
    }

    public void DeactivateTutorial()
    {
        showButt = true;
        button.SetActive(true);
        buttonsHolder.SetActive(false);
        if(currentPanel != null)
        {
            currentPanel.SetActive(false);
        }
        if(currentButton != null)
        {
            currentButton.GetComponent<Image>().color=Color.white;
        }

        SoundEffectsManager.Instance.PlayClickSound();
    }

    public void OnClick(int index)
    {
        SoundEffectsManager.Instance.PlayClickSound();

        GameObject panel = panels[index];
        GameObject button = buttons[index];
        panel.SetActive(true);
        button.GetComponent<Image>().color=Color.green;
        if(currentPanel != null && currentPanel != panel)
        {
            currentPanel.SetActive(false);
        }
        if(currentButton != null && currentButton != button)
        {
            currentButton.GetComponent<Image>().color=Color.white;
        }
        currentPanel = panel;
        currentButton = button;
    }
}
