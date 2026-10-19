using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public int portalID;

    public Transform singleBox;
    public Transform leftBox;
    public Transform rightBox;
    public Transform singleTri;
    public Transform leftTri;
    public Transform rightTri;

    private GameObject robot1;
    private GameObject robot2;

    private Transform robot1Transform;
    private Transform robot2Transform;

    private static bool singleBoxOnCooldown=false;
    private static bool leftBoxOnCooldown=false;
    private static bool rightBoxOnCooldown=false;
    private static bool singleTriOnCooldown=false;
    private static bool leftTriOnCooldown=false;
    private static bool rightTriOnCooldown=false;

    private void Start()
    {
        robot1=PartsDisplay.player1Robot;
        robot2=PartsDisplay.player2Robot;

        robot1Transform = robot1.transform;
        robot2Transform = robot2.transform;

        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject==robot1 || collision.gameObject==robot2)
        {
            if(portalID==1)
            {
                if(singleBoxOnCooldown)
                {
                    return;
                }

                gameObject.GetComponent<AudioSource>().Play();

                if(collision.gameObject==robot1)
                {
                    robot1Transform.position = leftBox.position;
                    leftBoxOnCooldown = true;
                    StartCoroutine(Cooldown(2));
                }
                else
                {
                    robot2Transform.position = rightBox.position;
                    rightBoxOnCooldown=true;
                    StartCoroutine(Cooldown(3));
                }
            }
            if(portalID==2 || portalID==3)
            {
                if(leftBoxOnCooldown && portalID==2)
                {
                    return;
                }
                if(rightBoxOnCooldown && portalID==3)
                {
                    return;
                }

                gameObject.GetComponent<AudioSource>().Play();

                if (collision.gameObject == robot1)
                {
                    robot1Transform.position = singleBox.position;
                    singleBoxOnCooldown=true;
                }
                else
                {
                    robot2Transform.position = singleBox.position;
                    singleBoxOnCooldown=true;
                }
                StartCoroutine(Cooldown(1));
            }
            if(portalID==4)
            {
                if(singleTriOnCooldown)
                {
                    return;
                }

                gameObject.GetComponent<AudioSource>().Play();

                if(collision.gameObject==robot1)
                {
                    robot1Transform.position = leftTri.position;
                    leftTriOnCooldown=true;
                    StartCoroutine(Cooldown(5));
                }
                else
                {
                    robot2Transform.position = rightTri.position;
                    rightTriOnCooldown=true;
                    StartCoroutine(Cooldown(6));
                }
            }
            if(portalID==5 || portalID==6)
            {
                if(leftTriOnCooldown && portalID==5)
                {
                    return;
                }
                if(rightTriOnCooldown && portalID==6)
                {
                    return;
                }

                gameObject.GetComponent<AudioSource>().Play();

                if (collision.gameObject == robot1)
                {
                    robot1Transform.position = singleTri.position;
                    singleTriOnCooldown=true;
                }
                else
                {
                    robot2Transform.position = singleTri.position;
                    singleTriOnCooldown=true;
                }
                StartCoroutine(Cooldown(4));
            }
        }
    }

    public IEnumerator Cooldown(int id)
    {
        yield return new WaitForSeconds(3f);
        
        switch(id)
        {
            case 1:
                singleBoxOnCooldown=false;
                break;
            case 2:
                leftBoxOnCooldown=false;
                break;
            case 3:
                rightBoxOnCooldown=false;
                break;
            case 4:
                singleTriOnCooldown=false;
                break;
            case 5:
                leftTriOnCooldown=false;
                break;
            case 6:
                rightTriOnCooldown=false;
                break;
        }
    }
}
