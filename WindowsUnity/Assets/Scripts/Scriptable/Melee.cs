using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMelee", menuName = "Robot Parts/Melee")]
public class Melee : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public int attack;
    public int uses;
    public int cooldown;
    public float speed;
    public List<string> functions;
    public GameObject prefab;
}
