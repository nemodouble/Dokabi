using System.Collections;
using _Project.Core.Scripts.FSM;

namespace _Project.Features.Boss.Scripts.State
{
    public abstract class BossState<TStateId, TContext> : IState<TContext>
    {
        public readonly TStateId ID;

        protected BossState(TStateId id)
        {
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
