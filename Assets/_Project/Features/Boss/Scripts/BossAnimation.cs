using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Boss.Scripts
{
    public class BossAnimation : MonoBehaviour
    {
        public Animator animator;
        public Component boss; // BossController<TStateId> 제네릭을 위해 Component로 완화

        // 공통 BossContext 캐시 (제네릭 파라미터를 모를 수 있으므로 Component로 보관)
        [SerializeField] protected Component bossContext;

        protected virtual void Awake()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            if (boss == null)
                boss = GetComponent(typeof(BossController<,>));

            if (bossContext == null)
                bossContext = GetComponent(typeof(BossContext<>));

            if (animator == null)
            {
                if (boss != null && boss.TryGetComponent(out Animator bossAnimator))
                {
                    animator = bossAnimator;
                }
                else
                {
                    animator = GetComponent<Animator>();
                }
            }

            SubscribeStateEvents();
        }

        protected virtual void OnEnable()
        {
            SubscribeStateEvents();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeStateEvents();
        }

        /// <summary>
        /// 상태 컨텍스트 이벤트 구독 패턴을 위한 템플릿 메서드. 자식에서 필요시 override.
        /// </summary>
        protected virtual void SubscribeStateEvents() { }

        /// <summary>
        /// 상태 컨텍스트 이벤트 해제 패턴을 위한 템플릿 메서드. 자식에서 필요시 override.
        /// </summary>
        protected virtual void UnsubscribeStateEvents() { }

        public void PlayTrigger(string triggerName)
        {
            if (animator == null || string.IsNullOrEmpty(triggerName))
                return;
            animator.SetTrigger(triggerName);
        }

        public void SetBool(string paramName, bool value)
        {
            if (animator == null || string.IsNullOrEmpty(paramName))
                return;
            animator.SetBool(paramName, value);
        }

        public void SetFloat(string paramName, float value)
        {
            if (animator == null || string.IsNullOrEmpty(paramName))
                return;
            animator.SetFloat(paramName, value);
        }
    }
}
