using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

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

    public List<GameObject> singleplayerMaps;
    public List<GameObject> multiplayerMaps;
    private List<GameObject> maps;

    public GameObject battleCanvas;

    public static string partsMessage;

    public static string player1PartsCode;
    public static string player2PartsCode;
    public static bool defaultInput1;
    public static bool defaultInput2;

    public static GameObject player1Robot;
    public static GameObject player2Robot;

    public static string player1Name;
    public static string player1Parts;
    public static string player2Name;
    public static string player2Parts;

    public static Dictionary<string, Dictionary<string, List<Tuple<string, string>>>> parsed1Code;
    public static Dictionary<string, Dictionary<string, List<Tuple<string, string>>>> parsed2Code;

    private List<int> partIndexes = new List<int>();

    private bool connectionLost = false;

    List<string> robotNames = new List<string>
{
    "Titan",
    "Warrior-X",
    "CyberCrusher",
    "MechaStorm",
    "SteelReaper",
    "OmegaPrime",
    "NeonPhantom",
    "Juggernaut",
    "ShadowCore",
    "VoltStriker",
    "Dreadnought",
    "MetalFury",
    "QuantumBlade",
    "Destructor",
    "HyperTalon",
    "TeraKnight",
    "XenonHunter",
    "NanoWarden",
    "InfernoDroid",
    "Predator",
    "Sentinel-9",
    "VortexRipper",
    "ThunderBot",
    "MechaViper",
    "EchoUnit",
    "BlitzRogue",
    "Shockwave",
    "PlasmaGolem",
    "AetherGuard",
    "RustPhantom",
    "Sentinel",
    "SolarFang",
    "CyberWraith",
    "TitaniumFang",
    "Eclipse",
    "FrostByte",
    "DoomCore",
    "Velocity-X",
    "Striker",
    "Reaper",
    "MetalTitan",
    "Destroyer",
    "SVanguard",
    "NovaCrush",
    "BoltSpecter",
    "GrimCharger",
    "WarpBreaker",
    "TurbineSlayer",
    "CyberNomad",
    "Executioner",
    "ApexSentinel",
    "ExoGladiator",
    "ShadowHavoc",
    "PsychoGear",
    "FusionStalker",
    "Colossus",
    "NeuralKnight",
    "TerraDominus",
    "OmegaBreaker",
    "BlazingGear",
    "OnyxPredator",
    "Nebula",
    "Phantom",
    "Drift",
    "CyberClaw",
    "Magnetron",
    "IronWarden",
    "Executor",
    "HavocStrider",
    "VoidEnforcer",
    "ThunderHex",
    "StormBrawler",
    "DarkCircuit",
    "GigaStrider",
    "MechaInferno",
    "ShrapnelFang",
    "Oblivion",
    "NeonRampage",
    "Berserker",
    "Overlord",
    "SolarTempest",
    "Plasma",
    "EchoKnight",
    "NitroRogue"
};

    private void Start()
    {
        partsMessage = "";
        player1PartsCode = "";
        player2PartsCode = "";
        player1Robot = null;
        player2Robot = null;
        defaultInput1 = false;
        defaultInput2 = false;
        player1Name = "";
        player1Parts = "";
        player2Name = "";
        player2Parts = "";
        parsed1Code = new();
        parsed2Code = new();

        if (Menu.isMultiplayer)
        {
            maps = multiplayerMaps;
        }
        else
        {
            maps = singleplayerMaps;
        }

        List<int> unusedIndexes = new List<int> { 0, 1, 2, 3, 4 };
        for (int j = 0; j < 3; j++)
        {
            int randomIndex = UnityEngine.Random.Range(0, unusedIndexes.Count);
            int index = unusedIndexes[randomIndex];

            partIndexes.Add(index);

            partsMessage += index.ToString();

            unusedIndexes.RemoveAt(randomIndex);
            Body body = bodies[index];

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in bodiesDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = body.id.ToUpper();
            children[1].GetComponent<Image>().sprite = body.sprite;
            children[2].GetComponent<TextMeshProUGUI>().text = "Health:" + body.health.ToString();
            children[3].GetComponent<TextMeshProUGUI>().text = "Armor:" + body.armor.ToString();
        }

        unusedIndexes = new List<int> { 0, 1, 2, 3, 4 };
        for (int j = 0; j < 3; j++)
        {
            int randomIndex = UnityEngine.Random.Range(0, unusedIndexes.Count);
            int index = unusedIndexes[randomIndex];

            partIndexes.Add(index);

            partsMessage += index.ToString();

            unusedIndexes.RemoveAt(randomIndex);
            Melee melee = melees[index];

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in meleesDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = melee.id.ToUpper();
            children[1].GetComponent<Image>().sprite = melee.sprite;
            children[2].GetComponent<TextMeshProUGUI>().text = "Attack:" + melee.attack.ToString();
            children[3].GetComponent<TextMeshProUGUI>().text = "Speed:" + melee.speed.ToString();
            children[4].GetComponent<TextMeshProUGUI>().text = "Uses:" + melee.uses.ToString();
            children[5].GetComponent<TextMeshProUGUI>().text = "Cooldown:" + melee.cooldown.ToString();
        }

        unusedIndexes = new List<int> { 0, 1, 2, 3, 4 };
        for (int j = 0; j < 3; j++)
        {
            int randomIndex = UnityEngine.Random.Range(0, unusedIndexes.Count);
            int index = unusedIndexes[randomIndex];

            partIndexes.Add(index);

            partsMessage += index.ToString();

            unusedIndexes.RemoveAt(randomIndex);
            Ranged ranged = ranges[index];

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in rangesDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = ranged.id.ToUpper();
            children[1].GetComponent<Image>().sprite = ranged.sprite;
            children[2].GetComponent<TextMeshProUGUI>().text = "Attack:" + ranged.attack.ToString();
            children[3].GetComponent<TextMeshProUGUI>().text = "Speed:" + ranged.speed.ToString();
            children[4].GetComponent<TextMeshProUGUI>().text = "Uses:" + ranged.uses.ToString();
            children[5].GetComponent<TextMeshProUGUI>().text = "Cooldown:" + ranged.cooldown.ToString();
        }

        unusedIndexes = new List<int> { 0, 1, 2, 3, 4 };
        for (int j = 0; j < 3; j++)
        {
            int randomIndex = UnityEngine.Random.Range(0, unusedIndexes.Count);
            int index = unusedIndexes[randomIndex];

            partIndexes.Add(index);

            partsMessage += index.ToString();

            unusedIndexes.RemoveAt(randomIndex);
            Special special = specials[index];

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in specialsDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = special.id.ToUpper();
            children[1].GetComponent<Image>().sprite = special.sprite;
            children[2].GetComponent<TextMeshProUGUI>().text = "Uses:" + special.uses.ToString();
            children[3].GetComponent<TextMeshProUGUI>().text = "Effect:\n" + special.effect;
        }

        unusedIndexes = new List<int> { 0, 1, 2, 3, 4 };
        for (int j = 0; j < 3; j++)
        {
            int randomIndex = UnityEngine.Random.Range(0, unusedIndexes.Count);
            int index = unusedIndexes[randomIndex];

            partIndexes.Add(index);

            partsMessage += index.ToString();

            unusedIndexes.RemoveAt(randomIndex);
            Wheel wheel = wheels[index];

            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in wheelsDisplays[j].transform)
            {
                children.Add(child.gameObject);
            }
            children[0].GetComponent<TextMeshProUGUI>().text = wheel.id.ToUpper();
            children[1].GetComponent<Image>().sprite = wheel.sprite;
            children[2].GetComponent<TextMeshProUGUI>().text = "Speed:" + wheel.speed.ToString();
            children[3].GetComponent<TextMeshProUGUI>().text = "Power:" + wheel.power.ToString();
        }
    }

    private bool isListening = false;
    private void Update()
    {
        if (connectionLost)
        {
            connectionLost = false;
            StartCoroutine(Server.CloseClients());
            return;
        }

        if (Connection.displayedParts)
        {
            if (!isListening)
            {
                isListening = true;
                StartListeningClients();
            }
        }

        if (!Menu.isMultiplayer && player1PartsCode != "")
        {
            Debug.Log("Player 1 parts code: " + player1PartsCode);

            int mapIndex = StartBattle();

            battleCanvas.SetActive(true);
            battleCanvas.GetComponent<Battle>().GetFirstMapIndex(mapIndex);

            gameObject.SetActive(false);
        }
        else if (Menu.isMultiplayer && player1PartsCode != "" && player2PartsCode != "")
        {
            Debug.Log("Player 1 parts code: " + player1PartsCode);
            Debug.Log("Player 2 parts code: " + player2PartsCode);

            int mapIndex = StartBattle();

            battleCanvas.SetActive(true);
            battleCanvas.GetComponent<Battle>().GetFirstMapIndex(mapIndex);

            gameObject.SetActive(false);
        }
    }

    private void StartListeningClients()
    {
        if (Menu.isMultiplayer)
        {
            var player1Thread = new System.Threading.Thread(HandleClient);
            player1Thread.Start(Server.player1);

            var player2Thread = new System.Threading.Thread(HandleClient);
            player2Thread.Start(Server.player2);
        }
        else
        {
            var player1Thread = new System.Threading.Thread(HandleClient);
            player1Thread.Start(Server.player1);
        }
    }

    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;

        string message;
        try
        {
            message = LanProtocol.ReadLine(client.GetStream());
            if (string.IsNullOrEmpty(message))
            {
                connectionLost = true;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[LAN] Reading robot code failed: " + e.Message);
            connectionLost = true;
            return;
        }

        if (client == Server.player1)
        {
            player1PartsCode = message;
        }
        else
        {
            player2PartsCode = message;
        }
    }

    public int StartBattle()
    {
        Camera.main.GetComponent<AudioSource>().Stop();

        int randomIndex = UnityEngine.Random.Range(0, maps.Count);
        GameObject map = maps[randomIndex];
        map.SetActive(true);

        map.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Music", 0.5f);

        GameObject spawn1 = map.transform.GetChild(1).gameObject;
        GameObject spawn2 = map.transform.GetChild(0).gameObject;

        int player1BodyIndex = int.Parse(player1PartsCode[0].ToString());
        int player1MeleeIndex = int.Parse(player1PartsCode[1].ToString());
        int player1RangedIndex = int.Parse(player1PartsCode[2].ToString());
        int player1SpecialIndex = int.Parse(player1PartsCode[3].ToString());
        int player1WheelIndex = int.Parse(player1PartsCode[4].ToString());

        player1Parts = player1BodyIndex.ToString() + player1MeleeIndex.ToString() + player1RangedIndex.ToString() + player1SpecialIndex.ToString() + player1WheelIndex.ToString();

        string defaultMessage = "";
        int index = 6;
        char currentChar = ' ';
        while (true)
        {
            currentChar = player1PartsCode[index];
            if (currentChar != '/')
            {
                defaultMessage += currentChar;
            }
            else
            {
                break;
            }
            index++;
        }

        List<string> tokens = player1PartsCode.Split('/').Skip(2).Where(t => !string.IsNullOrEmpty(t)).ToList();
        player1Name = tokens[0];
        tokens.RemoveAt(0);
        if (defaultMessage == "default")
        {
            defaultInput1 = true;
        }
        else
        {
            defaultInput1 = false;

            foreach (string token in tokens)
            {
                string[] mainParts = token.Split('-');

                if (mainParts[1] == "")
                {
                    parsed1Code[mainParts[0]] = new Dictionary<string, List<Tuple<string, string>>>();
                    continue;
                }

                string color = mainParts[0];
                string condition = mainParts[1];
                string actions = mainParts[2];

                List<string> actionGroups = actions.Split('|').ToList();

                List<Tuple<string, string>> detailedActions = actionGroups.Select(g => { var parts = g.Split('.'); return parts.Length == 2 ? Tuple.Create(parts[0], parts[1]) : null; }).Where(t => t != null).ToList();

                if (!parsed1Code.ContainsKey(color))
                    parsed1Code[color] = new Dictionary<string, List<Tuple<string, string>>>();

                parsed1Code[color][condition] = detailedActions;
            }
        }

        GameObject body = Instantiate(bodies[player1BodyIndex].prefab, spawn1.transform.position, Quaternion.identity);
        player1Robot = body;
        body.transform.position = new Vector3(body.transform.position.x, body.transform.position.y, 0);
        body.transform.localScale = new Vector3(2, 2, 1);
        body.layer = 6;
        RobotFunctions player1Script = body.GetComponent<RobotFunctions>();
        player1Script.maxHealth = bodies[player1BodyIndex].health;
        player1Script.armor = bodies[player1BodyIndex].armor;

        GameObject meleeHolder = body.transform.GetChild(0).gameObject;
        GameObject melee = Instantiate(melees[player1MeleeIndex].prefab, meleeHolder.transform);
        melee.transform.localPosition = new Vector3(0, 0, 0);
        Vector2 size = melee.transform.localScale;
        melee.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
        melee.layer = 7;
        player1Script.meleeCooldown = melees[player1MeleeIndex].cooldown;
        player1Script.meleeUses = melees[player1MeleeIndex].uses;
        player1Script.meleeDamage = melees[player1MeleeIndex].attack;
        player1Script.meleeSpeed = melees[player1MeleeIndex].speed;

        GameObject rangedHolder = body.transform.GetChild(1).gameObject;
        GameObject ranged = Instantiate(ranges[player1RangedIndex].prefab, rangedHolder.transform);
        ranged.transform.localPosition = new Vector3(0, 0, 0);
        size = ranged.transform.localScale;
        ranged.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
        player1Script.rangedCooldown = ranges[player1RangedIndex].cooldown;
        player1Script.rangedUses = ranges[player1RangedIndex].uses;
        player1Script.rangedDamage = ranges[player1RangedIndex].attack;
        player1Script.rangedSpeed = ranges[player1RangedIndex].speed;

        GameObject specialHolder = body.transform.GetChild(2).gameObject;
        GameObject special = Instantiate(specials[player1SpecialIndex].prefab, specialHolder.transform);
        special.transform.localPosition = new Vector3(0, 0, 0);
        size = special.transform.localScale;
        special.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
        player1Script.specialUses = specials[player1SpecialIndex].uses;

        GameObject wheelHolderFront = body.transform.GetChild(3).gameObject;
        GameObject wheelFront = Instantiate(wheels[player1WheelIndex].prefab, wheelHolderFront.transform);
        wheelFront.transform.localPosition = new Vector3(0, 0, 0);
        size = wheelFront.transform.localScale;
        wheelFront.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
        wheelFront.layer = 7;
        player1Script.wheelSpeed = wheels[player1WheelIndex].speed;
        player1Script.wheelPower = wheels[player1WheelIndex].power;

        GameObject wheelHolderBack = body.transform.GetChild(4).gameObject;
        GameObject wheelBack = Instantiate(wheels[player1WheelIndex].prefab, wheelHolderBack.transform);
        wheelBack.transform.localPosition = new Vector3(0, 0, 0);
        Vector2 sizeBack = wheelBack.transform.localScale;
        wheelBack.transform.localScale = new Vector3(sizeBack.x * 10, sizeBack.y * 10, 1);
        wheelBack.layer = 7;

        body.transform.localRotation = Quaternion.Euler(0, 180, 0);

        int player2BodyIndex;
        int player2MeleeIndex;
        int player2RangedIndex;
        int player2SpecialIndex;
        int player2WheelIndex;
        if (Menu.isMultiplayer)
        {
            player2BodyIndex = int.Parse(player2PartsCode[0].ToString());
            player2MeleeIndex = int.Parse(player2PartsCode[1].ToString());
            player2RangedIndex = int.Parse(player2PartsCode[2].ToString());
            player2SpecialIndex = int.Parse(player2PartsCode[3].ToString());
            player2WheelIndex = int.Parse(player2PartsCode[4].ToString());


            defaultMessage = "";
            index = 6;
            currentChar = ' ';
            while (true)
            {
                currentChar = player2PartsCode[index];
                if (currentChar != '/')
                {
                    defaultMessage += currentChar;
                }
                else
                {
                    break;
                }
                index++;
            }

            tokens = player2PartsCode.Split('/').Skip(2).Where(t => !string.IsNullOrEmpty(t)).ToList();
            player2Name = tokens[0];
            tokens.RemoveAt(0);
            if (defaultMessage == "default")
            {
                defaultInput2 = true;
            }
            else
            {
                defaultInput2 = false;

                foreach (string token in tokens)
                {
                    string[] mainParts = token.Split('-');

                    if (mainParts[1] == "")
                    {
                        parsed2Code[mainParts[0]] = new Dictionary<string, List<Tuple<string, string>>>();
                        continue;
                    }

                    string color = mainParts[0];
                    string condition = mainParts[1];
                    string actions = mainParts[2];

                    List<string> actionGroups = actions.Split('|').ToList();

                    List<Tuple<string, string>> detailedActions = actionGroups.Select(g => { var parts = g.Split('.'); return parts.Length == 2 ? Tuple.Create(parts[0], parts[1]) : null; }).Where(t => t != null).ToList();

                    if (!parsed2Code.ContainsKey(color))
                        parsed2Code[color] = new Dictionary<string, List<Tuple<string, string>>>();

                    parsed2Code[color][condition] = detailedActions;
                }
            }
        }
        else
        {
            int randomNameIndex = UnityEngine.Random.Range(0, robotNames.Count);
            player2Name = robotNames[randomNameIndex];

            int part = 0;
            int random = UnityEngine.Random.Range(0, 3);
            player2BodyIndex = partIndexes[part * 3 + random];
            part++;
            random = UnityEngine.Random.Range(0, 3);
            player2MeleeIndex = partIndexes[part * 3 + random];
            part++;
            random = UnityEngine.Random.Range(0, 3);
            player2RangedIndex = partIndexes[part * 3 + random];
            part++;
            random = UnityEngine.Random.Range(0, 3);
            player2SpecialIndex = partIndexes[part * 3 + random];
            part++;
            random = UnityEngine.Random.Range(0, 3);
            player2WheelIndex = partIndexes[part * 3 + random];
        }

        player2Parts = player2BodyIndex.ToString() + player2MeleeIndex.ToString() + player2RangedIndex.ToString() + player2SpecialIndex.ToString() + player2WheelIndex.ToString();

        GameObject body2 = Instantiate(bodies[player2BodyIndex].prefab, spawn2.transform.position, Quaternion.identity);
        player2Robot = body2;
        body2.transform.position = new Vector3(body2.transform.position.x, body2.transform.position.y, 0);
        body2.transform.localScale = new Vector3(2, 2, 1);
        body2.layer = 8;
        RobotFunctions player2Script = body2.GetComponent<RobotFunctions>();
        player2Script.maxHealth = bodies[player2BodyIndex].health;
        player2Script.armor = bodies[player2BodyIndex].armor;

        GameObject meleeHolder2 = body2.transform.GetChild(0).gameObject;
        GameObject melee2 = Instantiate(melees[player2MeleeIndex].prefab, meleeHolder2.transform);
        melee2.transform.localPosition = new Vector3(0, 0, 0);
        size = melee2.transform.localScale;
        melee2.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
        melee2.layer = 9;
        player2Script.meleeCooldown = melees[player2MeleeIndex].cooldown;
        player2Script.meleeUses = melees[player2MeleeIndex].uses;
        player2Script.meleeDamage = melees[player2MeleeIndex].attack;
        player2Script.meleeSpeed = melees[player2MeleeIndex].speed;

        GameObject rangedHolder2 = body2.transform.GetChild(1).gameObject;
        GameObject ranged2 = Instantiate(ranges[player2RangedIndex].prefab, rangedHolder2.transform);
        ranged2.transform.localPosition = new Vector3(0, 0, 0);
        size = ranged2.transform.localScale;
        ranged2.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
        player2Script.rangedCooldown = ranges[player2RangedIndex].cooldown;
        player2Script.rangedUses = ranges[player2RangedIndex].uses;
        player2Script.rangedDamage = ranges[player2RangedIndex].attack;
        player2Script.rangedSpeed = ranges[player2RangedIndex].speed;

        GameObject specialHolder2 = body2.transform.GetChild(2).gameObject;
        GameObject special2 = Instantiate(specials[player2SpecialIndex].prefab, specialHolder2.transform);
        special2.transform.localPosition = new Vector3(0, 0, 0);
        size = special2.transform.localScale;
        special2.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
        player2Script.specialUses = specials[player2SpecialIndex].uses;

        GameObject wheelHolderFront2 = body2.transform.GetChild(3).gameObject;
        GameObject wheelFront2 = Instantiate(wheels[player2WheelIndex].prefab, wheelHolderFront2.transform);
        wheelFront2.transform.localPosition = new Vector3(0, 0, 0);
        size = wheelFront2.transform.localScale;
        wheelFront2.transform.localScale = new Vector3(size.x * 10, size.y * 10, 1);
        wheelFront2.layer = 9;
        player2Script.wheelSpeed = wheels[player2WheelIndex].speed;
        player2Script.wheelPower = wheels[player2WheelIndex].power;

        GameObject wheelHolderBack2 = body2.transform.GetChild(4).gameObject;
        GameObject wheelBack2 = Instantiate(wheels[player2WheelIndex].prefab, wheelHolderBack2.transform);
        wheelBack2.transform.localPosition = new Vector3(0, 0, 0);
        sizeBack = wheelBack2.transform.localScale;
        wheelBack2.transform.localScale = new Vector3(sizeBack.x * 10, sizeBack.y * 10, 1);
        wheelBack2.layer = 9;

        if (!Menu.isMultiplayer)
        {
            body2.AddComponent<RobotAi>();
        }

        return randomIndex;
    }
}
