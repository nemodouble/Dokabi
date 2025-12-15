using System;
using _Project.Features.Battle.Scripts;
using _Project.Features.Boss.Scripts;
using Boss.MaeHwa;
using UnityEngine;

namespace _Project.Features.Maehwa.Scripts
{
    /// <summary>
    /// MaeHwa 전용 공격/히트박스 관리 컨트롤러.
    /// BossAttackController를 상속하여, 공격 프리팹 및 히트 범위를 관리한다.
    /// </summary>
    public class MaehwaAttack : BossAttack
    {
        [Header("Maehwa Attack Objects")]        
        [SerializeField] private FixedDangerRange horizonAttackRange;
        [SerializeField] private FixedDangerRange bodyStrongAttack;
        [SerializeField] private FixedDangerRange comboNormalAttack;
        [SerializeField] private FixedDangerRange comboStingAttack;
        [SerializeField] private GameObject rampageRangePrefab;
        [SerializeField] private GameObject bodyWall;
        [SerializeField] private GameObject bossDangerRange;

        /// <summary>
        /// 가로베기 공격 범위
        /// </summary>
        public FixedDangerRange HorizonAttackRange => horizonAttackRange;
        /// <summary>
        /// 바디 태클 강 공격 범위
        /// </summary>
        public FixedDangerRange BodyStrongAttack => bodyStrongAttack;
        /// <summary>
        /// 콤보 일반 공격 범위
        /// </summary>
        public FixedDangerRange ComboNormalAttack => comboNormalAttack;
        /// <summary>
        /// 콤보 찌르기 공격 범위
        /// </summary>
        public FixedDangerRange ComboStingAttack => comboStingAttack;
        /// <summary>
        /// 바디 월 오브젝트
        /// </summary>
        public GameObject BodyWall => bodyWall;
        /// <summary>
        /// 보스 데인저 범위 오브젝트
        /// </summary>
        public GameObject BossDangerRange => bossDangerRange;

        /// <summary>
        /// Rampage 공격 범위 프리팹 인스턴스 생성 헬퍼
        /// (기존 MaeHwaController.InstantiateRampageRange 래핑)
        /// </summary>
        public MaeHwaRampageRange InstantiateRampageRange(Vector2 rampagePos, Vector3 rotation)
        {
            if (rampageRangePrefab == null)
                return null;

            var go = Instantiate(rampageRangePrefab, rampagePos, Quaternion.Euler(rotation));
            return go.GetComponent<MaeHwaRampageRange>();
        }

        /// <summary>
        /// 바디 월 활성화/비활성화
        /// </summary>
        public void SetBodyWallActive(bool active)
        {
            if (bodyWall != null)
                bodyWall.SetActive(active);
        }

        /// <summary>
        /// 보스 데인저 범위 활성화/비활성화
        /// </summary>
        public void SetBossDangerRangeActive(bool active)
        {
            if (bossDangerRange != null)
                bossDangerRange.SetActive(active);
        }

        public void SetAttackColliderDir(BossContext<MaehwaStateId>.LookingDir dir)
        {
            bool isRight = dir == BossContext<MaehwaStateId>.LookingDir.RightDir;
            
            FlipLocalScaleX(horizonAttackRange.transform, isRight);
            FlipLocalScaleX(bodyStrongAttack.transform, isRight);
            FlipLocalScaleX(comboNormalAttack.transform, isRight);
            FlipLocalScaleX(comboStingAttack.transform, isRight);
        }
        
        private static void FlipLocalScaleX(Transform target, bool isRight = false)
        {
            if (target == null)
                return;

            var scale = target.localScale;
            scale.x = Math.Abs(scale.x) * (isRight ? 1 : -1);
            target.localScale = scale;
        }
    }
}
