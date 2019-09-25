using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public static Actor instance;
    Animator anim;
    Animator attack;
    Animator scanner;
    int facingDirection = 1;
    Vector2Int[] directionTable = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    public Vector2 forward;
    public GameObject armor;
    BoxCollider2D col;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        attack = transform.Find("Attack").GetComponentInChildren<Animator>();
        scanner = transform.Find("Scanner").GetComponentInChildren<Animator>();
        col = GetComponent<BoxCollider2D>();
    }

    public void Move()
    {
        RaycastHit2D floorHit = Physics2D.Raycast(transform.position + (Vector3)forward, Vector3.zero, .1f, 1 << 10);
        if (floorHit)
        {
            transform.position += (Vector3)forward;
        }
    }

    public void Attack()
    {
        attack.speed = Thread.instance.speed;
        attack.SetTrigger("Attack");
        RaycastHit2D enemyHit = Physics2D.Raycast(transform.position + (Vector3)forward, Vector3.zero, .1f, 1 << 9);
        if (enemyHit)
        {
            enemyHit.collider.GetComponent<Enemy>().HP -= 1;
        }
    }

    public void Turn()
    {
        facingDirection = (facingDirection + 1) % 4;
        anim.SetFloat("Direction", facingDirection);
        forward = directionTable[facingDirection];
        attack.transform.parent.position = transform.position + (Vector3)forward;
        scanner.transform.parent.right = forward;
    }

    public void Defend()
    {
        armor.SetActive(true);
    }

    public void Scan(Color c)
    {
        scanner.speed = Thread.instance.speed;
        scanner.GetComponent<SpriteRenderer>().color = c;
        scanner.SetTrigger("Scan");
    }

    public void LeftTurn()
    {
        facingDirection = (facingDirection - 1) % 4;
        if (facingDirection < 0)
        {
            facingDirection = 4 + facingDirection;
        }
        anim.SetFloat("Direction", facingDirection);
        forward = directionTable[facingDirection];
        attack.transform.parent.position = transform.position + (Vector3)forward;
        scanner.transform.parent.right = forward;
    }

    public void SetDirection(Vector2Int direction)
    {
        facingDirection = System.Array.IndexOf(directionTable, direction);
        anim.SetFloat("Direction", facingDirection);
        forward = direction;
        attack.transform.parent.position = transform.position + (Vector3)forward;
        scanner.transform.parent.right = forward;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
        {
            StartCoroutine(FindObjectOfType<WorldLevel>().CheckTestCase(collision.gameObject));
        }
        else if (collision.CompareTag("Enemy"))
        {
            // killed
            if (!armor.activeSelf)
            {
                StateManager.instance.TerminateCircuit(false);
            }
            // deflected
            else
            {
                AudioManager.instance.PlaySound("Parry");
            }
        }
    }

    public Vector2Int GetLeftDirection()
    {
        int res = (facingDirection - 1) % 4;
        if (res < 0)
        {
            res = 4 + res;
        }
        return directionTable[res];
    }

    public Vector2Int GetRightDirection()
    {
        return directionTable[(facingDirection + 1) % 4];
    }

    public IEnumerator Celebrate(GameObject donut)
    {
        col.enabled = false;
        Thread.instance.Pause();
        // carries donut
        Vector3 donutPosition = donut.transform.position;
        GameObject donutParent = donut.transform.parent.gameObject;
        donut.transform.parent = transform;
        donut.transform.localPosition = new Vector2(0, .5f);
        Vector3 startPos = transform.position;
        Vector3 height = new Vector3(0, .5f, 0);

        // jump up
        yield return MoveTo(startPos + height);
        // jump down
        yield return MoveTo(startPos);

        // puts down donut
        donut.transform.parent = donutParent.transform;
        donut.transform.position = donutPosition;
        col.enabled = true;
        Thread.instance.Run();
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while ((transform.position - target).magnitude > .05f)
        {
            transform.position = Vector2.Lerp(transform.position, target, 5 * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = target;
    }
}
