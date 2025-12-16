using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class RelativeTeleport<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        private Transform _targetTransform;
        private readonly Vector3 _offset;
        private readonly float? _fixX;
        private readonly float? _fixY;
        
        public RelativeTeleport(TStateId id,
            Vector3 offset,
            Transform targetTransform = null,
            float? fixX = null,
            float? fixY = null ) : base(id)
        {
            _offset = offset;
            _targetTransform = targetTransform;
            _fixX = fixX;
            _fixY = fixY;
        }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            ctx.NotifyStateEnter(ID);
            if (_targetTransform == null) _targetTransform = ctx.transform;
            var newPos = _targetTransform.position + _offset;
            if (_fixX.HasValue) newPos.x = _fixX.Value;
            if (_fixY.HasValue) newPos.y = _fixY.Value;
            ctx.transform.position = newPos;
            IsFinished = true;
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