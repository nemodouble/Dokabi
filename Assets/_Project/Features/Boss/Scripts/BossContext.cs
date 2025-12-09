using System;
using Character.Player;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossContext : MonoBehaviour
    {
        public BossController Controller { get; private set; }
        
        public Transform PlayerTransform { get; private set; }

        public event Action OnDead;
        public event Action<int, Vector2, float> OnHit;

        public void Initialize(BossController controller)
        {
            Controller = controller;
            PlayerTransform = FindObjectOfType<PlayerController>()?.transform;

            if (Controller.Health != null)
            {
                Controller.Health.Initialize();
                Controller.Health.OnDead += HandleDead;
                Controller.Health.OnHit += HandleHit;
            }
        }

        private void HandleDead()
        {
            OnDead?.Invoke();
        }

        private void HandleHit(int currentHp, Vector2 attackDir, float knockbackForce)
        {
            OnHit?.Invoke(currentHp, attackDir, knockbackForce);
        }

        public bool IsDead()
        {
            return Controller.Health != null && Controller.Health.IsDead();
        }

        // ===== Movement Facade =====
        public void Move(Vector2 velocity)
        {
            if (Controller == null || Controller.Movement == null)
                return;
            Controller.Movement.SetVelocity(velocity);
        }

        public void MoveX(float vx)
        {
            if (Controller == null || Controller.Movement == null)
                return;
            Controller.Movement.SetVelocityX(vx);
        }

        public void MoveY(float vy)
        {
            if (Controller == null || Controller.Movement == null)
                return;
            Controller.Movement.SetVelocityY(vy);
        }

        public void StopMove()
        {
            Move(Vector2.zero);
        }

        public bool IsHeading(Vector2 dir, float distance)
        {
            if (Controller == null || Controller.Movement == null)
                return false;

            var hit = Controller.Movement.IsHeading(dir, distance);
            return hit.collider != null;
        }

        // ===== Attack Facade =====
        public void SummonAttack(GameObject prefab, Vector2 relativePos)
        {
            if (Controller == null || Controller.Attack == null || prefab == null)
                return;

            Controller.Attack.CallInstantiate(prefab, relativePos);
        }

        // ===== Animation Facade =====
        public void PlayAnimTrigger(string triggerName)
        {
            if (Controller.Animation == null)
                return;
            Controller.Animation.PlayTrigger(triggerName);
        }

        public void SetAnimBool(string paramName, bool value)
        {
            if (Controller.Animation == null)
                return;
            Controller.Animation.SetBool(paramName, value);
        }

        public void SetAnimFloat(string paramName, float value)
        {
            if (Controller.Animation == null)
                return;
            Controller.Animation.SetFloat(paramName, value);
        }
    }
}