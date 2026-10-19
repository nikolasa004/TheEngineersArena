using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public int damage;
    public GameObject enemy;

    private bool isGrenade=false;
    private int layerOfEnemy;
    private int layerOfEnemyShield;
    private bool deltDamage = false;
    private RobotFunctions enemyScript;
    private void Start()
    {
        layerOfEnemy = enemy.layer;
        if(layerOfEnemy==6)
        {
            layerOfEnemyShield = 13;
        }
        else
        {
            layerOfEnemyShield = 14;
        }

        enemyScript = enemy.GetComponent<RobotFunctions>();

        if(gameObject.name=="Grenade(Clone)")
        {
            isGrenade = true;
            StartCoroutine(Explode());
        }

        float maxVolume=gameObject.GetComponent<AudioSource>().volume;
        gameObject.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Battle", 1)*maxVolume;
        gameObject.GetComponent<AudioSource>().Play();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer==3 || collision.gameObject.layer==layerOfEnemyShield || collision.gameObject.layer==15)
        {
            if(collision.gameObject.CompareTag("Destroyable") && gameObject.layer!=10)
            {
                collision.gameObject.GetComponent<BlockDestroy>().StartCoroutine("DestroyBlock");
            }

            gameObject.layer = 10;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        else if(collision.gameObject.layer == layerOfEnemy && !isGrenade)
        {
            if (!deltDamage)
            {
                enemyScript.TakeDamage(damage);
                deltDamage = true;

                gameObject.layer = 10;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(3);

        float distance = Vector2.Distance(transform.position, enemy.transform.position);

        if (distance <= 1.5f)
        {
            int explosionDamage=(int)((distance*damage)/1.5f);
            enemyScript.TakeDamage(explosionDamage);
        }
        
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponentInChildren<TrailRenderer>().enabled = false;
        gameObject.layer = 11;

        //disable the rigidbody so that the grenade doesn't move
        gameObject.GetComponent<Rigidbody2D>().simulated = false;

        gameObject.GetComponentInChildren<ParticleSystem>().Play();

        float maxVolume = gameObject.transform.GetChild(0).GetComponent<AudioSource>().volume;
        gameObject.transform.GetChild(0).GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Battle", 1)*maxVolume;
        gameObject.transform.GetChild(0).GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
