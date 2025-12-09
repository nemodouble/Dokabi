using _Project.Features.Boss.Scripts;

namespace _Project.Features.Boss.Scripts.State.Moving
{
    public class MoveLikeJump : BossState<BossContext>
    {
        private readonly float startSpeed;
        private readonly float jumpTime;
        private float jumpTimeNow;
        private float _currentVy;
        private readonly string _enterAnimTrigger;

        public MoveLikeJump(string id, float startSpeed, float jumpTime, string enterAnimTrigger = null)
            : base(id)
        {
            this.startSpeed = startSpeed;
            this.jumpTime = jumpTime;
            _enterAnimTrigger = enterAnimTrigger;
        }

        public override void OnEnter(BossContext ctx)
        {
            jumpTimeNow = 0f;
            _currentVy = startSpeed;

            if (!string.IsNullOrEmpty(_enterAnimTrigger))
                ctx.PlayAnimTrigger(_enterAnimTrigger);
        }

        public override void OnExit(BossContext ctx)
        {
            ctx.StopMove();
        }

        public override void Tick(BossContext ctx, float deltaTime)
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

        public override void FixedTick(BossContext ctx, float deltaTime)
        {
            if (IsFinished)
            {
                ctx.StopMove();
                return;
            }

            // 점프 곡선에 따라 계산된 수직 속도만 갱신
            ctx.MoveY(_currentVy);
        }

        public override void HandleEvent(BossContext ctx, object evt) { }
    }
}