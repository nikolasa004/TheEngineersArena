using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Battle : MonoBehaviour
{
    public List<GameObject> singlePlayerMaps = new List<GameObject>();
    public List<GameObject> multiPlayerMaps = new List<GameObject>();
    private List<GameObject> maps = new List<GameObject>();

    public GameObject smallRoof;
    public GameObject bigRoof;

    //public GameObject robot1;///test line
    //public GameObject robot2;///test line

    public RectTransform player1HealthBar;
    public RectTransform player2HealthBar;

    public Image player1Melee;
    public Image player1Ranged;
    public Image player1Left;
    public Image player1LeftArrow;
    public Image player1Right;
    public Image player1RightArrow;
    public Image player2Melee;
    public Image player2Ranged;
    public Image player2Left;
    public Image player2LeftArrow;
    public Image player2Right;
    public Image player2RightArrow;

    public TextMeshProUGUI mapName;
    public TextMeshProUGUI round;

    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;

    public List<GameObject> player1WinDots;
    public List<GameObject> player2WinDots;

    public GameObject endPanel;
    public TextMeshProUGUI endText;

    private string player1Buffer = "";
    private string player1Command;
    private string player2Buffer = "";
    private string player2Command;
    private readonly ConcurrentQueue<string> player1Commands = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> player2Commands = new ConcurrentQueue<string>();

    private RobotFunctions player1;
    private RobotFunctions player2;

    private List<int> usedMaps = new List<int>();

    private int player1Wins = 0;
    private int player2Wins = 0;

    private bool resettingMap = false;

    private string greenType1 = "if";
    private string redType1 = "while";
    private string blueType1 = "while";
    private string blackType1 = "for";
    private string whiteType1 = "for";

    private string greenType2 = "if";
    private string redType2 = "while";
    private string blueType2 = "while";
    private string blackType2 = "for";
    private string whiteType2 = "for";

    private bool green1 = false;
    private bool red1 = false;
    private bool blue1 = false;
    private bool black1 = false;
    private bool white1 = false;

    private bool green2 = false;
    private bool red2 = false;
    private bool blue2 = false;
    private bool black2 = false;
    private bool white2 = false;

    private string ignoreInput11 = "white";
    private string ignoreInput12 = "black";
    private string ignoreInput21 = "white";
    private string ignoreInput22 = "black";
    private bool ignore11 = false;
    private bool ignore12 = false;
    private bool ignore21 = false;
    private bool ignore22 = false;

    private Dictionary<string, Dictionary<string, List<Tuple<string, string>>>> code1 = new();
    private Dictionary<string, Dictionary<string, List<Tuple<string, string>>>> code2 = new();

    private List<Action> redFunctions1 = new();
    private List<Action> blueFunctions1 = new();
    private List<Action> greenFunctions1 = new();
    private List<Action> blackFunctions1 = new();
    private List<Action> whiteFunctions1 = new();

    private List<Action> redFunctions2 = new();
    private List<Action> blueFunctions2 = new();
    private List<Action> greenFunctions2 = new();
    private List<Action> blackFunctions2 = new();
    private List<Action> whiteFunctions2 = new();

    private bool connectionLost = false;
    private bool connectionError = false;
    private int playerLostConnection = 0;

    private bool endThread = false;

    private void Start()
    {
        Debug.Log("Battle Starting");

        code1 = PartsDisplay.parsed1Code;
        code2 = PartsDisplay.parsed2Code;

        player1NameText.text = PartsDisplay.player1Name;
        player2NameText.text = PartsDisplay.player2Name;

        if (Menu.isMultiplayer)
        {
            maps = multiPlayerMaps;
        }
        else
        {
            maps = singlePlayerMaps;
        }

        StartCoroutine(GetFirstMap());
        round.text = "Round-1";

        player1 = PartsDisplay.player1Robot.GetComponent<RobotFunctions>();
        player1.player = 1;
        player1.enemy = PartsDisplay.player2Robot;
        player1.healthBar = player1HealthBar;
        player1.meleeImage = player1Melee;
        player1.rangedImage = player1Ranged;
        player1.leftImage = player1Left;
        player1.leftArrowImage = player1LeftArrow;
        player1.rightImage = player1Right;
        player1.rightArrowImage = player1RightArrow;

        player2 = PartsDisplay.player2Robot.GetComponent<RobotFunctions>();
        player2.player = 2;
        player2.enemy = PartsDisplay.player1Robot;
        player2.healthBar = player2HealthBar;
        player2.meleeImage = player2Melee;
        player2.rangedImage = player2Ranged;
        player2.leftImage = player2Left;
        player2.leftArrowImage = player2LeftArrow;
        player2.rightImage = player2Right;
        player2.rightArrowImage = player2RightArrow;

        ParseFunctions();

        SendStartMessage();

        WaitForCommands();
    }

    private void ParseFunctions()
    {
        if (!PartsDisplay.defaultInput1)
        {
            ignoreInput11 = "";
            ignoreInput12 = "";

            AssignFunctions("red", ref redType1, player1, ref redFunctions1, ref ignoreInput11, ref ignoreInput12, ref code1);
            AssignFunctions("blue", ref blueType1, player1, ref blueFunctions1, ref ignoreInput11, ref ignoreInput12, ref code1);
            AssignFunctions("green", ref greenType1, player1, ref greenFunctions1, ref ignoreInput11, ref ignoreInput12, ref code1);
            AssignFunctions("black", ref blackType1, player1, ref blackFunctions1, ref ignoreInput11, ref ignoreInput12, ref code1);
            AssignFunctions("white", ref whiteType1, player1, ref whiteFunctions1, ref ignoreInput11, ref ignoreInput12, ref code1);
        }

        if (!PartsDisplay.defaultInput2 && Menu.isMultiplayer)
        {
            ignoreInput21 = "";
            ignoreInput22 = "";

            AssignFunctions("red", ref redType2, player2, ref redFunctions2, ref ignoreInput21, ref ignoreInput22, ref code2);
            AssignFunctions("blue", ref blueType2, player2, ref blueFunctions2, ref ignoreInput21, ref ignoreInput22, ref code2);
            AssignFunctions("green", ref greenType2, player2, ref greenFunctions2, ref ignoreInput21, ref ignoreInput22, ref code2);
            AssignFunctions("black", ref blackType2, player2, ref blackFunctions2, ref ignoreInput21, ref ignoreInput22, ref code2);
            AssignFunctions("white", ref whiteType2, player2, ref whiteFunctions2, ref ignoreInput21, ref ignoreInput22, ref code2);
        }
    }

    private void AssignFunctions(string color, ref string colorType, RobotFunctions robotFunctions, ref List<Action> actions, ref string ignoreInput1, ref string ignoreInput2, ref Dictionary<string, Dictionary<string, List<Tuple<string, string>>>> code)
    {
        Dictionary<string, List<Tuple<string, string>>> colorCode = code[color];
        if (colorCode.Count == 0)
        {
            colorType = "";
            return;
        }
        foreach (string key in colorCode.Keys)
        {
            colorType = key;
            break;
        }
        List<Tuple<string, string>> colorCodeFunctions = colorCode[colorType];
        foreach (Tuple<string, string> function in colorCodeFunctions)
        {
            if (function.Item1 == "Body")
            {
                actions.Add(robotFunctions.Demolish);
            }
            else if (function.Item1 == "Melee")
            {
                if (function.Item2 == "Cooldown")
                {
                    actions.Add(robotFunctions.MeleeCooldown);
                }
                else
                {
                    actions.Add(robotFunctions.MeleeAttack);
                }
            }
            else if (function.Item1 == "Ranged")
            {
                if (function.Item2 == "Cooldown")
                {
                    actions.Add(robotFunctions.RangedCooldown);
                }
                else
                {
                    actions.Add(robotFunctions.RangedAttack);
                }
            }
            else if (function.Item1 == "Special")
            {
                actions.Add(robotFunctions.Special);
            }
            else if (function.Item1 == "Tire")
            {
                if (function.Item2 == "Right")
                {
                    actions.Add(robotFunctions.MoveForward);
                    ignoreInput1 = color;
                }
                else
                {
                    actions.Add(robotFunctions.MoveBackward);
                    ignoreInput2 = color;
                }
            }
        }
    }

    public IEnumerator GetFirstMap()
    {
        while (true)
        {
            if (usedMaps.Count == 0)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            GameObject currentMap = maps[usedMaps[0]];
            currentMap.SetActive(true);
            mapName.text = currentMap.name.Replace("(Clone)", "");
            Camera.main.orthographicSize = currentMap.GetComponent<Map>().cameraSize;

            if (currentMap.GetComponent<Map>().cameraSize == 5)
            {
                smallRoof.SetActive(true);
                bigRoof.SetActive(false);
            }
            else
            {
                smallRoof.SetActive(false);
                bigRoof.SetActive(true);
            }

            if (!Menu.isMultiplayer)
            {
                if (currentMap.GetComponent<Map>().cameraSize == 5)
                {
                    PartsDisplay.player2Robot.GetComponent<RobotAi>().borderX = 8;
                }
                else
                {
                    PartsDisplay.player2Robot.GetComponent<RobotAi>().borderX = 16;
                }
            }

            break;
        }
    }

    public void GetFirstMapIndex(int ind)
    {
        usedMaps.Add(ind);
    }

    private void Update()
    {
        /////////////////////////Keyboard Input/////////////////////////
        /*if (Input.GetKeyDown(KeyCode.S))
        {
            player1Command = "green";
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            player1Command = "red";
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            player1Command = "blue";
        }*/

        if (connectionLost)
        {
            connectionLost = false;
            StartCoroutine(Server.CloseClients());
            return;
        }

        if (connectionError)
        {
            StartCoroutine(Server.TryReconnect(playerLostConnection));

            connectionError = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPositions();
        }

        if (!resettingMap)
        {
            CheckEnd();
        }

        CheckRotation();
    }

    private int leftPlayer = 1;
    private void CheckRotation()
    {
        if (leftPlayer == 1 && PartsDisplay.player1Robot.transform.position.x > PartsDisplay.player2Robot.transform.position.x)
        {
            PartsDisplay.player1Robot.transform.Rotate(0, 180, 0);
            PartsDisplay.player2Robot.transform.Rotate(0, 180, 0);
            leftPlayer = 2;

            if (player1.playerPosition == 1)
            {
                player1.playerPosition = 2;
                player2.playerPosition = 1;
            }
            else
            {
                player1.playerPosition = 1;
                player2.playerPosition = 2;
            }
        }
        else if (leftPlayer == 2 && PartsDisplay.player1Robot.transform.position.x < PartsDisplay.player2Robot.transform.position.x)
        {
            PartsDisplay.player1Robot.transform.Rotate(0, 180, 0);
            PartsDisplay.player2Robot.transform.Rotate(0, 180, 0);
            leftPlayer = 1;

            if (player1.playerPosition == 1)
            {
                player1.playerPosition = 2;
                player2.playerPosition = 1;
            }
            else
            {
                player1.playerPosition = 1;
                player2.playerPosition = 2;
            }
        }
    }

    public void ResetPositions()
    {
        PartsDisplay.player1Robot.transform.position = maps[usedMaps[usedMaps.Count - 1]].transform.GetChild(1).position;
        PartsDisplay.player2Robot.transform.position = maps[usedMaps[usedMaps.Count - 1]].transform.GetChild(0).position;
    }

    public void CheckEnd()
    {
        if (player1.currentHealth <= 0)
        {
            Debug.Log("Player 2 wins");

            player1.Demolish();

            player2Wins++;
            player2WinDots[player2Wins - 1].GetComponent<Image>().color = Color.blue;

            StartCoroutine(CheckForDraw(2));

            StartCoroutine(EndRound());
        }
        else if (player2.currentHealth <= 0)
        {
            Debug.Log("Player 1 wins");

            player2.Demolish();

            player1Wins++;
            player1WinDots[player1Wins - 1].GetComponent<Image>().color = Color.red;

            StartCoroutine(CheckForDraw(1));

            StartCoroutine(EndRound());
        }
    }

    private IEnumerator CheckForDraw(int player)
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.1f);

            if (player1.currentHealth <= 0 && player2.currentHealth <= 0)
            {
                if (player == 1 && player1Wins < 5)
                {
                    player2Wins++;
                    player2WinDots[player2Wins - 1].GetComponent<Image>().color = Color.blue;
                }
                else if (player == 2 && player2Wins < 5)
                {
                    player1Wins++;
                    player1WinDots[player1Wins - 1].GetComponent<Image>().color = Color.red;
                }

                break;
            }
        }
    }

    private IEnumerator EndRound()
    {
        resettingMap = true;

        player1.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        player2.gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        ResetRobotCommands();

        SendMessageToClient("reset");

        yield return new WaitForSeconds(3f);

        if (player1Wins == 5)
        {
            Debug.Log("Player 1 wins the game");

            SoundEffectsManager.Instance.PlayWinSound();

            endPanel.SetActive(true);
            endText.text = "Player 1 won\nReturning to main menu";

            SendEndMessage();

            yield return new WaitForSeconds(3f);

            SceneManager.LoadScene(1);

            yield break;
        }
        else if (player2Wins == 5)
        {
            Debug.Log("Player 2 wins the game");

            SoundEffectsManager.Instance.PlayWinSound();

            endPanel.SetActive(true);
            endText.text = "Player 2 won\nReturning to main menu";

            SendEndMessage();

            yield return new WaitForSeconds(3f);

            SceneManager.LoadScene(1);

            yield break;
        }

        while (true)
        {
            int randomIndex = UnityEngine.Random.Range(0, maps.Count);
            if (!usedMaps.Contains(randomIndex))
            {
                usedMaps.Add(randomIndex);
                break;
            }
        }

        GameObject currentMap = maps[usedMaps[usedMaps.Count - 1]];
        GameObject lastMap = maps[usedMaps[usedMaps.Count - 2]];
        lastMap.SetActive(false);
        currentMap.SetActive(true);
        Camera.main.orthographicSize = currentMap.GetComponent<Map>().cameraSize;
        currentMap.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Music", 0.5f);

        if (currentMap.GetComponent<Map>().cameraSize == 5)
        {
            smallRoof.SetActive(true);
            bigRoof.SetActive(false);
        }
        else
        {
            smallRoof.SetActive(false);
            bigRoof.SetActive(true);
        }

        if (!Menu.isMultiplayer)
        {
            if (currentMap.GetComponent<Map>().cameraSize == 5)
            {
                PartsDisplay.player2Robot.GetComponent<RobotAi>().borderX = 8;
            }
            else
            {
                PartsDisplay.player2Robot.GetComponent<RobotAi>().borderX = 16;
            }
        }

        mapName.text = currentMap.name.Replace("(Clone)", "");
        round.text = "Round-" + (player1Wins + player2Wins + 1);

        player1.ResetRobot(currentMap.transform.GetChild(1).position);
        player2.ResetRobot(currentMap.transform.GetChild(0).position);

        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(3f);

        Time.timeScale = 1;

        resettingMap = false;
    }

    private void SendEndMessage()
    {
        string message1 = "";
        string message2 = "";
        message1 += PartsDisplay.player1Name.Trim() + " VS " + PartsDisplay.player2Name.Trim() + "/";
        message2 += PartsDisplay.player2Name.Trim() + " VS " + PartsDisplay.player1Name.Trim() + "/";
        message1 += player1Wins + "-" + player2Wins + "/";
        message2 += player2Wins + "-" + player1Wins + "/";
        if (player1Wins > player2Wins)
        {
            message1 += "WON";
            message2 += "LOST";
        }
        else
        {
            message1 += "LOST";
            message2 += "WON";
        }
        message1 += "/" + PartsDisplay.player1Parts + "/" + PartsDisplay.player2Parts;
        message2 += "/" + PartsDisplay.player2Parts + "/" + PartsDisplay.player1Parts;

        LanProtocol.WriteLine(Server.player1Stream, message1);
        if (Menu.isMultiplayer)
            LanProtocol.WriteLine(Server.player2Stream, message2);
    }

    private void ResetRobotCommands()
    {
        red1 = false;
        green1 = false;
        blue1 = false;
        black1 = false;
        white1 = false;
        ignore11 = false;
        ignore12 = false;
        player1Command = "";
        player1Buffer = "";
        string unused;
        while (player1Commands.TryDequeue(out unused)) { }
        while (player2Commands.TryDequeue(out unused)) { }

        if (Menu.isMultiplayer)
        {
            red2 = false;
            green2 = false;
            blue2 = false;
            black2 = false;
            white2 = false;
            ignore11 = false;
            ignore12 = false;
            player2Command = "";
            player2Buffer = "";
        }
    }

    private void FixedUpdate()
    {
        string command;
        if (player1Commands.TryDequeue(out command)) player1Buffer += command;
        if (Menu.isMultiplayer && player2Commands.TryDequeue(out command)) player2Buffer += command;
        DefaultControl();
        CustomControl();
    }

    private void DefaultControl()
    {
        if (PartsDisplay.defaultInput1)
        {
            if (player1Buffer == "")
            {
                player1Command = "";
            }
            else if (player1Buffer[0] == 'r')
            {
                player1Command = "red";
                player1Buffer = player1Buffer.Remove(0, 3);
            }
            else if (player1Buffer[0] == 'g')
            {
                player1Command = "green";
                player1Buffer = player1Buffer.Remove(0, 5);
            }
            else if (player1Buffer[0] == 'b')
            {
                if (player1Buffer[2] == 'u')
                {
                    player1Command = "blue";
                    player1Buffer = player1Buffer.Remove(0, 4);
                }
                else
                {
                    player1Command = "black";
                    player1Buffer = player1Buffer.Remove(0, 5);
                }
            }
            else if (player1Buffer[0] == 'w')
            {
                player1Command = "white";
                player1Buffer = player1Buffer.Remove(0, 5);
            }

            if (player1Command == "green")
            {
                green1 = !green1;
            }
            if (player1Command == "red")
            {
                red1 = !red1;
            }
            if (player1Command == "blue")
            {
                blue1 = !blue1;
            }
            if (player1Command == "black")
            {
                if (!ignore12)
                {
                    black1 = !black1;

                    if (black1 && white1)
                    {
                        white1 = false;
                        ignore11 = true;
                    }
                }
                else
                {
                    ignore12 = false;
                }
            }
            if (player1Command == "white")
            {
                if (!ignore11)
                {
                    white1 = !white1;

                    if (black1 && white1)
                    {
                        black1 = false;
                        ignore12 = true;
                    }
                }
                else
                {
                    ignore11 = false;
                }
            }
            player1Command = "";

            if (green1)
            {
                player1.Special();

                green1 = false;
            }
            if (red1)
            {
                player1.MeleeAttack();
            }
            if (blue1)
            {
                player1.RangedAttack();
            }
            if (black1)
            {
                player1.MoveForward();
            }
            if (white1)
            {
                player1.MoveBackward();
            }
        }

        if (PartsDisplay.defaultInput2 && Menu.isMultiplayer)
        {
            if (player2Buffer == "")
            {
                player2Command = "";
            }
            else if (player2Buffer[0] == 'r')
            {
                player2Command = "red";
                player2Buffer = player2Buffer.Remove(0, 3);
            }
            else if (player2Buffer[0] == 'g')
            {
                player2Command = "green";
                player2Buffer = player2Buffer.Remove(0, 5);
            }
            else if (player2Buffer[0] == 'b')
            {
                if (player2Buffer[2] == 'u')
                {
                    player2Command = "blue";
                    player2Buffer = player2Buffer.Remove(0, 4);
                }
                else
                {
                    player2Command = "black";
                    player2Buffer = player2Buffer.Remove(0, 5);
                }
            }
            else if (player2Buffer[0] == 'w')
            {
                player2Command = "white";
                player2Buffer = player2Buffer.Remove(0, 5);
            }

            if (player2Command == "green")
            {
                green2 = !green2;
            }
            if (player2Command == "red")
            {
                red2 = !red2;
            }
            if (player2Command == "blue")
            {
                blue2 = !blue2;
            }
            if (player2Command == "black")
            {
                if (!ignore22)
                {
                    black2 = !black2;

                    if (black2 && white2)
                    {
                        white2 = false;
                        ignore21 = true;
                    }
                }
                else
                {
                    ignore22 = false;
                }
            }
            if (player2Command == "white")
            {
                if (!ignore21)
                {
                    white2 = !white2;

                    if (black2 && white2)
                    {
                        black2 = false;
                        ignore22 = true;
                    }
                }
                else
                {
                    ignore21 = false;
                }
            }
            player2Command = "";

            if (green2)
            {
                player2.Special();

                green2 = false;
            }
            if (red2)
            {
                player2.MeleeAttack();
            }
            if (blue2)
            {
                player2.RangedAttack();
            }
            if (black2)
            {
                player2.MoveForward();
            }
            if (white2)
            {
                player2.MoveBackward();
            }
        }
    }

    private bool GetOtherWheelColor(int player, int currentColor, bool currentWheelColor)
    {
        bool returnVal = false;
        if (player1)
        {
            if (currentColor == 1)
            {
                if (ignoreInput12 == "red")
                {
                    returnVal = red1;
                    if (red1 && currentWheelColor)
                    {
                        red1 = false;
                    }
                }
                else if (ignoreInput12 == "blue")
                {
                    returnVal = blue1;
                    if (blue1 && currentWheelColor)
                    {
                        blue1 = false;
                    }
                }
                else if (ignoreInput12 == "green")
                {
                    returnVal = green1;
                    if (green1 && currentWheelColor)
                    {
                        green1 = false;
                    }
                }
                else if (ignoreInput12 == "black")
                {
                    returnVal = black1;
                    if (black1 && currentWheelColor)
                    {
                        black1 = false;
                    }
                }
                else if (ignoreInput12 == "white")
                {
                    returnVal = white1;
                    if (white1 && currentWheelColor)
                    {
                        white1 = false;
                    }
                }
            }
            else
            {
                if (ignoreInput11 == "red")
                {
                    returnVal = red1;
                    if (red1 && currentWheelColor)
                    {
                        red1 = false;
                    }
                }
                else if (ignoreInput11 == "blue")
                {
                    returnVal = blue1;
                    if (blue1 && currentWheelColor)
                    {
                        blue1 = false;
                    }
                }
                else if (ignoreInput11 == "green")
                {
                    returnVal = green1;
                    if (green1 && currentWheelColor)
                    {
                        green1 = false;
                    }
                }
                else if (ignoreInput11 == "black")
                {
                    returnVal = black1;
                    if (black1 && currentWheelColor)
                    {
                        black1 = false;
                    }
                }
                else if (ignoreInput11 == "white")
                {
                    returnVal = white1;
                    if (white1 && currentWheelColor)
                    {
                        white1 = false;
                    }
                }
            }
        }
        else
        {
            if (currentColor == 1)
            {
                if (ignoreInput22 == "red")
                {
                    returnVal = red2;
                    if (red2 && currentWheelColor)
                    {
                        red2 = false;
                    }
                }
                else if (ignoreInput22 == "blue")
                {
                    returnVal = blue2;
                    if (blue2 && currentWheelColor)
                    {
                        blue2 = false;
                    }
                }
                else if (ignoreInput22 == "green")
                {
                    returnVal = green2;
                    if (green2 && currentWheelColor)
                    {
                        green2 = false;
                    }
                }
                else if (ignoreInput22 == "black")
                {
                    returnVal = black2;
                    if (black2 && currentWheelColor)
                    {
                        black2 = false;
                    }
                }
                else if (ignoreInput22 == "white")
                {
                    returnVal = white2;
                    if (white2 && currentWheelColor)
                    {
                        white2 = false;
                    }
                }
            }
            else
            {
                if (ignoreInput21 == "red")
                {
                    returnVal = red2;
                    if (red2 && currentWheelColor)
                    {
                        red2 = false;
                    }
                }
                else if (ignoreInput21 == "blue")
                {
                    returnVal = blue2;
                    if (blue2 && currentWheelColor)
                    {
                        blue2 = false;
                    }
                }
                else if (ignoreInput21 == "green")
                {
                    returnVal = green2;
                    if (green2 && currentWheelColor)
                    {
                        green2 = false;
                    }
                }
                else if (ignoreInput21 == "black")
                {
                    returnVal = black2;
                    if (black2 && currentWheelColor)
                    {
                        black2 = false;
                    }
                }
                else if (ignoreInput21 == "white")
                {
                    returnVal = white2;
                    if (white2 && currentWheelColor)
                    {
                        white2 = false;
                    }
                }
            }
        }

        return returnVal;
    }

    private void CustomControl()
    {
        if (!PartsDisplay.defaultInput1)
        {
            if (player1Buffer == "")
            {
                player1Command = "";
            }
            else if (player1Buffer[0] == 'r')
            {
                player1Command = "red";
                player1Buffer = player1Buffer.Remove(0, 3);
            }
            else if (player1Buffer[0] == 'g')
            {
                player1Command = "green";
                player1Buffer = player1Buffer.Remove(0, 5);
            }
            else if (player1Buffer[0] == 'b')
            {
                if (player1Buffer[2] == 'u')
                {
                    player1Command = "blue";
                    player1Buffer = player1Buffer.Remove(0, 4);
                }
                else
                {
                    player1Command = "black";
                    player1Buffer = player1Buffer.Remove(0, 5);
                }
            }
            else if (player1Buffer[0] == 'w')
            {
                player1Command = "white";
                player1Buffer = player1Buffer.Remove(0, 5);
            }

            if (player1Command == "green")
            {
                if (ignoreInput11 == "green")
                {
                    if (!ignore11)
                    {
                        green1 = !green1;

                        if (green1 && GetOtherWheelColor(1, 1, green1))
                        {
                            ignore12 = true;
                        }
                    }
                    else
                    {
                        ignore11 = false;
                    }
                }
                else if (ignoreInput12 == "green")
                {
                    if (!ignore12)
                    {
                        green1 = !green1;

                        if (green1 && GetOtherWheelColor(1, 2, green1))
                        {
                            ignore11 = true;
                        }
                    }
                    else
                    {
                        ignore12 = false;
                    }
                }
                else
                {
                    green1 = !green1;
                }
            }
            if (player1Command == "red")
            {
                if (ignoreInput11 == "red")
                {
                    if (!ignore11)
                    {
                        red1 = !red1;

                        if (red1 && GetOtherWheelColor(1, 1, red1))
                        {
                            ignore12 = true;
                        }
                    }
                    else
                    {
                        ignore11 = false;
                    }
                }
                else if (ignoreInput12 == "red")
                {
                    if (!ignore12)
                    {
                        red1 = !red1;

                        if (red1 && GetOtherWheelColor(1, 2, red1))
                        {
                            ignore11 = true;
                        }
                    }
                    else
                    {
                        ignore12 = false;
                    }
                }
                else
                {
                    red1 = !red1;
                }
            }
            if (player1Command == "blue")
            {
                if (ignoreInput11 == "blue")
                {
                    if (!ignore11)
                    {
                        blue1 = !blue1;

                        if (blue1 && GetOtherWheelColor(1, 1, blue1))
                        {
                            ignore12 = true;
                        }
                    }
                    else
                    {
                        ignore11 = false;
                    }
                }
                else if (ignoreInput12 == "blue")
                {
                    if (!ignore12)
                    {
                        blue1 = !blue1;

                        if (blue1 && GetOtherWheelColor(1, 2, blue1))
                        {
                            ignore11 = true;
                        }
                    }
                    else
                    {
                        ignore12 = false;
                    }
                }
                else
                {
                    blue1 = !blue1;
                }
            }
            if (player1Command == "black")
            {
                if (ignoreInput11 == "black")
                {
                    if (!ignore11)
                    {
                        black1 = !black1;

                        if (black1 && GetOtherWheelColor(1, 1, black1))
                        {
                            ignore12 = true;
                        }
                    }
                    else
                    {
                        ignore11 = false;
                    }
                }
                else if (ignoreInput12 == "black")
                {
                    if (!ignore12)
                    {
                        black1 = !black1;

                        if (black1 && GetOtherWheelColor(1, 2, black1))
                        {
                            ignore11 = true;
                        }
                    }
                    else
                    {
                        ignore12 = false;
                    }
                }
                else
                {
                    black1 = !black1;
                }
            }
            if (player1Command == "white")
            {
                if (ignoreInput11 == "white")
                {
                    if (!ignore11)
                    {
                        white1 = !white1;

                        if (white1 && GetOtherWheelColor(1, 1, white1))
                        {
                            ignore12 = true;
                        }
                    }
                    else
                    {
                        ignore11 = false;
                    }
                }
                else if (ignoreInput12 == "white")
                {
                    if (!ignore12)
                    {
                        white1 = !white1;

                        if (white1 && GetOtherWheelColor(1, 2, white1))
                        {
                            ignore11 = true;
                        }
                    }
                    else
                    {
                        ignore12 = false;
                    }
                }
                else
                {
                    white1 = !white1;
                }
            }
            player1Command = "";

            if (green1)
            {
                foreach (Action action in greenFunctions1)
                {
                    action();
                }

                if (greenType1 == "if")
                {
                    green1 = false;
                }
            }
            if (red1)
            {
                foreach (Action action in redFunctions1)
                {
                    action();
                }

                if (redType1 == "if")
                {
                    red1 = false;
                }
            }
            if (blue1)
            {
                foreach (Action action in blueFunctions1)
                {
                    action();
                }

                if (blueType1 == "if")
                {
                    blue1 = false;
                }
            }
            if (black1)
            {
                foreach (Action action in blackFunctions1)
                {
                    action();
                }

                if (blackType1 == "if")
                {
                    black1 = false;
                }
            }
            if (white1)
            {
                foreach (Action action in whiteFunctions1)
                {
                    action();
                }

                if (whiteType1 == "if")
                {
                    white1 = false;
                }
            }
        }

        if (!PartsDisplay.defaultInput2 && Menu.isMultiplayer)
        {
            if (player2Buffer == "")
            {
                player2Command = "";
            }
            else if (player2Buffer[0] == 'r')
            {
                player2Command = "red";
                player2Buffer = player2Buffer.Remove(0, 3);
            }
            else if (player2Buffer[0] == 'g')
            {
                player2Command = "green";
                player2Buffer = player2Buffer.Remove(0, 5);
            }
            else if (player2Buffer[0] == 'b')
            {
                if (player2Buffer[2] == 'u')
                {
                    player2Command = "blue";
                    player2Buffer = player2Buffer.Remove(0, 4);
                }
                else
                {
                    player2Command = "black";
                    player2Buffer = player2Buffer.Remove(0, 5);
                }
            }
            else if (player2Buffer[0] == 'w')
            {
                player2Command = "white";
                player2Buffer = player2Buffer.Remove(0, 5);
            }

            if (player2Command == "green")
            {
                if (ignoreInput21 == "green")
                {
                    if (!ignore21)
                    {
                        green2 = !green2;

                        if (green2 && GetOtherWheelColor(2, 1, green2))
                        {
                            ignore22 = true;
                        }
                    }
                    else
                    {
                        ignore21 = false;
                    }
                }
                else if (ignoreInput22 == "green")
                {
                    if (!ignore22)
                    {
                        green2 = !green2;

                        if (green2 && GetOtherWheelColor(2, 2, green2))
                        {
                            ignore21 = true;
                        }
                    }
                    else
                    {
                        ignore22 = false;
                    }
                }
                else
                {
                    green2 = !green2;
                }
            }
            if (player2Command == "red")
            {
                if (ignoreInput21 == "red")
                {
                    if (!ignore21)
                    {
                        red2 = !red2;

                        if (red2 && GetOtherWheelColor(2, 1, red2))
                        {
                            ignore22 = true;
                        }
                    }
                    else
                    {
                        ignore21 = false;
                    }
                }
                else if (ignoreInput22 == "red")
                {
                    if (!ignore22)
                    {
                        red2 = !red2;

                        if (red2 && GetOtherWheelColor(2, 2, red2))
                        {
                            ignore21 = true;
                        }
                    }
                    else
                    {
                        ignore22 = false;
                    }
                }
                else
                {
                    red2 = !red2;
                }
            }
            if (player2Command == "blue")
            {
                if (ignoreInput21 == "blue")
                {
                    if (!ignore21)
                    {
                        blue2 = !blue2;

                        if (blue2 && GetOtherWheelColor(2, 1, blue2))
                        {
                            ignore22 = true;
                        }
                    }
                    else
                    {
                        ignore21 = false;
                    }
                }
                else if (ignoreInput22 == "blue")
                {
                    if (!ignore22)
                    {
                        blue2 = !blue2;

                        if (blue2 && GetOtherWheelColor(2, 2, blue2))
                        {
                            ignore21 = true;
                        }
                    }
                    else
                    {
                        ignore22 = false;
                    }
                }
                else
                {
                    blue2 = !blue2;
                }
            }
            if (player2Command == "black")
            {
                if (ignoreInput21 == "black")
                {
                    if (!ignore21)
                    {
                        black2 = !black2;

                        if (black2 && GetOtherWheelColor(2, 1, black2))
                        {
                            ignore22 = true;
                        }
                    }
                    else
                    {
                        ignore21 = false;
                    }
                }
                else if (ignoreInput22 == "black")
                {
                    if (!ignore22)
                    {
                        black2 = !black2;

                        if (black2 && GetOtherWheelColor(2, 2, black2))
                        {
                            ignore21 = true;
                        }
                    }
                    else
                    {
                        ignore22 = false;
                    }
                }
                else
                {
                    black2 = !black2;
                }
            }
            if (player2Command == "white")
            {
                if (ignoreInput21 == "white")
                {
                    if (!ignore21)
                    {
                        white2 = !white2;

                        if (white2 && GetOtherWheelColor(2, 1, white2))
                        {
                            ignore22 = true;
                        }
                    }
                    else
                    {
                        ignore21 = false;
                    }
                }
                else if (ignoreInput22 == "white")
                {
                    if (!ignore22)
                    {
                        white2 = !white2;

                        if (white2 && GetOtherWheelColor(2, 2, white2))
                        {
                            ignore21 = true;
                        }
                    }
                    else
                    {
                        ignore22 = false;
                    }
                }
                else
                {
                    white2 = !white2;
                }
            }
            player2Command = "";

            if (green2)
            {
                foreach (Action action in greenFunctions2)
                {
                    action();
                }

                if (greenType2 == "if")
                {
                    green2 = false;
                }
            }
            if (red2)
            {
                foreach (Action action in redFunctions2)
                {
                    action();
                }

                if (redType2 == "if")
                {
                    red2 = false;
                }
            }
            if (blue2)
            {
                foreach (Action action in blueFunctions2)
                {
                    action();
                }

                if (blueType2 == "if")
                {
                    blue2 = false;
                }
            }
            if (black2)
            {
                foreach (Action action in blackFunctions2)
                {
                    action();
                }

                if (blackType2 == "if")
                {
                    black2 = false;
                }
            }
            if (white2)
            {
                foreach (Action action in whiteFunctions2)
                {
                    action();
                }

                if (whiteType2 == "if")
                {
                    white2 = false;
                }
            }
        }
    }

    private void SendStartMessage()
    {
        string message = "start";

        LanProtocol.WriteLine(Server.player1Stream, message);
        if (Menu.isMultiplayer)
            LanProtocol.WriteLine(Server.player2Stream, message);
    }

    private void SendMessageToClient(string message)
    {
        LanProtocol.WriteLine(Server.player1Stream, message);
        if (Menu.isMultiplayer)
            LanProtocol.WriteLine(Server.player2Stream, message);
    }

    private void WaitForCommands()
    {
        var player1Thread = new System.Threading.Thread(GetCommand);
        player1Thread.Start(Server.player1);

        if (Menu.isMultiplayer)
        {
            var player2Thread = new System.Threading.Thread(GetCommand);
            player2Thread.Start(Server.player2);
        }
    }

    private void GetCommand(object player)
    {
        TcpClient client = (TcpClient)player;

        while (true)
        {
            if (endThread)
            {
                break;
            }

            try
            {
                string command = LanProtocol.ReadLine(client.GetStream());
                if (command == null)
                {
                    Debug.Log("[LAN] Player disconnected");
                    connectionLost = true;
                    break;
                }
                if (!LanProtocol.IsGameCommand(command))
                {
                    Debug.LogWarning("[LAN] Ignoring unknown command: " + command);
                    continue;
                }

                if (player == Server.player1 && !resettingMap)
                {
                    player1Commands.Enqueue(command);
                }
                else if (player == Server.player2 && !resettingMap)
                {
                    player2Commands.Enqueue(command);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[LAN] Battle receive failed: " + e.Message);
                connectionLost = true;
                break;
            }
        }
    }

    private void OnDestroy()
    {
        endThread = true;
    }
}
