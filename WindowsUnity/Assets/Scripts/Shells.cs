using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shells : MonoBehaviour
{
    public Sprite sprite0;
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    public Sprite sprite4;
    public Sprite sprite5;

    private GameObject robot1;
    private GameObject robot2;

    private SpriteRenderer spriteRenderer;

    private bool opened = false;

    private void Start()
    {
        robot1 = PartsDisplay.player1Robot;
        robot2 = PartsDisplay.player2Robot;

        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(ChangeSprite());
    }

    private IEnumerator ChangeSprite()
    {
        spriteRenderer.sprite = sprite0;
        opened = false;

        int random = Random.Range(0, 30);
        yield return new WaitForSeconds(random);
        spriteRenderer.sprite = sprite1;

        random = Random.Range(0, 30);
        yield return new WaitForSeconds(random);
        spriteRenderer.sprite = sprite2;

        random = Random.Range(0, 30);
        yield return new WaitForSeconds(random);
        spriteRenderer.sprite = sprite3;

        random = Random.Range(0, 30);
        yield return new WaitForSeconds(random);
        spriteRenderer.sprite = sprite4;

        random = Random.Range(0, 30);
        yield return new WaitForSeconds(random);
        spriteRenderer.sprite = sprite5;

        opened = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == robot1 && opened)
        {
            robot1.GetComponent<RobotFunctions>().Heal(250);
            StartCoroutine(ChangeSprite());
        }
        else if (collision.gameObject == robot2 && opened)
        {
            robot2.GetComponent<RobotFunctions>().Heal(250);
            StartCoroutine(ChangeSprite());
        }
    }
}
