using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Query : Operator
{
    // Raycast queries
    public override IEnumerator ExecuteOperator()
    {
        Vector2 pos = Actor.instance.transform.position;
        Vector2 dir = Actor.instance.forward;
        if (symbol.Equals("?C"))
        {
            Actor.instance.Scan(Color.cyan);
            RaycastHit2D worldFloorHit = Physics2D.Raycast(pos + dir, Vector2.zero, .1f, 1 << 10);
            RaycastHit2D enemyHit = Physics2D.Raycast(pos + dir, Vector2.zero, .1f, 1 << 9);
            if (worldFloorHit && !enemyHit)
            {
                Thread.instance.Acc = 1;
            }
            else
            {
                Thread.instance.Acc = 0;
            }
        }
        else if (symbol.Equals("?A"))
        {
            Actor.instance.Scan(Color.blue);
            RaycastHit2D enemyHit = Physics2D.Raycast(pos + dir, Vector2.zero, .1f, 1 << 9);
            if (enemyHit)
            {
                Enemy e = enemyHit.collider.GetComponent<Enemy>();
                if (e.attacker && e.AttackMove == 1)
                {
                    Thread.instance.Acc = 1;
                }
                else
                {
                    Thread.instance.Acc = 0;
                }
            }
            else
            {
                Thread.instance.Acc = 0;
            }
        }
        else if (symbol.Equals("?W"))
        {
            Actor.instance.Scan(Color.grey);
            RaycastHit2D worldFloorHit = Physics2D.Raycast(pos + dir, Vector2.zero, .1f, 1 << 10);
            if (worldFloorHit)
            {
                Thread.instance.Acc = 0;
            }
            else
            {
                Thread.instance.Acc = 1;
            }
        }
        else if (symbol.Equals("?E"))
        {
            Actor.instance.Scan(Color.red);
            RaycastHit2D enemyHit = Physics2D.Raycast(pos + dir, Vector2.zero, .1f, 1 << 9);
            if (enemyHit)
            {
                Thread.instance.Acc = 1;
            }
            else
            {
                Thread.instance.Acc = 0;
            }
        }
        else if (symbol.Equals("?L"))
        {
            Actor.instance.Scan(Color.cyan);
            RaycastHit2D worldFloorHit = Physics2D.Raycast(pos + Actor.instance.GetLeftDirection(), Vector2.zero, .1f, 1 << 10);
            RaycastHit2D enemyHit = Physics2D.Raycast(pos + Actor.instance.GetLeftDirection(), Vector2.zero, .1f, 1 << 9);
            if (worldFloorHit && !enemyHit)
            {
                Thread.instance.Acc = 1;
            }
            else
            {
                Thread.instance.Acc = 0;
            }
        }
        else if (symbol.Equals("?R"))
        {
            Actor.instance.Scan(Color.cyan);
            RaycastHit2D worldFloorHit = Physics2D.Raycast(pos + Actor.instance.GetRightDirection(), Vector2.zero, .1f, 1 << 10);
            RaycastHit2D enemyHit = Physics2D.Raycast(pos + Actor.instance.GetRightDirection(), Vector2.zero, .1f, 1 << 9);
            if (worldFloorHit && !enemyHit)
            {
                Thread.instance.Acc = 1;
            }
            else
            {
                Thread.instance.Acc = 0;
            }
        }
        AudioManager.instance.PlaySound("Scan");
        yield return null;
    }

    public override void Turn()
    {
    }

    public override void Delete()
    {
        Destroy(gameObject);
    }
}
