using System;
using System.Collections.Generic;

namespace _Project.Core.Scripts.FSM
{
    public class StateMachine<TStateId, TContext>
    {
        private readonly TContext _context;

        private readonly Dictionary<TStateId, IState<TContext>> _states = new();

        private readonly List<Transition<TStateId, TContext>> _transitions = new();

        private readonly List<Transition<TStateId, TContext>> _anyTransitions = new();
        
        private readonly Random _random = new();

        public TStateId CurrentStateId { get; private set; }
        public IState<TContext> CurrentState { get; private set; }

        public event Action<TStateId, TStateId> OnStateChanged;

        public StateMachine(TContext context)
        {
            _context = context;
        }

        public void AddState(TStateId id, IState<TContext> state)
        {
            _states[id] = state;
        }

        public void AddTransition(TStateId from, TStateId to, Func<TContext, bool> condition, int priority = 0, float weight = 1f)
        {
            _transitions.Add(new Transition<TStateId, TContext>(from, to, condition, priority, weight));
        }

        // Any 상태에서 공통으로 나가는 전이 (예: HP<=0이면 무조건 Dead)
        public void AddAnyTransition(TStateId to, Func<TContext, bool> condition, int priority = 0, float weight = 1f)
        {
            _anyTransitions.Add(new Transition<TStateId, TContext>(default!, to, condition, priority, weight));
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
            // 1. AnyTransition 우선 처리
            var anyTransition = FindAnyTransition();
            if (!EqualityComparer<TStateId>.Default.Equals(anyTransition, CurrentStateId))
                return anyTransition;

            // 2. 현재 상태에서 나가는 후보 전이 모으기
            var candidates = FindTransitionCandidatesFromCurrentState();
            if (candidates.Count == 0)
                return CurrentStateId;

            // 3. 후보 1개면 그대로, 여러 개면 가중치 랜덤 선택
            return SelectTargetStateFromCandidates(candidates);
        }

        private TStateId FindAnyTransition()
        {
            foreach (var t in _anyTransitions)
            {
                if (t.Condition(_context))
                    return t.To;
            }

            return CurrentStateId;
        }

        private List<Transition<TStateId, TContext>> FindTransitionCandidatesFromCurrentState()
        {
            List<Transition<TStateId, TContext>> candidates = null;
            var bestPriority = int.MinValue;

            foreach (var t in _transitions)
            {
                if (!EqualityComparer<TStateId>.Default.Equals(t.From, CurrentStateId))
                    continue;

                if (!t.Condition(_context))
                    continue;

                if (t.Priority > bestPriority)
                {
                    bestPriority = t.Priority;
                    candidates = new List<Transition<TStateId, TContext>> { t };
                }
                else if (t.Priority == bestPriority)
                {
                    candidates ??= new List<Transition<TStateId, TContext>>();
                    candidates.Add(t);
                }
            }

            return candidates ?? new List<Transition<TStateId, TContext>>();
        }

        private TStateId SelectTargetStateFromCandidates(
            List<Transition<TStateId, TContext>> candidates)
        {
            if (candidates.Count == 1)
                return candidates[0].To;

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

            // 부동소수점 오차 대비용 fallback
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