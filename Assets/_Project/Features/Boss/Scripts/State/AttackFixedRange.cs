using UnityEngine;

namespace _Project.Features.Boss.Scripts.State
{
    public class AttackFixedRange : BossState
    {
        private readonly GameObject _attackRange;
        private readonly string _enterAnimTrigger;

        public AttackFixedRange(string id, GameObject attackRange, string enterAnimTrigger = null)
            : base(id)
        {
            _attackRange = attackRange;
            _enterAnimTrigger = enterAnimTrigger;
        }

        public override void OnEnter(BossContext ctx)
        {
            IsFinished = false;

            if (!string.IsNullOrEmpty(_enterAnimTrigger))
                ctx.PlayAnimTrigger(_enterAnimTrigger);

            if (_attackRange != null)
            {
                _attackRange.SetActive(true);
            }

            // 한 번 켜고 바로 종료
            IsFinished = true;
        }

        public override void OnExit(BossContext ctx)
        {
        }

        public override void Tick(BossContext ctx, float deltaTime)
        {
        }

        public override void FixedTick(BossContext ctx, float deltaTime)
        {
        }

        public override void HandleEvent(BossContext ctx, object evt)
        {
        }
    }
}