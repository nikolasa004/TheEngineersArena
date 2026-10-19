using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    private float horizontalLimit = 8f;
    private float verticalSpeed = 10f;

    private Vector3 startPosition;
    private bool reachedBottom = false;

    private bool gotBody1 = false;
    private bool gotBody2 = false;
    private bool body1Waiting = false;
    private bool body2Waiting = false;

    private GameObject body1;
    private GameObject body2;

    private bool alreadyChanged = false;

    private void Start()
    {
        body1=PartsDisplay.player1Robot;
        body2=PartsDisplay.player2Robot;

        startPosition = transform.position;
        StartCoroutine(HookRoutine());
    }

    private IEnumerator HookRoutine()
    {
        float waitTime = Random.Range(0,60);
        yield return new WaitForSeconds(waitTime);

        while (!reachedBottom)
        {
            transform.position += Vector3.down * verticalSpeed * Time.deltaTime;
            yield return null;
        }

        float moveTime = Random.Range(0,30);
        yield return StartCoroutine(MoveHorizontally(moveTime));

        bool destroyRobot1 = false;
        bool destroyRobot2 = false;
        if(gotBody1)
        {
            body1.GetComponent<Rigidbody2D>().simulated = false;
            body1.transform.parent = transform;
            destroyRobot1 = true;
        }
        else if(gotBody2)
        {
            body2.GetComponent<Rigidbody2D>().simulated = false;
            body2.transform.parent = transform;
            destroyRobot2 = true;
        }

        while (transform.position.y < startPosition.y)
        {
            transform.position += Vector3.up * verticalSpeed*2 * Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);

        if(destroyRobot1)
        {
            body1.transform.parent = null;
            body1.GetComponent<RobotFunctions>().Demolish();
        }
        else if(destroyRobot2)
        {
            body2.transform.parent = null;
            body2.GetComponent<RobotFunctions>().Demolish();
        }

        reachedBottom = false;
        StartCoroutine(HookRoutine());
    }

    private IEnumerator MoveHorizontally(float duration)
    {
        float endTime = Time.time + duration;

        int direction=Random.Range(0,2);
        if (direction == 0)
        {
            direction = -1;
        }

        while (Time.time < endTime)
        {
            transform.position += direction * Time.deltaTime * verticalSpeed * Vector3.right;

            if (Mathf.Abs(transform.position.x) > horizontalLimit && !alreadyChanged)
            {
                direction *= -1;

                alreadyChanged = true;

                StartCoroutine(ChangePause());
            }

            yield return null;
        }
    }

    private IEnumerator ChangePause()
    {
        yield return new WaitForSeconds(0.5f);
        alreadyChanged = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer==3)
        {
            reachedBottom = true;
        }

        if (collision.gameObject.layer == 6 && !gotBody1 && !body1Waiting)
        {
            if(gotBody2)
            {
                body1Waiting = true;
            }
            else
            {
                gotBody1 = true;
            }
        }
        if (collision.gameObject.layer == 8 && !gotBody2 && !body2Waiting)
        {
            if(gotBody1)
            {
                body2Waiting = true;
            }
            else
            {
                gotBody2 = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            gotBody1 = false;
            body1Waiting = false;

            if(body2Waiting)
            {
                gotBody2 = true;
                body2Waiting = false;
            }
        }
        if (collision.gameObject.layer == 8)
        {
            gotBody2 = false;
            body2Waiting = false;

            if(body1Waiting)
            {
                gotBody1 = true;
                body1Waiting = false;
            }
        }
    }
}
