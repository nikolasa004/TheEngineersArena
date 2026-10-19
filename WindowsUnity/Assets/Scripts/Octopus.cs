using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : MonoBehaviour
{
    private GameObject robot1;
    private GameObject robot2;

    private Transform robot1Transform;
    private Transform robot2Transform;
    private float moveSpeed = 2f;
    private float rotationSpeed = 5f;

    private bool robot1In = false;
    private bool robot2In = false;

    private bool dealingDamage = false;

    private void Start()
    {
        robot1 = PartsDisplay.player1Robot;
        robot2 = PartsDisplay.player2Robot;

        robot1Transform = robot1.transform;
        robot2Transform = robot2.transform;

        int random = Random.Range(0, 2);
        if(random == 0)
        {
            transform.position = new Vector2(transform.position.x + 1, transform.position.y);
        }
        else
        {
            transform.position = new Vector3(transform.position.x - 1, transform.position.y);
        }

        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Maps", 1);
    }

    void Update()
    { 
        float distanceToRobot1 = Vector3.Distance(transform.position, robot1Transform.position);
        float distanceToRobot2 = Vector3.Distance(transform.position, robot2Transform.position);

        Transform targetRobot;
        float smallerDistance = Mathf.Min(distanceToRobot1, distanceToRobot2);

        if(smallerDistance<0.1f)
        {
            return;
        }

        if (distanceToRobot1 < distanceToRobot2)
        {
            targetRobot = robot1Transform;
        }
        else
        {
            targetRobot = robot2Transform;
        }

        Vector3 direction = (targetRobot.position - transform.position).normalized;
        transform.position += moveSpeed * Time.deltaTime * direction;

        RotateTowards(direction);
    }

    void RotateTowards(Vector3 direction)
    {
        float baseAngleZ = Mathf.Atan2(direction.y, Mathf.Abs(direction.x)) * Mathf.Rad2Deg;
        float adjustedAngleZ = direction.y >= 0 ? 90f - baseAngleZ : 180f + baseAngleZ;
        float angleY = direction.x >= 0 ? 180f : 0f;
        Quaternion targetRotation = Quaternion.Euler(0, angleY, adjustedAngleZ);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == robot1)
        {
            robot1In = true;

            if(!dealingDamage)
            {
                StartCoroutine(DealDamage());
            }
        }
        else if (collision.gameObject == robot2)
        {
            robot2In = true;

            if (!dealingDamage)
            {
                StartCoroutine(DealDamage());
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

    private IEnumerator DealDamage()
    {
        dealingDamage = true;

        while (robot1In || robot2In)
        {
            if (robot1In)
            {
                robot1.GetComponent<RobotFunctions>().TakeDamage(500);
            }
            if (robot2In)
            {
                robot2.GetComponent<RobotFunctions>().TakeDamage(500);
            }

            yield return new WaitForSeconds(1);
        }

        dealingDamage = false;
    }
}
