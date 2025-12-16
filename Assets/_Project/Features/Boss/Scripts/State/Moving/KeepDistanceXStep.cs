using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class KeepDistanceXStep<TStateId> : Step<TStateId>
    {
        private Transform _keepDistanceTarget;
        private float _maintainingDistance;
        
        public KeepDistanceXStep(TStateId id,
            Vector2 relativePos,
            float maxSpeed,
            float accel,
            float decelLengthRate,
            Transform keepDistanceTarget,
            float maintainingDistance) : base(id,
            relativePos,
            maxSpeed,
            accel,
            decelLengthRate)
        {
            _keepDistanceTarget = keepDistanceTarget;
            _maintainingDistance = maintainingDistance;
        }
        
        public override void OnEnter(BossContext<TStateId> ctx)
        {
            base.OnEnter(ctx);
            
            // target position recalculation
            var nowDistance = Mathf.Abs(ctx.Transform.position.x - _keepDistanceTarget.position.x);
            if (nowDistance < _maintainingDistance)
            {
                var direction = ctx.Transform.position.x < _keepDistanceTarget.position.x ? -1f : 1f;
                TargetPos = new Vector2(_keepDistanceTarget.position.x + direction * _maintainingDistance,
                    TargetPos.y);
            }
            else
            {
                var direction = ctx.Transform.position.x < _keepDistanceTarget.position.x ? 1f : -1f;
                TargetPos = new Vector2(_keepDistanceTarget.position.x + direction * _maintainingDistance,
                    TargetPos.y);
            }
            StartingPos = ctx.Transform.position;
        }
    }
}