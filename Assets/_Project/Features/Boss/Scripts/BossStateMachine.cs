using _Project.Core.Scripts.FSM;
using _Project.Features.Boss.Scripts.State;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossStateMachine<TContext> : MonoBehaviour
    {
        protected TContext Context;
        
        protected StateMachine<string, TContext> StateMachine;
        
        public virtual void Initialize()
        {
            StateMachine = new StateMachine<string, TContext>(Context);
        }
    }
}