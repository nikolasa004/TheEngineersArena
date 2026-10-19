using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject inputItems;
    public TMP_InputField inputField;
    public Animator animator;
    public Profile profile;

    public TextMeshProUGUI errorText;

    public static string errorMessage = "";

    private string playerInput;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        inputField.characterLimit = 12;
        inputField.onEndEdit.AddListener(SubmitInput);
        if(PlayerPrefs.GetString("PlayerName") == "")
        {
            inputItems.SetActive(true);
            animator.SetTrigger("NameGo");
        }

        if(errorMessage != "")
        {
            errorText.text = errorMessage;
            errorMessage = "";

            SoundEffectsManager.Instance.PlayErrorSound();
        }

        Tutorial.tutorialIndex = 1;
    }

    void SubmitInput(string input)
    {
        playerInput = input;
    }

    public void SaveName()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        Leaderboard.oldName = PlayerPrefs.GetString("PlayerName");

        PlayerPrefs.SetString("PlayerName", playerInput);
        animator.SetTrigger("NameGo");
        profile.UpdateName();

        Leaderboard.nameChanged = true;
    }

    public void Play()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        SceneManager.LoadScene(2);
    }

    public void Quit()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        SoundEffectsManager.Instance.PlayClickSound();

        Application.Quit();
    }
}
