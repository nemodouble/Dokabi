using System;
using _Project.Features.Battle.Scripts;
using _Project.Features.Boss.Scripts;
using Boss.MaeHwa;
using Mechanics.System;
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
        [SerializeField] private GrabRange grabRange;
        [SerializeField] private EnemyBody enemyBody;
        
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
        /// 그랩(잡기) 범위
        /// </summary>
        public GrabRange GrabRange => grabRange;
        /// <summary>
        /// 매화 접촉 데미지 바디
        /// </summary>
        public EnemyBody EnemyBody => enemyBody;

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

        public void ActiveBodyDash(bool activate)
        {
            if (GrabRange != null)
            {
                GrabRange.gameObject.SetActive(activate);
            }
        }

        public void SetAttackColliderDir(BossContext<MaehwaStateId>.LookingDir dir)
        {
            bool isRight = dir == BossContext<MaehwaStateId>.LookingDir.Right;
            
            FlipLocalScaleX(horizonAttackRange.transform, isRight);
            FlipLocalScaleX(bodyStrongAttack.transform, isRight);
            FlipLocalScaleX(comboNormalAttack.transform, isRight);
            FlipLocalScaleX(comboStingAttack.transform, isRight);
            FlipLocalScaleX(grabRange.transform, isRight);
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
