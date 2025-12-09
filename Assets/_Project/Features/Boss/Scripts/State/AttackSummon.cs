using UnityEngine;

namespace _Project.Features.Boss.Scripts.State
{
    public class AttackSummon : BossState<BossContext>
    {
        private readonly GameObject _prefab;
        private readonly Vector2 _relativePos;
        private readonly string _enterAnimTrigger;

        public AttackSummon(string id, GameObject gameObject, Vector2? relativePos = null, string enterAnimTrigger = null) : base(id)
        {
            _prefab = gameObject;
            _relativePos = relativePos ?? Vector2.zero;
            _enterAnimTrigger = enterAnimTrigger;
        }

        public override void OnEnter(BossContext ctx)
        {
            IsFinished = false;

            if (!string.IsNullOrEmpty(_enterAnimTrigger))
                ctx.PlayAnimTrigger(_enterAnimTrigger);

            ctx.SummonAttack(_prefab, _relativePos);

            // 한 번만 소환하고 바로 종료
            IsFinished = true;
        }

        public override void OnExit(BossContext ctx) { }

        public override void Tick(BossContext ctx, float deltaTime) { }

        public override void FixedTick(BossContext ctx, float deltaTime) { }

        public override void HandleEvent(BossContext ctx, object evt) { }
    }
}