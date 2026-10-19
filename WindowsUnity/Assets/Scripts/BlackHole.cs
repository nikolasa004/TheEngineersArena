using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private GameObject robot1;
    private GameObject robot2;

    private Rigidbody2D rb1;
    private Rigidbody2D rb2;

    public float attractionThreshold = 0.1f;
    public float attractionForce = 10f;

    private void Start()
    {
        robot1 = PartsDisplay.player1Robot;
        robot2 = PartsDisplay.player2Robot;

        rb1 = robot1.GetComponent<Rigidbody2D>();
        rb2 = robot2.GetComponent<Rigidbody2D>();

        StartCoroutine(Attract());
    }

    private IEnumerator Attract()
    {
        while (true)
        {
            AttractRobot(rb1, robot1.transform.position);
            AttractRobot(rb2, robot2.transform.position);

            yield return null;
        }
    }

    private void AttractRobot(Rigidbody2D rb, Vector3 position)
    {
        if (Mathf.Abs(position.x) > attractionThreshold)
        {
            float direction = position.x > 0 ? -1f : 1f;
            Vector2 attraction = new(direction * attractionForce, 0);

            rb.AddForce(attraction);
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}
