using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalAndMeteor : MonoBehaviour
{
    public bool isCrystal;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer==6 || collision.gameObject.layer==8)
        {
            if(isCrystal)
            {
                collision.gameObject.GetComponent<RobotFunctions>().Heal(500);
            }
            else
            {
                collision.gameObject.GetComponent<RobotFunctions>().TakeDamage(500);
            }

            StartCoroutine(Demolish());
        }
    }

    private IEnumerator Demolish()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
