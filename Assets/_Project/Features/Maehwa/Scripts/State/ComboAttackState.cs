using _Project.Features.Boss.Scripts;
using _Project.Features.Boss.Scripts.State;
using _Project.Features.Boss.Scripts.State.Moving;
using UnityEngine;

namespace _Project.Features.Maehwa.Scripts.State
{
    /// <summary>
    /// 매화 전용 콤보 공격 상태.
    /// 한 프레임 안에 콜라이더 활성(AttackFixedRange 역할)과 대시/대기를 함께 처리한다.
    /// </summary>
    public class ComboAttackState : BossState<MaehwaStateId, MaehwaContext>
    {
        private readonly Vector2 _baseDashDir;
        private readonly float _dashSpeed;
        private readonly float _dashTime;
        private readonly float _noDashTime;
        private readonly BossAttackRangeType _attackType;

        private float _elapsed;
        private bool _colliderActivated;
        private Vector2 _runtimeDashDir;

        public enum BossAttackRangeType
        {
            None,
            ComboNormal,
            ComboSting
        }

        public ComboAttackState(
            MaehwaStateId id,
            Vector2 dashDir,
            float dashSpeed,
            float dashTime,
            float noDashTime,
            BossAttackRangeType attackType)
            : base(id)
        {
            _baseDashDir = dashDir;
            _dashSpeed = dashSpeed;
            _dashTime = dashTime;
            _noDashTime = noDashTime;
            _attackType = attackType;
        }

        public override void OnEnter(MaehwaContext ctx)
        {
            IsFinished = false;
            _elapsed = 0f;
            _colliderActivated = false;

            ctx.NotifyStateEnter(ID);

            // ComboSelectDash 결과를 기반으로 실제 대시 방향 결정
            _runtimeDashDir = GetDashDirFromContext(ctx, _baseDashDir);

            // 콜라이더 활성 (원래 AttackFixedRange 에서 하던 역할)
            var atk = ctx.Attack as MaehwaAttack;
            if (atk != null)
            {
                switch (_attackType)
                {
                    case BossAttackRangeType.ComboNormal:
                        if (atk.ComboNormalAttack != null)
                            atk.ComboNormalAttack.gameObject.SetActive(true);
                        break;
                    case BossAttackRangeType.ComboSting:
                        if (atk.ComboStingAttack != null)
                            atk.ComboStingAttack.gameObject.SetActive(true);
                        break;
                }
            }

            _colliderActivated = true;
        }

        private Vector2 GetDashDirFromContext(MaehwaContext ctx, Vector2 fallback)
        {
            // MaehwaContext 안에 ComboSelectDash 결과가 저장되어 있다고 가정하고,
            // 없으면 생성자에서 넘겨준 기본 방향(fallback)을 사용.
            if (ctx.SelectedDashDir == ComboSelectDash.DashDir.Left)
                return Vector2.left;
            if (ctx.SelectedDashDir == ComboSelectDash.DashDir.Right)
                return Vector2.right;

            // Dash 없음이거나 아직 셋업이 안 됐으면 기본값 사용
            return fallback;
        }

        public override void Tick(MaehwaContext ctx, float deltaTime)
        {
            if (IsFinished)
                return;

            _elapsed += deltaTime;

            // 대시 시간 혹은 노대시 시간 기준으로 종료
            float timeLimit = _runtimeDashDir == Vector2.zero ? _noDashTime : _dashTime;
            if (_elapsed >= timeLimit)
            {
                IsFinished = true;
            }
        }

        public override void FixedTick(MaehwaContext ctx, float deltaTime)
        {
            if (IsFinished)
            {
                ctx.StopMove();
                return;
            }

            if (_runtimeDashDir != Vector2.zero)
            {
                ctx.Move(_runtimeDashDir.normalized * _dashSpeed);
            }
            else
            {
                ctx.StopMove();
            }
        }

        public override void OnExit(MaehwaContext ctx)
        {
            ctx.StopMove();
        }

        public override void HandleEvent(MaehwaContext ctx, object evt)
        {
        }
    }
}
