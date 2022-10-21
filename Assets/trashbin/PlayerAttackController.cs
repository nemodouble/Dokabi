using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    Animator animator;
    Animator sideEffectAnimator;
    Animator topEffectAnimator;
    Animator downEffectAnimator;

    int atkDir = 0;
    float atkCool = 0.3f;
    float curAtkCool = 0;
    public Transform sideAtkPos;
    public Transform topAtkPos;
    public Transform downAtkPos;
    public Vector2 atkBoxSize;
    public int playerHP = 10;

    void Start()
    {
        animator = GetComponent<Animator>();
        sideEffectAnimator = GameObject.Find("SideAtkPos").GetComponent<Animator>();
        topEffectAnimator = GameObject.Find("TopAtkPos").GetComponent<Animator>();
        downEffectAnimator = GameObject.Find("DownAtkPos").GetComponent<Animator>();
    }
    
    void Update()
    {
        // 상하 공격 판단
        if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
            atkDir = 1;
        else if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
            atkDir = -1;
        else
            atkDir = 0;
        // 공격
        if (curAtkCool <= 0)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Collider2D[] atkedCollider2Ds;
                if (atkDir == 0)
                {
                    atkedCollider2Ds = Physics2D.OverlapBoxAll(sideAtkPos.position, atkBoxSize, 0);

                    animator.SetTrigger("AtkSide1 Trigger");
                    sideEffectAnimator.SetTrigger("AtkSide1");
                }
                else if (atkDir == 1)
                {
                    atkedCollider2Ds = Physics2D.OverlapBoxAll(topAtkPos.position, atkBoxSize, 0);

                    animator.SetTrigger("AtkUp Trigger");
                    topEffectAnimator.SetTrigger("AtkTop");
                }
                else
                {
                    atkedCollider2Ds = Physics2D.OverlapBoxAll(downAtkPos.position, atkBoxSize, 0);

                    animator.SetTrigger("AtkDown Trigger");
                    downEffectAnimator.SetTrigger("AtkDown");
                }
                foreach (Collider2D atkdColider in atkedCollider2Ds)
                {
                    if (atkdColider.tag == "Enemy")
                    {
                        //GameObject.Find(atkdColider.name).GetComponent<EnemyController>().isHitted = true;
                        //Debug.Log("EnemyHitted" + atkdColider.name);
                    }
                }
                curAtkCool = atkCool;
            }
        }
        else
        {
            curAtkCool -= Time.deltaTime;
        }
    }
    private void OnDrawGizmos()
    {
        /*
        //공격 범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAtkPos.position, atkBoxSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(topAtkPos.position, atkBoxSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(downAtkPos.position, atkBoxSize);
        */
    }
}

