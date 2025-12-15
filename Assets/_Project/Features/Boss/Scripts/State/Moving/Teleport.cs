using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class Teleport<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        private bool _isRelative = false;
        private Transform _targetTransform;
        private Vector3 _offset;
        
        public Teleport(TStateId id, bool isRelative, Vector3 offset, Transform targetTransform = null) : base(id)
        {
            _isRelative = isRelative;
            _offset = offset;
            _targetTransform = targetTransform;
        }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            ctx.NotifyStateEnter(ID);
            if (_targetTransform == null) _targetTransform = ctx.transform;
            ctx.Transform.position = _targetTransform.position + _offset;
        }

        public override void OnExit(BossContext<TStateId> ctx)
        {
            ctx.NotifyStateExit(ID);
        }

        public override void Tick(BossContext<TStateId> ctx, float deltaTime)
        {
        }

        public override void FixedTick(BossContext<TStateId> ctx, float deltaTime)
        {
        }

        public override void HandleEvent(BossContext<TStateId> ctx, object evt)
        {
        }
    }
}