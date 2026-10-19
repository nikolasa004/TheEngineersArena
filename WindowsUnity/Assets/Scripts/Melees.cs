using System.Collections;
using UnityEngine;

public class Melees : MonoBehaviour
{
    public GameObject enemy;
    public int damage;
    public float speed;

    private int enemyLayer;
    private bool colliding = false;
    private string type;
    private Animator chainsawAnimator;
    private bool alreadyActive = false;
    private RobotFunctions enemyScript;

    private AudioSource meleeSound;

    private void Start()
    {
        if (gameObject.name == "Saw(Clone)")
        {
            type = "Saw";
        }
        else  if (gameObject.name == "Chainsaw(Clone)")
        {
            type = "Chainsaw";
            chainsawAnimator = GetComponent<Animator>();
        }
        else if (gameObject.name == "Sword(Clone)")
        {
            type = "Sword";
        }
        else if(gameObject.name=="Axe(Clone)")
        {
            type = "Axe";
        }
        else if(gameObject.name=="Hammer(Clone)")
        {
            type = "Hammer";
        }
        else
        {
            return;
        }

        meleeSound = GetComponent<AudioSource>();
        float maxVolume = meleeSound.volume;
        meleeSound.volume = PlayerPrefs.GetFloat("Battle", 1)*maxVolume;

        StartCoroutine(GetEnemy());
        StartCoroutine(TryToPlaySound());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == enemyLayer)
        {
            colliding = true;

            if(!alreadyActive)
            {
                StartCoroutine(TryToDealDamage());
                alreadyActive = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == enemyLayer)
        {
            colliding = false;
        }
    }

    private void OnEnable()
    {
        if(alreadyActive)
        {
            alreadyActive = false;
        }

        StartCoroutine(TryToPlaySound());
    }

    private IEnumerator TryToDealDamage()
    {
        while (colliding)
        {
            if (type == "Saw")
            {
                float firstRotation = transform.localRotation.z;
                yield return new WaitForSeconds(0.1f);
                float secondRotation = transform.localRotation.z;

                if (firstRotation != secondRotation && colliding)
                {
                    enemyScript.TakeDamage(damage);
                    yield return new WaitForSeconds(speed-0.1f);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (type == "Chainsaw")
            {
                if (chainsawAnimator.GetCurrentAnimatorStateInfo(0).IsName("Chainsaw"))
                {
                    enemyScript.TakeDamage(damage);
                    yield return new WaitForSeconds(speed);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (type == "Sword")
            {
                float firstPosition = transform.localPosition.x;
                yield return new WaitForSeconds(0.1f);
                float secondPosition = transform.localPosition.x;

                if(secondPosition!=firstPosition && colliding)
                {
                    enemyScript.TakeDamage(damage);
                    yield return new WaitForSeconds(speed-0.1f);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (type == "Axe")
            {
                float firstRotation = transform.localRotation.z;
                yield return new WaitForSeconds(0.1f);
                float secondRotation = transform.localRotation.z;

                if (firstRotation != secondRotation && colliding)
                {
                    enemyScript.TakeDamage(damage);
                    yield return new WaitForSeconds(speed - 0.1f);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (type == "Hammer")
            {
                float firstRotation = transform.rotation.z;
                yield return new WaitForSeconds(0.1f);
                float secondRotation = transform.rotation.z;

                if (secondRotation != firstRotation && colliding)
                {
                    enemyScript.TakeDamage(damage);
                    yield return new WaitForSeconds(speed - 0.1f);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                yield break;
            }
        }
        alreadyActive = false;
    }

    private IEnumerator TryToPlaySound()
    {
        while (true)
        {
            if (type == "Saw")
            {
                float firstRotation = transform.localRotation.z;
                yield return new WaitForSeconds(0.1f);
                float secondRotation = transform.localRotation.z;

                if (firstRotation != secondRotation)
                {
                    if(!meleeSound.isPlaying)
                        meleeSound.Play();
                    yield return new WaitForSeconds(speed - 0.1f);
                }
                else
                {
                    if(meleeSound.isPlaying)
                    {
                        meleeSound.Stop();
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (type == "Chainsaw")
            {
                if (chainsawAnimator.GetCurrentAnimatorStateInfo(0).IsName("Chainsaw"))
                {
                    if (!meleeSound.isPlaying)
                        meleeSound.Play();
                    yield return new WaitForSeconds(speed);
                }
                else
                {
                    if(meleeSound.isPlaying)
                    {
                        meleeSound.Stop();
                    }
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (type == "Sword")
            {
                float firstPosition = transform.localPosition.x;
                yield return new WaitForSeconds(0.1f);
                float secondPosition = transform.localPosition.x;

                if (secondPosition != firstPosition)
                {
                    meleeSound.Play();
                    yield return new WaitForSeconds(speed - 0.1f);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (type == "Axe")
            {
                float firstRotation = transform.localRotation.z;
                yield return new WaitForSeconds(0.1f);
                float secondRotation = transform.localRotation.z;

                if (firstRotation != secondRotation)
                {
                    meleeSound.Play();
                    yield return new WaitForSeconds(speed - 0.1f);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else if (type == "Hammer")
            {
                float firstRotation = transform.rotation.z;
                yield return new WaitForSeconds(0.1f);
                float secondRotation = transform.rotation.z;

                if (secondRotation != firstRotation)
                {
                    meleeSound.Play();
                    yield return new WaitForSeconds(speed - 0.1f);
                }
                else
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            else
            {
                yield break;
            }
        }
    }

    public IEnumerator GetEnemy()
    {
        if (enemy != null)
        {
            enemyLayer = enemy.layer;
            enemyScript = enemy.GetComponent<RobotFunctions>();
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(GetEnemy());
        }
    }
}
