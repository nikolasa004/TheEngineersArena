using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
    public TextMeshProUGUI codeText;
    public TextMeshProUGUI battleEndText;

    public Button redButton;
    public Button greenButton;
    public Button blueButton;
    public Button whiteButton;
    public Button blackButton;

    public TextMeshProUGUI redText;
    public TextMeshProUGUI greenText;
    public TextMeshProUGUI blueText;
    public TextMeshProUGUI whiteText;
    public TextMeshProUGUI blackText;

    public GameObject cameraImage;
    public TextMeshProUGUI colorText;

    public GameObject buttons;
    public GameObject cameraPanel;
    public GameObject endPanel;

    private string colorName;
    private WebCamTexture webcamTexture;

    [HideInInspector]
    public string redType = "while";
    public string greenType = "if";
    public string blueType = "while";
    public string whiteType = "for";
    public string blackType = "for";

    private int width;
    private int height;
    private int step;
    private float tolerance;
    private int cameraAreaCheck;

    private bool startDelay = true;

    private bool clearingColor = false;

    private string oldColor = "";

    private string halfRed;
    private string halfGreen;
    private string halfBlue;
    private string halfWhite;
    private string halfBlack;

    private string red;
    private string green;
    private string blue;
    private string white;
    private string black;

    private bool redActive = false;
    private bool greenActive = false;
    private bool blueActive = false;
    private bool whiteActive = false;
    private bool blackActive = false;

    private bool redOnCooldown = false;
    private bool greenOnCooldown = false;
    private bool blueOnCooldown = false;
    private bool whiteOnCooldown = false;
    private bool blackOnCooldown = false;

    private bool resetInput = false;
    private string endMessage = "";

    private bool ignoreUp1 = false;
    private string ignoring1 = "";
    private bool ignoreUp2 = false;
    private string ignoring2 = "";

    private bool connectionLost = false;

    private bool reseting = false;

    private Thread startThread;

    private bool endThread = false;

    private void Start()
    {
        startThread = new Thread(HandleServer);
        startThread.Start();

        codeText.text = Programming.outputCode;

        if (Programming.defaultInput)
        {
            codeText.fontSize = 100;
        }
        else
        {
            codeText.fontSize = 50;

            if (!Programming.cameraInput)
            {
                redType = Programming.colorTypes["red"];
                greenType = Programming.colorTypes["green"];
                blueType = Programming.colorTypes["blue"];
                whiteType = Programming.colorTypes["white"];
                blackType = Programming.colorTypes["black"];

                List<Tuple<string, string>> functions = Programming.colorFunction["red"];
                int count = 0;
                string buttonText = "";
                foreach (Tuple<string, string> function in functions)
                {
                    count++;
                    buttonText = ButtonText(buttonText, function, count, functions.Count);
                }
                redText.text = buttonText;

                functions = Programming.colorFunction["green"];
                count = 0;
                buttonText = "";
                foreach (Tuple<string, string> function in functions)
                {
                    count++;
                    buttonText = ButtonText(buttonText, function, count, functions.Count);
                }
                greenText.text = buttonText;

                functions = Programming.colorFunction["blue"];
                count = 0;
                buttonText = "";
                foreach (Tuple<string, string> function in functions)
                {
                    count++;
                    buttonText = ButtonText(buttonText, function, count, functions.Count);
                }
                blueText.text = buttonText;

                functions = Programming.colorFunction["white"];
                count = 0;
                buttonText = "";
                foreach (Tuple<string, string> function in functions)
                {
                    count++;
                    buttonText = ButtonText(buttonText, function, count, functions.Count);
                }
                whiteText.text = buttonText;

                functions = Programming.colorFunction["black"];
                count = 0;
                buttonText = "";
                foreach (Tuple<string, string> function in functions)
                {
                    count++;
                    buttonText = ButtonText(buttonText, function, count, functions.Count);
                }
                blackText.text = buttonText;
            }
        }

        halfRed = "<color=#FF000066>Red-Melee</color>\n";
        halfGreen = "<color=#00FF0066>Green-Special</color>\n";
        halfBlue = "<color=#0000FF66>Blue-Ranged</color>\n";
        halfWhite = "<color=#FFFFFF66>White-Left</color>\n";
        halfBlack = "<color=#00000066>Black-Right</color>";

        red = "<color=red>Red-Melee</color>\n";
        green = "<color=green>Green-Special</color>\n";
        blue = "<color=blue>Blue-Ranged</color>\n";
        white = "<color=white>White-Left</color>\n";
        black = "<color=black>Black-Right</color>";

        if (Programming.cameraInput)
        {
            WebCamDevice[] devices = WebCamTexture.devices;

            if (devices.Length > 0)
            {
                string frontCameraName = null;
                foreach (var device in devices)
                {
                    if (device.isFrontFacing)
                    {
                        frontCameraName = device.name;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(frontCameraName))
                {
                    webcamTexture = new WebCamTexture(frontCameraName);
                }
                else
                {
                    Debug.Log("Front camera was not found, using other camera");

                    webcamTexture = new WebCamTexture(devices[0].name);
                }

                //setting to lowest resolution
                webcamTexture.requestedWidth = 320;
                webcamTexture.requestedHeight = 240;

                cameraImage.GetComponent<RawImage>().texture = webcamTexture;
                webcamTexture.Play();
            }
            else
            {
                Debug.Log("Not a single camera is available");
            }

            width = webcamTexture.width;
            height = webcamTexture.height;
            step = 25; //check every 25th pixel

            tolerance = 0.4f;
            cameraAreaCheck = (width * height) / (step * step);

            StartCoroutine(StartDelay());
        }
    }

    private void HandleServer()
    {
        while (!endThread)
        {
            try
            {
                string message = LanProtocol.ReadLine(Client.stream);
                if (message == null)
                {
                    connectionLost = true;
                    break;
                }
                if (message == "reset")
                {
                    resetInput = true;
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    endMessage = message;
                    break;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[LAN] Battle connection lost: " + e.Message);
                connectionLost = true;
                break;
            }
        }
    }

    private string ButtonText(string buttonText, Tuple<string, string> function, int count, int functionsCount)
    {
        if (function.Item1 == "Body")
        {
            buttonText += "Destroy";
        }
        else if (function.Item1 == "Melee")
        {
            if (function.Item2 == "Cooldown")
            {
                buttonText += "CoolM";
            }
            else
            {
                buttonText += "Melee";
            }
        }
        else if (function.Item1 == "Ranged")
        {
            if (function.Item2 == "Cooldown")
            {
                buttonText += "CoolR";
            }
            else
            {
                buttonText += "Ranged";
            }
        }
        else if (function.Item1 == "Special")
        {
            buttonText += "Special";
        }
        else
        {
            if (function.Item2 == "Left")
            {
                buttonText += "Left";
            }
            else
            {
                buttonText += "Right";
            }
        }

        if (count != functionsCount)
        {
            buttonText += "\n";
        }

        return buttonText;
    }

    private IEnumerator ClearColor()
    {
        clearingColor = true;

        yield return new WaitForSeconds(1f);

        colorName = "";

        colorText.text = "Color";
        colorText.color = Color.gray;

        clearingColor = false;
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(2);
        startDelay = false;
    }

    private void Update()
    {
        if (connectionLost)
        {
            connectionLost = false;
            StartCoroutine(Client.Disconnect());
            return;
        }

        if (Programming.cameraInput && !startDelay)
        {
            RecognizeColors();

            if (!clearingColor)
            {
                StartCoroutine(ClearColor());
            }

            if (colorName != oldColor)
            {
                oldColor = colorName;
                SendCommand(colorName);
            }
        }

        if (resetInput && Programming.cameraInput)
        {
            resetInput = false;

            redActive = false;
            greenActive = false;
            blueActive = false;
            whiteActive = false;
            blackActive = false;

            redOnCooldown = false;
            greenOnCooldown = false;
            blueOnCooldown = false;
            whiteOnCooldown = false;
            blackOnCooldown = false;
        }
        else if (resetInput)
        {
            resetInput = false;

            StartCoroutine(WaitWithCommands());

            redButton.image.color = new Color(redButton.image.color.r, redButton.image.color.g, redButton.image.color.b, 1);
            greenButton.image.color = new Color(greenButton.image.color.r, greenButton.image.color.g, greenButton.image.color.b, 1);
            blueButton.image.color = new Color(blueButton.image.color.r, blueButton.image.color.g, blueButton.image.color.b, 1);
            whiteButton.image.color = new Color(whiteButton.image.color.r, whiteButton.image.color.g, whiteButton.image.color.b, 1);
            blackButton.image.color = new Color(blackButton.image.color.r, blackButton.image.color.g, blackButton.image.color.b, 1);

            ignoreUp1 = true;
            ignoreUp2 = true;

            if (redType == "for")
            {
                if (ignoring1 == "")
                {
                    ignoring1 = "red";
                }
                else
                {
                    ignoring2 = "red";
                }
            }
            if (greenType == "for")
            {
                if (ignoring1 == "")
                {
                    ignoring1 = "green";
                }
                else
                {
                    ignoring2 = "green";
                }
            }
            if (blueType == "for")
            {
                if (ignoring1 == "")
                {
                    ignoring1 = "blue";
                }
                else
                {
                    ignoring2 = "blue";
                }
            }
            if (whiteType == "for")
            {
                if (ignoring1 == "")
                {
                    ignoring1 = "white";
                }
                else
                {
                    ignoring2 = "white";
                }
            }
            if (blackType == "for")
            {
                if (ignoring1 == "")
                {
                    ignoring1 = "black";
                }
                else
                {
                    ignoring2 = "black";
                }
            }
        }

        if (endMessage != "")
        {
            Debug.Log("End message: " + endMessage);

            buttons.SetActive(false);
            cameraPanel.SetActive(false);
            endPanel.SetActive(true);

            Vibration.Vibrate();

            List<string> tokens = endMessage.Split('/').Where(t => !string.IsNullOrEmpty(t)).ToList();

            PlayerPrefs.SetString("History3Names", PlayerPrefs.GetString("History2Names", ""));
            PlayerPrefs.SetString("History2Names", PlayerPrefs.GetString("History1Names", ""));
            PlayerPrefs.SetString("History1Names", tokens[0]);

            PlayerPrefs.SetString("History3Result", PlayerPrefs.GetString("History2Result", ""));
            PlayerPrefs.SetString("History2Result", PlayerPrefs.GetString("History1Result", ""));
            PlayerPrefs.SetString("History1Result", tokens[1]);

            PlayerPrefs.SetString("History3ResultText", PlayerPrefs.GetString("History2ResultText", ""));
            PlayerPrefs.SetString("History2ResultText", PlayerPrefs.GetString("History1ResultText", ""));
            PlayerPrefs.SetString("History1ResultText", tokens[2]);

            PlayerPrefs.SetString("History3Parts1", PlayerPrefs.GetString("History2Parts1", ""));
            PlayerPrefs.SetString("History2Parts1", PlayerPrefs.GetString("History1Parts1", ""));
            PlayerPrefs.SetString("History1Parts1", tokens[3]);

            PlayerPrefs.SetString("History3Parts2", PlayerPrefs.GetString("History2Parts2", ""));
            PlayerPrefs.SetString("History2Parts2", PlayerPrefs.GetString("History1Parts2", ""));
            PlayerPrefs.SetString("History1Parts2", tokens[4]);

            if (tokens[2] == "WON")
            {
                battleEndText.text = "Battle ended\nYou won!";
                battleEndText.color = Color.green;

                SoundEffectsManager.Instance.PlayWinSound();
            }
            else
            {
                battleEndText.text = "Battle ended\nYou lost!";
                battleEndText.color = Color.red;

                SoundEffectsManager.Instance.PlayLoseSound();
            }

            int playerLevel = PlayerPrefs.GetInt("Level", 0);
            int currentExperience = PlayerPrefs.GetInt("Exp", 0);
            int neededExp = 0;
            if (playerLevel < 51)
            {
                neededExp = 100;
            }
            else if (playerLevel < 102)
            {
                neededExp = 200;
            }
            else if (playerLevel < 153)
            {
                neededExp = 300;
            }
            else if (playerLevel < 204)
            {
                neededExp = 400;
            }
            else if (playerLevel < 255)
            {
                neededExp = 500;
            }
            else
            {
                neededExp = 600;
            }

            if (tokens[2] == "WON")
            {
                currentExperience += 100;
            }
            else
            {
                currentExperience += 25;
            }

            if (currentExperience >= neededExp)
            {
                playerLevel++;
                currentExperience -= neededExp;
            }
            if (currentExperience < 0)
            {
                currentExperience = 0;
            }

            PlayerPrefs.SetInt("Level", playerLevel);
            PlayerPrefs.SetInt("Exp", currentExperience);

            endMessage = "";

            StartCoroutine(End());
        }
    }

    public IEnumerator WaitWithCommands()
    {
        reseting = true;

        Vibration.Vibrate();

        yield return new WaitForSeconds(6.25f);

        Vibration.Vibrate();

        reseting = false;
    }

    public IEnumerator End()
    {
        yield return new WaitForSeconds(3);

        SceneManager.LoadScene(1);
    }

    public void RedClickDown()
    {
        if (reseting)
        {
            return;
        }

        if (redType == "for")
        {
            SendCommand("red");

            if (ignoring1 == "red")
            {
                ignoreUp1 = false;
            }
            else if (ignoring2 == "red")
            {
                ignoreUp2 = false;
            }

            Color currentColor = redButton.image.color;
            redButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
        }
    }

    public void RedClick()
    {
        if (reseting)
        {
            return;
        }

        if (redType == "if" || redType == "while")
        {
            SendCommand("red");

            if (redType == "while")
            {
                Color currentColor = redButton.image.color;
                float currentAlpha = currentColor.a;

                if (currentAlpha == 1)
                {
                    redButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
                }
                else
                {
                    redButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
                }
            }
        }
    }

    public void RedClickUp()
    {
        if (reseting)
        {
            return;
        }

        if (redType == "for")
        {
            if (ignoring1 == "red" && ignoreUp1)
            {
                ignoreUp1 = false;
                return;
            }
            else if (ignoring2 == "red" && ignoreUp2)
            {
                ignoreUp2 = false;
                return;
            }

            SendCommand("red");

            Color currentColor = redButton.image.color;
            redButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
        }
    }

    public void GreenClickDown()
    {
        if (reseting)
        {
            return;
        }

        if (greenType == "for")
        {
            SendCommand("green");

            if (ignoring1 == "green")
            {
                ignoreUp1 = false;
            }
            else if (ignoring2 == "green")
            {
                ignoreUp2 = false;
            }

            Color currentColor = greenButton.image.color;
            greenButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
        }
    }

    public void GreenClick()
    {
        if (reseting)
        {
            return;
        }

        if (greenType == "if" || greenType == "while")
        {
            SendCommand("green");

            if (greenType == "while")
            {
                Color currentColor = greenButton.image.color;
                float currentAlpha = currentColor.a;

                if (currentAlpha == 1)
                {
                    greenButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
                }
                else
                {
                    greenButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
                }
            }
        }
    }

    public void GreenClickUp()
    {
        if (reseting)
        {
            return;
        }

        if (greenType == "for")
        {
            if (ignoring1 == "green" && ignoreUp1)
            {
                ignoreUp1 = false;
                return;
            }
            else if (ignoring2 == "green" && ignoreUp2)
            {
                ignoreUp2 = false;
                return;
            }

            SendCommand("green");

            Color currentColor = greenButton.image.color;
            greenButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
        }
    }

    public void BlueClickDown()
    {
        if (reseting)
        {
            return;
        }

        if (blueType == "for")
        {
            if (ignoring1 == "blue")
            {
                ignoreUp1 = false;
            }
            else if (ignoring2 == "blue")
            {
                ignoreUp2 = false;
            }

            SendCommand("blue");

            Color currentColor = blueButton.image.color;
            blueButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
        }
    }

    public void BlueClick()
    {
        if (reseting)
        {
            return;
        }

        if (blueType == "if" || blueType == "while")
        {
            SendCommand("blue");

            if (blueType == "while")
            {
                Color currentColor = blueButton.image.color;
                float currentAlpha = currentColor.a;

                if (currentAlpha == 1)
                {
                    blueButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
                }
                else
                {
                    blueButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
                }
            }
        }
    }

    public void BlueClickUp()
    {
        if (reseting)
        {
            return;
        }

        if (blueType == "for")
        {
            if (ignoring1 == "blue" && ignoreUp1)
            {
                ignoreUp1 = false;
                return;
            }
            else if (ignoring2 == "blue" && ignoreUp2)
            {
                ignoreUp2 = false;
                return;
            }

            SendCommand("blue");

            Color currentColor = blueButton.image.color;
            blueButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
        }
    }

    public void WhiteClickDown()
    {
        if (reseting)
        {
            return;
        }

        if (whiteType == "for")
        {
            if (ignoring1 == "white")
            {
                ignoreUp1 = false;
            }
            else if (ignoring2 == "white")
            {
                ignoreUp2 = false;
            }

            SendCommand("white");

            Color currentColor = whiteButton.image.color;
            whiteButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
        }
    }

    public void WhiteClick()
    {
        if (reseting)
        {
            return;
        }

        if (whiteType == "if" || whiteType == "while")
        {
            SendCommand("white");

            if (whiteType == "while")
            {
                Color currentColor = whiteButton.image.color;
                float currentAlpha = currentColor.a;

                if (currentAlpha == 1)
                {
                    whiteButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
                }
                else
                {
                    whiteButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
                }
            }
        }
    }

    public void WhiteClickUp()
    {
        if (reseting)
        {
            return;

        }
        if (whiteType == "for")
        {
            if (ignoring1 == "white" && ignoreUp1)
            {
                ignoreUp1 = false;
                return;
            }
            else if (ignoring2 == "white" && ignoreUp2)
            {
                ignoreUp2 = false;
                return;
            }

            SendCommand("white");

            Color currentColor = whiteButton.image.color;
            whiteButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
        }
    }

    public void BlackClickDown()
    {
        if (reseting)
        {
            return;
        }

        if (blackType == "for")
        {
            if (ignoring1 == "black")
            {
                ignoreUp1 = false;
            }
            else if (ignoring2 == "black")
            {
                ignoreUp2 = false;
            }

            SendCommand("black");

            Color currentColor = blackButton.image.color;
            blackButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
        }
    }

    public void BlackClick()
    {
        if (reseting)
        {
            return;
        }

        if (blackType == "if" || blackType == "while")
        {
            SendCommand("black");

            if (blackType == "while")
            {
                Color currentColor = blackButton.image.color;
                float currentAlpha = currentColor.a;

                if (currentAlpha == 1)
                {
                    blackButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
                }
                else
                {
                    blackButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
                }
            }
        }
    }

    public void BlackClickUp()
    {
        if (reseting)
        {
            return;
        }

        if (blackType == "for")
        {
            if (ignoring1 == "black" && ignoreUp1)
            {
                ignoreUp1 = false;
                return;
            }
            else if (ignoring2 == "black" && ignoreUp2)
            {
                ignoreUp2 = false;
                return;
            }

            SendCommand("black");

            Color currentColor = blackButton.image.color;
            blackButton.image.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
        }
    }

    public void SendCommand(string message)
    {
        try
        {
            LanProtocol.WriteLine(Client.stream, message);
        }
        catch (Exception e)
        {
            Debug.LogWarning("[LAN] Sending command failed: " + e.Message);
            connectionLost = true;
        }
    }

    private void RecognizeColors()
    {
        Color[] pixels = webcamTexture.GetPixels();//getting the pixels of the camera

        int redPixels = 0;
        int greenPixels = 0;
        int bluePixels = 0;
        int whitePixels = 0;
        int blackPixels = 0;

        for (int y = 0; y < height; y += step)
        {
            for (int x = 0; x < width; x += step)
            {
                Color pixel = pixels[y * width + x];

                if (IsColorMatching(pixel, Color.red, tolerance))//checking if the pixel is red
                {
                    if (colorName != "red")
                    {
                        redPixels++;
                    }
                }
                else if (IsColorMatching(pixel, Color.green, 0.5f))//checking if the pixel is green
                {
                    if (colorName != "green")
                    {
                        greenPixels++;
                    }
                }
                else if (IsColorMatching(pixel, Color.blue, tolerance))//checking if the pixel is blue
                {
                    if (colorName != "blue")
                    {
                        bluePixels++;
                    }
                }
                else if (IsColorMatching(pixel, Color.white, tolerance))//checking if the pixel is white
                {
                    if (colorName != "white")
                    {
                        whitePixels++;
                    }
                }
                else if (IsColorMatching(pixel, Color.black, 0.2f))//checking if the pixel is black
                {
                    if (colorName != "black")
                    {
                        blackPixels++;
                    }
                }
            }
        }

        if (redPixels >= cameraAreaCheck / 20 && !redOnCooldown)
        {
            colorText.text = "Red";
            colorText.color = Color.red;
            colorName = "red";

            if (redType == "while" || redType == "for")
            {
                if (!redActive)
                {
                    codeText.text = codeText.text.Replace(halfRed, red);
                }
                else
                {
                    codeText.text = codeText.text.Replace(red, halfRed);
                }
                redActive = !redActive;

                StartCoroutine(redCooldown());
            }
        }
        if (greenPixels >= cameraAreaCheck / 20 && !greenOnCooldown)
        {
            colorText.text = "Green";
            colorText.color = Color.green;
            colorName = "green";

            if (greenType == "while" || greenType == "for")
            {
                if (!greenActive)
                {
                    codeText.text = codeText.text.Replace(halfGreen, green);
                }
                else
                {
                    codeText.text = codeText.text.Replace(green, halfGreen);
                }
                greenActive = !greenActive;
            }

            StartCoroutine(greenCooldown());
        }
        if (bluePixels >= cameraAreaCheck / 25 && !blueOnCooldown)
        {
            colorText.text = "Blue";
            colorText.color = Color.blue;
            colorName = "blue";

            if (blueType == "while" || blueType == "for")
            {
                if (!blueActive)
                {
                    codeText.text = codeText.text.Replace(halfBlue, blue);
                }
                else
                {
                    codeText.text = codeText.text.Replace(blue, halfBlue);
                }
                blueActive = !blueActive;
            }

            StartCoroutine(blueCooldown());
        }
        if (whitePixels >= cameraAreaCheck / 5 && !whiteOnCooldown)
        {
            colorText.text = "White";
            colorText.color = Color.white;
            colorName = "white";

            if (whiteType == "while" || whiteType == "for")
            {
                if (!whiteActive)
                {
                    codeText.text = codeText.text.Replace(halfWhite, white);

                    if (blackActive)
                    {
                        codeText.text = codeText.text.Replace(black, halfBlack);
                    }
                }
                else
                {
                    codeText.text = codeText.text.Replace(white, halfWhite);
                }
                whiteActive = !whiteActive;
            }

            StartCoroutine(whiteCooldown());
        }
        if (blackPixels >= cameraAreaCheck / 5 && !blackOnCooldown)
        {
            colorText.text = "Black";
            colorText.color = Color.black;
            colorName = "black";

            if (blackType == "while" || blackType == "for")
            {
                if (!blackActive)
                {
                    codeText.text = codeText.text.Replace(halfBlack, black);

                    if (whiteActive)
                    {
                        codeText.text = codeText.text.Replace(white, halfWhite);
                    }
                }
                else
                {
                    codeText.text = codeText.text.Replace(black, halfBlack);
                }
                blackActive = !blackActive;
            }

            StartCoroutine(blackCooldown());
        }
    }

    public IEnumerator redCooldown()
    {
        redOnCooldown = true;

        yield return new WaitForSeconds(1f);

        redOnCooldown = false;
    }

    public IEnumerator greenCooldown()
    {
        greenOnCooldown = true;

        yield return new WaitForSeconds(1f);

        greenOnCooldown = false;
    }

    public IEnumerator blueCooldown()
    {
        blueOnCooldown = true;

        yield return new WaitForSeconds(1f);

        blueOnCooldown = false;
    }

    public IEnumerator whiteCooldown()
    {
        whiteOnCooldown = true;

        yield return new WaitForSeconds(1f);

        whiteOnCooldown = false;
    }

    public IEnumerator blackCooldown()
    {
        blackOnCooldown = true;

        yield return new WaitForSeconds(1f);

        blackOnCooldown = false;
    }

    private bool IsColorMatching(Color color, Color targetColor, float tolerance)
    {
        //checking if the pixel color is within the tolerance range of the target color
        return Mathf.Abs(color.r - targetColor.r) < tolerance &&
               Mathf.Abs(color.g - targetColor.g) < tolerance &&
               Mathf.Abs(color.b - targetColor.b) < tolerance;
    }

    private void OnApplicationQuit()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }

    private void OnDestroy()
    {
        endThread = true;
    }
}
