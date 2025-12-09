using System;
using System.Collections.Generic;

namespace _Project.Core.Scripts.FSM
{
    public class StateMachine<TStateId, TContext>
    {
        private readonly TContext _context;

        private readonly Dictionary<TStateId, IState<TContext>> _states
            = new Dictionary<TStateId, IState<TContext>>();

        private readonly List<Transition<TStateId, TContext>> _transitions
            = new List<Transition<TStateId, TContext>>();

        private readonly List<Transition<TStateId, TContext>> _anyTransitions
            = new List<Transition<TStateId, TContext>>();

        public TStateId CurrentStateId { get; private set; }
        public IState<TContext> CurrentState { get; private set; }

        public event Action<TStateId, TStateId>? OnStateChanged;

        public StateMachine(TContext context)
        {
            _context = context;
        }

        public void AddState(TStateId id, IState<TContext> state)
        {
            _states[id] = state;
        }

        public void AddTransition(TStateId from, TStateId to, Func<TContext, bool> condition)
        {
            _transitions.Add(new Transition<TStateId, TContext>(from, to, condition));
        }

        // Any 상태에서 공통으로 나가는 전이 (예: HP<=0이면 무조건 Dead)
        public void AddAnyTransition(TStateId to, Func<TContext, bool> condition)
        {
            _anyTransitions.Add(new Transition<TStateId, TContext>(default!, to, condition));
        }

        public void SetInitialState(TStateId id)
        {
            CurrentStateId = id;
            CurrentState = _states[id];
            CurrentState.OnEnter(_context);
        }

        public void Tick(float deltaTime)
        {
            // 1. 현재 상태 로직
            CurrentState.Tick(_context, deltaTime);

            // 2. 전이 검사
            var next = FindNextState();
            if (!EqualityComparer<TStateId>.Default.Equals(next, CurrentStateId))
            {
                ChangeState(next);
            }
        }
        
        public void FixedTick(float fixedDeltaTime)
        {
            // 1. 현재 상태 로직
            CurrentState.FixedTick(_context, fixedDeltaTime);

            // 2. 전이 검사
            var next = FindNextState();
            if (!EqualityComparer<TStateId>.Default.Equals(next, CurrentStateId))
            {
                ChangeState(next);
            }
        }

        public void HandleEvent(object evt)
        {
            CurrentState.HandleEvent(_context, evt);

            var next = FindNextState();
            if (!EqualityComparer<TStateId>.Default.Equals(next, CurrentStateId))
            {
                ChangeState(next);
            }
        }

        private TStateId FindNextState()
        {
            // 우선 AnyTransition 체크
            foreach (var t in _anyTransitions)
            {
                if (t.Condition(_context))
                    return t.To;
            }

            // 그 다음 현재 상태에서의 전이
            foreach (var t in _transitions)
            {
                if (!EqualityComparer<TStateId>.Default.Equals(t.From, CurrentStateId))
                    continue;

                if (t.Condition(_context))
                    return t.To;
            }

            return CurrentStateId;
        }

        private void ChangeState(TStateId nextStateId)
        {
            var prev = CurrentStateId;
            CurrentState.OnExit(_context);

            CurrentStateId = nextStateId;
            CurrentState = _states[nextStateId];
            CurrentState.OnEnter(_context);

            OnStateChanged?.Invoke(prev, nextStateId);
        }
    }

}