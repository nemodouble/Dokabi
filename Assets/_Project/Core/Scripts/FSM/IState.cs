namespace _Project.Core.Scripts.FSM
{
    public interface IState<in TContext> 
    {
        void OnEnter(TContext ctx);
        void OnExit(TContext ctx);
        void Tick(TContext ctx, float deltaTime);
        
        void FixedTick(TContext ctx, float deltaTime);
        
        void HandleEvent(TContext ctx, object evt);
    }
}