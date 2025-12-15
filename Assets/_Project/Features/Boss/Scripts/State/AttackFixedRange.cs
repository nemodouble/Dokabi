using _Project.Features.Battle.Scripts;
using Mechanics.System;
using UnityEngine;

namespace _Project.Features.Boss.Scripts.State
{
    public class AttackFixedRange<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        private readonly FixedDangerRange _attackRange;

        public AttackFixedRange(TStateId id, FixedDangerRange attackRange)
            : base(id)
        {
            _attackRange = attackRange;
        }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            IsFinished = false;

            // 상태 진입 알림 (연출은 BossContext를 구독한 레이어가 담당)
            ctx.NotifyStateEnter(ID);

            if (_attackRange != null)
            {
                _attackRange.gameObject.SetActive(true);
            }

            // 한 번 켜고 바로 종료
            IsFinished = true;
        }

        public override void OnExit(BossContext<TStateId> ctx)
        {
        }

        public override void Tick(BossContext<TStateId> ctx, float deltaTime)
        {
        }

        public override void FixedTick(BossContext<TStateId> ctx, float deltaTime)
        {
        }

        public override void HandleEvent(BossContext<TStateId> ctx, object evt)
        {
        }
    }
}