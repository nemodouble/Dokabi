namespace _Project.Features.Boss.Scripts.State
{
    public class EmptyState<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        public EmptyState(TStateId id) : base(id) { }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            // 상태 진입 알림 (연출은 BossContext를 구독한 레이어가 담당)
            ctx.NotifyStateEnter(ID);

            // 즉시 종료되는 빈 상태
            IsFinished = true;
        }

        public override void OnExit(BossContext<TStateId> ctx) { }
        public override void Tick(BossContext<TStateId> ctx, float deltaTime) { }
        public override void FixedTick(BossContext<TStateId> ctx, float deltaTime) { }
        public override void HandleEvent(BossContext<TStateId> ctx, object evt) { }
    }
}