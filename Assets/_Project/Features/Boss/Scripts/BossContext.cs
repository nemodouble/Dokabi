using System;
using Character.Player;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossContext<TStateId> : MonoBehaviour
    {
        public GameObject GameObject => gameObject;
        public Transform Transform => transform;
        public Transform PlayerTransform { get; private set; }
        
        public BossAnimation Anim { get; private set; }
        public BossAttack Attack { get; private set; }
        public BossEffect Effect { get; private set; }
        public BossMovement Movement { get; private set; }
        public BossSound Sound { get; private set; }
        public BossStateMachine<TStateId, BossContext<TStateId>> StateMachine { get; private set; }
        public BossHealth Health { get; private set; }

        public event Action OnDead;
        public event Action<int, Vector2, float> OnHit;

        // 상태 진입 이벤트: 상태 ID를 전달해 연출 전담 컴포넌트가 처리하도록 한다.
        public event Action<TStateId> OnStateEntered;
        public event Action<TStateId> OnStateExited;
        
        public enum LookingDir
        {
            Right = 1,
            Left = -1
        }
        protected LookingDir _lookingDir = LookingDir.Left;

        public virtual void Initialize()
        {
            PlayerTransform = FindObjectOfType<PlayerController>()?.transform;

            if (Health != null)
            {
                Health.Initialize();
                Health.OnDead += HandleDead;
                Health.OnHit += HandleHit;
            }
        }
        
        public void BindModules(
            BossHealth health,
            BossMovement movement,
            BossAttack attack,
            BossAnimation anim,
            BossSound sound,
            BossEffect effect)
        {
            Health = health;
            Movement = movement;
            Attack = attack;
            Anim = anim;
            Sound = sound;
            Effect = effect;
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
            return Health != null && Health.IsDead();
        }

        // ===== Movement Facade =====
        public void Move(Vector2 velocity)
        {
            if (Movement == null)
                return;
            Movement.SetVelocity(velocity);
        }

        public void MoveX(float vx)
        {
            if (Movement == null)
                return;
            Movement.SetVelocityX(vx);
        }

        public void MoveY(float vy)
        {
            if (Movement == null)
                return;
            Movement.SetVelocityY(vy);
        }

        public void StopMove()
        {
            Move(Vector2.zero);
        }

        public bool IsHeading(Vector2 dir, float distance)
        {
            if (Movement == null)
                return false;

            var hit = Movement.IsHeading(dir, distance);
            return hit.collider != null;
        }

        // 현재 플랫폼 위에 서 있는지 여부를 컨텍스트에서 바로 조회할 수 있도록 래핑
        public bool IsOnPlatform()
        {
            if (Movement == null)
                return false;
            return Movement.IsOnPlatform();
        }

        // ===== Attack Facade =====
        public void SummonAttack(GameObject prefab, Vector2 relativePos)
        {
            if (Attack == null || prefab == null)
                return;

            Attack.CallInstantiate(prefab, relativePos);
        }

        // ===== Animation Facade =====
        public void PlayAnimTrigger(string triggerName)
        {
            if (Anim == null)
                return;
            Anim.PlayTrigger(triggerName);
        }

        public void SetAnimBool(string paramName, bool value)
        {
            if (Anim == null)
                return;
            Anim.SetBool(paramName, value);
        }

        public void SetAnimFloat(string paramName, float value)
        {
            if (Anim == null)
                return;
            Anim.SetFloat(paramName, value);
        }

        // FSM State에서 상태 진입을 알릴 때 사용. 연출은 이 이벤트를 구독하는 쪽에서 처리.
        public void NotifyStateEnter(TStateId id)
        {
            OnStateEntered?.Invoke(id);
        }

        public void NotifyStateExit(TStateId id)
        {
            OnStateExited?.Invoke(id);
        }
        
        public void SetToLookPlayer()
        {
            SetLookingDir(PlayerTransform.position.x < transform.position.x ? LookingDir.Left : LookingDir.Right);
        }

        public virtual void SetLookingDir(LookingDir dir)
        {
            _lookingDir = dir;
            Effect.SetLookDirection(dir == LookingDir.Right);
        }
    }
}