using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Boss.Scripts
{
    [RequireComponent(typeof(SpriteFlasher))]
    public class BossController<TStateId, TContext> : MonoBehaviour
        where TContext : BossContext<TStateId>
    {
        [Header("Boss Components")]
        [SerializeField] private BossAnimation bossAnimation;
        [FormerlySerializedAs("attackController")] [SerializeField] private BossAttack attack;
        [FormerlySerializedAs("effectPlayer")] [SerializeField] private BossEffect effect;
        [SerializeField] private BossMovement movement;
        [FormerlySerializedAs("soundPlayer")] [SerializeField] private BossSound sound;
        [SerializeField] private BossStateMachine<TStateId, TContext> stateMachine;
        [SerializeField] private BossHealth health;
        [SerializeField] private TContext context;

        protected virtual void Awake()
        {
            InitializeBoss();
        }

        /// <summary>
        /// 공통 보스 구성요소 초기화. 자식에서 base.Awake() 또는 base.InitializeBoss() 호출.
        /// </summary>
        protected void InitializeBoss()
        {
            CacheOrGetComponent(ref health);
            CacheOrGetComponent(ref bossAnimation);
            CacheOrGetComponent(ref sound);
            CacheOrGetComponent(ref effect);
            CacheOrGetComponent(ref stateMachine);
            CacheOrGetComponent(ref attack);
            CacheOrGetComponent(ref movement);
            CacheOrGetComponent(ref context);

            // 의존 관계 연결 및 세부 초기화
            health?.Initialize();
            bossAnimation?.Initialize();
            sound?.Initialize();
            effect?.Initialize();
            stateMachine?.Initialize();
            attack?.Initialize();
            movement?.Initialize();
            context?.Initialize();
            
            context.BindModules(health, movement, attack, bossAnimation, sound, effect);
        }

        private void CacheOrGetComponent<T>(ref T field) where T : Component
        {
            if (field == null)
                field = GetComponent<T>();
        }
    }
}
