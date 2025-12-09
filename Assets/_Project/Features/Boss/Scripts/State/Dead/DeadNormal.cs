using UnityEngine;

namespace _Project.Features.Boss.Scripts.State.Dead
{
    public class DeadNormal : BossState<BossContext>
    {
        private readonly string _enterAnimTrigger;

        public DeadNormal(string id, string enterAnimTrigger = null) : base(id)
        {
            _enterAnimTrigger = enterAnimTrigger;
        }

        public override void OnEnter(BossContext ctx)
        {
            if (!string.IsNullOrEmpty(_enterAnimTrigger))
                ctx.PlayAnimTrigger(_enterAnimTrigger);

            var boss = ctx.Controller.gameObject;
            boss.layer = LayerMask.NameToLayer("Dummy");
            boss.tag = "Dummy";
        }
        public override void OnExit(BossContext ctx) { }
        public override void Tick(BossContext ctx, float deltaTime) { }
        public override void FixedTick(BossContext ctx, float deltaTime) { }
        public override void HandleEvent(BossContext ctx, object evt) { }
    }
}