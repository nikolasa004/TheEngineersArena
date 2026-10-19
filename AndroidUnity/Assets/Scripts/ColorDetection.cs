using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorDetection : MonoBehaviour
{
    public TextMeshProUGUI colorText;
    public string colorName;

    private WebCamTexture webcamTexture;

    private int width;
    private int height;
    private float tolerance;
    private int step;
    private int cameraAreaCheck;

    private bool startDelay = true;

    private void Start()
    {
        webcamTexture = new WebCamTexture(640, 480);//setting the resolution of the camera
        gameObject.GetComponent<RawImage>().texture = webcamTexture;
        webcamTexture.Play();//starting the camera

        width = webcamTexture.width;
        height = webcamTexture.height;

        tolerance = 0.4f;
        step = 25; //check every 50th pixel

        cameraAreaCheck = (width * height) / (step * step);

        StartCoroutine(StartDelay());
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(2);
        startDelay = false;
    }

    private void Update()
    {
        if (startDelay)
        {
            return;
        }
        RecognizeColors();
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
                    redPixels++;
                }
                else if (IsColorMatching(pixel, Color.green, 0.5f))//checking if the pixel is green
                {
                    greenPixels++;
                }
                else if (IsColorMatching(pixel, Color.blue, tolerance))//checking if the pixel is blue
                {
                    bluePixels++;
                }
                else if(IsColorMatching(pixel, Color.white, tolerance))//checking if the pixel is white
                {
                    whitePixels++;
                }
                else if (IsColorMatching(pixel, Color.black, 0.2f))//checking if the pixel is black
                {
                    blackPixels++;
                }
            }
        }

        if (redPixels >= cameraAreaCheck / 20)
        {
            colorText.text = "Red";
            colorText.color = Color.red;
            colorName = "red";

            Debug.Log(redPixels+" "+cameraAreaCheck/20);
        }
        if (greenPixels >= cameraAreaCheck / 20)
        {
            colorText.text = "Green";
            colorText.color = Color.green;
            colorName = "green";
            Debug.Log(greenPixels + " " + cameraAreaCheck / 20);
        }
        if (bluePixels >= cameraAreaCheck / 25)
        {
            colorText.text = "Blue";
            colorText.color = Color.blue;
            colorName = "blue";
            Debug.Log(bluePixels + " " + cameraAreaCheck / 25);
        }
        if (whitePixels >= cameraAreaCheck / 10)
        {
            colorText.text = "White";
            colorText.color = Color.white;
            colorName = "white";
            Debug.Log(whitePixels + " " + cameraAreaCheck / 10);
        }
        if (blackPixels >= cameraAreaCheck / 5)
        {
            colorText.text = "Black";
            colorText.color = Color.black;
            colorName = "black";
            Debug.Log(blackPixels + " " + cameraAreaCheck / 5);
        }

        Debug.Log("----------------------------------------End of frame----------------------------------------");
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

        webcamTexture.Stop();
    }
}
