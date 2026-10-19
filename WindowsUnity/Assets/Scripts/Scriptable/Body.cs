using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewBody", menuName = "Robot Parts/Body")]
public class Body:ScriptableObject
{
    public string id;
    public Sprite sprite;
    public int health;
    public int armor;
    public List<string> functions;
    public GameObject prefab;
}
