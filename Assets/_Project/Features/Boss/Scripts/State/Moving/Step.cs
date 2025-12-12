using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class Step<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        private readonly Vector2 _relativePos;
        private readonly float _maxSpeed;
        private readonly float _accel;
        private readonly float _decelLengthRate;

        private Vector2 _startingPos;
        private Vector2 _targetPos;

        private Vector2 _moveDir;
        private float _currentSpeed;

        public Step(TStateId id, Vector2 relativePos, float maxSpeed, float accel, float decelLengthRate) : base(id)
        {
            _relativePos = relativePos;
            _maxSpeed = maxSpeed;
            _accel = accel;
            _decelLengthRate = decelLengthRate;
        }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            IsFinished = false;

            // 상태 진입 알림 (연출은 BossContext를 구독한 레이어가 담당)
            ctx.NotifyStateEnter(ID);

            var bossController = ctx.Controller;
            _startingPos = bossController.transform.position;
            _targetPos = _startingPos + _relativePos;
            _moveDir = (_targetPos - _startingPos).normalized;
            _currentSpeed = 0f;
        }

        public override void Tick(BossContext<TStateId> ctx, float deltaTime)
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

        public override void FixedTick(BossContext<TStateId> ctx, float deltaTime)
        {
            if (IsFinished)
            {
                ctx.StopMove();
                return;
            }

            ctx.Move(_moveDir * _currentSpeed);
        }

        public override void OnExit(BossContext<TStateId> ctx)
        {
            ctx.StopMove();
        }

        public override void HandleEvent(BossContext<TStateId> ctx, object evt)
        {
            // 이벤트 처리 필요 시 구현
        }
    }
}