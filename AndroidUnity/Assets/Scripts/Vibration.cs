using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour
{
    public static void Vibrate()
    {
        if (SystemInfo.supportsVibration)
        {
            Handheld.Vibrate();
        }
    }
}
