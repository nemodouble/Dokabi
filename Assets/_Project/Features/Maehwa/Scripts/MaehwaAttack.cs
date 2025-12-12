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
        [SerializeField] private GameObject horizonAttackRange;
        [SerializeField] private GameObject bodyStrongAttack;
        [SerializeField] private GameObject comboNormalAttack;
        [SerializeField] private GameObject comboStingAttack;
        [SerializeField] private GameObject rampageRangePrefab;
        [SerializeField] private GameObject downEffect;
        [SerializeField] private GameObject bodyWall;
        [SerializeField] private GameObject bossDangerRange;

        /// <summary>
        /// 가로베기 공격 범위
        /// </summary>
        public GameObject HorizonAttackRange => horizonAttackRange;
        /// <summary>
        /// 바디 태클 강 공격 범위
        /// </summary>
        public GameObject BodyStrongAttack => bodyStrongAttack;
        /// <summary>
        /// 콤보 일반 공격 범위
        /// </summary>
        public GameObject ComboNormalAttack => comboNormalAttack;
        /// <summary>
        /// 콤보 찌르기 공격 범위
        /// </summary>
        public GameObject ComboStingAttack => comboStingAttack;
        /// <summary>
        /// 다운 스매시 추가 경직 시 사용하는 이펙트/히트 오브젝트
        /// </summary>
        public GameObject DownEffect => downEffect;
        /// <summary>
        /// 바디 월 오브젝트
        /// </summary>
        public GameObject BodyWall => bodyWall;
        /// <summary>
        /// 보스 데인저 범위 오브젝트
        /// </summary>
        public GameObject BossDangerRange => bossDangerRange;

        /// <summary>
        /// 다운 스매시 추가 경직 시 사용하는 이펙트/히트 오브젝트 활성/비활성
        /// </summary>
        public void SetDownEffectActive(bool active)
        {
            if (downEffect != null)
                downEffect.SetActive(active);
        }

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
    }
}
