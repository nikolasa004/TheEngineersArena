using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartsDisplay : MonoBehaviour
{
    public List<Body> bodies;
    public List<Melee> melees;
    public List<Ranged> ranges;
    public List<Special> specials;
    public List<Wheel> wheels;

    public List<GameObject> bodiesDisplays;
    public List<GameObject> meleesDisplays;
    public List<GameObject> rangesDisplays;
    public List<GameObject> specialsDisplays;
    public List<GameObject> wheelsDisplays;

    public List<GameObject> buttons;
    private bool[] buttonsClicked = new bool[15];

    public GameObject robotRoot;

    public GameObject constructButton;

    public GameObject codeChoicePanel;

    private GameObject selectedBody;
    private GameObject selectedMelee;
    private GameObject selectedRanged;
    private GameObject selectedSpecial;
    private GameObject selectedWheel;

    private bool generatedRobot = false;

    private int[] selectedParts = new int[5];
    public static string selectedPartsMessage;
    public static string[] classes = new string[5];
    public static Dictionary<string, List<string>> functions = new Dictionary<string, List<string>>();
    public static List<Sprite> selectedSprites = new List<Sprite>();

    private void Start()
    {
        Tutorial.tutorialIndex = 4;

        selectedPartsMessage = "";
        classes = new string[5];
        functions = new Dictionary<string, List<string>>();
        selectedSprites = new List<Sprite>();

        int ind=0;

        for(int i = 0; i < 15; i++)
        {
            buttonsClicked[i] = false;
        }

        for(int j = 0; j < 3; j++)
        {
            int index = int.Parse(Connection.partsMessage.Substring(ind, 1));
            Body body = bodies[index];

            List<GameObject> children = new List<GameObject>();
            foreach(Transform child in bodiesDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = body.id.ToUpper();
            children[1].GetComponent<Image>().sprite = body.sprite;

            string functionText = "";
            for (int i = 0; i < body.functions.Count; i++)
            {
                functionText += "."+body.functions[i]+"()\n";
            }
            children[2].GetComponent<TextMeshProUGUI>().text = functionText;

            ind++;
        }

        for (int j = 0; j < 3; j++)
        {
            int index = int.Parse(Connection.partsMessage.Substring(ind, 1));
            Melee melee = melees[index];

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in meleesDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = melee.id.ToUpper();
            children[1].GetComponent<Image>().sprite = melee.sprite;
            
            string functionText = "";
            for (int i = 0; i < melee.functions.Count; i++)
            {
                functionText += "." + melee.functions[i]+"()\n";
            }
            children[2].GetComponent<TextMeshProUGUI>().text = functionText;

            ind++;
        }

        for (int j = 0; j < 3; j++)
        {
            int index = int.Parse(Connection.partsMessage.Substring(ind, 1));
            Ranged ranged = ranges[index];

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in rangesDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = ranged.id.ToUpper();
            children[1].GetComponent<Image>().sprite = ranged.sprite;
            
            string functionText = "";
            for (int i = 0; i < ranged.functions.Count; i++)
            {
                functionText += "." + ranged.functions[i] + "()\n";
            }
            children[2].GetComponent<TextMeshProUGUI>().text = functionText;

            ind++;
        }

        for (int j = 0; j < 3; j++)
        {
            int index = int.Parse(Connection.partsMessage.Substring(ind, 1));
            Special special = specials[index];

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in specialsDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = special.id.ToUpper();
            children[1].GetComponent<Image>().sprite = special.sprite;
            
            string functionText = "";
            for (int i = 0; i < special.functions.Count; i++)
            {
                functionText += "." + special.functions[i] + "()\n";
            }
            children[2].GetComponent<TextMeshProUGUI>().text = functionText;

            ind++;
        }

        for (int j = 0; j < 3; j++)
        {
            int index = int.Parse(Connection.partsMessage.Substring(ind, 1));
            Wheel wheel = wheels[index];

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in wheelsDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = wheel.id.ToUpper();
            children[1].GetComponent<Image>().sprite = wheel.sprite;
            
            string functionText = "";
            for (int i = 0; i < wheel.functions.Count; i++)
            {
                functionText += "." + wheel.functions[i] + "()\n";
            }
            children[2].GetComponent<TextMeshProUGUI>().text = functionText;

            ind++;
        }
    }

    private void Update()
    {
        if(selectedBody!=null && selectedMelee!=null && selectedRanged!=null && selectedSpecial!=null && selectedWheel!=null)
        {
            constructButton.SetActive(true);
        }
        else
        {
            constructButton.SetActive(false);
        }
    }

    public void OnClick(int indexOfButton)
    {
        SoundEffectsManager.Instance.PlayClickSound();

        int indInGroup = indexOfButton % 3;
        int group = indexOfButton / 3;

        if (!buttonsClicked[indexOfButton])
        {
            if (indInGroup == 0)
            {
                buttons[indexOfButton + 1].GetComponent<Button>().interactable = false;
                buttons[indexOfButton + 2].GetComponent<Button>().interactable = false;
            }
            else if (indInGroup == 1)
            {
                buttons[indexOfButton - 1].GetComponent<Button>().interactable = false;
                buttons[indexOfButton + 1].GetComponent<Button>().interactable = false;
            }
            else
            {
                buttons[indexOfButton - 1].GetComponent<Button>().interactable = false;
                buttons[indexOfButton - 2].GetComponent<Button>().interactable = false;
            }

            buttonsClicked[indexOfButton] = true;

            int index = int.Parse(Connection.partsMessage.Substring(indexOfButton, 1));
            if(group == 0)
            {
                selectedBody = bodies[index].prefab;
            }
            else if (group == 1)
            {
               selectedMelee = melees[index].prefab;
            }
            else if (group == 2)
            {
                selectedRanged = ranges[index].prefab;
            }
            else if (group == 3)
            {
                selectedSpecial = specials[index].prefab;   
            }
            else
            {
                selectedWheel = wheels[index].prefab;
            }

            selectedParts[group] = index;

            RobotDisplay(group);
        }
        else
        {
            if (indInGroup == 0)
            {
                buttons[indexOfButton + 1].GetComponent<Button>().interactable = true;
                buttons[indexOfButton + 2].GetComponent<Button>().interactable = true;
            }
            else if (indInGroup == 1)
            {
                buttons[indexOfButton - 1].GetComponent<Button>().interactable = true;
                buttons[indexOfButton + 1].GetComponent<Button>().interactable = true;
            }
            else
            {
                buttons[indexOfButton - 1].GetComponent<Button>().interactable = true;
                buttons[indexOfButton - 2].GetComponent<Button>().interactable = true;
            }

            buttonsClicked[indexOfButton] = false;

            if (group == 0)
            {
                selectedBody = null;
            }
            else if (group == 1)
            {
                selectedMelee = null;
            }
            else if (group == 2)
            {
                selectedRanged = null;
            }
            else if (group == 3)
            {
                selectedSpecial = null;
            }
            else
            {
                selectedWheel = null;
            }

            RobotDisplay(group);
        }
    }

    public void RobotDisplay(int group)
    {
        if(selectedBody==null && !generatedRobot)
        {
            return;
        }

        if(group==0)
        {
            if(selectedBody!=null)
            {
                if (robotRoot.transform.childCount > 0)
                {
                    Destroy(robotRoot.transform.GetChild(0).gameObject);
                }
                GameObject body = Instantiate(selectedBody, robotRoot.transform);
                body.transform.localPosition = new Vector3(0, 0, 0);
                body.transform.localScale = new Vector3(3, 3, 1);
                generatedRobot = true;

                if(selectedMelee!=null)
                {
                    GameObject meleeHolder = body.transform.GetChild(0).gameObject;
                    GameObject melee = Instantiate(selectedMelee, meleeHolder.transform);
                    melee.transform.localPosition = new Vector3(0, 0, 0);
                    Vector2 size = melee.transform.localScale;
                    melee.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
                }
                if(selectedRanged!=null)
                {
                    GameObject rangedHolder = body.transform.GetChild(1).gameObject;
                    GameObject ranged = Instantiate(selectedRanged, rangedHolder.transform);
                    ranged.transform.localPosition = new Vector3(0, 0, 0);
                    Vector2 size = ranged.transform.localScale;
                    ranged.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
                }
                if(selectedSpecial!=null)
                {
                    GameObject specialHolder = body.transform.GetChild(2).gameObject;
                    GameObject special = Instantiate(selectedSpecial, specialHolder.transform);
                    special.transform.localPosition = new Vector3(0, 0, 0);
                    Vector2 size = special.transform.localScale;
                    special.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
                }
                if(selectedWheel!=null)
                {
                    GameObject wheelHolderFront = body.transform.GetChild(3).gameObject;
                    GameObject wheelFront = Instantiate(selectedWheel, wheelHolderFront.transform);
                    wheelFront.transform.localPosition = new Vector3(0, 0, 0);
                    Vector2 size = wheelFront.transform.localScale;
                    wheelFront.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);

                    GameObject wheelHolderBack = body.transform.GetChild(4).gameObject;
                    GameObject wheelBack = Instantiate(selectedWheel, wheelHolderBack.transform);
                    wheelBack.transform.localPosition = new Vector3(0, 0, 0);
                    Vector2 sizeBack = wheelBack.transform.localScale;
                    wheelBack.transform.localScale = new Vector3(sizeBack.x * 10, sizeBack.y * 10, 1);
                }
            }
            else
            {
                Destroy(robotRoot.transform.GetChild(0).gameObject);
                generatedRobot = false;
            }
        }
        else if(group==1)
        {
            if(selectedMelee!=null)
            {
                GameObject robotBody = robotRoot.transform.GetChild(0).gameObject;
                GameObject meleeHolder = robotBody.transform.GetChild(0).gameObject;

                if (meleeHolder.transform.childCount > 0)
                {
                    Destroy(meleeHolder.transform.GetChild(0).gameObject);
                }

                GameObject melee=Instantiate(selectedMelee, meleeHolder.transform);
                melee.transform.localPosition = new Vector3(0, 0, 0);
                Vector2 size=melee.transform.localScale;
                melee.transform.localScale = new Vector3(size.x*10, size.y*10, 1);
            }
            else
            {
                Destroy(robotRoot.transform.GetChild(0).GetChild(0).GetChild(0).gameObject);
            }
        }
        else if(group==2)
        {
            if(selectedRanged!=null)
            {
                GameObject robotBody = robotRoot.transform.GetChild(0).gameObject;
                GameObject rangedHolder = robotBody.transform.GetChild(1).gameObject;

                if (rangedHolder.transform.childCount > 0)
                {
                    Destroy(rangedHolder.transform.GetChild(0).gameObject);
                }

                GameObject ranged = Instantiate(selectedRanged, rangedHolder.transform);
                ranged.transform.localPosition = new Vector3(0, 0, 0);
                Vector2 size = ranged.transform.localScale;
                ranged.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
            }
            else
            {
                Destroy(robotRoot.transform.GetChild(0).GetChild(1).GetChild(0).gameObject);
            }
        }
        else if(group==3)
        {
            if(selectedSpecial!=null)
            {
                GameObject robotBody = robotRoot.transform.GetChild(0).gameObject;
                GameObject specialHolder = robotBody.transform.GetChild(2).gameObject;

                if (specialHolder.transform.childCount > 0)
                {
                    Destroy(specialHolder.transform.GetChild(0).gameObject);
                }

                GameObject special = Instantiate(selectedSpecial, specialHolder.transform);
                special.transform.localPosition = new Vector3(0, 0, 0);
                Vector2 size = special.transform.localScale;
                special.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
            }
            else
            {
                Destroy(robotRoot.transform.GetChild(0).GetChild(2).GetChild(0).gameObject);
            }
        }
        else
        {
            if(selectedWheel!=null)
            {
                GameObject robotBody = robotRoot.transform.GetChild(0).gameObject;
                GameObject wheelHolderFront = robotBody.transform.GetChild(3).gameObject;

                if (wheelHolderFront.transform.childCount > 0)
                {
                    Destroy(wheelHolderFront.transform.GetChild(0).gameObject);
                }

                GameObject Front = Instantiate(selectedWheel, wheelHolderFront.transform);
                Front.transform.localPosition = new Vector3(0, 0, 0);
                Vector2 size = Front.transform.localScale;
                Front.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);

                GameObject wheelHolderBack = robotBody.transform.GetChild(4).gameObject;

                if (wheelHolderBack.transform.childCount > 0)
                {
                    Destroy(wheelHolderBack.transform.GetChild(0).gameObject);
                }

                GameObject wheelBack = Instantiate(selectedWheel, wheelHolderBack.transform);
                wheelBack.transform.localPosition = new Vector3(0, 0, 0);
                Vector2 sizeBack = wheelBack.transform.localScale;
                wheelBack.transform.localScale = new Vector3(sizeBack.x * 10, sizeBack.y * 10, 1);
            }
            else
            {
                Destroy(robotRoot.transform.GetChild(0).GetChild(3).GetChild(0).gameObject);
                Destroy(robotRoot.transform.GetChild(0).GetChild(4).GetChild(0).gameObject);
            }
        }
    }

    public void Construct()
    {
        SoundEffectsManager.Instance.PlayClickSound();

        for (int i = 0; i < 5; i++)
        {
            selectedPartsMessage += selectedParts[i].ToString();
        }

        string bodyClass = bodies[selectedParts[0]].id;
        bodyClass = bodyClass.Substring(0, 1).ToUpper() + bodyClass.Substring(1).ToLower();
        classes[0] = bodyClass;
        for (int i = 0; i < bodies[selectedParts[0]].functions.Count; i++)
        {
            if (!functions.ContainsKey(bodyClass))
            {
                functions.Add(bodyClass, new List<string>());
            }
            functions[bodyClass].Add(bodies[selectedParts[0]].functions[i]);
        }
        selectedSprites.Add(bodies[selectedParts[0]].sprite);

        string meleeClass = melees[selectedParts[1]].id;
        meleeClass = meleeClass.Substring(0, 1).ToUpper() + meleeClass.Substring(1).ToLower();
        classes[1] = meleeClass;
        for (int i = 0; i < melees[selectedParts[1]].functions.Count; i++)
        {
            if (!functions.ContainsKey(meleeClass))
            {
                functions.Add(meleeClass, new List<string>());
            }
            functions[meleeClass].Add(melees[selectedParts[1]].functions[i]);
        }
        selectedSprites.Add(melees[selectedParts[1]].sprite);

        string rangedClass = ranges[selectedParts[2]].id;
        rangedClass = rangedClass.Substring(0, 1).ToUpper() + rangedClass.Substring(1).ToLower();
        classes[2] = rangedClass;
        for (int i = 0; i < ranges[selectedParts[2]].functions.Count; i++)
        {
            if (!functions.ContainsKey(rangedClass))
            {
                functions.Add(rangedClass, new List<string>());
            }
            functions[rangedClass].Add(ranges[selectedParts[2]].functions[i]);
        }
        selectedSprites.Add(ranges[selectedParts[2]].sprite);

        string specialClass = specials[selectedParts[3]].id;
        specialClass = specialClass.Substring(0, 1).ToUpper() + specialClass.Substring(1).ToLower();
        classes[3] = specialClass;
        for (int i = 0; i < specials[selectedParts[3]].functions.Count; i++)
        {
            if (!functions.ContainsKey(specialClass))
            {
                functions.Add(specialClass, new List<string>());
            }
            functions[specialClass].Add(specials[selectedParts[3]].functions[i]);
        }
        selectedSprites.Add(specials[selectedParts[3]].sprite);

        string wheelClass = wheels[selectedParts[4]].id;
        wheelClass = wheelClass.Substring(0, 1).ToUpper() + wheelClass.Substring(1).ToLower();
        classes[4] = wheelClass;
        for (int i = 0; i < wheels[selectedParts[4]].functions.Count; i++)
        {
            if (!functions.ContainsKey(wheelClass))
            {
                functions.Add(wheelClass, new List<string>());
            }
            functions[wheelClass].Add(wheels[selectedParts[4]].functions[i]);
        }
        selectedSprites.Add(wheels[selectedParts[4]].sprite);

        codeChoicePanel.SetActive(true);

        robotRoot.transform.GetChild(0).gameObject.SetActive(false);

        gameObject.SetActive(false);
    }
}
