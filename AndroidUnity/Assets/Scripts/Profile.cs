using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{

    public Image background;
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI exp;
    public Image levelOrb;
    public Image barHolder;
    public RectTransform expBar;
    public TextMeshProUGUI league;

    public GameObject history1;
    public TextMeshProUGUI history1Names;
    public TextMeshProUGUI history1ResultText;
    public TextMeshProUGUI history1Result;
    public Image history1Body1;
    public Image history1Melee1;
    public Image history1Ranged1;
    public Image history1Special1;
    public Image history1Wheel1;
    public Image history1Body2;
    public Image history1Melee2;
    public Image history1Ranged2;
    public Image history1Special2;
    public Image history1Wheel2;

    public GameObject history2;
    public TextMeshProUGUI history2Names;
    public TextMeshProUGUI history2ResultText;
    public TextMeshProUGUI history2Result;
    public Image history2Body1;
    public Image history2Melee1;
    public Image history2Ranged1;
    public Image history2Special1;
    public Image history2Wheel1;
    public Image history2Body2;
    public Image history2Melee2;
    public Image history2Ranged2;
    public Image history2Special2;
    public Image history2Wheel2;

    public GameObject history3;
    public TextMeshProUGUI history3Names;
    public TextMeshProUGUI history3ResultText;
    public TextMeshProUGUI history3Result;
    public Image history3Body1;
    public Image history3Melee1;
    public Image history3Ranged1;
    public Image history3Special1;
    public Image history3Wheel1;
    public Image history3Body2;
    public Image history3Melee2;
    public Image history3Ranged2;
    public Image history3Special2;
    public Image history3Wheel2;

    public List<Body> bodies;
    public List<Melee> melees;
    public List<Ranged> ranges;
    public List<Special> specials;
    public List<Wheel> wheels;

    public GameObject nameItems;

    private Color paper= new Color(1,1,1);
    private Color bronze= new Color(0.8f, 0.5f, 0.2f);
    private Color silver= new Color(0.8f, 0.8f, 0.8f);
    private Color gold= new Color(1f, 0.8f, 0f);
    private Color diamond= new Color(0, 1, 1);
    private Color hell= new Color(0.8f, 0, 0);

    private void Start()
    {
        playerName.text=PlayerPrefs.GetString("PlayerName","Player");

        int playerLevel = PlayerPrefs.GetInt("Level", 0);
        level.text=playerLevel.ToString();

        if (playerLevel <= 255)
        {
            background.color = new Color(playerLevel / 255f, playerLevel / 255f, playerLevel / 255f);
        }
        else if(playerLevel<1000)
        {
            float colorValue = 1 / (1000 - 255);
            int currentLevelDifference = playerLevel - 255;
            background.color = new Color(1, 1 - currentLevelDifference * colorValue, 1 - currentLevelDifference * colorValue);
        }
        else
        {
            level.fontSize = 70;
            background.color = new Color(1, 0, 0);
        }

        int neededExp = 0;
        int currentExp=PlayerPrefs.GetInt("Exp", 0);

        Color currentColor;
        Color nextColor;
        if (playerLevel<51)
        {
            neededExp = 100;
            league.text="Paper League";
            league.color = paper;
            
            currentColor = paper;
            nextColor = bronze;
        }
        else if(playerLevel<102)
        {
            neededExp = 200;
            league.text="Bronze League";
            league.color = bronze;

            currentColor = bronze;
            nextColor = silver;
        }
        else if(playerLevel<153)
        {
            neededExp = 300;
            league.text="Silver League";
            league.color = silver;

            currentColor = silver;
            nextColor = gold;
        }
        else if(playerLevel<204)
        {
            neededExp = 400;
            league.text="Gold League";
            league.color = gold;

            currentColor = gold;
            nextColor = diamond;
        }
        else if(playerLevel<255)
        {
            neededExp = 500;
            league.text="Diamond League";
            league.color = diamond;

            currentColor = diamond;
            nextColor = hell;
        }
        else
        {
            neededExp = 600;
            league.text="Hell League";
            league.color = hell;

            currentColor = hell;
            nextColor = hell;
        }

        expBar.sizeDelta = new Vector2(737 * currentExp / neededExp, expBar.sizeDelta.y);

        Color color = Color.Lerp(currentColor, nextColor, playerLevel % 51 / 51f);

        levelOrb.color = color;
        barHolder.color = color;

        exp.text = currentExp.ToString() + "/" + neededExp.ToString();

        string history1NamesText = PlayerPrefs.GetString("History1Names", "");
        if(history1NamesText=="")
        {
            history1.SetActive(false);
        }
        else
        {
            history1Names.text = history1NamesText;
            history1ResultText.text = PlayerPrefs.GetString("History1ResultText", "");
            history1Result.text = PlayerPrefs.GetString("History1Result", "");
            if(history1ResultText.text=="WON")
            {
                history1ResultText.color = new Color(0, 1, 0);
                history1Result.color = new Color(0, 1, 0);
            }
            else
            {
                history1ResultText.color = new Color(1, 0, 0);
                history1Result.color = new Color(1, 0, 0);
            }

            string history1Parts1= PlayerPrefs.GetString("History1Parts1", "");
            history1Body1.sprite = bodies[int.Parse(history1Parts1[0].ToString())].sprite;
            history1Melee1.sprite = melees[int.Parse(history1Parts1[1].ToString())].sprite;
            history1Ranged1.sprite = ranges[int.Parse(history1Parts1[2].ToString())].sprite;
            history1Special1.sprite = specials[int.Parse(history1Parts1[3].ToString())].sprite;
            history1Wheel1.sprite = wheels[int.Parse(history1Parts1[4].ToString())].sprite;

            string history1Parts2 = PlayerPrefs.GetString("History1Parts2", "");
            history1Body2.sprite = bodies[int.Parse(history1Parts2[0].ToString())].sprite;
            history1Melee2.sprite = melees[int.Parse(history1Parts2[1].ToString())].sprite;
            history1Ranged2.sprite = ranges[int.Parse(history1Parts2[2].ToString())].sprite;
            history1Special2.sprite = specials[int.Parse(history1Parts2[3].ToString())].sprite;
            history1Wheel2.sprite = wheels[int.Parse(history1Parts2[4].ToString())].sprite;
        }

        string history2NamesText = PlayerPrefs.GetString("History2Names", "");
        if (history2NamesText == "")
        {
            history2.SetActive(false);
        }
        else
        {
            history2Names.text = history2NamesText;
            history2ResultText.text = PlayerPrefs.GetString("History2ResultText", "");
            history2Result.text = PlayerPrefs.GetString("History2Result", "");
            if (history2ResultText.text == "WON")
            {
                history2ResultText.color = new Color(0, 1, 0);
                history2Result.color = new Color(0, 1, 0);
            }
            else
            {
                history2ResultText.color = new Color(1, 0, 0);
                history2Result.color = new Color(1, 0, 0);
            }

            string history2Parts1 = PlayerPrefs.GetString("History2Parts1", "");
            history2Body1.sprite = bodies[int.Parse(history2Parts1[0].ToString())].sprite;
            history2Melee1.sprite = melees[int.Parse(history2Parts1[1].ToString())].sprite;
            history2Ranged1.sprite = ranges[int.Parse(history2Parts1[2].ToString())].sprite;
            history2Special1.sprite = specials[int.Parse(history2Parts1[3].ToString())].sprite;
            history2Wheel1.sprite = wheels[int.Parse(history2Parts1[4].ToString())].sprite;

            string history2Parts2 = PlayerPrefs.GetString("History2Parts2", "");
            history2Body2.sprite = bodies[int.Parse(history2Parts2[0].ToString())].sprite;
            history2Melee2.sprite = melees[int.Parse(history2Parts2[1].ToString())].sprite;
            history2Ranged2.sprite = ranges[int.Parse(history2Parts2[2].ToString())].sprite;
            history2Special2.sprite = specials[int.Parse(history2Parts2[3].ToString())].sprite;
            history2Wheel2.sprite = wheels[int.Parse(history2Parts2[4].ToString())].sprite;
        }

        string history3NamesText = PlayerPrefs.GetString("History3Names", "");

        if (history3NamesText == "")
        {
            history3.SetActive(false);
        }
        else
        {
            history3Names.text = history3NamesText;
            history3ResultText.text = PlayerPrefs.GetString("History3ResultText", "");
            history3Result.text = PlayerPrefs.GetString("History3Result", "");
            if (history3ResultText.text == "WON")
            {
                history3ResultText.color = new Color(0, 1, 0);
                history3Result.color = new Color(0, 1, 0);
            }
            else
            {
                history3ResultText.color = new Color(1, 0, 0);
                history3Result.color = new Color(1, 0, 0);
            }

            string history3Parts1 = PlayerPrefs.GetString("History3Parts1", "");
            history3Body1.sprite = bodies[int.Parse(history3Parts1[0].ToString())].sprite;
            history3Melee1.sprite = melees[int.Parse(history3Parts1[1].ToString())].sprite;
            history3Ranged1.sprite = ranges[int.Parse(history3Parts1[2].ToString())].sprite;
            history3Special1.sprite = specials[int.Parse(history3Parts1[3].ToString())].sprite;
            history3Wheel1.sprite = wheels[int.Parse(history3Parts1[4].ToString())].sprite;

            string history3Parts2 = PlayerPrefs.GetString("History3Parts2", "");
            history3Body2.sprite = bodies[int.Parse(history3Parts2[0].ToString())].sprite;
            history3Melee2.sprite = melees[int.Parse(history3Parts2[1].ToString())].sprite;
            history3Ranged2.sprite = ranges[int.Parse(history3Parts2[2].ToString())].sprite;
            history3Special2.sprite = specials[int.Parse(history3Parts2[3].ToString())].sprite;
            history3Wheel2.sprite = wheels[int.Parse(history3Parts2[4].ToString())].sprite;
        }
    }

    public void UpdateTexts()
    {
        int playerLevel = PlayerPrefs.GetInt("Level", 0);
        level.text = playerLevel.ToString();

        if (playerLevel <= 255)
        {
            background.color = new Color(playerLevel / 255f, playerLevel / 255f, playerLevel / 255f);
        }
        else if (playerLevel < 1000)
        {
            float colorValue = 1 / (1000 - 255);
            int currentLevelDifference = playerLevel - 255;
            background.color = new Color(1, 1 - currentLevelDifference * colorValue, 1 - currentLevelDifference * colorValue);
        }
        else
        {
            level.fontSize = 70;
            background.color = new Color(1, 0, 0);
        }

        int neededExp = 0;
        int currentExp = PlayerPrefs.GetInt("Exp", 0);

        Color currentColor;
        Color nextColor;
        if (playerLevel < 51)
        {
            neededExp = 100;
            league.text = "Paper League";
            league.color = paper;

            currentColor = paper;
            nextColor = bronze;
        }
        else if (playerLevel < 102)
        {
            neededExp = 200;
            league.text = "Bronze League";
            league.color = bronze;

            currentColor = bronze;
            nextColor = silver;
        }
        else if (playerLevel < 153)
        {
            neededExp = 300;
            league.text = "Silver League";
            league.color = silver;

            currentColor = silver;
            nextColor = gold;
        }
        else if (playerLevel < 204)
        {
            neededExp = 400;
            league.text = "Gold League";
            league.color = gold;

            currentColor = gold;
            nextColor = diamond;
        }
        else if (playerLevel < 255)
        {
            neededExp = 500;
            league.text = "Diamond League";
            league.color = diamond;

            currentColor = diamond;
            nextColor = hell;
        }
        else
        {
            neededExp = 600;
            league.text = "Hell League";
            league.color = hell;

            currentColor = hell;
            nextColor = hell;
        }

        expBar.sizeDelta = new Vector2(737 * currentExp / neededExp, expBar.sizeDelta.y);

        Color color = Color.Lerp(currentColor, nextColor, playerLevel % 51 / 51f);

        levelOrb.color = color;
        barHolder.color = color;

        exp.text = currentExp.ToString() + "/" + neededExp.ToString();
    }

    public void ProfileClick()
    {
        gameObject.GetComponent<Animator>().SetTrigger("Move");

        if(Tutorial.tutorialIndex==1)
        {
            Tutorial.tutorialIndex = 2;
        }
        else if(Tutorial.tutorialIndex==2)
        {
            Tutorial.tutorialIndex = 1;
        }

        SoundEffectsManager.Instance.PlayClickSound();
    }

    public void ChangeName()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        nameItems.SetActive(true);
        gameObject.GetComponent<Animator>().SetTrigger("NameGo");
    }

    public void UpdateName()
    {
        playerName.text = PlayerPrefs.GetString("PlayerName", "Player");
    }
}
