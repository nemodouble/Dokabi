using _Project.Features.Boss.Scripts;
using _Project.Features.Maehwa.Scripts.State;
using Boss.MaeHwa;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Maehwa.Scripts
{
    public class MaehwaContext : BossContext<MaehwaStateId>
    {
        [SerializeField] public MaeHwaStats stats;

        public ComboSelectDash.DashDir SelectedDashDir { get; set; } = ComboSelectDash.DashDir.None;

        public override void SetLookingDir(LookingDir dir)
        {
            base.SetLookingDir(dir);
            var atk = Attack as MaehwaAttack;
            if (atk != null)
                atk.SetAttackColliderDir(dir);
        }

        /// <summary>
        /// 플레이어와 보스 간의 거리와 좌우 위치를 기반으로 이동할 방향을 선택합니다.
        /// 가까우면 플레이어의 반대 방향, 멀면 플레이어가 있는 방향으로 이동하도록 합니다.
        /// </summary>
        /// <returns> true : 오른쪽으로 이동, false : 왼쪽으로 이동 </returns>
        public bool SelectMoveDir()
        {
            var isNear = IsInDistance(0f, stats.walkCloselyDistance);
            var playerOnRight = PlayerTransform.position.x > transform.position.x;

            if (isNear)
                return !playerOnRight;
            else
                return playerOnRight;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ComboSelectDash.DashDir SelectDashDir()
        {
            var isNear = IsInDistance(0f, stats.comboAttackWithDashDistance);
            var playerOnRight = PlayerTransform.position.x > transform.position.x;

            if (isNear)
                return ComboSelectDash.DashDir.None;
            else
                return playerOnRight ? ComboSelectDash.DashDir.Right : ComboSelectDash.DashDir.Left;
        }
        
        public bool IsInDistance(float minDistance, float maxDistance)
        {
            var distance = Mathf.Abs(transform.position.x - PlayerTransform.position.x);
            return distance >= minDistance && distance <= maxDistance;
        }
    }
}