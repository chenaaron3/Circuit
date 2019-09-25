using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutator : Operator
{
    // mutates the thread's value
    public override IEnumerator ExecuteOperator()
    {
        if (symbol.Equals("++"))
        {
            Thread.instance.Acc++;
        }
        else if (symbol.Equals("--"))
        {
            Thread.instance.Acc--;
        }
        else if (symbol.Equals("!"))
        {
            if (Thread.instance.Acc > 0)
            {
                Thread.instance.Acc = 0;
            }
            else if (Thread.instance.Acc == 0)
            {
                Thread.instance.Acc = 1;
            }
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
