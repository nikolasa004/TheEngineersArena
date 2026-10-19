using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jellyfish : MonoBehaviour
{
    private GameObject electricity;

    private bool someoneOn=false;

    private RobotFunctions robotFunctions1;
    private RobotFunctions robotFunctions2;

    private bool robot1On=false;
    private bool robot2On=false;

    private int secondsOn=0;
    private int maxSecondsOn=10;
    private void Start()
    {
        electricity = transform.GetChild(0).gameObject;

        robotFunctions1 = PartsDisplay.player1Robot.GetComponent<RobotFunctions>();
        robotFunctions2 = PartsDisplay.player2Robot.GetComponent<RobotFunctions>();

        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Battle", 1);
        
        StartCoroutine(CheckElectricity());
    }

    public IEnumerator CheckElectricity()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            if(someoneOn)
            {
                secondsOn++;

                if(secondsOn>=maxSecondsOn)
                {
                    StartCoroutine(ActivateElectricity());
                    yield return new WaitForSeconds(1);
                    secondsOn=0;
                }
            }
            else
            {
                if(secondsOn>0)
                {
                    secondsOn--;
                }
            }
        }
    }

    public IEnumerator ActivateElectricity()
    {
        electricity.SetActive(true);

        gameObject.GetComponent<AudioSource>().Play();

        if(someoneOn)
        {
            if(robot1On)
            {
                robotFunctions1.TakeDamage(500);
            }
            if(robot2On)
            {
                robotFunctions2.TakeDamage(500);
            }
        }
        yield return new WaitForSeconds(1);
        electricity.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer==6 || collision.gameObject.layer==8)
        {
            someoneOn = true;

            if (collision.gameObject.layer == 6)
            {
                robot1On = true;
            }
            else if (collision.gameObject.layer == 8)
            {
                robot2On = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 9)
        {
            someoneOn = false;

            if (collision.gameObject.layer == 6)
            {
                robot1On = false;
            }
            else if (collision.gameObject.layer == 8)
            {
                robot2On = false;
            }
        }
    }
}
