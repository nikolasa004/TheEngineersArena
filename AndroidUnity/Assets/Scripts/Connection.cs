using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Text;
using System.Net.Sockets;

public class Connection : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public GameObject partsPanel;

    private bool startedReading = false;
    private bool startGame = false;
    private string errorMessage = "";

    public static string partsMessage;

    private void Start()
    {
        partsMessage = "";
        Tutorial.tutorialIndex = 3;
    }

    private void Update()
    {
        if (Client.connected && !startedReading)//if the client is connected and we haven't started reading the data
        {
            statusText.text = "Connected to server\n\nWaiting for game to start";

            Vibration.Vibrate();

            var startThread = new System.Threading.Thread(CheckIfGameStarted);//create a new thread to handle the start of the game information
            startThread.Start();

            startedReading = true;
        }

        if (startGame)//if the game has started
        {
            startGame = false;
            StartGame();
        }

        if (errorMessage != "")//if there is an error message
        {
            statusText.text = "Error: " + errorMessage;
            errorMessage = "";

            StartCoroutine(Client.Disconnect());
        }
    }

    private void CheckIfGameStarted()
    {
        try
        {
            string message = LanProtocol.ReadLine(Client.stream);
            if (!string.IsNullOrEmpty(message) && message.Length == 15)
            {
                partsMessage = message;
                startGame = true;
            }
            else
            {
                errorMessage = "Invalid parts message or server disconnected";
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Error: " + e.Message);
            errorMessage = e.Message;
        }
    }

    public void StartGame()
    {
        partsPanel.SetActive(true);//show the parts panel
        gameObject.SetActive(false);//hide the connection panel
    }
}