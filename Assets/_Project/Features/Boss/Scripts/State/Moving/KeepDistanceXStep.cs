using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class KeepDistanceXStep<TStateId> : Step<TStateId>
    {
        private Transform _keepDistanceTarget;
        private float _maintainDistance;
        private TStateId _frontStepId;
        private TStateId _backStepId;
        
        public KeepDistanceXStep(TStateId id,
            Vector2 relativePos,
            float maxSpeed,
            float decelAccel,
            float decelStartRatio,
            Transform keepDistanceTarget,
            float maintainDistance,
            TStateId frontStepId,
            TStateId backStepId) : base(id,
            relativePos,
            maxSpeed,
            decelAccel,
            decelStartRatio)
        {
            _keepDistanceTarget = keepDistanceTarget;
            _maintainDistance = maintainDistance;
            _frontStepId = frontStepId;
            _backStepId = backStepId;
        }
        
        public override void OnEnter(BossContext<TStateId> ctx)
        {
            base.OnEnter(ctx);
            
            // target position recalculation
            var nowDistance = Mathf.Abs(ctx.Transform.position.x - _keepDistanceTarget.position.x);
            var targetAtRight = ctx.Transform.position.x < _keepDistanceTarget.position.x;
            float dashDir;
            float dashDistance = RelativePos.magnitude;
            if (nowDistance < _maintainDistance)
            {
                dashDir = targetAtRight ? -1f : 1f;
                ctx.NotifyStateEnter(_backStepId);
            }
            else
            {
                dashDir = targetAtRight ? 1f : -1f;
                ctx.NotifyStateEnter(_frontStepId);
            }
            TargetPos = new Vector2(_keepDistanceTarget.position.x + dashDir * dashDistance, TargetPos.y);
            StartingPos = ctx.Transform.position;
        }
    }
}