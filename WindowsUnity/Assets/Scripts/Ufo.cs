using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ufo : MonoBehaviour
{
    private GameObject robot1;
    private GameObject robot2;

    public Animator laserAnimator;

    private float speed = 5f;
    private float horizontalLimit = 15f;

    private bool gotRobot=false;
    private bool otherRobotWaiting=false;
    private int robotNumber=0;
    private int holdingRobotNumber=0;

    private bool alreadyChanged = false;

    private void Start()
    {
        robot1 = PartsDisplay.player1Robot;
        robot2 = PartsDisplay.player2Robot;

        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);

        StartCoroutine(UfoMove());
    }

    private IEnumerator UfoMove()
    {
        while (true)
        {
            int random = Random.Range(0, 60);

            float endTime = Time.time + random;

            int direction = Random.Range(0, 2);
            if (direction == 0)
            {
                direction = -1;
            }

            while (Time.time < endTime)
            {
                transform.position += direction * Time.deltaTime * speed * Vector3.right;

                if (Mathf.Abs(transform.position.x) > horizontalLimit && !alreadyChanged)
                {
                    direction *= -1;
                    alreadyChanged = true;

                    StartCoroutine(ChangePause());
                }

                yield return null;
            }

            gameObject.GetComponent<AudioSource>().Play();

            laserAnimator.SetTrigger("Laser");
            yield return new WaitForSeconds(0.5f);

            while(!gotRobot)
            {
                yield return null;
            }

            Transform transformOfRobot=null;
            if (robotNumber == 1)
            {
                robot1.GetComponent<Rigidbody2D>().simulated = false;

                if(robot1.name=="Body2(Clone)")
                {
                    robot1.GetComponent<CircleCollider2D>().enabled = false;
                }
                else if(robot1.name=="Body5(Clone)")
                {
                    robot1.GetComponent<PolygonCollider2D>().enabled = false;
                }
                else
                {
                    robot1.GetComponent<BoxCollider2D>().enabled = false;
                }
                robot1.transform.position = new Vector3(transform.position.x, robot1.transform.position.y, robot1.transform.position.z);
                robot1.transform.parent = transform;
                transformOfRobot = robot1.transform;
                holdingRobotNumber = 1;
            }
            else
            {
                robot2.GetComponent<Rigidbody2D>().simulated = false;
                
                if (robot2.name == "Body2(Clone)")
                {
                    robot2.GetComponent<CircleCollider2D>().enabled = false;
                }
                else if (robot2.name == "Body5(Clone)")
                {
                    robot2.GetComponent<PolygonCollider2D>().enabled = false;
                }
                else
                {
                    robot2.GetComponent<BoxCollider2D>().enabled = false;
                }
                robot2.transform.position = new Vector3(transform.position.x, robot2.transform.position.y, robot2.transform.position.z);
                robot2.transform.parent = transform;
                transformOfRobot = robot2.transform;
                holdingRobotNumber = 2;
            }

            while(transformOfRobot.localPosition.y<-0.35f)
            {
                transformOfRobot.position += speed * Time.deltaTime * Vector3.up;
                yield return null;
            }

            random = Random.Range(0, 30);

            endTime = Time.time + random;

            direction = Random.Range(0, 2);
            if (direction == 0)
            {
                direction = -1;
            }

            while (Time.time < endTime)
            {
                transform.position += direction * Time.deltaTime * speed * Vector3.right;

                if (Mathf.Abs(transform.position.x) > horizontalLimit && !alreadyChanged)
                {
                    direction *= -1;
                    alreadyChanged = true;

                    StartCoroutine(ChangePause());
                }

                yield return null;
            }

            laserAnimator.SetTrigger("Laser");
            yield return new WaitForSeconds(0.5f);

            gameObject.GetComponent<AudioSource>().Stop();

            if (holdingRobotNumber == 1)
            {
                robot1.GetComponent<Rigidbody2D>().simulated = true;

                if (robot1.name == "Body2(Clone)")
                {
                    robot1.GetComponent<CircleCollider2D>().enabled = true;
                }
                else if (robot1.name == "Body5(Clone)")
                {
                    robot1.GetComponent<PolygonCollider2D>().enabled = true;
                }
                else
                {
                    robot1.GetComponent<BoxCollider2D>().enabled = true;
                }
                robot1.transform.parent = null;
            }
            else
            {
                robot2.GetComponent<Rigidbody2D>().simulated = true;

                if (robot2.name == "Body2(Clone)")
                {
                    robot2.GetComponent<CircleCollider2D>().enabled = true;
                }
                else if (robot2.name == "Body5(Clone)")
                {
                    robot2.GetComponent<PolygonCollider2D>().enabled = true;
                }
                else
                {
                    robot2.GetComponent<BoxCollider2D>().enabled = true;
                }
                robot2.transform.parent = null;
            }

            holdingRobotNumber = 0;
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator ChangePause()
    {
        yield return new WaitForSeconds(1f);
        alreadyChanged = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject==robot1 || collision.gameObject == robot2)
        {
            if (!gotRobot)
            {
                gotRobot = true;
                
                if (collision.gameObject == robot1)
                {
                    robotNumber = 1;
                }
                else
                {
                    robotNumber = 2;
                }
            }
            else
            {
                otherRobotWaiting = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == robot1 || collision.gameObject == robot2)
        {
            if (gotRobot)
            {
                int numberOfRobotOut = 0;
                if (collision.gameObject == robot1)
                {
                    numberOfRobotOut = 1;
                }
                else
                {
                    numberOfRobotOut = 2;
                }

                if (robotNumber != numberOfRobotOut)
                {
                    otherRobotWaiting = false;
                }
                else
                {
                    if (otherRobotWaiting)
                    {
                        otherRobotWaiting = false;

                        if (robotNumber == 1)
                        {
                            robotNumber = 2;
                        }
                        else
                        {
                            robotNumber = 1;
                        }
                    }
                    else
                    {
                        gotRobot = false;
                        robotNumber = 0;
                    }
                }
            }
        }
    }
}
