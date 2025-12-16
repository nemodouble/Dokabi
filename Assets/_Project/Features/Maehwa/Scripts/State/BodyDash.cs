using _Project.Features.Boss.Scripts.State;
using Character.Player;
using UnityEngine;

namespace _Project.Features.Maehwa.Scripts.State
{
    public class BodyDash : BossState<MaehwaStateId, MaehwaContext>
    {
        private float _dashSpeed;
        private float _dashTime;
        
        private Vector2 _runtimeDashDir;
        private float _dashTimeElapsed;
        
        public BodyDash(MaehwaStateId id, float dashSpeed, float dashTime) : base(id)
        {
            _dashSpeed = dashSpeed;
            _dashTime = dashTime;
        }

        public override void OnEnter(MaehwaContext ctx)
        {
            var a = ctx.Attack as MaehwaAttack;
            if (a == null)
            {
                Debug.LogError("BodyDash: MaehwaAttack을 찾지 못했습니다.");
                IsFinished = true;
                return;
            }
            a.GrabRange.gameObject.SetActive(true);
            a.EnemyBody.enabled = false;
            IsFinished = false;
            _dashTimeElapsed = _dashTime;
            _runtimeDashDir = ctx.PlayerTransform.position.x < ctx.transform.position.x ? Vector2.left : Vector2.right;
            _runtimeDashDir.Normalize();
            _runtimeDashDir *= _dashSpeed;
            ctx.NotifyStateEnter(ID);
        }

        public override void OnExit(MaehwaContext ctx)
        {
            var a = ctx.Attack as MaehwaAttack;
            if (a == null)
            {
                Debug.LogError("BodyDash: MaehwaAttack을 찾지 못했습니다.");
                IsFinished = true;
                return;
            }
            a.GrabRange.gameObject.SetActive(false);
            a.EnemyBody.enabled = true;
            ctx.StopMove();
            _runtimeDashDir = Vector2.zero;
            ctx.NotifyStateExit(ID);
        }

        public override void Tick(MaehwaContext ctx, float deltaTime)
        {
        }

        public override void FixedTick(MaehwaContext ctx, float deltaTime)
        {
            if (_dashTimeElapsed > 0f)
            {
                var platformLayer = LayerMask.NameToLayer("Platform");
                var layerMask = 1 << platformLayer;
                var range = ctx.stats.bodyDashStopBeforeObstacleDistance;
                var hasObstacle = ctx.IsThereSomethingInFront(range, layerMask);

                if (!hasObstacle)
                {
                    ctx.Move(_runtimeDashDir);
                }
                else
                {
                    ctx.StopMove();
                }

                _dashTimeElapsed -= deltaTime;
            }
            else
            {
                IsFinished = true;
            }
        }

        public override void HandleEvent(MaehwaContext ctx, object evt)
        {
        }
    }
}