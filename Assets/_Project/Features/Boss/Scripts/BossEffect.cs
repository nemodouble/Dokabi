using System;
using _Project.Features.Battle.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Project.Features.Boss.Scripts
{
    public class BossEffect : MonoBehaviour
    {
        [Header("Common Boss Effect References")]
        [SerializeField] protected Component boss; // BossController<TStateId>
        [SerializeField] protected ParticleSystem hitPS;
        [SerializeField] protected ParticleSystem deadPS;
        
        // 여러 개의 오브젝트를 별도로 관리하기 위한 리스트들
        [Header("Sprite Targets")] 
        [SerializeField] protected SpriteView[] flashTargets; // 피격 등에서 Flash 할 대상들
        [SerializeField] protected SpriteView[] flipTargets;  // SpriteFlip 할 대상들
        [SerializeField] protected GameObject[] scaleFlipTargets; // ScaleFlip 할 대상들

        public void Initialize()
        {
            if (boss == null)
                boss = GetComponent(typeof(BossController<,>));

            if (boss == null)
                return;

            if (boss.TryGetComponent(out BossContext<object> ctx))
            {
                ctx.OnHit += OnHit;
                ctx.OnDead += OnDead;
            }
        }
        
        public void OnHit(int damage, Vector2 attackDir, float knockbackForce)
        {
            if (hitPS != null)
            {
                var angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
                var hitPSTransform = hitPS.transform;
                hitPSTransform.position = transform.position;
                hitPSTransform.eulerAngles = new Vector3(0, 0, angle);
                hitPS.Play();
            }

            // 기존 spriteView 대신 flashTargets 리스트를 사용
            FlashOnHit();
        }

        // 피격 시 Flash용 헬퍼
        protected virtual void FlashOnHit()
        {

            if (flashTargets != null)
            {
                foreach (var sv in flashTargets)
                {
                    if (sv == null) continue;
                    sv.Flash();
                }
            }
        }

        public void OnDead()
        {
            if (deadPS != null)
            {
                deadPS.Play();
            }
        }

        /// <summary>
        /// 파티클 재생용 공통 헬퍼 (상속에서 사용)
        /// </summary>
        /// <param name="ps"></param>
        protected void PlayParticle(ParticleSystem ps)
        {
            if (ps == null) return;
            ps.Play();
        }

        /// <summary>
        /// 파티클 정지용 공통 헬퍼 (상속에서 사용)
        /// </summary>
        /// <param name="ps"></param>
        protected void StopParticle(ParticleSystem ps)
        {
            if (ps == null) return;
            ps.Stop();
        }
        
        public void SetLookDirection(bool isLookingRight)
        {
            // flipTargets에 들어있는 대상들만 방향 전환 추가 적용
            if (flipTargets == null) return;

            foreach (var sv in flipTargets)
            {
                if (sv == null) continue;
                sv.SetLookDirection(isLookingRight);
            }
            // scaleFlipTargets에 들어있는 대상들만 스케일 반전 적용
            if (scaleFlipTargets == null) return;
            foreach (var go in scaleFlipTargets)
            {
                if (go == null) continue;
                var localScale = go.transform.localScale;
                localScale.x = Math.Abs(localScale.x) * (isLookingRight ? 1 : -1);
                go.transform.localScale = localScale;
            }
        }
    }
}
