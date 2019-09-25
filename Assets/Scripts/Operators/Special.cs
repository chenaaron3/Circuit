using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Special : Operator
{
    public override IEnumerator ExecuteOperator()
    {
        // changes direction of the thread
        if (symbol.Equals("Arrow"))
        {
            // if arrow changes direction
            if (!(Vector2Int.RoundToInt(transform.right.normalized)).Equals(Vector2Int.RoundToInt(Thread.instance.rb.velocity.normalized)))
            {
                Thread.instance.SetVelocity(transform.right);
                gameObject.SetActive(false);
                AudioManager.instance.PlaySound("Bounce");
            }
        }
        // exits out of current function
        else if (symbol.Equals("Cross"))
        {
            foreach (Function function in transform.parent.GetComponentsInChildren<Function>())
            {
                if (function.symbol.Equals("Definition"))
                {
                    function.ExitFunction();
                    break;
                }
            }
        }
        yield return null;
    }

    public override void Turn()
    {
        if (symbol.Equals("Arrow"))
        {
            transform.Rotate(new Vector3(0, 0, 90));
        }
    }

    public override void Delete()
    {
        Destroy(gameObject);
    }
}
