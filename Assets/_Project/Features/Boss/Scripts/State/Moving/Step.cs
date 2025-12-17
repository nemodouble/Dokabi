using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class Step<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        protected readonly Vector2 RelativePos;
        private readonly float _maxSpeed;
        private readonly float _decelAccel;
        private readonly float _decelStartRatio;

        protected Vector2 StartingPos;
        protected Vector2 TargetPos;

        private Vector2 _moveDir;
        private float _currentSpeed;

        public Step(TStateId id, Vector2 relativePos, float maxSpeed, float decelAccel, float decelStartRatio) : base(id)
        {
            RelativePos = relativePos;
            _maxSpeed = maxSpeed;
            _decelAccel = decelAccel;
            _decelStartRatio = decelStartRatio;
        }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            IsFinished = false;

            // 상태 진입 알림 (연출은 BossContext를 구독한 레이어가 담당)
            ctx.NotifyStateEnter(ID);

            StartingPos = ctx.transform.position;
            TargetPos = StartingPos + RelativePos;
            _moveDir = (TargetPos - StartingPos).normalized;
            _currentSpeed = 0f;
        }

        public override void Tick(BossContext<TStateId> ctx, float deltaTime)
        {
            if (IsFinished)
                return;

            var bossTransform = ctx.transform;
            var bossPos = (Vector2)bossTransform.position;
            var toTarget = TargetPos - bossPos;
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
            if (leftLength < RelativePos.magnitude * _decelStartRatio)
            {
                _currentSpeed = _maxSpeed * leftLength / RelativePos.magnitude;
                if (_currentSpeed <= 0.01f)
                {
                    IsFinished = true;
                }
            }
            // 가속 구간
            else
            {
                var dv = (_maxSpeed - _currentSpeed) * _decelAccel * deltaTime;
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