namespace _Project.Features.Boss.Scripts.State
{
    public class EmptyState : BossState
    {
        private readonly string _enterAnimTrigger;

        public EmptyState(string id, string enterAnimTrigger = null) : base(id)
        {
            _enterAnimTrigger = enterAnimTrigger;
        }

        public override void OnEnter(BossContext ctx)
        {
            if (!string.IsNullOrEmpty(_enterAnimTrigger))
                ctx.PlayAnimTrigger(_enterAnimTrigger);

            // 즉시 종료되는 빈 상태
            IsFinished = true;
        }

        public override void OnExit(BossContext ctx) { }
        public override void Tick(BossContext ctx, float deltaTime) { }
        public override void FixedTick(BossContext ctx, float deltaTime) { }
        public override void HandleEvent(BossContext ctx, object evt) { }
    }
}