using System;
using _Project.Core.Scripts.FSM;
using _Project.Features.Boss.Scripts.State;
using UnityEngine;

namespace _Project.Features.Boss.Scripts
{
    public class BossStateMachine<TStateId, TContext> : MonoBehaviour
    {
        protected TContext Context;
        
        protected StateMachine<TStateId, TContext> StateMachine;
        
        public virtual void Initialize()
        {
            StateMachine = new StateMachine<TStateId, TContext>(Context);
        }
        
        public void HandleEvent(object evt)
        {
            StateMachine?.HandleEvent(evt);
        }

        private void Update()
        {
            if (StateMachine == null) return;
            StateMachine.Tick(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            if (StateMachine == null) return;
            StateMachine.FixedTick(Time.fixedDeltaTime);
        }
        
    }
}