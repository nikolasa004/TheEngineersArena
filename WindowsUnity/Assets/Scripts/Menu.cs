using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public TextMeshProUGUI errorText;

    public Animator settingsMenu;

    public Slider musicSlider;
    public Slider battleSlider;
    public Slider mapsSlider;
    public Slider UISlider;

    public static bool isMultiplayer;
    public static string errorMessage="";

    private void Start()
    {
        isMultiplayer = false;

        PreventSleep.PreventScreenSleep();

        if (errorMessage != "")
        {
            SoundEffectsManager.Instance.PlayErrorSound();

            errorText.text = errorMessage;
            errorMessage = "";
        }

        musicSlider.value = PlayerPrefs.GetFloat("Music", 0.5f);
        battleSlider.value = PlayerPrefs.GetFloat("Battle", 1);
        mapsSlider.value = PlayerPrefs.GetFloat("Maps", 1);
        UISlider.value = PlayerPrefs.GetFloat("UI", 1);

        musicSlider.onValueChanged.AddListener(delegate { UpdateSliderValue(1); });
        battleSlider.onValueChanged.AddListener(delegate { UpdateSliderValue(2); });
        mapsSlider.onValueChanged.AddListener(delegate { UpdateSliderValue(3); });
        UISlider.onValueChanged.AddListener(delegate { UpdateSliderValue(4); });
    }

    void UpdateSliderValue(int sliderID)
    {
        if(sliderID==1)
        {
            PlayerPrefs.SetFloat("Music", musicSlider.value);
        }
        else if(sliderID==2)
        {
            PlayerPrefs.SetFloat("Battle", battleSlider.value);
        }
        else if(sliderID==3)
        {
            PlayerPrefs.SetFloat("Maps", mapsSlider.value);
        }
        else if(sliderID==4)
        {
            PlayerPrefs.SetFloat("UI", UISlider.value);
        }
    }

    public void Settings()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        settingsMenu.SetTrigger("Move");
    }

    public void Singleplayer()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        SceneManager.LoadScene(2);
    }

    public void Multiplayer()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        isMultiplayer = true;
        SceneManager.LoadScene(2);
    }

    public void Quit()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        PreventSleep.RestoreSleepSettings();

        Application.Quit();
    }
}
