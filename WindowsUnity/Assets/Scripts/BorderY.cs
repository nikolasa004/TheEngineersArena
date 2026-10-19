using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderY : MonoBehaviour
{
    public Battle battleScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer==6 || collision.gameObject.layer==8)
        {
            battleScript.ResetPositions();
        }
    }
}
