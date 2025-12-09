using UnityEngine;

namespace _Project.Features.Boss.Scripts.State
{
    public class WaitState : BossState<BossContext>
    {
        private readonly float _waitingSecond;
        private readonly bool _notMoving;
        private readonly string _enterAnimTrigger;
        private readonly string _exitAnimTrigger;

        private float _elapsed;

        public WaitState(string id, float waitingSecond, bool notMoving = false,
                         string enterAnimTrigger = null, string exitAnimTrigger = null) : base(id)
        {
            _waitingSecond = waitingSecond;
            _notMoving = notMoving;
            _enterAnimTrigger = enterAnimTrigger;
            _exitAnimTrigger = exitAnimTrigger;
        }

        public override void OnEnter(BossContext ctx)
        {
            _elapsed = 0f;
            IsFinished = false;

            if (!string.IsNullOrEmpty(_enterAnimTrigger))
                ctx.PlayAnimTrigger(_enterAnimTrigger);
        }

        public override void OnExit(BossContext ctx)
        {
            if (_notMoving)
            {
                ctx.StopMove();
            }

            if (!string.IsNullOrEmpty(_exitAnimTrigger))
                ctx.PlayAnimTrigger(_exitAnimTrigger);
        }

        public override void Tick(BossContext ctx, float deltaTime)
        {
            if (IsFinished)
                return;

            _elapsed += deltaTime;
            if (_elapsed >= _waitingSecond)
            {
                IsFinished = true;
            }
        }

        public override void FixedTick(BossContext ctx, float deltaTime)
        {
            if (_notMoving && !IsFinished)
            {
                ctx.StopMove();
            }
        }

        public override void HandleEvent(BossContext ctx, object evt) { }
    }
}