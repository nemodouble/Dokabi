namespace _Project.Features.Boss.Scripts.State
{
    public class WaitWithoutGravity<TStateID> : WaitState<TStateID>
    {
        private float originalGravityScale;
        
        public WaitWithoutGravity(TStateID id, float waitingSecond, bool notMoving = false) : base(id, waitingSecond, notMoving)
        {
        }
        
        public override void OnEnter(BossContext<TStateID> ctx)
        {
            base.OnEnter(ctx);
            ctx.SetGravityEnabled(false);
        }
        
        public override void OnExit(BossContext<TStateID> ctx)
        {
            base.OnExit(ctx);
            ctx.SetGravityEnabled(true);
        }
    }
}