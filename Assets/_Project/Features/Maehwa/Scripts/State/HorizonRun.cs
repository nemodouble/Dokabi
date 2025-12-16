using System.Collections;
using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using UnityEngine;

namespace _Project.Features.Maehwa.Scripts.State
{
    public class HorizonRun : BossState<MaehwaStateId, MaehwaContext>
    {
        private readonly float _velocity;
        private readonly float _timeMax;
        private readonly float _waitOnTeleport;

        private float _timeNow;
        private Vector2 _moveDir;
        private Vector3? _targetPos;
        private bool _useTeleport;

        public HorizonRun(
            MaehwaStateId id,
            float velocity,
            float timeMax,
            float waitOnTeleport)
            : base(id)
        {
            _velocity = velocity;
            _timeMax = timeMax;
            _waitOnTeleport = waitOnTeleport;
        }

        public override void OnEnter(MaehwaContext ctx)
        {
            ctx.NotifyStateEnter(ID);

            _timeNow = 0f;
            IsFinished = false;
            _useTeleport = false;
            _targetPos = null;

            var bossPos = ctx.transform.position;
            var playerPos = ctx.PlayerTransform.position;

            // 최종 목적지: Context에 세팅된 HorizonLeft/RightPosition
            var leftDest = ctx.horizonLeftPosition.position;
            var rightDest = ctx.horizonRightPosition.position;

            // 1) 텔레포트 모드
            if ((bossPos.x < playerPos.x && playerPos.x < leftDest.x) || 
                (bossPos.x > playerPos.x && playerPos.x > rightDest.x))
            {
                bool playerOnRight = playerPos.x > bossPos.x;
                _targetPos = playerOnRight ? leftDest : rightDest;
                _useTeleport = true;
                ctx.StartCoroutine(TeleportRoutine(ctx));
                return;
            }

            // 2) 그 외: 플레이어 반대편 HorizonPosition으로 MoveByVelocity 스타일 이동
            var runLeft = playerPos.x > bossPos.x;
            var runTarget = runLeft ? leftDest : rightDest;
            ctx.IsLeftHorizonRunUsed = runLeft;

            _moveDir = (runTarget - bossPos).normalized;
            ctx.SetLookingDir(_moveDir.x < 0 ? Vector2.left : Vector2.right );
            _targetPos = runTarget;
        }

        public override void Tick(MaehwaContext ctx, float deltaTime)
        {
            if (IsFinished || _useTeleport)
                return;

            _timeNow += deltaTime;
            if (_timeNow >= _timeMax)
                IsFinished = true;
        }

        public override void FixedTick(MaehwaContext ctx, float deltaTime)
        {
            if (IsFinished || _useTeleport)
            {
                ctx.StopMove();
                return;
            }

            if (_targetPos.HasValue)
            {
                var current = (Vector2)ctx.transform.position;
                var toTarget = (Vector2)_targetPos.Value - current;

                // 목표 지점에 충분히 가깝거나, 이미 지나친 경우 -> 정확히 스냅
                if (toTarget.magnitude <= 0.1f || Vector2.Dot(toTarget, _moveDir) <= 0f)
                {
                    ctx.transform.position = _targetPos.Value;
                    IsFinished = true;
                    ctx.StopMove();
                    return;
                }

                _moveDir = toTarget.normalized;
            }

            ctx.Move(_moveDir * _velocity);
        }

        public override void OnExit(MaehwaContext ctx)
        {
            ctx.StopMove();
            ctx.NotifyStateExit(ID);
        }

        public override void HandleEvent(MaehwaContext ctx, object evt) { }

        private IEnumerator TeleportRoutine(MaehwaContext ctx)
        {
            ctx.StopMove();

            if (_targetPos.HasValue)
                ctx.transform.position = _targetPos.Value;

            float t = 0f;
            while (t < _waitOnTeleport)
            {
                if (ctx == null) yield break;
                t += Time.deltaTime;
                yield return null;
            }

            IsFinished = true;
        }
    }
}