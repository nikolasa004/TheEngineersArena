using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRanged", menuName = "Robot Parts/Ranged")]
public class Ranged : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public int attack;
    public int uses;
    public int cooldown;
    public int speed;
    public List<string> functions;
    public GameObject prefab;
}
