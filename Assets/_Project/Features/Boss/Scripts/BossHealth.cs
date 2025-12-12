using System;
using _Project.Features.Battle.Scripts;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossHealth : MonoBehaviour, IHitAble
    {
        private Health _health;

        public event Action OnDead;
        public event Action<int, Vector2, float> OnHit;

        public void Initialize()
        {
            if (_health == null)
                _health = new Health(400);
            _health.CurrentHp = _health.MaxHp;
        }

        public void Hit(int attackDamage, Vector2 attackDir, float knockbackForce = 1)
        {
            _health.TakeDamage(attackDamage);
            if (attackDamage > 0)
            {
                OnHit?.Invoke(_health.CurrentHp, attackDir, knockbackForce);
            }

            if (_health.IsDead())
            {
                OnDead?.Invoke();
            }
        }

        public bool IsDead()
        {
            return _health != null && _health.IsDead();
        }
    }
}

