using UnityEngine;

namespace _Project.Features.Boss.Scripts.State
{
    public class AttackSummon<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        private readonly GameObject _prefab;
        private readonly Vector2 _relativePos;

        public AttackSummon(TStateId id, GameObject gameObject, Vector2? relativePos = null) : base(id)
        {
            _prefab = gameObject;
            _relativePos = relativePos ?? Vector2.zero;
        }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            IsFinished = false;

            // 상태 진입 알림 (연출은 BossContext를 구독한 레이어가 담당)
            ctx.NotifyStateEnter(ID);

            ctx.SummonAttack(_prefab, _relativePos);

            // 한 번만 소환하고 바로 종료
            IsFinished = true;
        }

        public override void OnExit(BossContext<TStateId> ctx) { }

        public override void Tick(BossContext<TStateId> ctx, float deltaTime) { }

        public override void FixedTick(BossContext<TStateId> ctx, float deltaTime) { }

        public override void HandleEvent(BossContext<TStateId> ctx, object evt) { }
    }
}