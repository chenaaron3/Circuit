using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Function : Operator
{
    // for replacing definitions in invalid places
    [HideInInspector]
    public Vector2 lastPosition;
    // remembers caller position and velocity of call
    [HideInInspector]
    public Function caller;
    [HideInInspector]
    public Vector2 callerVelocity;
    public string functionName;

    private void Start()
    {
        functionName = transform.Find("Graphics").GetComponent<SpriteRenderer>().sprite.name;
    }

    // only destroy if not definition
    public override void Delete()
    {
        if (symbol != "Definition")
        {
            Destroy(gameObject);
        }
    }

    // function call
    public override IEnumerator ExecuteOperator()
    {
        if (symbol.Equals("Definition"))
        {
            // exiting function to function call
            ExitFunction();
        }
        else if (symbol.Equals("Call"))
        {
            // entering function
            Function[] functions = FindObjectsOfType<Function>();
            foreach (Function function in functions)
            {
                // find function definition
                if (function.symbol.Equals("Definition") && function.functionName.Equals(functionName))
                {
                    // if caller and definition not in the same function
                    if (!function.transform.parent.Equals(transform.parent))
                    {
                        function.caller = this;
                        function.callerVelocity = Thread.instance.rb.velocity.normalized;
                    }
                    else
                    {
                        FunctionManager.instance.ResetFunction(transform.parent.parent.gameObject);
                    }
                    // moves thread
                    Thread.instance.transform.position = function.transform.position;
                    Thread.instance.SetVelocity(function.transform.Find("Arrow").right);
                    StateManager.instance.circuitCamera.transform.position = function.transform.parent.position + new Vector3(0, 0, -10);
                    AudioManager.instance.PlaySound("Execute");
                    break;
                }
            }
        }
        yield return null;
    }

    public override void Turn()
    {
        if (symbol.Equals("Definition"))
        {
            transform.Find("Arrow").Rotate(new Vector3(0, 0, 90));
        }
    }

    // Exits function back to function call
    public void ExitFunction()
    {
        // if not S, go to function call
        if (!functionName.Equals("S"))
        {
            Thread.instance.transform.position = caller.transform.position;
            Thread.instance.SetVelocity(callerVelocity);
            StateManager.instance.circuitCamera.transform.position = caller.transform.parent.position + new Vector3(0, 0, -10);
            FunctionManager.instance.ResetFunction(transform.parent.parent.gameObject);
        }
        // else end the circuit
        else
        {
            StateManager.instance.TerminateCircuit(false);
        }
    }
}
