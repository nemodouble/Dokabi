using System.Collections;
using UnityEngine;

public interface IHitAble
{ 
    IEnumerator Attacked(int attackDamage, Vector2 attackDir, float attackForceScale = 1);
}