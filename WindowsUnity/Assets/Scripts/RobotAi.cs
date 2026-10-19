using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class RobotAi : MonoBehaviour
{
    private RobotFunctions robotFunctions;
    private GameObject player;
    private RobotFunctions playerRobotFunctions;

    private float sameLevelInterval = 2f;

    private float meleeRange = 3.5f;

    private float cannonRange = 6;
    private float cannonInterval = 2;
    private float crossbowRange = 15;
    private float crossbowInterval = 3;
    private float grenadeRange = 3;
    private float grenadeInterval = 1;
    private float gunRange = 30;
    private float gunInterval = 5;
    private float flameThrowerRange = 5;
    private float flameThrowerInterval = 2;

    private float rangedMinDistance;
    private float rangedMaxDistance;

    private bool doSpecial = false;
    private bool moving=false;
    private bool movingForward=false;

    public int borderX;

    private void Start()
    {
        robotFunctions=GetComponent<RobotFunctions>();

        player=PartsDisplay.player1Robot;
        playerRobotFunctions=player.GetComponent<RobotFunctions>();

        if(robotFunctions.playerRanged.name=="Cannon(Clone)")
        {
            rangedMinDistance=cannonRange-cannonInterval;
            rangedMaxDistance=cannonRange+cannonInterval;
        }
        else if(robotFunctions.playerRanged.name=="Crossbow(Clone)")
        {
            rangedMinDistance=0;
            rangedMaxDistance=crossbowRange+crossbowInterval;
        }
        else if(robotFunctions.playerRanged.name=="GrenadeThrower(Clone)")
        {
            rangedMinDistance = 0;
            rangedMaxDistance=grenadeRange+grenadeInterval;
        }
        else if(robotFunctions.playerRanged.name=="Gun(Clone)")
        {
            rangedMinDistance=0;
            rangedMaxDistance=gunRange+gunInterval;
        }
        else if(robotFunctions.playerRanged.name=="FlameThrower(Clone)")
        {
            rangedMinDistance=0;
            rangedMaxDistance=flameThrowerRange+flameThrowerInterval;
        }

        StartCoroutine(Special());
        StartCoroutine(Wander());
    }

    private void Update()
    {
        CheckMelee();
        CheckRange();
        CheckSpecial();
        CheckMoveForward();
        CheckMoveBackwards();
    }

    private bool CheckSameLevel()
    {
        if(Mathf.Abs(player.transform.position.y-transform.position.y)<sameLevelInterval)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CheckMelee()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < meleeRange)
        {
            if (CheckSameLevel())
            {
                if (!robotFunctions.meleeIsCooling)
                {
                    robotFunctions.MeleeAttack();
                }
            }
        }
    }

    private void CheckRange()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < rangedMaxDistance && distance > rangedMinDistance && (CheckSameLevel() || ((robotFunctions.playerRanged.name == "Cannon(Clone)" || robotFunctions.playerRanged.name == "GrenadeThrower(Clone)") && transform.position.y>player.transform.position.y)))
        {
            if (!robotFunctions.rangedIsCooling)
            {
                robotFunctions.RangedAttack();
            }
        }
    }

    private void CheckSpecial()
    {
        if (doSpecial)
        {
            robotFunctions.Special();
            doSpecial = false;
        }
    }

    private IEnumerator Special()
    {
        while(true)
        {
            if (!robotFunctions.specialHasUsed)
            {
                if (doSpecial)
                {
                    yield return new WaitForSeconds(1);
                }

                int random = Random.Range(0, 120 / robotFunctions.specialUses);

                yield return new WaitForSeconds(random);
                doSpecial = true;
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }

    private IEnumerator Wander()
    {
        while(true)
        {
            if(robotFunctions.meleeIsCooling)
            {
                moving=true;
                
                if(transform.position.x>player.transform.position.x)
                {
                    movingForward=true;
                }
                else
                {
                    movingForward=false;
                }
               
                yield return new WaitForSeconds(1f);

                continue;
            }

            if(playerRobotFunctions.meleeIsCooling)
            {
                moving=true;

                if(transform.position.x>player.transform.position.x)
                {
                    movingForward=false;
                }
                else
                {
                    movingForward=true;
                }

                yield return new WaitForSeconds(1f);

                continue;
            }


            int random = Random.Range(0, 6);

            if(transform.position.x+2>borderX)
            {
                moving=true;
                movingForward=false;

                yield return new WaitForSeconds(random);
                continue;
            }

            if(transform.position.x-2<-borderX)
            {
                moving=true;
                movingForward=true;

                yield return new WaitForSeconds(random);
                continue;
            }


            int random1 = Random.Range(0, 3);
            if(random1==0)
            {
                moving=true;
                movingForward=true;
            }
            else if(random1==1)
            {
                moving=true;
                movingForward=false;
            }
            else
            {
                moving=false;
            }
            yield return new WaitForSeconds(random);
        }
    }

    private void CheckMoveForward()
    {
        if(moving && movingForward && transform.position.x<borderX)
        {
            robotFunctions.MoveForward();
        }
    }

    private void CheckMoveBackwards()
    {
        if(moving && !movingForward && transform.position.x>-borderX)
        {
            robotFunctions.MoveBackward();
        }
    }
}
