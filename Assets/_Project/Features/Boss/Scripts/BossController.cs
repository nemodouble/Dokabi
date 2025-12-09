using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Boss.Scripts
{
    [RequireComponent(typeof(SpriteFlasher))]
    public abstract class BossController : MonoBehaviour
    {
        [Header("Boss Components")]
        [SerializeField] private BossAnimation bossAnimation;
        [SerializeField] private BossAttackController attackController;
        [SerializeField] private BossEffectPlayer effectPlayer;
        [SerializeField] private BossContext context;
        [SerializeField] private BossMovement movement;
        [SerializeField] private BossSoundPlayer soundPlayer;
        [SerializeField] private BossStateMachine<BossContext> stateMachine;
        [SerializeField] private BossHealth health;

        public BossAnimation Animation => bossAnimation;
        public BossAttackController Attack => attackController;
        public BossEffectPlayer Effect => effectPlayer;
        public BossContext Context => context;
        public BossMovement Movement => movement;
        public BossSoundPlayer Sound => soundPlayer;
        public BossStateMachine<BossContext> FSM => stateMachine;
        public BossHealth Health => health;

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
            CacheOrGetComponent(ref context);
            CacheOrGetComponent(ref bossAnimation);
            CacheOrGetComponent(ref soundPlayer);
            CacheOrGetComponent(ref effectPlayer);
            CacheOrGetComponent(ref stateMachine);
            CacheOrGetComponent(ref attackController);
            CacheOrGetComponent(ref movement);

            // 의존 관계 연결 및 세부 초기화
            context?.Initialize(this);
            bossAnimation?.Initialize();
            soundPlayer?.Initialize();
            effectPlayer?.Initialize();
            stateMachine?.Initialize();
            attackController?.Initialize();
        }

        private void CacheOrGetComponent<T>(ref T field) where T : Component
        {
            if (field == null)
                field = GetComponent<T>();
        }
    }
}
