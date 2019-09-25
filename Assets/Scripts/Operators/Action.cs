using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : Operator
{
    // delegates for action operators
    public delegate void OnActionDelegate();
    public static OnActionDelegate OnAction;

    // directly moves robot
    public override IEnumerator ExecuteOperator()
    {
        if (OnAction != null)
        {
            OnAction();
        }
        Actor.instance.armor.SetActive(false);
        if (symbol.Equals("M"))
        {
            Actor.instance.Move();
            AudioManager.instance.PlaySound("Move");
        }
        else if (symbol.Equals("A"))
        {
            Actor.instance.Attack();
            AudioManager.instance.PlaySound("Attack");
        }
        else if (symbol.Equals("T"))
        {
            Actor.instance.Turn();
            AudioManager.instance.PlaySound("Turn");
        }
        else if (symbol.Equals("D"))
        {
            Actor.instance.Defend();
            AudioManager.instance.PlaySound("Defend");
        }
        else if (symbol.Equals("LT"))
        {
            Actor.instance.LeftTurn();
            AudioManager.instance.PlaySound("Turn");
        }
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
