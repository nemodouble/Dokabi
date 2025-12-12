using UnityEngine;

namespace _Project.Features.Boss.Scripts.State
{
    public class WaitState<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        private readonly float _waitingSecond;
        private readonly bool _notMoving;

        private float _elapsed;

        public WaitState(TStateId id, float waitingSecond, bool notMoving = false) : base(id)
        {
            _waitingSecond = waitingSecond;
            _notMoving = notMoving;
        }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            _elapsed = 0f;
            IsFinished = false;

            // 상태 진입 알림 (연출은 BossContext를 구독한 레이어가 담당)
            ctx.NotifyStateEnter(ID);
        }

        public override void OnExit(BossContext<TStateId> ctx)
        {
            if (_notMoving)
            {
                ctx.StopMove();
            }
        }

        public override void Tick(BossContext<TStateId> ctx, float deltaTime)
        {
            if (IsFinished)
                return;

            _elapsed += deltaTime;
            if (_elapsed >= _waitingSecond)
            {
                IsFinished = true;
            }
        }

        public override void FixedTick(BossContext<TStateId> ctx, float deltaTime)
        {
            if (_notMoving && !IsFinished)
            {
                ctx.StopMove();
            }
        }

        public override void HandleEvent(BossContext<TStateId> ctx, object evt) { }
    }
}