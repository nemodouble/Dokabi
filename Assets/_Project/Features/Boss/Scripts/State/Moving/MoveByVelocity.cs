using UnityEngine;
using _Project.Features.Boss.Scripts;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class MoveByVelocity<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        private readonly float _velocity;
        private readonly Vector2 _dir;
        private readonly float _timeMax;
        protected float _length;
        protected bool haveTargetPos = true;

        private float _timeNow;
        private Vector2 _startPos;
        protected Vector3? targetPos;

        private float _currentSpeed;
        private Vector2 _moveDir;

        public MoveByVelocity(TStateId id, Vector2 dir, float velocity, float timeMax, float length = 0)
            : base(id)
        {
            _velocity = velocity;
            _dir = dir;
            _timeMax = timeMax;
            if (length != 0)
                _length = length;
            else
                haveTargetPos = false;
        }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            ctx.NotifyStateEnter(ID);

            _startPos = ctx.Transform.position;
            if (targetPos == null && haveTargetPos)
            {
                targetPos = (Vector2)_startPos + _dir.normalized * _length;
            }

            _moveDir = _dir.normalized;
            _currentSpeed = _velocity;
            _timeNow = 0f;
            IsFinished = false;
        }

        public override void Tick(BossContext<TStateId> ctx, float deltaTime)
        {
            if (IsFinished)
                return;

            _timeNow += deltaTime;
            if (_timeNow >= _timeMax)
            {
                IsFinished = true;
            }
        }

        public override void FixedTick(BossContext<TStateId> ctx, float deltaTime)
        {
            if (IsFinished)
            {
                ctx.StopMove();
                return;
            }

            if (haveTargetPos && targetPos.HasValue)
            {
                var current = (Vector2)ctx.transform.position;
                var toTarget = (Vector2)targetPos.Value - current;
                if (toTarget.magnitude <= 0.1f)
                {
                    IsFinished = true;
                    ctx.StopMove();
                    return;
                }
                _moveDir = toTarget.normalized;
            }

            ctx.Move(_moveDir * _currentSpeed);
        }

        public override void OnExit(BossContext<TStateId> ctx)
        {
            ctx.StopMove();
        }

        public override void HandleEvent(BossContext<TStateId> ctx, object evt) { }
    }
}