using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpecial", menuName = "Robot Parts/Special")]
public class Special : ScriptableObject
{
    public string id;
    public Sprite sprite;
    public int uses;
    public string effect;
    public List<string> functions;
    public GameObject prefab;
}
