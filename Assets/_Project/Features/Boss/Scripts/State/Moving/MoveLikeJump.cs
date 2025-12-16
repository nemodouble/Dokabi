using _Project.Features.Boss.Scripts;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class MoveLikeJump<TStateId> : BossState<TStateId, BossContext<TStateId>>
    {
        private readonly float startSpeed;
        private readonly float jumpTime;
        private float jumpTimeNow;
        private float _currentVy;

        public MoveLikeJump(TStateId id, float startSpeed, float jumpTime)
            : base(id)
        {
            this.startSpeed = startSpeed;
            this.jumpTime = jumpTime;
        }

        public override void OnEnter(BossContext<TStateId> ctx)
        {
            jumpTimeNow = 0f;
            IsFinished = false;
            _currentVy = startSpeed;

            ctx.NotifyStateEnter(ID);
        }

        public override void OnExit(BossContext<TStateId> ctx)
        {
            ctx.StopMove();
        }

        public override void Tick(BossContext<TStateId> ctx, float deltaTime)
        {
            if (IsFinished)
                return;

            if (jumpTimeNow <= jumpTime)
            {
                jumpTimeNow += deltaTime;
                _currentVy = startSpeed - (jumpTimeNow / jumpTime) * startSpeed;
            }
            else
            {
                IsFinished = true;
                _currentVy = 0f;
            }
        }

        public override void FixedTick(BossContext<TStateId> ctx, float deltaTime)
        {
            if (IsFinished)
            {
                ctx.StopMove();
                return;
            }

            // 점프 곡선에 따라 계산된 수직 속도만 갱신
            ctx.MoveY(_currentVy);
        }

        public override void HandleEvent(BossContext<TStateId> ctx, object evt) { }
    }
}