using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;

namespace _Project.Features.Maehwa.Scripts
{
    public class MaehwaStateMachine : BossStateMachine
    {
        
        public override void Initialize()
        {
            base.Initialize();
            
            var StartState = new WaitState("Start", Context.Stats);

            StateMachine.AddState("Start", StartState);
            StateMachine.AddState("Dead", DeadState);

            StateMachine.SetInitialState("Start");
            StateMachine.AddAnyTransition("Dead", ctx => ctx.IsDead());
        }
    }
}