using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Operator : MonoBehaviour
{
    public string symbol;
    public SpriteRenderer sr;
    public Color initColor;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        initColor = sr.color;
    }

    // execute operator appropriately
    public void TriggerOperator()
    {
        if (CompareTag("ActivateOn0"))
        {
            Destroy(Instantiate(PrefabManager.instance.GetPrefab("Particle"), transform.position, Quaternion.identity), .25f);
            StartCoroutine(ExecuteOperator());
        }
        else
        {
            if (Thread.instance.Acc > 0)
            {
                Destroy(Instantiate(PrefabManager.instance.GetPrefab("Particle"), transform.position, Quaternion.identity), .25f);
                StartCoroutine(ExecuteOperator());
            }
        }
    }

    public abstract IEnumerator ExecuteOperator();
    public abstract void Turn();
    public abstract void Delete();
}
