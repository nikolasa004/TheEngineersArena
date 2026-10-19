using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Connection : MonoBehaviour
{
    public Button startButton;

    public static bool displayedParts;

    private bool playedSound = false;

    private void Start()
    {
        displayedParts = false;
    }

    private void Update()
    {
        if ((Server.player1 != null && Server.player2 != null && Menu.isMultiplayer) || (Server.player1 != null && !Menu.isMultiplayer))
        {
            startButton.interactable = true;
            TextMeshProUGUI startText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            startText.color = new Color32(255, 255, 255, 255);

            if (!playedSound)
            {
                SoundEffectsManager.Instance.PlaySuccessSound();
                playedSound = true;
            }
        }
        else
        {
            startButton.interactable = false;
            TextMeshProUGUI startText = startButton.GetComponentInChildren<TextMeshProUGUI>();
            startText.color = new Color32(255, 255, 255, 100);
        }
    }

    public void StartGame()
    {
        if ((Server.player1 != null && Server.player2 != null && Menu.isMultiplayer) || (Server.player1 != null && !Menu.isMultiplayer))
        {
            Debug.Log("Starting the game");

            SoundEffectsManager.Instance.PlayClickSound();

            string message = PartsDisplay.partsMessage;

            LanProtocol.WriteLine(Server.player1Stream, message);
            if (Menu.isMultiplayer)
                LanProtocol.WriteLine(Server.player2Stream, message);

            displayedParts = true;
            gameObject.SetActive(false);
        }
    }
}
