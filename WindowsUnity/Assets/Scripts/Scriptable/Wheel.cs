using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWheel", menuName = "Robot Parts/Wheel")]
public class Wheel : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public int speed;
    public int power;
    public List<string> functions;
    public GameObject prefab;
}
