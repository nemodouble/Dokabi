using _Project.Core.Scripts.FSM;
using _Project.Features.Boss.Scripts.State;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossStateMachine : MonoBehaviour
    {
        protected BossContext Context;
        
        protected StateMachine<string, BossContext> StateMachine;
        
        public virtual void Initialize()
        {
            StateMachine = new StateMachine<string, BossContext>(Context);
        }
    }
}