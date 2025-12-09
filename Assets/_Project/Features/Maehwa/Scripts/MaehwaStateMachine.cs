using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using _Project.Features.Boss.Scripts.State.Dead;

namespace _Project.Features.Maehwa.Scripts
{
    public class MaehwaStateMachine : BossStateMachine<MaehwaContext>
    {
        
        public override void Initialize()
        {
            base.Initialize();
            
            var startState = new WaitState("Start", Context.Stats.startWaitTime, false, "Start-Wait");
            var deadState = new DeadNormal("Dead", "Dead");

            StateMachine.AddState("Start", startState);
            StateMachine.AddState("Dead", deadState);

            StateMachine.SetInitialState("Start");
            StateMachine.AddAnyTransition("Dead", ctx => ctx.IsDead());
        }
    }
}