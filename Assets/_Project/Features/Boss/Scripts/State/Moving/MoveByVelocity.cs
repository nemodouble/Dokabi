using UnityEngine;
using _Project.Features.Boss.Scripts;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class MoveByVelocity : BossState
    {
        private readonly string _enterAnimTrigger;
        private readonly float velocity;
        private readonly Vector2 dir;
        private readonly float timeMax;
        protected float length;
        protected bool haveTargetPos = true;
        
        private float timeNow;
        private Vector2 startPos;
        protected Vector3? targetPos;

        private float _currentSpeed;
        private Vector2 _moveDir;

        public MoveByVelocity(string id, Vector2 dir, float velocity, float timeMax, float length = 0,
                              string enterAnimTrigger = null) : base(id)
        {
            this.velocity = velocity;
            this.dir = dir;
            this.timeMax = timeMax;
            if (length != 0)
                this.length = length;
            else
                haveTargetPos = false;

            _enterAnimTrigger = enterAnimTrigger;
        }

        public override void OnEnter(BossContext ctx)
        {   
            if (!string.IsNullOrEmpty(_enterAnimTrigger))
                ctx.PlayAnimTrigger(_enterAnimTrigger);

            startPos = ctx.Controller.transform.position;
            if (targetPos == null && haveTargetPos)
            {
                targetPos = (Vector2)startPos + dir.normalized * length;
            }
            else if (targetPos != null && length == 0)
            {
                length = ((Vector2)targetPos - startPos).magnitude;
            }

            timeNow = 0f;
            IsFinished = false;
            _moveDir = dir.normalized;
            _currentSpeed = velocity;
        }

        public override void OnExit(BossContext ctx)
        {
            ctx.StopMove();
        }

        public override void Tick(BossContext ctx, float deltaTime)
        {
            if (IsFinished)
                return;

            timeNow += deltaTime;
            if (timeNow >= timeMax)
            {
                IsFinished = true;
                return;
            }

            var pos = (Vector2)ctx.Controller.transform.position;

            if (targetPos != null)
            {
                var toTarget = ((Vector2)targetPos - pos);
                var leftLength = toTarget.magnitude;
                if (leftLength <= 0.05f || (pos - startPos).magnitude > length)
                {
                    IsFinished = true;
                    _currentSpeed = 0f;
                    return;
                }

                _moveDir = toTarget.normalized;
                _currentSpeed = velocity;
            }
            else
            {
                _moveDir = dir.normalized;
                _currentSpeed = velocity;
            }
        }

        public override void FixedTick(BossContext ctx, float deltaTime)
        {
            if (IsFinished)
            {
                ctx.StopMove();
                return;
            }

            ctx.Move(_moveDir * _currentSpeed);
        }

        public override void HandleEvent(BossContext ctx, object evt)
        {
        }
    }
}