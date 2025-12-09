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
        
        private readonly Random _random = new Random();

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

        public void AddTransition(TStateId from, TStateId to, Func<TContext, bool> condition, float weight = 1f)
        {
            _transitions.Add(new Transition<TStateId, TContext>(from, to, condition, weight));
        }

        // Any 상태에서 공통으로 나가는 전이 (예: HP<=0이면 무조건 Dead)
        public void AddAnyTransition(TStateId to, Func<TContext, bool> condition, float weight = 1f)
        {
            _anyTransitions.Add(new Transition<TStateId, TContext>(default!, to, condition, weight));
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
            // 1. AnyTransition 우선 (이건 지금처럼 우선순위 고정)
            foreach (var t in _anyTransitions)
            {
                if (t.Condition(_context))
                    return t.To;
            }

            // 2. 현재 상태에서 나가는 전이 중 조건 만족하는 것들 모으기
            var candidates = new List<Transition<TStateId, TContext>>();

            foreach (var t in _transitions)
            {
                if (!EqualityComparer<TStateId>.Default.Equals(t.From, CurrentStateId))
                    continue;

                if (t.Condition(_context))
                    candidates.Add(t);
            }

            if (candidates.Count == 0)
                return CurrentStateId;

            if (candidates.Count == 1)
                return candidates[0].To;

            // 3. 여러 개면 랜덤으로 하나 고르기 (가중치 랜덤)
            var totalWeight = 0f;
            foreach (var c in candidates)
                totalWeight += c.Weight;

            var pick = (float)_random.NextDouble() * totalWeight;

            foreach (var c in candidates)
            {
                pick -= c.Weight;
                if (pick <= 0f)
                    return c.To;
            }

            // floating point 오차 대비해서 마지막 것 반환
            return candidates[^1].To;
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