using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class Programming : MonoBehaviour
{
    public static bool defaultInput;

    public TextMeshProUGUI outputText;

    public List<Image> buttonImages;

    public TextMeshProUGUI errorText;

    public GameObject waitingForServerPanel;

    public GameObject programmingItems;
    public GameObject inputItems;
    public GameObject testItems;

    public TextMeshProUGUI instructions;
    public TextMeshProUGUI errors;
    public TextMeshProUGUI currentColor;

    public GameObject cameraImage;

    private string colorName;
    private WebCamTexture webcamTexture;

    private bool clearingColor = false;

    private string code = "";
    public static string outputCode;

    public static bool cameraInput;

    private string message;

    private int width;
    private int height;
    private int step;
    private float tolerance;
    private int cameraAreaCheck;

    private bool startDelay = true;

    public Animator partPanel;
    public Image partImage;
    public GameObject function1;
    public GameObject function2;
    public Button redButton;
    public Button greenButton;
    public Button blueButton;
    public Button whiteButton;
    public Button blackButton;

    public static Dictionary<string, List<Tuple<string, string>>> colorFunction = new Dictionary<string, List<Tuple<string, string>>>();
    public static Dictionary<string, string> colorTypes = new Dictionary<string, string>();
    private string chosenPart;
    private string chosenFunction;
    private string chosenFunctionType;

    private bool clearing = false;

    private void Start()
    {
        colorFunction.Clear();
        colorTypes.Clear();
        cameraInput = false;

        if (!defaultInput)
        {
            programmingItems.SetActive(true);
            outputCode = "";

            for (int i = 0; i < 5; i++)
            {
                buttonImages[i].sprite = PartsDisplay.selectedSprites[i];
            }

            colorFunction.Add("red", new List<Tuple<string, string>>());
            colorFunction.Add("green", new List<Tuple<string, string>>());
            colorFunction.Add("blue", new List<Tuple<string, string>>());
            colorFunction.Add("white", new List<Tuple<string, string>>());
            colorFunction.Add("black", new List<Tuple<string, string>>());

            colorTypes.Add("red", "");
            colorTypes.Add("green", "");
            colorTypes.Add("blue", "");
            colorTypes.Add("white", "");
            colorTypes.Add("black", "");
        }
        else
        {
            inputItems.SetActive(true);
            Tutorial.tutorialIndex = 6;

            string halfRed = "<color=#FF000066>Red-Melee</color>\n";
            string halfGreen = "<color=#00FF0066>Green-Special</color>\n";
            string halfBlue = "<color=#0000FF66>Blue-Ranged</color>\n";
            string halfWhite = "<color=#FFFFFF66>White-Left</color>\n";
            string halfBlack = "<color=#00000066>Black-Right</color>";

            outputCode = halfRed + halfGreen + halfBlue + halfWhite + halfBlack;
        }

        message = PartsDisplay.selectedPartsMessage + "/";
        if (defaultInput)
        {
            message += "default/";
        }
        else
        {
            message += "custom/";
        }
        message += PlayerPrefs.GetString("PlayerName", "Player") + "/";
    }

    private void Update()
    {
        if (cameraInput && !startDelay)
        {
            RecognizeColors();

            if (!clearingColor)
            {
                StartCoroutine(ClearColor());
            }
        }
    }

    public void BodyClick()
    {
        PartClick();
        partImage.sprite = buttonImages[0].sprite;
        function1.GetComponentInChildren<TextMeshProUGUI>().text = PartsDisplay.functions[PartsDisplay.classes[0]][0];
        function2.SetActive(false);
        chosenPart = "Body";
    }

    public void MeleeClick()
    {
        PartClick();
        partImage.sprite = buttonImages[1].sprite;
        function1.GetComponentInChildren<TextMeshProUGUI>().text = PartsDisplay.functions[PartsDisplay.classes[1]][0];
        function2.SetActive(true);
        function2.GetComponentInChildren<TextMeshProUGUI>().text = PartsDisplay.functions[PartsDisplay.classes[1]][1];
        chosenPart = "Melee";
    }
    public void RangedClick()
    {
        PartClick();
        partImage.sprite = buttonImages[2].sprite;
        function1.GetComponentInChildren<TextMeshProUGUI>().text = PartsDisplay.functions[PartsDisplay.classes[2]][0];
        function2.SetActive(true);
        function2.GetComponentInChildren<TextMeshProUGUI>().text = PartsDisplay.functions[PartsDisplay.classes[2]][1];
        chosenPart = "Ranged";
    }

    public void SpecialClick()
    {
        PartClick();
        partImage.sprite = buttonImages[3].sprite;
        function1.GetComponentInChildren<TextMeshProUGUI>().text = PartsDisplay.functions[PartsDisplay.classes[3]][0];
        function2.SetActive(false);
        chosenPart = "Special";
    }


    public void TireClick()
    {
        PartClick();
        partImage.sprite = buttonImages[4].sprite;
        function1.GetComponentInChildren<TextMeshProUGUI>().text = PartsDisplay.functions[PartsDisplay.classes[4]][0];
        function2.SetActive(true);
        function2.GetComponentInChildren<TextMeshProUGUI>().text = PartsDisplay.functions[PartsDisplay.classes[4]][1];
        chosenPart = "Tire";
    }

    public void PartClick()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        partPanel.SetTrigger("Move");
        redButton.interactable = false;
        greenButton.interactable = false;
        blueButton.interactable = false;
        whiteButton.interactable = false;
        blackButton.interactable = false;
        function1.GetComponent<Button>().interactable = true;
        function2.GetComponent<Button>().interactable = true;
        chosenPart = "";
        chosenFunctionType = "";
        errorText.text = "";
        clearing = false;

        if (function1.GetComponentInChildren<TextMeshProUGUI>().fontSize != function2.GetComponentInChildren<TextMeshProUGUI>().fontSize)
        {
            function1.GetComponentInChildren<TextMeshProUGUI>().fontSize = Mathf.Max(function1.GetComponentInChildren<TextMeshProUGUI>().fontSize, function2.GetComponentInChildren<TextMeshProUGUI>().fontSize);
            function2.GetComponentInChildren<TextMeshProUGUI>().fontSize = function1.GetComponentInChildren<TextMeshProUGUI>().fontSize;
        }
    }

    public void FunctionClick(int function)
    {
        SoundEffectsManager.Instance.PlayClickSound();

        clearing = false;
        if (function == 1)
        {
            if (!redButton.interactable)
            {
                function2.GetComponentInChildren<TextMeshProUGUI>().fontSize /= 1.5f;

                if (function2.activeInHierarchy)
                {
                    function2.GetComponent<Button>().interactable = false;
                }

                redButton.interactable = true;
                greenButton.interactable = true;
                blueButton.interactable = true;
                whiteButton.interactable = true;
                blackButton.interactable = true;

                chosenFunction = function1.GetComponentInChildren<TextMeshProUGUI>().text;

                if (chosenPart == "Body" || chosenPart == "Special")
                {
                    chosenFunctionType = "if";
                }
                else if (chosenPart == "Melee" || chosenPart == "Ranged")
                {
                    chosenFunctionType = "while";
                }
                else
                {
                    chosenFunctionType = "for";
                }
            }
            else
            {
                function2.GetComponentInChildren<TextMeshProUGUI>().fontSize *= 1.5f;


                if (function2.activeInHierarchy)
                {
                    function2.GetComponent<Button>().interactable = true;
                }

                redButton.interactable = false;
                greenButton.interactable = false;
                blueButton.interactable = false;
                whiteButton.interactable = false;
                blackButton.interactable = false;

                chosenFunction = "";
                chosenFunctionType = "";
            }
        }
        else
        {
            if (!redButton.interactable)
            {
                function1.GetComponentInChildren<TextMeshProUGUI>().fontSize /= 1.5f;

                function1.GetComponent<Button>().interactable = false;

                redButton.interactable = true;
                greenButton.interactable = true;
                blueButton.interactable = true;
                whiteButton.interactable = true;
                blackButton.interactable = true;

                chosenFunction = function2.GetComponentInChildren<TextMeshProUGUI>().text;

                if (chosenPart == "Melee" || chosenPart == "Ranged")
                {
                    chosenFunctionType = "if";
                }
                else
                {
                    chosenFunctionType = "for";
                }
            }
            else
            {
                function1.GetComponentInChildren<TextMeshProUGUI>().fontSize *= 1.5f;

                function1.GetComponent<Button>().interactable = true;

                redButton.interactable = false;
                greenButton.interactable = false;
                blueButton.interactable = false;
                whiteButton.interactable = false;
                blackButton.interactable = false;

                chosenFunction = "";
                chosenFunctionType = "";
            }
        }
    }

    public void ColorClick(string color)
    {
        if (colorTypes[color] != chosenFunctionType && colorTypes[color] != "")
        {
            errorText.text = "You can't have two different types of functions for the same color";

            SoundEffectsManager.Instance.PlayErrorSound();

            return;
        }

        for (int i = 0; i < colorFunction[color].Count; i++)
        {
            if (colorFunction[color][i].Item1 == chosenPart)
            {
                if (chosenPart == "Tire")
                {
                    if (colorFunction[color][i].Item2 != chosenFunction)
                    {
                        errorText.text = "You can't go left and right with the same color";

                        SoundEffectsManager.Instance.PlayErrorSound();

                        return;
                    }
                }

                if (colorFunction[color][i].Item2 == chosenFunction)
                {
                    if (!clearing)
                    {
                        errorText.text = "You already have this function for this color, if you click again, it will be removed";

                        SoundEffectsManager.Instance.PlayErrorSound();

                        clearing = true;
                        return;
                    }
                }

            }
        }

        int classOfPart = 0;
        if (chosenPart == "Melee")
        {
            classOfPart = 1;
        }
        else if (chosenPart == "Ranged")
        {
            classOfPart = 2;
        }
        else if (chosenPart == "Special")
        {
            classOfPart = 3;
        }
        else if (chosenPart == "Tire")
        {
            classOfPart = 4;
        }

        if (clearing)
        {
            if (colorFunction[color].Count == 1)
            {
                code = code.Replace(chosenFunctionType + " " + color + " " + "{" + PartsDisplay.classes[classOfPart] + "." + chosenFunction + "()" + "}", "");
                colorFunction[color].Clear();
                colorTypes[color] = "";
            }
            else
            {
                code = code.Replace(PartsDisplay.classes[classOfPart] + "." + chosenFunction + "()", "");
                colorFunction[color].Remove(new Tuple<string, string>(chosenPart, chosenFunction));
            }

            outputText.text = FormatText(code);
            clearing = false;
            PartClick();

            return;
        }

        if (colorFunction[color].Count == 0)
        {
            colorTypes[color] = chosenFunctionType;
        }

        colorFunction[color].Add(new Tuple<string, string>(chosenPart, chosenFunction));
        errorText.text = "";

        if (code == "")
        {
            code = chosenFunctionType + " " + color + " " + "{" + PartsDisplay.classes[classOfPart] + "." + chosenFunction + "()" + "}";
        }
        else if (code.Contains(color))
        {
            code = code.Replace(chosenFunctionType + " " + color + " " + "{", chosenFunctionType + " " + color + " " + "{" + PartsDisplay.classes[classOfPart] + "." + chosenFunction + "()");
        }
        else
        {
            code += "\n" + chosenFunctionType + " " + color + " " + "{" + PartsDisplay.classes[classOfPart] + "." + chosenFunction + "()" + "}";
        }

        outputText.text = FormatText(code);

        PartClick();
    }

    private string FormatText(string text)//maybe change this
    {
        //Regex
        string keywordsPattern = @"(?<=^|\s|\{)(if|while|for)(?=\s|$)";
        string colorsPattern = @"(?<=^|\s)(RED|GREEN|BLUE|WHITE|BLACK)(?=\s|$|(?=\{))";
        string methodCallPattern = @"([A-Za-z_][A-Za-z0-9_]*)\.([A-Za-z_][A-Za-z0-9_]*)\s*\(\)";

        string formattedCode = text;

        formattedCode = Regex.Replace(formattedCode, @"<[^>]+>", "");

        formattedCode = Regex.Replace(formattedCode, keywordsPattern, match => $"<color=yellow>{match.Value}</color>", RegexOptions.IgnoreCase);

        formattedCode = Regex.Replace(formattedCode, colorsPattern, match => $"<color={match.Value.ToLower()}>{match.Value.ToUpper()}</color>", RegexOptions.IgnoreCase);

        formattedCode = Regex.Replace(formattedCode, methodCallPattern, match =>
        {
            string className = match.Groups[1].Value;
            string methodName = match.Groups[2].Value;

            return $"<color=purple>{match.Value}</color>\n";
        });

        formattedCode = Regex.Replace(formattedCode, @"\s+", " ").Trim();
        formattedCode = Regex.Replace(formattedCode, @"(.+?)(?=\b(if|while|for)\b)", "$1\n", RegexOptions.Multiline);
        formattedCode = Regex.Replace(formattedCode, @"\b(if|while|for)\b", match => match.Value.Trim(), RegexOptions.Multiline);
        formattedCode = Regex.Replace(formattedCode, @"\s*{\s*", "\n{\n");
        formattedCode = Regex.Replace(formattedCode, @"\s*}\s*", "\n}\n");

        formattedCode = IndentCheck(formattedCode);

        formattedCode = CheckBrackets(formattedCode);

        return formattedCode;
    }

    string IndentCheck(string text)
    {
        text = Regex.Replace(text, @"(\)\s*)(?=\S)", "$1\n");

        var lines = text.Split('\n');
        StringBuilder formattedCode = new StringBuilder();
        int indentLevel = 0;

        foreach (var rawLine in lines)
        {
            string line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line)) continue;

            if (line.StartsWith("}"))
            {
                indentLevel = System.Math.Max(0, indentLevel - 1);
            }

            formattedCode.AppendLine(new string('\t', indentLevel) + line);

            if (line.EndsWith("{"))
            {
                indentLevel++;
            }
        }

        return formattedCode.ToString();
    }

    string CheckBrackets(string text)
    {
        Stack<int> openBraces = new Stack<int>();
        HashSet<int> matchingBraces = new HashSet<int>();

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '{')
            {
                openBraces.Push(i);
            }
            else if (text[i] == '}')
            {
                if (openBraces.Count > 0)
                {
                    int openIndex = openBraces.Pop();
                    matchingBraces.Add(openIndex);
                    matchingBraces.Add(i);
                }
            }
        }

        string formattedText = "";
        for (int i = 0; i < text.Length; i++)
        {
            if (matchingBraces.Contains(i))
            {
                if (text[i] == '{')
                    formattedText += "<color=yellow>{</color>";
                else if (text[i] == '}')
                    formattedText += "<color=yellow>}</color>";
            }
            else
            {
                formattedText += text[i];
            }
        }

        return formattedText;
    }

    public void OnSubmit()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        List<string> colors = new List<string> { "red", "green", "blue", "white", "black" };

        foreach (var color in colors)
        {
            string functions = "";
            foreach (var function in colorFunction[color])
            {
                functions += function.Item1 + "." + function.Item2 + "|";
            }
            message += color + "-" + colorTypes[color] + "-" + functions + "/";
        }

        programmingItems.SetActive(false);
        inputItems.SetActive(true);

        Tutorial.tutorialIndex = 6;

        outputCode = outputText.text;
    }

    public void ButtonsInput()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        ChooseInput();
    }

    public void CameraInput()
    {
        SoundEffectsManager.Instance.PlayClickSound();

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

        step = 25; //check every 50th pixel
        tolerance = 0.4f;

        cameraAreaCheck = (width * height) / (step * step);

        cameraInput = true;

        inputItems.SetActive(false);
        testItems.SetActive(true);

        StartCoroutine(StartDelay());

        TestCamera();
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(2);
        startDelay = false;
    }

    public void ChooseInput()
    {
        SendMessageToServer(message);

        waitingForServerPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void TestCamera()
    {
        testItems.SetActive(true);

        instructions.text = "Set phone where you think is the best, don't hold it in your hand, find stable position. Make sure that there are no red, green or blue objects in the background. Test will start in 10 seconds.";

        StartCoroutine(StartTest());
    }

    private IEnumerator StartTest()
    {
        yield return new WaitForSeconds(10);

        instructions.text = "Test started, you need to be sure that not a single red, green, blue, white or black has been detected. You need to wait for 10 seconds to be sure that your background won't be a problem. If any of the colors has been detected, test will be restarted.";

        StartCoroutine(NoColorTest());
    }

    public IEnumerator NoColorTest()
    {
        float timer = 0f;

        while (timer < 10)
        {
            yield return new WaitForEndOfFrame();

            if (webcamTexture.didUpdateThisFrame)
            {
                if (colorName != "")
                {
                    SoundEffectsManager.Instance.PlayErrorSound();

                    Debug.Log("Detected color: " + colorName);
                    errors.text = "Detected color: " + colorName + ". Test will be restarted in 3 seconds.";
                    timer = 0f;

                    yield return new WaitForSeconds(3);

                    errors.text = "";
                }
                else
                {
                    timer += Time.deltaTime;
                }
            }
        }

        SoundEffectsManager.Instance.PlaySuccessSound();

        instructions.text = "Test finished, your background does not contain any red, green, blue, white or black objects. Next test is recognizing colors, get objects of red, green, blue, white and black color and show one that is asked. Test will start in 10 seconds.";

        yield return new WaitForSeconds(10);

        StartCoroutine(RecognizeColorsTest());
    }

    public IEnumerator RecognizeColorsTest()
    {
        List<string> colors = new List<string> { "red", "green", "blue", "white", "black", "red", "green", "blue", "white", "black" };
        int index = 0;

        while (index < colors.Count)
        {
            instructions.text = "Show " + colors[index] + " object";

            if (colorName == colors[index])
            {
                Debug.Log("Detected color: " + colorName);
                index++;

                SoundEffectsManager.Instance.PlaySuccessSound();

                instructions.text = "Correct color detected, next color will be shown in 3 seconds. Hide current object.";

                yield return new WaitForSeconds(3);
            }
            else if (colorName != "")
            {
                SoundEffectsManager.Instance.PlayErrorSound();

                Debug.Log("Detected color: " + colorName);
                errors.text = "Detected color: " + colorName + ". Test will be restarted in 3 seconds.";
                index = 0;

                yield return new WaitForSeconds(3);

                errors.text = "";
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }

        instructions.text = "Test finished, you have found good position for the camera and good objects for the colors. The game will start in 10 seconds.";

        yield return new WaitForSeconds(10);

        ChooseInput();
    }

    private IEnumerator ClearColor()
    {
        clearingColor = true;

        yield return new WaitForSeconds(0.5f);

        colorName = "";

        currentColor.text = "Color";
        currentColor.color = Color.gray;

        clearingColor = false;
    }

    void SendMessageToServer(string message)
    {
        LanProtocol.WriteLine(Client.stream, message);

        Debug.Log("Message " + message + " sent to server");
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

        if (redPixels >= cameraAreaCheck / 20)
        {
            colorName = "red";
            currentColor.text = "Red";
            currentColor.color = Color.red;
        }
        else if (greenPixels >= cameraAreaCheck / 20)
        {
            colorName = "green";
            currentColor.text = "Green";
            currentColor.color = Color.green;
        }
        else if (bluePixels >= cameraAreaCheck / 25)
        {
            colorName = "blue";
            currentColor.text = "Blue";
            currentColor.color = Color.blue;
        }
        else if (whitePixels >= cameraAreaCheck / 5)
        {
            colorName = "white";
            currentColor.text = "White";
            currentColor.color = Color.white;
        }
        else if (blackPixels >= cameraAreaCheck / 5)
        {
            colorName = "black";
            currentColor.text = "Black";
            currentColor.color = Color.black;
        }
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
}