using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;

    private int damage = 750;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 9)
        {
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        float distance = Vector2.Distance(transform.position, enemy.transform.position);
        if (distance < 1.5f)
        {
            int explosionDamage = (int)((distance * damage) / 1.5f);
            enemy.GetComponent<RobotFunctions>().TakeDamage(explosionDamage);
        }

        distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < 1.5f)
        {
            int explosionDamage = (int)((distance * damage) / 1.5f);
            player.GetComponent<RobotFunctions>().TakeDamage(explosionDamage);
        }

        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.layer = 11;

        gameObject.GetComponent<Rigidbody2D>().simulated = false;

        gameObject.GetComponentInChildren<ParticleSystem>().Play();

        float maxVolume = gameObject.transform.GetChild(0).GetComponent<AudioSource>().volume;
        gameObject.transform.GetChild(0).GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("Battle", 1)*maxVolume;
        gameObject.transform.GetChild(0).GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
