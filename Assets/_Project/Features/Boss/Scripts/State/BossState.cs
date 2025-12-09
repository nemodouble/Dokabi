using System.Collections;
using _Project.Core.Scripts.FSM;

namespace _Project.Features.Boss.Scripts.State
{
    public abstract class BossState : IState<BossContext>
    {
        public readonly string ID;
        
        protected BossState(string id) {
            ID = id;
        }

        public bool IsFinished { get; protected set; }

        public abstract void OnEnter(BossContext ctx);

        public abstract void OnExit(BossContext ctx);

        public abstract void Tick(BossContext ctx, float deltaTime);

        public abstract void FixedTick(BossContext ctx, float deltaTime);

        public abstract void HandleEvent(BossContext ctx, object evt);
    }
}