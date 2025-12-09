using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class Step : BossState<BossContext>
    {
        private readonly Vector2 _relativePos;
        private readonly float _maxSpeed;
        private readonly float _accel;
        private readonly float _decelLengthRate;

        private Vector2 _startingPos;
        private Vector2 _targetPos;

        private Vector2 _moveDir;
        private float _currentSpeed;

        private readonly string _enterAnimTrigger;

        public Step(string id, Vector2 relativePos, float maxSpeed, float accel, float decelLengthRate,
                    string enterAnimTrigger = null) : base(id)
        {
            _relativePos = relativePos;
            _maxSpeed = maxSpeed;
            _accel = accel;
            _decelLengthRate = decelLengthRate;
            _enterAnimTrigger = enterAnimTrigger;
        }

        public override void OnEnter(BossContext ctx)
        {
            IsFinished = false;

            if (!string.IsNullOrEmpty(_enterAnimTrigger))
                ctx.PlayAnimTrigger(_enterAnimTrigger);

            var bossController = ctx.Controller;
            _startingPos = bossController.transform.position;
            _targetPos = _startingPos + _relativePos;
            _moveDir = (_targetPos - _startingPos).normalized;
            _currentSpeed = 0f;
        }

        public override void Tick(BossContext ctx, float deltaTime)
        {
            if (IsFinished)
                return;

            var bossTransform = ctx.Controller.transform;
            var bossPos = (Vector2)bossTransform.position;
            var toTarget = _targetPos - bossPos;
            var leftLength = toTarget.magnitude;

            // 목표 지점 도달
            if (leftLength <= 0.1f)
            {
                IsFinished = true;
                _currentSpeed = 0f;
                return;
            }

            // 벽 정지
            if (ctx.IsHeading(toTarget, 0.1f))
            {
                IsFinished = true;
                _currentSpeed = 0f;
                return;
            }

            // 방향은 항상 타겟을 향하도록 갱신
            _moveDir = toTarget.normalized;

            // 감속 구간이면 남은 거리 비율에 따라 속도 줄이기
            if (leftLength < _relativePos.magnitude * _decelLengthRate)
            {
                _currentSpeed = _maxSpeed * leftLength / _relativePos.magnitude;
                if (_currentSpeed <= 0.01f)
                {
                    IsFinished = true;
                }
            }
            // 가속 구간
            else
            {
                var dv = (_maxSpeed - _currentSpeed) * _accel * deltaTime;
                _currentSpeed = Mathf.Clamp(_currentSpeed + dv, 0f, _maxSpeed);
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

        public override void OnExit(BossContext ctx)
        {
            ctx.StopMove();
        }

        public override void HandleEvent(BossContext ctx, object evt)
        {
            // 이벤트 처리 필요 시 구현
        }
    }
}