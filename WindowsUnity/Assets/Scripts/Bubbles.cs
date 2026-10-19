using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubbles : MonoBehaviour
{
    private GameObject robot1;
    private GameObject robot2;

    private Rigidbody2D rb1;
    private Rigidbody2D rb2;

    private float bubblesStrength = 7000f;
    private float bubblesStrength1;
    private float bubblesStrength2;

    private bool robot1In = false;
    private bool robot2In = false;

    private bool working = false;

    private bool onDelay = true;

    private void Start()
    {
        robot1 = PartsDisplay.player1Robot;
        robot2 = PartsDisplay.player2Robot;

        rb1 = robot1.GetComponent<Rigidbody2D>();
        rb2 = robot2.GetComponent<Rigidbody2D>();

        StartCoroutine(SettingStrength());

        StartCoroutine(StartDelay());
    }

    public IEnumerator SettingStrength()
    {
        yield return new WaitForSeconds(0.25f);

        bubblesStrength1 = (bubblesStrength * rb1.mass) / 5;
        bubblesStrength2 = (bubblesStrength * rb2.mass) / 5;
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1f);
        onDelay = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (onDelay)
        {
            if (collision.gameObject == robot1)
            {
                robot1In = true;
            }
            else if (collision.gameObject == robot2)
            {
                robot2In = true;
            }

            StartCoroutine(ActivateAfterDelay(collision));
            return;
        }

        if(collision.gameObject == robot1)
        {
            robot1In = true;

            if(!working)
            {
                working = true;
                StartCoroutine(BlowBubbles());
            }
        }
        else if (collision.gameObject == robot2)
        {
            robot2In = true;

            if (!working)
            {
                working = true;
                StartCoroutine(BlowBubbles());
            }
        }
    }

    public IEnumerator ActivateAfterDelay(Collider2D collision)
    {
        while (onDelay)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (collision.gameObject == robot1)
        {
            if(!robot1In)
            { 
                yield break;
            }

            if (!working)
            {
                working = true;
                StartCoroutine(BlowBubbles());
            }
        }
        else if (collision.gameObject == robot2)
        {
            if (!robot2In)
            {
                yield break;
            }

            if (!working)
            {
                working = true;
                StartCoroutine(BlowBubbles());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == robot1)
        {
            robot1In = false;
        }
        else if (collision.gameObject == robot2)
        {
            robot2In = false;
        }
    }

    public IEnumerator BlowBubbles()
    {
        while (robot1In || robot2In)
        {
            if (robot1In)
            {
                rb1.AddForce(Vector2.up * bubblesStrength1);
            }
            if (robot2In)
            {
                rb2.AddForce(Vector2.up * bubblesStrength2);
            }

            yield return new WaitForSeconds(0.5f);
        }

        working = false;
    }
}
