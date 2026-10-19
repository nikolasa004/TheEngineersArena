using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StartBattle : MonoBehaviour
{
    public GameObject battlePanel;
    public GameObject buttonsPanel;
    public GameObject cameraPanel;

    private bool startGame = false;

    private bool connectionLost = false;

    private void Start()
    {
        Tutorial.tutorialIndex = 7;

        var startThread = new System.Threading.Thread(CheckIfBattleStarted);
        startThread.Start();

        Debug.Log("Waiting for game to start");
    }

    private void Update()
    {
        if (connectionLost)
        {
            connectionLost = false;
            StartCoroutine(Client.Disconnect());
            return;
        }

        if (startGame)
        {
            startGame = false;
            Debug.Log("Game started");
            StartGame();
        }
    }

    private void CheckIfBattleStarted()
    {
        try
        {
            string message = LanProtocol.ReadLine(Client.stream);
            if (message == "start")
                startGame = true;
            else
            {
                Debug.LogWarning("Unexpected battle start message: " + message);
                connectionLost = true;
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("Error: " + e.Message);
            Debug.Log("Server disconnected");

            connectionLost = true;
        }
    }

    public void StartGame()
    {
        battlePanel.SetActive(true);

        Tutorial.tutorialIndex = -1;

        Vibration.Vibrate();

        if (Programming.cameraInput)
        {
            cameraPanel.SetActive(true);
        }
        else
        {
            buttonsPanel.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
