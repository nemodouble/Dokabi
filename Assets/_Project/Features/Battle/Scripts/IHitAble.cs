using UnityEngine;

namespace _Project.Features.Battle.Scripts
{
    public interface IHitAble
    { 
        void Hit(int attackDamage, Vector2 attackDir, float knockbackForce = 1);
    }
}