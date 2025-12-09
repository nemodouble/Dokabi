using System.Collections;
using _Project.Core.Scripts.FSM;

namespace _Project.Features.Boss.Scripts.State
{
    public abstract class BossState<TContext> : IState<TContext>
    {
        public readonly string ID;
        
        protected BossState(string id) {
            ID = id;
        }

        public bool IsFinished { get; protected set; }

        public abstract void OnEnter(TContext ctx);

        public abstract void OnExit(TContext ctx);

        public abstract void Tick(TContext ctx, float deltaTime);

        public abstract void FixedTick(TContext ctx, float deltaTime);

        public abstract void HandleEvent(TContext ctx, object evt);
    }
}