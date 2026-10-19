using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlameThrower : MonoBehaviour
{
    public GameObject enemy;
    public int damage;

    private int enemyLayer;
    private RobotFunctions enemyRobot;
    private bool onCooldown = false;

    private void Start()
    {
        StartCoroutine(GetEnemy());
    }
    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == enemyLayer)
        {
            if(!onCooldown)
            {
                enemyRobot.TakeDamage(damage);
                StartCoroutine(Cooldown());
            }
        }
    }

    private void OnEnable()
    {
        if(onCooldown)
        {
            onCooldown = false;
        }
    }

    private IEnumerator Cooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(0.1f);
        onCooldown = false;
    }
    
    private IEnumerator GetEnemy()
    {
        if (enemy == null)
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(GetEnemy());
        }
        else
        {
            enemyLayer = enemy.layer;
            enemyRobot = enemy.GetComponent<RobotFunctions>();
        }
    }
}
