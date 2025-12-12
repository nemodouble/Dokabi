using System.Collections;
using _Project.Features.Battle.Scripts;
using Character;
using UnityEngine;

public class BreakAblePlatform : MonoBehaviour, IHitAble
{
    [SerializeField] private int hp;
    [SerializeField] private Vector2 ableAttackDir;

    public void Hit(int attackDamage, Vector2 attackDir, float attackForceScale = 1)
    {
        if(ableAttackDir == Vector2.zero || attackDir == ableAttackDir)
        {
            hp -= 1;
            if (hp <= 0)
                Destroy(gameObject.transform.parent.gameObject);
        }
    }

}