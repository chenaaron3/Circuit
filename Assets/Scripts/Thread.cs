using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Thread : MonoBehaviour
{
    public float speed = 1;
    float maxSpeed = 10;
    float minSpeed = 1;
    public static Thread instance;
    int acc;
    public int Acc
    {
        get
        {
            return acc;
        }
        set
        {
            // going to 0
            if (acc > 0 && value == 0)
            {
                foreach (Operator o in FindObjectsOfType<Operator>())
                {
                    if (!o.CompareTag("ActivateOn0"))
                    {
                        o.sr.color = Color.gray;
                    }
                }
            }
            // going out from 0
            else if (acc == 0 && value > 0)
            {
                foreach (Operator o in FindObjectsOfType<Operator>())
                {
                    if (!o.CompareTag("ActivateOn0"))
                    {
                        o.sr.color = o.initColor;
                    }
                }
            }
            acc = value;
            if (acc < 0)
            {
                acc = 0;
            }
            accText.text = "" + acc;
        }
    }
    public Text accText;
    public Rigidbody2D rb;

    public delegate void Tick();
    public static Tick OnTick;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        Acc = 1;
    }

    private void OnEnable()
    {
        // repositions 
        GameObject starter = GameObject.FindGameObjectWithTag("Respawn");
        transform.position = starter.transform.position;
        SetVelocity(starter.transform.Find("Arrow").right);

        // resets and runs
        Acc = 1;
        Run();
    }

    IEnumerator TriggerOperator()
    {
        Debug.Log("running thread");
        yield return new WaitForSeconds(1 / speed);
        RaycastHit2D circuitBoardHit = Physics2D.Raycast(transform.position, Vector2.zero, .1f, 1 << 11);
        if (!circuitBoardHit)
        {
            StateManager.instance.TerminateCircuit(false);
        }
        else
        {
            transform.position = circuitBoardHit.collider.transform.position;
            RaycastHit2D operatorHit = Physics2D.Raycast(transform.position, Vector2.zero, .1f, 1 << 8);
            if (operatorHit)
            {
                Operator o = operatorHit.collider.GetComponent<Operator>();
                o.TriggerOperator();
            }
            if (gameObject.activeSelf)
            {
                StartCoroutine(TriggerOperator());
                if (OnTick != null)
                {
                    OnTick();
                }
            }
        }
    }

    public void SetVelocity(Vector2 vel)
    {
        rb.velocity = vel.normalized * speed;
    }

    public void SetSpeed(float f)
    {
        if (f >= minSpeed && f <= maxSpeed)
        {
            speed = f;
            SetVelocity(rb.velocity);
        }
    }

    public void Run()
    {
        StopAllCoroutines();
        StartCoroutine(TriggerOperator());
    }

    public void Pause()
    {
        StopAllCoroutines();
        SetVelocity(Vector2.zero);
    }
}
