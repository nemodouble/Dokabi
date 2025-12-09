namespace _Project.Core.Scripts.FSM
{
    public class Transition<TStateId, TContext>
    {
        public TStateId From { get;}
        public TStateId To { get; }
        public System.Func<TContext, bool> Condition { get; }
        public int Priority { get; }
        public float Weight { get; }
        
        public Transition(TStateId from, TStateId to, System.Func<TContext, bool> condition, int priority = 0,  float weight = 1f)
        {
            From = from;
            To = to;
            Condition = condition;
            Priority = priority;
            Weight = weight;
        }
    }
}