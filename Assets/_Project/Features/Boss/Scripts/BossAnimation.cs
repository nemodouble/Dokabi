using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Boss.Scripts
{
    public class BossAnimation : MonoBehaviour
    {
        public List<Animator> animator;
        public Component boss; // BossController<TStateId> 제네릭을 위해 Component로 완화

        // 공통 BossContext 캐시 (제네릭 파라미터를 모를 수 있으므로 Component로 보관)
        [SerializeField] protected Component bossContext;

        // Animator별 파라미터 캐시 (이름+타입 기준)
        private readonly Dictionary<Animator, HashSet<(string name, AnimatorControllerParameterType type)>> _parameterCache
            = new();

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

            animator ??= new List<Animator>();
            if (boss != null && boss.TryGetComponent(out Animator bossAnimator))
            {
                animator.Add(bossAnimator);
            }
            else
            {
                animator.Add(GetComponent<Animator>());
            }

            BuildParameterCache();
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

            foreach (var anim in animator)
            {
                if (anim == null)
                    continue;

                if (!HasCachedParameter(anim, triggerName, AnimatorControllerParameterType.Trigger))
                    continue;

                anim.SetTrigger(triggerName);
            }
        }

        public void SetBool(string paramName, bool value)
        {
            if (animator == null || string.IsNullOrEmpty(paramName))
                return;

            foreach (var anim in animator)
            {
                if (anim == null)
                    continue;

                if (!HasCachedParameter(anim, paramName, AnimatorControllerParameterType.Bool))
                    continue;

                anim.SetBool(paramName, value);
            }
        }

        public void SetFloat(string paramName, float value)
        {
            if (animator == null || string.IsNullOrEmpty(paramName))
                return;

            foreach (var anim in animator)
            {
                if (anim == null)
                    continue;

                if (!HasCachedParameter(anim, paramName, AnimatorControllerParameterType.Float))
                    continue;

                anim.SetFloat(paramName, value);
            }
        }

        /// <summary>
        /// Animator별 파라미터 정보를 캐싱.
        /// Animator.runtimeAnimatorController가 바뀌면 다시 호출해줘야 함.
        /// </summary>
        private void BuildParameterCache()
        {
            _parameterCache.Clear();
            if (animator == null)
                return;

            foreach (var anim in animator)
            {
                if (anim == null)
                    continue;

                var set = new HashSet<(string, AnimatorControllerParameterType)>();
                foreach (var p in anim.parameters)
                {
                    set.Add((p.name, p.type));
                }

                _parameterCache[anim] = set;
            }
        }

        private bool HasCachedParameter(Animator anim, string name, AnimatorControllerParameterType type)
        {
            if (anim == null || string.IsNullOrEmpty(name))
                return false;

            if (!_parameterCache.TryGetValue(anim, out var set))
                return false;

            return set.Contains((name, type));
        }
    }
}
