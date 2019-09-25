using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator anim;
    SpriteRenderer sr;
    public bool attacker;
    public int initAttackMove = 0;
    int attackMove;
    public int AttackMove
    {
        get
        {
            return attackMove;
        }
        set
        {
            attackMove = value;
            if (attacker)
            {
                if (attackMove == 1)
                {
                    if (sr == null)
                    {
                        sr = GetComponent<SpriteRenderer>();
                    }
                    sr.color = Color.red;
                }
                else if (attackMove == 0)
                {
                    anim.speed = Thread.instance.speed;
                    anim.SetTrigger("Attack");
                    AttackMove = initAttackMove;
                }
            }
        }
    }
    public int initHP;
    int hp;
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            if (hp <= 0)
            {
                Die();
            }
        }
    }

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!attacker)
        {
            anim.gameObject.SetActive(false);
            return;
        }
        Reset();
    }

    public void Die()
    {
        gameObject.SetActive(false);
        Destroy(Instantiate(PrefabManager.instance.GetPrefab("Explosion"), transform.position, Quaternion.identity), 1);
    }

    private void OnEnable()
    {
        if (attacker)
        {
            Action.OnAction += DecrementAttackMove;
        }
    }

    private void OnDisable()
    {
        if (attacker)
        {
            Action.OnAction -= DecrementAttackMove;
        }
    }

    void DecrementAttackMove()
    {
        if ((Actor.instance.transform.position - transform.position).magnitude == 1)
        {
            AttackMove--;
        }
    }

    public void Reset()
    {
        HP = initHP;
        AttackMove = initAttackMove;
    }
}
